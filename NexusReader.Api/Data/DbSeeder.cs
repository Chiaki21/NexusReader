using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NexusReader.Server.Data;
using NexusReader.Shared.Models;

namespace NexusReader.Api.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();

            foreach (var role in new[] { "Admin", "User" })
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            var adminEmail = config["Seed:AdminEmail"];
            var adminPassword = config["Seed:AdminPassword"];
            if (!string.IsNullOrWhiteSpace(adminEmail) && !string.IsNullOrWhiteSpace(adminPassword))
            {
                var admin = await userManager.FindByEmailAsync(adminEmail);
                if (admin == null)
                {
                    admin = new ApplicationUser
                    {
                        UserName = adminEmail,
                        Email = adminEmail,
                        EmailConfirmed = true,
                        FirstName = "Admin",
                        LastName = "User",
                        IsOver18 = true
                    };
                    var result = await userManager.CreateAsync(admin, adminPassword);
                    if (result.Succeeded)
                        await userManager.AddToRoleAsync(admin, "Admin");
                }
                else if (!await userManager.IsInRoleAsync(admin, "Admin"))
                    await userManager.AddToRoleAsync(admin, "Admin");
            }

            if (!await context.Books.AnyAsync())
            {
                var book = new BookModel
                {
                    Title = "Sample: The Midnight Library",
                    Author = "Matt Haig",
                    Description = "Between life and death there is a library, and within that library, the shelves go on forever.",
                    Category = "Fiction",
                    ColorTheme = "slate",
                    CoverImageUrl = null,
                    UploadDate = DateTime.UtcNow,
                    Progress = 0,
                    Chapters = new List<ChapterModel>
                    {
                        new()
                        {
                            ChapterNumber = 1,
                            Title = "Chapter One",
                            Content = "<p>The universe is a library of lives. Every book is a possibility you could have lived.</p>"
                        },
                        new()
                        {
                            ChapterNumber = 2,
                            Title = "Chapter Two",
                            Content = "<p>She opened the next volume and the room shifted around her, soft as breath.</p>"
                        }
                    }
                };
                context.Books.Add(book);
                await context.SaveChangesAsync();
            }
        }
    }
}
