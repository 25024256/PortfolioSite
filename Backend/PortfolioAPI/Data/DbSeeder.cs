using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PortfolioAPI.Features.Blogposts;
using PortfolioAPI.Features.Projects;

namespace PortfolioAPI.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<PortfolioContext>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

        await context.Database.MigrateAsync();

        // Make roles
        string[] roles = ["Admin", "Editor"];
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        // Creating standard Admin and Editor accounts
        await EnsureUserWithRoleAsync(userManager, "admin@portfolio.nl", "Admin123!", "Admin");
        await EnsureUserWithRoleAsync(userManager, "editor@portfolio.nl", "Editor123!", "Editor");

        // Adding starting data from projectlist.js
        if (!await context.Projects.AnyAsync())
        {
            context.Projects.AddRange(
                new Project
                {
                    Titel = "Trashtag",
                    Beschrijving = "A web application designed to help users report and track litter in their community.",
                    Categorie = "Websites",
                    GitHubUrl = "https://github.com/25024256/TrashTag.git",
                    Datum = new DateTime(2025, 12, 6, 0, 0, 0, DateTimeKind.Utc)
                },
                new Project
                {
                    Titel = "HotelSimulator",
                    Beschrijving = "I completed the HotelSimulator project, which was a project coded in java and a great opportunity to practice my programming skills.",
                    Categorie = "Java",
                    GitHubUrl = "https://github.com/hhs-se-semester-2-onderwijs/klas-3-groep-1.git",
                    Datum = new DateTime(2026, 7, 26, 0, 0, 0, DateTimeKind.Utc)
                },
                new Project
                {
                    Titel = "Game of Life",
                    Beschrijving = "This is a web application that simulates the Game of Life, a cellular automaton devised by mathematician John Conway.",
                    Categorie = "Websites",
                    GitHubUrl = "https://github.com/25024256/Game-of-Life.git",
                    Datum = new DateTime(2026, 6, 30, 0, 0, 0, DateTimeKind.Utc)
                },
                new Project
                {
                    Titel = "Portfolio Website",
                    Beschrijving = "This is my personal portfolio website which I coded myself, which showcases my skills, projects, and experiences as a web developer.",
                    Categorie = "Websites",
                    GitHubUrl = "https://github.com/25024256/PortfolioSite.git",
                    Datum = new DateTime(2026, 9, 9, 0, 0, 0, DateTimeKind.Utc)
                }
            );
        }

        // Adding starting data from blog.html
        if (!await context.Blogposts.AnyAsync())
        {
            context.Blogposts.AddRange(
                new Blogpost
                {
                    Titel = "My First Web Development Project",
                    Inhoud = "I recently completed my first web development project. It was a great learning experience, and I faced many challenges along the way...",
                    Publicatiedatum = new DateTime(2025, 12, 6, 0, 0, 0, DateTimeKind.Utc)
                },
                new Blogpost
                {
                    Titel = "Understanding Semantic HTML",
                    Inhoud = "Today I learned about the importance of using tags like header, main, section, and article. It helps with accessibility and SEO...",
                    Publicatiedatum = new DateTime(2026, 9, 9, 0, 0, 0, DateTimeKind.Utc)
                },
                new Blogpost
                {
                    Titel = "Responsive Design Tips",
                    Inhoud = "I learned about Responsive design and wanted to share some tips and best practices for creating responsive web designs that look great on all devices...",
                    Publicatiedatum = new DateTime(2026, 9, 9, 0, 0, 0, DateTimeKind.Utc)
                }
            );
        }

        await context.SaveChangesAsync();
    }

    private static async Task EnsureUserWithRoleAsync(
        UserManager<IdentityUser> userManager,
        string email,
        string password,
        string role)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
        {
            user = new IdentityUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                return;
            }
        }

        if (!await userManager.IsInRoleAsync(user, role))
        {
            await userManager.AddToRoleAsync(user, role);
        }
    }
}