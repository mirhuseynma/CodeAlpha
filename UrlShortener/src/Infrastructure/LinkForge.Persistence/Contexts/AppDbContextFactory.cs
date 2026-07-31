using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace LinkForge.Persistence.Contexts;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<AppDbContext>();
        var connectionString = "Host=localhost;Port=5432;Database=linkforge_db;Username=postgres;Password=postgres";

        builder.UseNpgsql(connectionString);

        return new AppDbContext(builder.Options);
    }
}
