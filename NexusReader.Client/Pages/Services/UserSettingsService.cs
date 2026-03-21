using Microsoft.JSInterop;

namespace NexusReader.Services
{
    public class UserSettingsService
    {
        private readonly IJSRuntime _js;
        private const string StorageKey = "nexus_user_settings";

        public bool IsDarkMode { get; private set; } = true;
        public double FontSize { get; private set; } = 18;

        public event Action? OnChange;

        public UserSettingsService(IJSRuntime js)
        {
            _js = js;
        }

        // We call this once when the app starts
        public async Task InitializeAsync()
        {
            var savedData = await _js.InvokeAsync<string>("localStorage.getItem", StorageKey);
            if (!string.IsNullOrEmpty(savedData))
            {
                var data = System.Text.Json.JsonSerializer.Deserialize<SettingsData>(savedData);
                if (data != null)
                {
                    IsDarkMode = data.IsDarkMode;
                    FontSize = data.FontSize;
                    NotifyStateChanged();
                }
            }
        }

        public async Task ToggleDarkMode()
        {
            IsDarkMode = !IsDarkMode;
            await SaveToStorage();
            NotifyStateChanged();
        }

        public async Task SetFontSize(double size)
        {
            FontSize = size;
            await SaveToStorage();
            NotifyStateChanged();
        }

        private async Task SaveToStorage()
        {
            var data = new SettingsData { IsDarkMode = IsDarkMode, FontSize = FontSize };
            await _js.InvokeVoidAsync("localStorage.setItem", StorageKey, System.Text.Json.JsonSerializer.Serialize(data));
        }

        private void NotifyStateChanged() => OnChange?.Invoke();

        private class SettingsData
        {
            public bool IsDarkMode { get; set; }
            public double FontSize { get; set; }
        }
    }
}