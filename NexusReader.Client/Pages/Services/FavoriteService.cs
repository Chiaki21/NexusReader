using System.Net.Http.Json;
using NexusReader.Shared.Models;

namespace NexusReader.Services
{
    public class FavoriteService
    {
        private readonly HttpClient _http;
        private readonly AuthStateService _auth;

        public FavoriteService(HttpClient http, AuthStateService auth)
        {
            _http = http;
            _auth = auth;
        }

        public async Task<List<BookSummaryResponse>> GetFavoritesAsync()
        {
            _auth.ApplyAuthorizationHeader(_http);
            var list = await _http.GetFromJsonAsync<List<BookSummaryResponse>>("api/favorites");
            return list ?? new List<BookSummaryResponse>();
        }

        public async Task<bool> AddFavoriteAsync(int bookId)
        {
            _auth.ApplyAuthorizationHeader(_http);
            var response = await _http.PostAsJsonAsync("api/favorites", new FavoriteRequest(bookId));
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> RemoveFavoriteAsync(int bookId)
        {
            _auth.ApplyAuthorizationHeader(_http);
            var response = await _http.DeleteAsync($"api/favorites/{bookId}");
            return response.IsSuccessStatusCode;
        }
    }
}
