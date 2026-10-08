using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace SkyrimWIKI.Data;

// Builds an AppDbContext for the `dotnet ef` command line tools. When they run,
// the web application is not started, so there is no dependency injection
// container; EF Core uses this factory instead. It reads the same
// appsettings.json, so migrations target the provider the app will use.
public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        // No host to build configuration for us, so we do it by hand.
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var options = new DbContextOptionsBuilder<AppDbContext>();
        DatabaseProviderSelector.Configure(options, configuration);

        return new AppDbContext(options.Options);
    }
}
