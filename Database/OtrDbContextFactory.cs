using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Database;

public class OtrDbContextFactory : IDesignTimeDbContextFactory<OtrContext>
{
    public OtrContext CreateDbContext(string[] args)
    {
        try
        {
            // Load the startup project appsettings.json file, not optional.
            // If missing, an exception is raised.
            IConfigurationRoot configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json", false).Build();

            var builder = new DbContextOptionsBuilder<OtrContext>();
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            builder.UseNpgsql(connectionString);

            return new OtrContext(builder.Options);
        }
        catch (Exception)
        {
            // If any errors are encountered (e.g. fetching the appsettings file fails),
            // we want to return a default OtrContext.
            //
            // Without this specific implementation, the deployment will fail
            // with a "Host can't be null" error.
            //
            // With this exception handling, the deployment will default to returning
            // this value - this is enough for generating the migrations script
            // used in deployments.

            return new OtrContext(new DbContextOptionsBuilder<OtrContext>().UseNpgsql().Options);
        }
    }
}
