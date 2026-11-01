using Microsoft.EntityFrameworkCore;
using PortfolioAPI.Features.Projects;
using PortfolioAPI.Features.Blogposts;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace PortfolioAPI.Data;

public class PortfolioContext : IdentityDbContext <IdentityUser>
{
    public PortfolioContext(DbContextOptions<PortfolioContext> options) : base(options)
    {
        
    }
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<Blogpost> Blogposts => Set<Blogpost>();
}
