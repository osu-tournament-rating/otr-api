using DWS.Configurations;
using DWS.Services;
using DWS.Services.Implementations;
using DWS.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using OsuApiClient.Enums;
using StackExchange.Redis;

namespace DWS.Tests.Services;

/// <summary>
/// Unit tests for the MessageDeduplicationService
/// </summary>
public class MessageDeduplicationServiceTests
{
    private readonly Mock<IConnectionMultiplexer> _mockRedis;
    private readonly Mock<IDatabase> _mockDatabase;
    private readonly Mock<ILogger<MessageDeduplicationService>> _mockLogger;
    private readonly MessageDeduplicationService _service;
    private static readonly long[] resourceIds = new[] { 1L, 2L, 3L };

    public MessageDeduplicationServiceTests()
    {
        _mockRedis = new Mock<IConnectionMultiplexer>();
        _mockDatabase = new Mock<IDatabase>();
        _mockLogger = new Mock<ILogger<MessageDeduplicationService>>();

        var configuration = new MessageDeduplicationConfiguration
        {
            Enabled = true,
            PlayerFetch = new FetchDeduplicationSettings
            {
                Enabled = true,
                PendingTtlSeconds = 3600,
                ProcessedTtlSeconds = 1800
            },
            PlayerOsuTrackFetch = new FetchDeduplicationSettings
            {
                Enabled = true,
                PendingTtlSeconds = 3600,
                ProcessedTtlSeconds = 1800
            },
            BeatmapFetch = new FetchDeduplicationSettings
            {
                Enabled = true,
                PendingTtlSeconds = 1800,
                ProcessedTtlSeconds = 7200
            },
            MatchFetch = new FetchDeduplicationSettings
            {
                Enabled = true,
                PendingTtlSeconds = 3600,
                ProcessedTtlSeconds = 3600
            }
        };

        _mockRedis.Setup(r => r.GetDatabase(It.IsAny<int>(), It.IsAny<object>()))
            .Returns(_mockDatabase.Object);

        IOptions<MessageDeduplicationConfiguration> options = Options.Create(configuration);
        _service = new MessageDeduplicationService(_mockRedis.Object, options, _mockLogger.Object);
    }

    [Fact]
    public async Task TryReserveFetchAsync_WhenResourceIsAvailable_ReturnsTrue()
    {
        // Arrange
        const long resourceId = 123456;
        const FetchResourceType resourceType = FetchResourceType.Player;
        const FetchPlatform platform = FetchPlatform.Osu;

        _mockDatabase.Setup(db => db.KeyExistsAsync(
                It.Is<RedisKey>(k => k.ToString()!.Contains("processed")),
                It.IsAny<CommandFlags>()))
            .ReturnsAsync(false);

        _mockDatabase.Setup(db => db.StringSetAsync(
                It.Is<RedisKey>(k => k.ToString()!.Contains("pending")),
                It.IsAny<RedisValue>(),
                It.IsAny<TimeSpan?>(),
                When.NotExists,
                It.IsAny<CommandFlags>()))
            .ReturnsAsync(true);

        // Act
        bool result = await _service.TryReserveFetchAsync(resourceType, resourceId, platform);

        // Assert
        Assert.True(result);
        _mockDatabase.Verify(db => db.KeyExistsAsync(
            It.Is<RedisKey>(k => k.ToString()!.Contains("processed")),
            It.IsAny<CommandFlags>()), Times.Once);
        _mockDatabase.Verify(db => db.StringSetAsync(
            It.Is<RedisKey>(k => k.ToString()!.Contains("pending")),
            It.IsAny<RedisValue>(),
            It.IsAny<TimeSpan?>(),
            When.NotExists,
            It.IsAny<CommandFlags>()), Times.Once);
    }

