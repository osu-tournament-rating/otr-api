using API;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace APITests;

// https://learn.microsoft.com/en-us/ef/core/testing/testing-with-the-database#creating-seeding-and-managing-a-test-database
public class TestDatabaseFixture : IDisposable
{
	public OtrContext Context { get; private set; }
	public IConfiguration Configuration { get; private set; }

	public TestDatabaseFixture()
	{
		// Set up configuration to provide the connection string to DbContext
		var configBuilder = new ConfigurationBuilder().AddJsonFile("apptests.json");
		Configuration = configBuilder.Build();

		// Set up DbContext
		var optionsBuilder = new DbContextOptionsBuilder<OtrContext>();
		optionsBuilder.UseNpgsql(Configuration.GetConnectionString("DefaultConnection"));
		Context = new OtrContext(optionsBuilder.Options, Configuration);
		
		// Apply pending migrations
		Context.Database.Migrate();
	}

	public void Dispose()
	{
		Context?.Dispose();
	}
}

[CollectionDefinition("DatabaseCollection")]
public class DatabaseCollection : ICollectionFixture<TestDatabaseFixture>
{
	// This class has no code. It is used to associate the collection fixture with test classes.
	// Test classes that use this collection fixture must be annotated with [Collection("DatabaseCollection")].
}