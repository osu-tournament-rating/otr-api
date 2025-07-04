using Database;
using Database.Repositories.Implementations;
using Database.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Moq;
using OsuApiClient;
using Serilog.Extensions.Logging;

namespace DWS.Tests.DataFetching.TestFixtures;

/// <summary>
/// Base class for integration tests that require database access with transaction isolation.
/// Provides automatic setup/teardown of database context, transaction management, and common repositories.
/// </summary>
public abstract class IntegrationTestBase : PostgreSqlTestFixture, IAsyncLifetime
{
    /// <summary>
    /// The database context for the test. Automatically created and disposed.
    /// </summary>
    protected OtrContext Context { get; private set; } = null!;

    /// <summary>
    /// Mock for the osu! API client. Automatically created for each test.
    /// </summary>
    protected Mock<IOsuClient> MockOsuClient { get; private set; } = null!;

    /// <summary>
    /// Logger factory for creating loggers in tests.
    /// </summary>
    protected ILoggerFactory LoggerFactory { get; private set; } = null!;

    /// <summary>
    /// Beatmaps repository instance. Automatically created with the test context.
    /// </summary>
    protected IBeatmapsRepository BeatmapsRepository { get; private set; } = null!;

    /// <summary>
    /// Players repository instance. Automatically created with the test context.
    /// </summary>
    protected IPlayersRepository PlayersRepository { get; private set; } = null!;

    private IDbContextTransaction? _transaction;

    /// <summary>
    /// Override this method to perform additional initialization after the base setup.
    /// The context, repositories, and mocks will already be available.
    /// </summary>
    protected virtual Task OnInitializeAsync()
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Override this method to perform additional cleanup before the base teardown.
    /// </summary>
    protected virtual Task OnDisposeAsync()
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Override this method to create additional repositories that your test needs.
    /// Called after the context is created but before OnInitializeAsync.
    /// </summary>
    protected virtual void CreateRepositories()
    {
        // Base implementation creates common repositories.
        // Override to add test-specific repositories.
    }

    async Task IAsyncLifetime.InitializeAsync()
    {
        // Initialize the PostgreSQL container
        await base.InitializeAsync();

        // Set up mocks
        MockOsuClient = new Mock<IOsuClient>();
        LoggerFactory = new SerilogLoggerFactory();

        // Create context and begin transaction for test isolation
        Context = CreateContext();
        _transaction = await Context.Database.BeginTransactionAsync();

        // Create common repositories
        BeatmapsRepository = new BeatmapsRepository(Context);
        PlayersRepository = new PlayersRepository(Context);

        // Allow derived classes to create additional repositories
        CreateRepositories();

        // Allow derived classes to perform additional setup
        await OnInitializeAsync();
    }

    async Task IAsyncLifetime.DisposeAsync()
    {
        // Allow derived classes to perform cleanup
        await OnDisposeAsync();

        // Rollback transaction to ensure test isolation
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
        }

        await Context.DisposeAsync();

        // Dispose the PostgreSQL container
        await base.DisposeAsync();
    }

    /// <summary>
    /// Creates a logger for the specified type using the test's logger factory.
    /// </summary>
    protected ILogger<T> CreateLogger<T>()
    {
        return new Logger<T>(LoggerFactory);
    }
}
