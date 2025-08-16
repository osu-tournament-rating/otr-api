using Common.Enums;
using Database.Entities;
using DWS.Tests.DataFetching.TestFixtures;
using Microsoft.EntityFrameworkCore;

namespace DWS.Tests.DataFetching;

/// <summary>
/// Tests to verify PostgreSQL container setup is working correctly
/// </summary>
public class PostgreSqlContainerTests : PostgreSqlTestFixture, IAsyncLifetime
{
    private Database.OtrContext _context = null!;

    async Task IAsyncLifetime.InitializeAsync()
    {
        await base.InitializeAsync();
        _context = CreateContext();
    }

    async Task IAsyncLifetime.DisposeAsync()
    {
        _context.Dispose();
        await base.DisposeAsync();
    }

    [Fact]
    public async Task CanCreateAndQueryBeatmap()
    {
        // Arrange
        var beatmap = new Beatmap
        {
            OsuId = 12345,
            DataFetchStatus = DataFetchStatus.Fetched,
            DiffName = "Test Difficulty",
            Sr = 5.5
        };

        // Act
        _context.Beatmaps.Add(beatmap);
        await _context.SaveChangesAsync();

        // Assert
        Beatmap? retrievedBeatmap = await _context.Beatmaps
            .FirstOrDefaultAsync(b => b.OsuId == 12345);

        Assert.NotNull(retrievedBeatmap);
        Assert.Equal("Test Difficulty", retrievedBeatmap.DiffName);
        Assert.Equal(5.5, retrievedBeatmap.Sr);
    }

    [Fact]
    public async Task TransactionIsolation_EnsuresTestsDoNotInterfere()
    {
        // This test runs after CanCreateAndQueryBeatmap
        // but should not see its data due to transaction isolation

        // Act
        int beatmapCount = await _context.Beatmaps.CountAsync();

        // Assert
        Assert.Equal(0, beatmapCount);
    }
}
