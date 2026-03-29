using System.Net.Http.Headers;
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
        public string UserLastName { get; private set; } = string.Empty;
        public string UserEmail { get; private set; } = string.Empty;
        public string? AccessToken { get; private set; }

        public event Action? OnAuthStateChanged;

        public void ApplyAuthorizationHeader(HttpClient http)
        {
            if (string.IsNullOrEmpty(AccessToken))
                http.DefaultRequestHeaders.Authorization = null;
            else
                http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);
        }

        public async Task RestoreSessionAsync()
        {
            var localJson = await _js.InvokeAsync<string?>("localStorage.getItem", LocalKey);
            var sessionJson = await _js.InvokeAsync<string?>("sessionStorage.getItem", SessionKey);

            var payloadJson = !string.IsNullOrEmpty(localJson) ? localJson : sessionJson;
            if (string.IsNullOrEmpty(payloadJson))
            {
                ClearMemoryState();
                return;
            }

            try
            {
                var payload = JsonSerializer.Deserialize<StoredAuthPayload>(payloadJson);
                if (payload == null || string.IsNullOrEmpty(payload.Email))
                {
                    await ClearPersistedSessionAsync();
                    ClearMemoryState();
                    return;
                }

                UserEmail = payload.Email;
                UserFirstName = payload.FirstName ?? string.Empty;
                UserLastName = payload.LastName ?? string.Empty;
                AccessToken = payload.AccessToken;
                IsAdmin = payload.IsAdmin;
                IsLoggedIn = true;
                NotifyStateChanged();
            }
            catch
            {
                await ClearPersistedSessionAsync();
                ClearMemoryState();
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
            UserLastName = body?.LastName ?? string.Empty;
            AccessToken = body?.Token;
            IsAdmin = body?.Roles?.Contains("Admin") == true;
            await PersistSessionAsync(rememberMe);
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
            ClearMemoryState();
            await ClearPersistedSessionAsync();
            NotifyStateChanged();
        }

        private void ClearMemoryState()
        {
            IsLoggedIn = false;
            IsAdmin = false;
            UserFirstName = string.Empty;
            UserLastName = string.Empty;
            UserEmail = string.Empty;
            AccessToken = null;
        }

        private async Task PersistSessionAsync(bool rememberMe)
        {
            var payload = JsonSerializer.Serialize(new StoredAuthPayload
            {
                Email = UserEmail,
                FirstName = UserFirstName,
                LastName = UserLastName,
                AccessToken = AccessToken,
                IsAdmin = IsAdmin
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

        

        private void NotifyStateChanged() => OnAuthStateChanged?.Invoke();

        private sealed class StoredAuthPayload
        {
            public string Email { get; set; } = string.Empty;
            public string? FirstName { get; set; }
            public string? LastName { get; set; }
            public string? AccessToken { get; set; }
            public bool IsAdmin { get; set; }
        }

        private sealed class LoginResponseDto
        {
            public string? Message { get; set; }
            public string? Token { get; set; }
            public string? FirstName { get; set; }
            public string? LastName { get; set; }
            public string[]? Roles { get; set; }
        }
    }
}
