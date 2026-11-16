using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using PortfolioAPI.Data;
using PortfolioAPI.Features.Blogposts;
using Xunit;

namespace Tests;

public class BlogpostsTest : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public BlogpostsTest(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    private async Task<HttpClient> CreateAuthenticatedClientAsync(string? role = null)
    {
        var email = $"test_{Guid.NewGuid()}@example.com";
        var password = "TestPassword123!";

        using (var scope = _factory.Services.CreateScope())
        {
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            var user = new IdentityUser { UserName = email, Email = email, EmailConfirmed = true };
            await userManager.CreateAsync(user, password);

            if (!string.IsNullOrEmpty(role))
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
                await userManager.AddToRoleAsync(user, role);
            }
        }

        var authenticatedClient = _factory.CreateClient();
        var loginResponse = await authenticatedClient.PostAsJsonAsync(
            "/api/auth/login?useCookies=true",
            new { email, password }
        );
        loginResponse.EnsureSuccessStatusCode();

        return authenticatedClient;
    }

    private async Task<int> SeedBlogpostAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PortfolioContext>();
        var blogpost = new Blogpost
        {
            Titel = "Seeded Blogpost",
            Inhoud = "Seeded content",
            Publicatiedatum = DateTime.UtcNow
        };
        db.Blogposts.Add(blogpost);
        await db.SaveChangesAsync();
        return blogpost.Id;
    }

    // READ (Public)

    [Fact]
    public async Task GetBlogposts_NotLoggedIn_ReturnsOk()
    {
        // Arrange
        var url = "/api/blogposts";

        // Act
        var response = await _client.GetAsync(url);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetBlogpostById_ExistingId_ReturnsOk()
    {
        // Arrange
        var blogpostId = await SeedBlogpostAsync();
        var url = $"/api/blogposts/{blogpostId}";

        // Act
        var response = await _client.GetAsync(url);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetBlogpostById_NonExistingId_ReturnsNotFound()
    {
        // Arrange
        var url = "/api/blogposts/999999";

        // Act
        var response = await _client.GetAsync(url);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    // UNAUTHENTICATED (401 Unauthorized)

    [Fact]
    public async Task PostBlogpost_NotLoggedIn_ReturnsUnauthorized()
    {
        // Arrange
        var url = "/api/blogposts";
        var newBlogpost = new BlogpostRequestDto("Test Blog", "Blog content", DateTime.UtcNow);

        // Act
        var response = await _client.PostAsJsonAsync(url, newBlogpost);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task PutBlogpost_NotLoggedIn_ReturnsUnauthorized()
    {
        // Arrange
        var url = "/api/blogposts/1";
        var updatedBlogpost = new BlogpostRequestDto("Updated Blog", "New content", DateTime.UtcNow);

        // Act
        var response = await _client.PutAsJsonAsync(url, updatedBlogpost);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task DeleteBlogpost_NotLoggedIn_ReturnsUnauthorized()
    {
        // Arrange
        var url = "/api/blogposts/1";

        // Act
        var response = await _client.DeleteAsync(url);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    // UNAUTHORIZED ROLE (403 Forbidden)

    [Fact]
    public async Task PostBlogpost_LoggedInWithoutEditorRole_ReturnsForbidden()
    {
        // Arrange
        var userClient = await CreateAuthenticatedClientAsync();
        var url = "/api/blogposts";
        var newBlogpost = new BlogpostRequestDto("Test Blog", "Blog content", DateTime.UtcNow);

        // Act
        var response = await userClient.PostAsJsonAsync(url, newBlogpost);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task PutBlogpost_LoggedInWithoutEditorRole_ReturnsForbidden()
    {
        // Arrange
        var userClient = await CreateAuthenticatedClientAsync();
        var url = "/api/blogposts/1";
        var updatedBlogpost = new BlogpostRequestDto("Updated Blog", "New content", DateTime.UtcNow);

        // Act
        var response = await userClient.PutAsJsonAsync(url, updatedBlogpost);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task DeleteBlogpost_LoggedInWithoutEditorRole_ReturnsForbidden()
    {
        // Arrange
        var userClient = await CreateAuthenticatedClientAsync();
        var url = "/api/blogposts/1";

        // Act
        var response = await userClient.DeleteAsync(url);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    // AUTHORIZED EDITOR / ADMIN (201, 204, 404)

    [Fact]
    public async Task PostBlogpost_LoggedInAsEditor_ReturnsCreated()
    {
        // Arrange
        var editorClient = await CreateAuthenticatedClientAsync("Editor");
        var url = "/api/blogposts";
        var newBlogpost = new BlogpostRequestDto("New Blog", "Content", DateTime.UtcNow);

        // Act
        var response = await editorClient.PostAsJsonAsync(url, newBlogpost);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task PutBlogpost_LoggedInAsAdminWithExistingId_ReturnsNoContent()
    {
        // Arrange
        var adminClient = await CreateAuthenticatedClientAsync("Admin");
        var blogpostId = await SeedBlogpostAsync();
        var url = $"/api/blogposts/{blogpostId}";
        var updatedBlogpost = new BlogpostRequestDto("Updated Blog", "Updated Content", DateTime.UtcNow);

        // Act
        var response = await adminClient.PutAsJsonAsync(url, updatedBlogpost);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task PutBlogpost_LoggedInAsEditorWithNonExistingId_ReturnsNotFound()
    {
        // Arrange
        var editorClient = await CreateAuthenticatedClientAsync("Editor");
        var url = "/api/blogposts/999999";
        var updatedBlogpost = new BlogpostRequestDto("Updated Blog", "Updated Content", DateTime.UtcNow);

        // Act
        var response = await editorClient.PutAsJsonAsync(url, updatedBlogpost);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteBlogpost_LoggedInAsAdminWithExistingId_ReturnsNoContent()
    {
        // Arrange
        var adminClient = await CreateAuthenticatedClientAsync("Admin");
        var blogpostId = await SeedBlogpostAsync();
        var url = $"/api/blogposts/{blogpostId}";

        // Act
        var response = await adminClient.DeleteAsync(url);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task DeleteBlogpost_LoggedInAsEditorWithNonExistingId_ReturnsNotFound()
    {
        // Arrange
        var editorClient = await CreateAuthenticatedClientAsync("Editor");
        var url = "/api/blogposts/999999";

        // Act
        var response = await editorClient.DeleteAsync(url);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}