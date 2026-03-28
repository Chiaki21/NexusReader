using System.Net.Http.Json;
using NexusReader.Shared.Models;

namespace NexusReader.Services
{
    public class BookService
    {
        private readonly HttpClient _http;
        private readonly AuthStateService _auth;

        public BookService(HttpClient http, AuthStateService auth)
        {
            _http = http;
            _auth = auth;
        }

        private void ApplyAuth() => _auth.ApplyAuthorizationHeader(_http);

        public async Task<List<BookSummaryResponse>> GetBooksAsync()
        {
            ApplyAuth();
            var list = await _http.GetFromJsonAsync<List<BookSummaryResponse>>("api/books");
            return list ?? new List<BookSummaryResponse>();
        }

        public async Task<BookModel?> GetBookAsync(int id)
        {
            ApplyAuth();
            var response = await _http.GetAsync($"api/books/{id}");
            if (!response.IsSuccessStatusCode)
                return null;
            return await response.Content.ReadFromJsonAsync<BookModel>();
        }

        public async Task<List<ChapterListItemResponse>> GetChapterListAsync(int bookId)
        {
            ApplyAuth();
            var list = await _http.GetFromJsonAsync<List<ChapterListItemResponse>>($"api/books/{bookId}/chapters");
            return list ?? new List<ChapterListItemResponse>();
        }

        public async Task<ChapterContentResponse?> GetChapterContentAsync(int chapterId)
        {
            ApplyAuth();
            return await _http.GetFromJsonAsync<ChapterContentResponse>($"api/chapters/{chapterId}");
        }

        public async Task<ChapterModel?> CreateChapterAsync(UpsertChapterRequest request)
        {
            ApplyAuth();
            var response = await _http.PostAsJsonAsync("api/chapters", request);
            if (!response.IsSuccessStatusCode)
                return null;
            return await response.Content.ReadFromJsonAsync<ChapterModel>();
        }

        public async Task<bool> UpdateChapterAsync(int id, UpsertChapterRequest request)
        {
            ApplyAuth();
            var response = await _http.PutAsJsonAsync($"api/chapters/{id}", request);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteChapterAsync(int id)
        {
            ApplyAuth();
            var response = await _http.DeleteAsync($"api/chapters/{id}");
            return response.IsSuccessStatusCode;
        }

        public async Task<BookModel?> CreateBookAsync(CreateBookRequest request)
        {
            ApplyAuth();
            var response = await _http.PostAsJsonAsync("api/books", request);
            if (!response.IsSuccessStatusCode)
                return null;
            return await response.Content.ReadFromJsonAsync<BookModel>();
        }

        public async Task<bool> UpdateBookAsync(int id, UpdateBookRequest request)
        {
            ApplyAuth();
            var response = await _http.PutAsJsonAsync($"api/books/{id}", request);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteBookAsync(int id)
        {
            ApplyAuth();
            var response = await _http.DeleteAsync($"api/books/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}
