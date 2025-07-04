using Database;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;

namespace DWS.Tests.DataFetching.TestFixtures;

/// <summary>
/// Base fixture that manages a PostgreSQL container for testing
/// </summary>
public class PostgreSqlTestFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgresContainer;
    private string ConnectionString => _postgresContainer.GetConnectionString();

    protected PostgreSqlTestFixture()
    {
        _postgresContainer = new PostgreSqlBuilder()
            .WithImage("postgres:17-alpine")
            .WithDatabase("test_db")
            .WithUsername("test_user")
            .WithPassword("test_password")
            .Build();
    }

    public async Task InitializeAsync()
    {
        await _postgresContainer.StartAsync();

        // Create the database schema
        await using OtrContext context = CreateContext();
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        await _postgresContainer.DisposeAsync();
    }

    protected OtrContext CreateContext()
    {
        DbContextOptions<OtrContext> options = new DbContextOptionsBuilder<OtrContext>()
            .UseNpgsql(ConnectionString)
            .UseSnakeCaseNamingConvention()
            .Options;

        return new OtrContext(options);
    }
}
