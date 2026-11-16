using Microsoft.AspNetCore.Identity;

namespace PortfolioAPI.Features.Auth;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth").WithTags("Authentication");

        group.MapIdentityApi<IdentityUser>();
    }
}