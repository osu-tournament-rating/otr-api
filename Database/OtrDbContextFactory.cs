using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Database;

/// <summary>
/// Factory for creating <see cref="OtrContext"/> instances at design time
/// </summary>
public class OtrDbContextFactory : IDesignTimeDbContextFactory<OtrContext>
{
    public OtrContext CreateDbContext(string[] args)
    {
        try
        {
            // Load the startup project appsettings.json or
            // appsettings.Development.json files, not optional.
            // If missing, an exception is raised.
            IConfigurationRoot configuration =
#if DEBUG
                new ConfigurationBuilder().AddJsonFile("appsettings.Development.json", false).Build();
#else
                new ConfigurationBuilder().AddJsonFile("appsettings.json", false).Build();
#endif

            var builder = new DbContextOptionsBuilder<OtrContext>();
            builder
                .UseNpgsql(configuration.GetConnectionString("DefaultConnection"))
                .UseSnakeCaseNamingConvention();

            return new OtrContext(builder.Options);
        }
        catch (Exception)
        {
            /*
             * If any errors are encountered (e.g. fetching the appsettings file fails),
             * we want to return a default OtrContext.
             *
             * Without this specific implementation, the deployment will fail
             * with a "Host can't be null" error.
             *
             * With this exception handling, the deployment will default to returning
             * this value - this is enough for generating the migrations script
             * used in deployments.
             */

            var builder = new DbContextOptionsBuilder<OtrContext>();
            builder
                .UseNpgsql()
                .UseSnakeCaseNamingConvention();
            return new OtrContext(builder.Options);
        }
    }
}
