using Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Repository;

public class RepositoryContext : IdentityDbContext<User>
{
    public RepositoryContext(DbContextOptions<RepositoryContext> options): base(options) {}
    
    // Relations are configured in the entity which has a FK.
    // Entities are configured in "Configurations" folder. 
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(RepositoryContext).Assembly);
    }

    public DbSet<ActionLog> ActionLogs { get; set; }
    public DbSet<Ban> Bans { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<Topic> Topics { get; set; }
    public new DbSet<User> Users { get; set; }
}