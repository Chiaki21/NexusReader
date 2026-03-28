using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using NexusReader.Server.Data;
using NexusReader.Shared.Models;

var builder = WebApplication.CreateBuilder(args);

// 1. Add Controllers
builder.Services.AddControllers();

// 2. Configure Database
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
}

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

// 3. Add Identity Services (ONLY THIS ONE)
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => {
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false; // Added this for extra flexibility
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

// 4. Update CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorApp", policy =>
    {
        policy.WithOrigins("http://localhost:5031") 
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddMemoryCache();
var app = builder.Build();

// 5. Middleware Pipeline
// If you are using HTTP (5087) locally, you might want to comment out HttpsRedirection 
// to avoid the SSL protocol errors we saw earlier.
// app.UseHttpsRedirection(); 

app.UseCors("AllowBlazorApp");

app.UseAuthentication(); 
app.UseAuthorization();

app.MapControllers();

app.Run();