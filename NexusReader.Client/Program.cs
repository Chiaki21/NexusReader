using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using NexusReader;
using NexusReader.Services;
using NexusReader.Shared.Models;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient 
{ 
    BaseAddress = new Uri("http://localhost:5277/") 
});

builder.Services.AddScoped<AuthStateService>();
builder.Services.AddScoped<UserSettingsService>();
builder.Services.AddSingleton<BookService>();
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:5087/") });

await builder.Build().RunAsync();
