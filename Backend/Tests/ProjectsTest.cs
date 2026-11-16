using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using PortfolioAPI.Data;
using PortfolioAPI.Features.Projects;
using Xunit;

namespace Tests;

public class ProjectsTest : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public ProjectsTest(WebApplicationFactory<Program> factory)
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

    private async Task<int> SeedProjectAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PortfolioContext>();
        var project = new Project
        {
            Titel = "Seeded Project",
            Beschrijving = "Seeded description",
            Categorie = "Web",
            GitHubUrl = "https://github.com/example/repo",
            Datum = DateTime.UtcNow
        };
        db.Projects.Add(project);
        await db.SaveChangesAsync();
        return project.Id;
    }

    // READ (Public)

    [Fact]
    public async Task GetProjects_NotLoggedIn_ReturnsOk()
    {
        // Arrange
        var url = "/api/projects";

        // Act
        var response = await _client.GetAsync(url);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetProjectById_ExistingId_ReturnsOk()
    {
        // Arrange
        var projectId = await SeedProjectAsync();
        var url = $"/api/projects/{projectId}";

        // Act
        var response = await _client.GetAsync(url);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetProjectById_NonExistingId_ReturnsNotFound()
    {
        // Arrange
        var url = "/api/projects/999999";

        // Act
        var response = await _client.GetAsync(url);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    // UNAUTHENTICATED (401 Unauthorized)

    [Fact]
    public async Task PostProject_NotLoggedIn_ReturnsUnauthorized()
    {
        // Arrange
        var url = "/api/projects";
        var newProject = new ProjectRequestDto("Title", "Desc", "Web", "https://github.com/test", DateTime.UtcNow);

        // Act
        var response = await _client.PostAsJsonAsync(url, newProject);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task PutProject_NotLoggedIn_ReturnsUnauthorized()
    {
        // Arrange
        var url = "/api/projects/1";
        var updatedProject = new ProjectRequestDto("Updated", "Desc", "Web", "https://github.com/test", DateTime.UtcNow);

        // Act
        var response = await _client.PutAsJsonAsync(url, updatedProject);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task DeleteProject_NotLoggedIn_ReturnsUnauthorized()
    {
        // Arrange
        var url = "/api/projects/1";

        // Act
        var response = await _client.DeleteAsync(url);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    // UNAUTHORIZED ROLE (403 Forbidden)

    [Fact]
    public async Task PostProject_LoggedInWithoutEditorRole_ReturnsForbidden()
    {
        // Arrange
        var userClient = await CreateAuthenticatedClientAsync();
        var url = "/api/projects";
        var newProject = new ProjectRequestDto("Title", "Desc", "Web", "https://github.com/test", DateTime.UtcNow);

        // Act
        var response = await userClient.PostAsJsonAsync(url, newProject);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task PutProject_LoggedInWithoutEditorRole_ReturnsForbidden()
    {
        // Arrange
        var userClient = await CreateAuthenticatedClientAsync();
        var url = "/api/projects/1";
        var updatedProject = new ProjectRequestDto("Updated", "Desc", "Web", "https://github.com/test", DateTime.UtcNow);

        // Act
        var response = await userClient.PutAsJsonAsync(url, updatedProject);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task DeleteProject_LoggedInWithoutEditorRole_ReturnsForbidden()
    {
        // Arrange
        var userClient = await CreateAuthenticatedClientAsync();
        var url = "/api/projects/1";

        // Act
        var response = await userClient.DeleteAsync(url);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    // AUTHORIZED EDITOR / ADMIN (201, 204, 404)

    [Fact]
    public async Task PostProject_LoggedInAsEditor_ReturnsCreated()
    {
        // Arrange
        var editorClient = await CreateAuthenticatedClientAsync("Editor");
        var url = "/api/projects";
        var newProject = new ProjectRequestDto("New Project", "Description", "API", "https://github.com/new", DateTime.UtcNow);

        // Act
        var response = await editorClient.PostAsJsonAsync(url, newProject);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task PutProject_LoggedInAsAdminWithExistingId_ReturnsNoContent()
    {
        // Arrange
        var adminClient = await CreateAuthenticatedClientAsync("Admin");
        var projectId = await SeedProjectAsync();
        var url = $"/api/projects/{projectId}";
        var updatedProject = new ProjectRequestDto("Updated Title", "Updated Desc", "Fullstack", "https://github.com/upd", DateTime.UtcNow);

        // Act
        var response = await adminClient.PutAsJsonAsync(url, updatedProject);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task PutProject_LoggedInAsEditorWithNonExistingId_ReturnsNotFound()
    {
        // Arrange
        var editorClient = await CreateAuthenticatedClientAsync("Editor");
        var url = "/api/projects/999999";
        var updatedProject = new ProjectRequestDto("Updated Title", "Updated Desc", "Fullstack", "https://github.com/upd", DateTime.UtcNow);

        // Act
        var response = await editorClient.PutAsJsonAsync(url, updatedProject);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteProject_LoggedInAsAdminWithExistingId_ReturnsNoContent()
    {
        // Arrange
        var adminClient = await CreateAuthenticatedClientAsync("Admin");
        var projectId = await SeedProjectAsync();
        var url = $"/api/projects/{projectId}";

        // Act
        var response = await adminClient.DeleteAsync(url);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task DeleteProject_LoggedInAsEditorWithNonExistingId_ReturnsNotFound()
    {
        // Arrange
        var editorClient = await CreateAuthenticatedClientAsync("Editor");
        var url = "/api/projects/999999";

        // Act
        var response = await editorClient.DeleteAsync(url);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}