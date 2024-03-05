using API;
using API.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace APITests;

// https://learn.microsoft.com/en-us/ef/core/testing/testing-with-the-database#creating-seeding-and-managing-a-test-database
public class TestDatabaseFixture : IDisposable
{
    public IConfiguration Configuration { get; private set; }
    public IOptions<ConnectionStringsConfiguration> ConnectionStringsOptions { get; private set; }

    public TestDatabaseFixture()
    {
        // Set up configuration to provide the connection string to DbContext
#if DEBUG
        var configBuilder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
#else
        var configBuilder = new ConfigurationBuilder().AddEnvironmentVariables();
#endif
        Configuration = configBuilder.Build();
        ConnectionStringsOptions = new OptionsWrapper<ConnectionStringsConfiguration>(
            new ConnectionStringsConfiguration
            {
                DefaultConnection = Configuration.GetConnectionString("DefaultConnection")!
            }
        );
    }

    public OtrContext CreateContext()
    {
        var optionsBuilder = new DbContextOptionsBuilder<OtrContext>();
        optionsBuilder.UseNpgsql(Configuration.GetConnectionString("DefaultConnection"));
        return new OtrContext(optionsBuilder.Options, ConnectionStringsOptions);
    }

    public void Dispose() { }
}

[CollectionDefinition("DatabaseCollection")]
public class DatabaseCollection : ICollectionFixture<TestDatabaseFixture>
{
    // This class has no code. It is used to associate the collection fixture with test classes.
    // Test classes that use this collection fixture must be annotated with [Collection("DatabaseCollection")].
}
