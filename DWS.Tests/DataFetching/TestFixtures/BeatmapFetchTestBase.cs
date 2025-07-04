using System.Linq.Expressions;
using Database;
using DWS.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Moq;
using OsuApiClient;
using Serilog.Extensions.Logging;

namespace DWS.Tests.DataFetching.TestFixtures;

public abstract class BeatmapFetchTestBase : PostgreSqlTestFixture, IAsyncLifetime
{
    protected OtrContext Context { get; private set; } = null!;
    protected Mock<IOsuClient> MockOsuClient { get; private set; } = null!;
    protected BeatmapFetchService Service { get; private set; } = null!;
    private IDbContextTransaction? _transaction;

    async Task IAsyncLifetime.InitializeAsync()
    {
        await base.InitializeAsync();

        // Set up mocks
        MockOsuClient = new Mock<IOsuClient>();

        // Create context and begin transaction for test isolation
        Context = CreateContext();
        _transaction = await Context.Database.BeginTransactionAsync();

        // Create service with the context
        var loggerFactory = new SerilogLoggerFactory();
        ILogger<BeatmapFetchService> logger = new Logger<BeatmapFetchService>(loggerFactory);
        Service = new BeatmapFetchService(logger, Context, MockOsuClient.Object);
    }

    async Task IAsyncLifetime.DisposeAsync()
    {
        // Rollback transaction to ensure test isolation
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
        }

        await Context.DisposeAsync();
        await base.DisposeAsync();
    }

    /// <summary>
    /// Helper method to verify database state after test execution
    /// </summary>
    protected async Task<T?> GetEntityAsync<T>(Expression<Func<T, bool>> predicate) where T : class
    {
        return await Context.Set<T>().FirstOrDefaultAsync(predicate);
    }

    /// <summary>
    /// Helper method to count entities in database
    /// </summary>
    protected async Task<int> CountEntitiesAsync<T>() where T : class
    {
        return await Context.Set<T>().CountAsync();
    }
}
