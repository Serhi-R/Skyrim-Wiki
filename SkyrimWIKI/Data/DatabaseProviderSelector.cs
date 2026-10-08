using Microsoft.EntityFrameworkCore;

namespace SkyrimWIKI.Data;

// Picks the database provider from configuration so switching between SQLite
// and PostgreSQL never requires a C# change. Both the application (Program.cs)
// and the migration tools (DesignTimeDbContextFactory) call this method, so
// they can never disagree.
public static class DatabaseProviderSelector
{
    public static void Configure(DbContextOptionsBuilder options, IConfiguration configuration)
    {
        // SQLite is the default: a fresh project runs with no database server.
        var provider = configuration["Database:Provider"] ?? "Sqlite";

        switch (provider.ToLowerInvariant())
        {
            case "sqlite":
                options.UseSqlite(GetConnectionString(configuration, "Sqlite"));
                break;

            case "postgresql":
                options.UseNpgsql(GetConnectionString(configuration, "PostgreSql"));
                break;

            // Fail loudly: a typo here would surface much later as a confusing
            // error.
            default:
                throw new InvalidOperationException(
                    $"Unknown value \"{provider}\" for Database:Provider in appsettings.json. " +
                    "Expected \"Sqlite\" or \"PostgreSql\".");
        }
    }

    private static string GetConnectionString(IConfiguration configuration, string name)
        => configuration.GetConnectionString(name)
           ?? throw new InvalidOperationException(
               $"Connection string \"{name}\" was not found in appsettings.json.");
}
