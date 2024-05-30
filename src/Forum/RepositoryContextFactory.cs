using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Repository;

namespace Forum;

/// <summary>
/// This class is used to create the database context for the repository. Used in migrations.
/// </summary>
public class RepositoryContextFactory : IDesignTimeDbContextFactory<RepositoryContext>
{
    /// <summary>
    /// Creates the database context for the repository.
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    public RepositoryContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<RepositoryContext>()
            .UseLazyLoadingProxies()
            .UseSqlServer(Environment.GetEnvironmentVariable("SQLCONFIG") ?? throw new Exception("Connection string not found."),
                b => b.MigrationsAssembly("Migrations"));

        return new RepositoryContext(builder.Options);
    }
}