using Microsoft.EntityFrameworkCore;
using PortfolioAPI.Features.Projects;
using PortfolioAPI.Features.Blogposts;

namespace PortfolioAPI.Data;

public class PortfolioContext : DbContext
{
    public PortfolioContext(DbContextOptions<PortfolioContext> options) : base(options)
    {
        
    }
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<Blogpost> Blogposts => Set<Blogpost>();
}