    [Fact]
    public async Task TryReserveFetchAsync_WhenResourceIsRecentlyProcessed_ReturnsFalse()
    {
        // Arrange
        const long resourceId = 123456;
        const FetchResourceType resourceType = FetchResourceType.Player;
        const FetchPlatform platform = FetchPlatform.Osu;

        _mockDatabase.Setup(db => db.KeyExistsAsync(
                It.Is<RedisKey>(k => k.ToString()!.Contains("processed")),
                It.IsAny<CommandFlags>()))
            .ReturnsAsync(true);

        // Act
        bool result = await _service.TryReserveFetchAsync(resourceType, resourceId, platform);

        // Assert
        Assert.False(result);
        _mockDatabase.Verify(db => db.KeyExistsAsync(
            It.Is<RedisKey>(k => k.ToString()!.Contains("processed")),
            It.IsAny<CommandFlags>()), Times.Once);
        _mockDatabase.Verify(db => db.StringSetAsync(
            It.IsAny<RedisKey>(),
            It.IsAny<RedisValue>(),
            It.IsAny<TimeSpan?>(),
            It.IsAny<When>(),
            It.IsAny<CommandFlags>()), Times.Never);
    }

    [Fact]
    public async Task TryReserveFetchAsync_WhenResourceIsAlreadyPending_ReturnsFalse()
    {
        // Arrange
        const long resourceId = 123456;
        const FetchResourceType resourceType = FetchResourceType.Player;
        const FetchPlatform platform = FetchPlatform.Osu;

        _mockDatabase.Setup(db => db.KeyExistsAsync(
                It.Is<RedisKey>(k => k.ToString()!.Contains("processed")),
                It.IsAny<CommandFlags>()))
            .ReturnsAsync(false);

        _mockDatabase.Setup(db => db.StringSetAsync(
                It.Is<RedisKey>(k => k.ToString()!.Contains("pending")),
                It.IsAny<RedisValue>(),
                It.IsAny<TimeSpan?>(),
                When.NotExists,
                It.IsAny<CommandFlags>()))
            .ReturnsAsync(false);

        // Act
        bool result = await _service.TryReserveFetchAsync(resourceType, resourceId, platform);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task MarkFetchCompletedAsync_RemovesPendingAndSetsProcessed()
    {
        // Arrange
        const long resourceId = 123456;
        const FetchResourceType resourceType = FetchResourceType.Beatmap;
        const FetchPlatform platform = FetchPlatform.Osu;

        _mockDatabase.Setup(db => db.KeyDeleteAsync(
                It.Is<RedisKey>(k => k.ToString()!.Contains("pending")),
                It.IsAny<CommandFlags>()))
            .ReturnsAsync(true);

        _mockDatabase.Setup(db => db.StringSetAsync(
                It.Is<RedisKey>(k => k.ToString()!.Contains("processed")),
                It.IsAny<RedisValue>(),
                It.IsAny<TimeSpan?>(),
                It.IsAny<When>(),
                It.IsAny<CommandFlags>()))
            .ReturnsAsync(true);

        // Act
        await _service.MarkFetchCompletedAsync(resourceType, resourceId, platform);

        // Assert
        _mockDatabase.Verify(db => db.KeyDeleteAsync(
            It.Is<RedisKey>(k => k.ToString()!.Contains("pending")),
            It.IsAny<CommandFlags>()), Times.Once);
        _mockDatabase.Verify(db => db.StringSetAsync(
            It.Is<RedisKey>(k => k.ToString()!.Contains("processed")),
            It.IsAny<RedisValue>(),
            It.IsAny<TimeSpan?>(),
            It.IsAny<When>(),
            It.IsAny<CommandFlags>()), Times.Once);
    }

    [Fact]
    public async Task ReleaseFetchAsync_DeletesPendingKey()
    {
        // Arrange
        const long resourceId = 123456;
        const FetchResourceType resourceType = FetchResourceType.Match;
        const FetchPlatform platform = FetchPlatform.Osu;

        _mockDatabase.Setup(db => db.KeyDeleteAsync(
                It.Is<RedisKey>(k => k.ToString()!.Contains("pending")),
                It.IsAny<CommandFlags>()))
            .ReturnsAsync(true);

        // Act
        await _service.ReleaseFetchAsync(resourceType, resourceId, platform);

        // Assert
        _mockDatabase.Verify(db => db.KeyDeleteAsync(
            It.Is<RedisKey>(k => k.ToString()!.Contains("pending")),
            It.IsAny<CommandFlags>()), Times.Once);
    }

    [Fact]
    public async Task GetFetchStatusAsync_WhenPending_ReturnsPending()
    {
        // Arrange
        const long resourceId = 123456;
        const FetchResourceType resourceType = FetchResourceType.Player;
        const FetchPlatform platform = FetchPlatform.OsuTrack;

        _mockDatabase.Setup(db => db.KeyExistsAsync(
                It.Is<RedisKey>(k => k.ToString()!.Contains("pending")),
                It.IsAny<CommandFlags>()))
            .ReturnsAsync(true);

        // Act
        DeduplicationStatus result = await _service.GetFetchStatusAsync(resourceType, resourceId, platform);

        // Assert
        Assert.Equal(DeduplicationStatus.Pending, result);
    }

    [Fact]
    public async Task GetFetchStatusAsync_WhenRecentlyProcessed_ReturnsRecentlyProcessed()
    {
        // Arrange
        const long resourceId = 123456;
        const FetchResourceType resourceType = FetchResourceType.Player;
        const FetchPlatform platform = FetchPlatform.Osu;

        _mockDatabase.Setup(db => db.KeyExistsAsync(
                It.Is<RedisKey>(k => k.ToString()!.Contains("pending")),
                It.IsAny<CommandFlags>()))
            .ReturnsAsync(false);

        _mockDatabase.Setup(db => db.KeyExistsAsync(
                It.Is<RedisKey>(k => k.ToString()!.Contains("processed")),
                It.IsAny<CommandFlags>()))
            .ReturnsAsync(true);

        // Act
        DeduplicationStatus result = await _service.GetFetchStatusAsync(resourceType, resourceId, platform);

        // Assert
        Assert.Equal(DeduplicationStatus.RecentlyProcessed, result);
    }

    [Fact]
    public async Task GetFetchStatusAsync_WhenAvailable_ReturnsAvailable()
    {
        // Arrange
        const long resourceId = 123456;
        const FetchResourceType resourceType = FetchResourceType.Player;
        const FetchPlatform platform = FetchPlatform.Osu;

        _mockDatabase.Setup(db => db.KeyExistsAsync(
                It.IsAny<RedisKey>(),
                It.IsAny<CommandFlags>()))
            .ReturnsAsync(false);

        // Act
        DeduplicationStatus result = await _service.GetFetchStatusAsync(resourceType, resourceId, platform);

        // Assert
        Assert.Equal(DeduplicationStatus.Available, result);
    }

    [Fact]
    public async Task GetBatchFetchStatusAsync_ReturnsCorrectStatuses()
    {
        // Arrange
        var resourceIds = new List<long> { 1, 2, 3 };
        const FetchResourceType resourceType = FetchResourceType.Player;
        const FetchPlatform platform = FetchPlatform.Osu;

        var mockBatch = new Mock<IBatch>();
        _mockDatabase.Setup(db => db.CreateBatch(It.IsAny<object>())).Returns(mockBatch.Object);

        var pendingTasks = new Dictionary<long, TaskCompletionSource<bool>>
        {
            [1] = new TaskCompletionSource<bool>(),
            [2] = new TaskCompletionSource<bool>(),
            [3] = new TaskCompletionSource<bool>()
        };

        var processedTasks = new Dictionary<long, TaskCompletionSource<bool>>
        {
            [1] = new TaskCompletionSource<bool>(),
            [2] = new TaskCompletionSource<bool>(),
            [3] = new TaskCompletionSource<bool>()
        };

        // Setup batch operations to return tasks
        mockBatch.Setup(b => b.KeyExistsAsync(
                It.Is<RedisKey>(k => k.ToString()!.Contains("pending:1")),
                It.IsAny<CommandFlags>()))
            .Returns(pendingTasks[1].Task);

        mockBatch.Setup(b => b.KeyExistsAsync(
                It.Is<RedisKey>(k => k.ToString()!.Contains("pending:2")),
                It.IsAny<CommandFlags>()))
            .Returns(pendingTasks[2].Task);

        mockBatch.Setup(b => b.KeyExistsAsync(
                It.Is<RedisKey>(k => k.ToString()!.Contains("pending:3")),
                It.IsAny<CommandFlags>()))
            .Returns(pendingTasks[3].Task);

        mockBatch.Setup(b => b.KeyExistsAsync(
                It.Is<RedisKey>(k => k.ToString()!.Contains("processed:1")),
                It.IsAny<CommandFlags>()))
            .Returns(processedTasks[1].Task);

        mockBatch.Setup(b => b.KeyExistsAsync(
                It.Is<RedisKey>(k => k.ToString()!.Contains("processed:2")),
                It.IsAny<CommandFlags>()))
            .Returns(processedTasks[2].Task);

        mockBatch.Setup(b => b.KeyExistsAsync(
                It.Is<RedisKey>(k => k.ToString()!.Contains("processed:3")),
                It.IsAny<CommandFlags>()))
            .Returns(processedTasks[3].Task);

        // Set results: 1 is pending, 2 is processed, 3 is available
        pendingTasks[1].SetResult(true);
        pendingTasks[2].SetResult(false);
        pendingTasks[3].SetResult(false);

        processedTasks[1].SetResult(false);
        processedTasks[2].SetResult(true);
        processedTasks[3].SetResult(false);

        // Act
        Dictionary<long, DeduplicationStatus> result = await _service.GetBatchFetchStatusAsync(resourceType, resourceIds, platform);

        // Assert
        Assert.Equal(3, result.Count);
        Assert.Equal(DeduplicationStatus.Pending, result[1]);
        Assert.Equal(DeduplicationStatus.RecentlyProcessed, result[2]);
        Assert.Equal(DeduplicationStatus.Available, result[3]);
    }

    [Fact]
    public async Task TryReserveBatchFetchAsync_ReservesOnlyAvailable()
    {
        // Arrange
        var resourceIds = new List<long> { 1, 2, 3 };
        const FetchResourceType resourceType = FetchResourceType.Match;
        const FetchPlatform platform = FetchPlatform.Osu;

        // Setup GetBatchFetchStatusAsync to return mixed statuses
        var mockBatch = new Mock<IBatch>();
        _mockDatabase.Setup(db => db.CreateBatch(It.IsAny<object>())).Returns(mockBatch.Object);

        // Resource 1: Available
        mockBatch.Setup(b => b.KeyExistsAsync(
                It.Is<RedisKey>(k => k.ToString()!.Contains("pending:1")),
                It.IsAny<CommandFlags>()))
            .Returns(Task.FromResult(false));
        mockBatch.Setup(b => b.KeyExistsAsync(
                It.Is<RedisKey>(k => k.ToString()!.Contains("processed:1")),
                It.IsAny<CommandFlags>()))
            .Returns(Task.FromResult(false));

        // Resource 2: Pending
        mockBatch.Setup(b => b.KeyExistsAsync(
                It.Is<RedisKey>(k => k.ToString()!.Contains("pending:2")),
                It.IsAny<CommandFlags>()))
            .Returns(Task.FromResult(true));

        // Resource 3: Available
        mockBatch.Setup(b => b.KeyExistsAsync(
                It.Is<RedisKey>(k => k.ToString()!.Contains("pending:3")),
                It.IsAny<CommandFlags>()))
            .Returns(Task.FromResult(false));
        mockBatch.Setup(b => b.KeyExistsAsync(
                It.Is<RedisKey>(k => k.ToString()!.Contains("processed:3")),
                It.IsAny<CommandFlags>()))
            .Returns(Task.FromResult(false));

        // Setup reservation attempts
        mockBatch.Setup(b => b.StringSetAsync(
                It.Is<RedisKey>(k => k.ToString()!.Contains("pending:1")),
                It.IsAny<RedisValue>(),
                It.IsAny<TimeSpan?>(),
                When.NotExists,
                It.IsAny<CommandFlags>()))
            .Returns(Task.FromResult(true));

        mockBatch.Setup(b => b.StringSetAsync(
                It.Is<RedisKey>(k => k.ToString()!.Contains("pending:3")),
                It.IsAny<RedisValue>(),
                It.IsAny<TimeSpan?>(),
                When.NotExists,
                It.IsAny<CommandFlags>()))
            .Returns(Task.FromResult(true));

        // Act
        List<long> result = await _service.TryReserveBatchFetchAsync(resourceType, resourceIds, platform);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Contains(1L, result);
        Assert.Contains(3L, result);
        Assert.DoesNotContain(2L, result);
    }

    [Fact]
    public async Task WhenDeduplicationDisabled_AllOperationsSucceed()
    {
        // Arrange
        var disabledConfig = new MessageDeduplicationConfiguration
        {
            Enabled = false, // Globally disabled
            PlayerFetch = new FetchDeduplicationSettings { Enabled = true }
        };

        IOptions<MessageDeduplicationConfiguration> options = Options.Create(disabledConfig);
        var service = new MessageDeduplicationService(_mockRedis.Object, options, _mockLogger.Object);

        // Act
        bool reserveResult = await service.TryReserveFetchAsync(FetchResourceType.Player, 123, FetchPlatform.Osu);
        DeduplicationStatus statusResult = await service.GetFetchStatusAsync(FetchResourceType.Player, 123, FetchPlatform.Osu);
        List<long> batchResult = await service.TryReserveBatchFetchAsync(FetchResourceType.Player, resourceIds, FetchPlatform.Osu);

        // Assert
        Assert.True(reserveResult);
        Assert.Equal(DeduplicationStatus.Available, statusResult);
        Assert.Equal(3, batchResult.Count);

        // Verify no Redis operations were performed
        _mockDatabase.Verify(db => db.KeyExistsAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>()), Times.Never);
        _mockDatabase.Verify(db => db.StringSetAsync(It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), It.IsAny<TimeSpan?>(), It.IsAny<When>(), It.IsAny<CommandFlags>()), Times.Never);
    }

    [Theory]
    [InlineData(FetchResourceType.Player, FetchPlatform.Osu, 3600, 1800)]
    [InlineData(FetchResourceType.Player, FetchPlatform.OsuTrack, 3600, 1800)]
    [InlineData(FetchResourceType.Beatmap, FetchPlatform.Osu, 1800, 7200)]
    [InlineData(FetchResourceType.Match, FetchPlatform.Osu, 3600, 3600)]
    public async Task CorrectTtlsUsedForDifferentResourceTypes(
        FetchResourceType resourceType,
        FetchPlatform platform,
        int expectedPendingTtl,
        int expectedProcessedTtl)
    {
        // Arrange
        const long resourceId = 123456;
        TimeSpan? capturedPendingTtl = null;
        TimeSpan? capturedProcessedTtl = null;

        _mockDatabase.Setup(db => db.KeyExistsAsync(
                It.IsAny<RedisKey>(),
                It.IsAny<CommandFlags>()))
            .ReturnsAsync(false);

        _mockDatabase.Setup(db => db.StringSetAsync(
                It.Is<RedisKey>(k => k.ToString()!.Contains("pending")),
                It.IsAny<RedisValue>(),
                It.IsAny<TimeSpan?>(),
                When.NotExists,
                It.IsAny<CommandFlags>()))
            .Callback<RedisKey, RedisValue, TimeSpan?, When, CommandFlags>((_, _, ttl, _, _) => capturedPendingTtl = ttl)
            .ReturnsAsync(true);

        _mockDatabase.Setup(db => db.StringSetAsync(
                It.Is<RedisKey>(k => k.ToString()!.Contains("processed")),
                It.IsAny<RedisValue>(),
                It.IsAny<TimeSpan?>(),
                It.IsAny<When>(),
                It.IsAny<CommandFlags>()))
            .Callback<RedisKey, RedisValue, TimeSpan?, When, CommandFlags>((_, _, ttl, _, _) => capturedProcessedTtl = ttl)
            .ReturnsAsync(true);

        // Act
        await _service.TryReserveFetchAsync(resourceType, resourceId, platform);
        await _service.MarkFetchCompletedAsync(resourceType, resourceId, platform);

        // Assert
        Assert.NotNull(capturedPendingTtl);
        Assert.NotNull(capturedProcessedTtl);
        Assert.Equal(expectedPendingTtl, capturedPendingTtl.Value.TotalSeconds);
        Assert.Equal(expectedProcessedTtl, capturedProcessedTtl.Value.TotalSeconds);
    }
}
