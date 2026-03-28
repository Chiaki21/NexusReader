using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.JSInterop;
using NexusReader.Shared.Models;

namespace NexusReader.Services
{
    public class AuthStateService
    {
        private readonly HttpClient _http;
        private readonly IJSRuntime _js;

        private const string LocalKey = "nexusreader_auth";
        private const string SessionKey = "nexusreader_auth";

        public AuthStateService(HttpClient http, IJSRuntime js)
        {
            _http = http;
            _js = js;
        }

        public bool IsLoggedIn { get; private set; }
        public bool IsAdmin { get; private set; }
        public string UserFirstName { get; private set; } = string.Empty;
        public string UserEmail { get; private set; } = string.Empty;

        public event Action? OnAuthStateChanged;

        public async Task RestoreSessionAsync()
        {
            var localJson = await _js.InvokeAsync<string?>("localStorage.getItem", LocalKey);
            var sessionJson = await _js.InvokeAsync<string?>("sessionStorage.getItem", SessionKey);

            var payloadJson = !string.IsNullOrEmpty(localJson) ? localJson : sessionJson;
            if (string.IsNullOrEmpty(payloadJson))
            {
                IsLoggedIn = false;
                UserFirstName = string.Empty;
                UserEmail = string.Empty;
                return;
            }

            try
            {
                var payload = JsonSerializer.Deserialize<StoredAuthPayload>(payloadJson);
                if (payload == null || string.IsNullOrEmpty(payload.Email))
                {
                    await ClearPersistedSessionAsync();
                    IsLoggedIn = false;
                    return;
                }

                UserEmail = payload.Email;
                UserFirstName = payload.FirstName ?? string.Empty;
                IsLoggedIn = true;
                NotifyStateChanged();
            }
            catch
            {
                await ClearPersistedSessionAsync();
                IsLoggedIn = false;
                UserFirstName = string.Empty;
                UserEmail = string.Empty;
            }
        }

        public async Task<bool> Register(string email, string password, string confirmPassword)
        {
            var model = new RegisterModel
            {
                Email = email,
                Password = password,
                ConfirmPassword = confirmPassword
            };

            var response = await _http.PostAsJsonAsync("api/account/register", model);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> Login(string email, string password, bool rememberMe)
        {
            var model = new LoginModel { Email = email, Password = password };
            var response = await _http.PostAsJsonAsync("api/account/login", model);

            if (!response.IsSuccessStatusCode)
                return false;

            var body = await response.Content.ReadFromJsonAsync<LoginResponseDto>();
            IsLoggedIn = true;
            UserEmail = email;
            UserFirstName = body?.FirstName ?? string.Empty;
            await PersistSessionAsync(email, UserFirstName, rememberMe);
            NotifyStateChanged();
            return true;
        }

        public void SetAdmin(bool isAdmin)
        {
            IsAdmin = isAdmin;
            IsLoggedIn = true;
            NotifyStateChanged();
        }

        public async Task LogoutAsync()
        {
            IsLoggedIn = false;
            IsAdmin = false;
            UserFirstName = string.Empty;
            UserEmail = string.Empty;
            await ClearPersistedSessionAsync();
            NotifyStateChanged();
        }

        private async Task PersistSessionAsync(string email, string firstName, bool rememberMe)
        {
            var payload = JsonSerializer.Serialize(new StoredAuthPayload
            {
                Email = email,
                FirstName = firstName
            });

            if (rememberMe)
            {
                await _js.InvokeVoidAsync("localStorage.setItem", LocalKey, payload);
                await _js.InvokeVoidAsync("sessionStorage.removeItem", SessionKey);
            }
            else
            {
                await _js.InvokeVoidAsync("sessionStorage.setItem", SessionKey, payload);
                await _js.InvokeVoidAsync("localStorage.removeItem", LocalKey);
            }
        }

        private async Task ClearPersistedSessionAsync()
        {
            await _js.InvokeVoidAsync("localStorage.removeItem", LocalKey);
            await _js.InvokeVoidAsync("sessionStorage.removeItem", SessionKey);
        }

        private void NotifyStateChanged() => OnAuthStateChanged?.Invoke();

        private sealed class StoredAuthPayload
        {
            public string Email { get; set; } = string.Empty;
            public string? FirstName { get; set; }
        }

        private sealed class LoginResponseDto
        {
            public string? Message { get; set; }
            public string? FirstName { get; set; }
        }
    }
}
