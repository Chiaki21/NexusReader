using System.Net.Http.Json;
using NexusReader.Shared.Models;

namespace NexusReader.Services
{
    public class AuthStateService 
    {
        private readonly HttpClient _http;

        public AuthStateService(HttpClient http)
        {
            _http = http;
        }

        public bool IsLoggedIn { get; private set; } = false;
        public bool IsAdmin { get; private set; } = false;

        public event Action? OnAuthStateChanged;

        public async Task<bool> Register(string email, string password, string confirmPassword)
        {
            var model = new RegisterModel 
            { 
                Email = email, 
                Password = password, 
                ConfirmPassword = confirmPassword 
            };
            
            // This hits the AccountController.cs we built in the Server
            var response = await _http.PostAsJsonAsync("api/account/register", model);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> Login(string email, string password)
        {
            var model = new LoginModel { Email = email, Password = password };
            var response = await _http.PostAsJsonAsync("api/account/login", model);

            if (response.IsSuccessStatusCode)
            {
                IsLoggedIn = true;
                NotifyStateChanged();
                return true;
            }
            return false;
        }

        public void SetAdmin(bool isAdmin)
        {
            IsAdmin = isAdmin;
            IsLoggedIn = true; 
            NotifyStateChanged();
        }

       public async Task<bool> Register(string email, string password, string confirmPassword)
{
    var model = new RegisterModel 
    { 
        Email = email, 
        Password = password, 
        ConfirmPassword = confirmPassword // Server needs this to pass the [Compare] check!
    };
    
    var response = await _http.PostAsJsonAsync("api/account/register", model);
    return response.IsSuccessStatusCode;
}
        public async Task<bool> Login(string email, string password)
{
    var model = new LoginModel { Email = email, Password = password };
    var response = await _http.PostAsJsonAsync("api/account/login", model);

    if (response.IsSuccessStatusCode)
    {
        IsLoggedIn = true;
        NotifyStateChanged();
        return true;
    }
    return false;
}

        public void Logout()
        {
            IsLoggedIn = false;
            IsAdmin = false;
            NotifyStateChanged();
        }

        private void NotifyStateChanged() => OnAuthStateChanged?.Invoke();
    }
}