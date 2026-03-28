using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using NexusReader;
using NexusReader.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:5087/") });
builder.Services.AddScoped<AuthStateService>();
builder.Services.AddScoped<UserSettingsService>();
builder.Services.AddScoped<BookService>();
builder.Services.AddScoped<FavoriteService>();

await builder.Build().RunAsync();
