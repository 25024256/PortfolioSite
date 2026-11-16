using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Tests;

public class AuthTest : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public AuthTest(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Register_ValidCredentials_ReturnsOk()
    {
        // Arrange
        var url = "/api/auth/register";
        var registerRequest = new
        {
            email = $"newuser_{Guid.NewGuid()}@example.com",
            password = "StrongPassword123!"
        };

        // Act
        var response = await _client.PostAsJsonAsync(url, registerRequest);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Register_WeakPassword_ReturnsBadRequest()
    {
        // Arrange
        var url = "/api/auth/register";
        var registerRequest = new
        {
            email = $"weakuser_{Guid.NewGuid()}@example.com",
            password = "123"
        };

        // Act
        var response = await _client.PostAsJsonAsync(url, registerRequest);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Login_ValidCredentialsWithCookies_ReturnsOkAndSetsCookie()
    {
        // Arrange
        var email = $"loginuser_{Guid.NewGuid()}@example.com";
        var password = "StrongPassword123!";

        using (var scope = _factory.Services.CreateScope())
        {
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var user = new IdentityUser { UserName = email, Email = email, EmailConfirmed = true };
            await userManager.CreateAsync(user, password);
        }

        var url = "/api/auth/login?useCookies=true";
        var loginRequest = new { email, password };

        // Act
        var response = await _client.PostAsJsonAsync(url, loginRequest);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(response.Headers.Contains("Set-Cookie"));
    }

    [Fact]
    public async Task Login_InvalidPassword_ReturnsUnauthorized()
    {
        // Arrange
        var email = $"wrongpass_{Guid.NewGuid()}@example.com";
        var correctPassword = "StrongPassword123!";

        using (var scope = _factory.Services.CreateScope())
        {
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var user = new IdentityUser { UserName = email, Email = email, EmailConfirmed = true };
            await userManager.CreateAsync(user, correctPassword);
        }

        var url = "/api/auth/login?useCookies=true";
        var loginRequest = new { email, password = "WrongPassword999!" };

        // Act
        var response = await _client.PostAsJsonAsync(url, loginRequest);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}