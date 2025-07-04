using API.Messages;
using DWS.Consumers;
using DWS.Services;
using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.Logging;
using Moq;
using Serilog.Extensions.Logging;

namespace DWS.Tests.Consumers;

public class BeatmapFetchConsumerTests
{
    private readonly Mock<IBeatmapsetFetchService> _mockBeatmapsetFetchService;
    private readonly BeatmapFetchConsumer _consumer;
    private readonly ILogger<BeatmapFetchConsumer> _logger;

    public BeatmapFetchConsumerTests()
    {
        _mockBeatmapsetFetchService = new Mock<IBeatmapsetFetchService>();
        var loggerFactory = new SerilogLoggerFactory();
        _logger = new Logger<BeatmapFetchConsumer>(loggerFactory);
        _consumer = new BeatmapFetchConsumer(_logger, _mockBeatmapsetFetchService.Object);
    }

    [Fact]
    public async Task Consume_WhenBeatmapFetchSucceeds_ProcessesSuccessfully()
    {
        // Arrange
        var message = new FetchBeatmapMessage
        {
            BeatmapId = 123,
            CorrelationId = Guid.NewGuid()
        };

        _mockBeatmapsetFetchService
            .Setup(x => x.FetchAndPersistBeatmapsetByBeatmapIdAsync(message.BeatmapId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var harness = new InMemoryTestHarness();
        ConsumerTestHarness<BeatmapFetchConsumer>? consumerHarness = harness.Consumer(() => _consumer);

        await harness.Start();

        try
        {
            // Act
            await harness.InputQueueSendEndpoint.Send(message);

            // Assert
            Assert.True(await harness.Consumed.Any<FetchBeatmapMessage>());
            Assert.True(await consumerHarness.Consumed.Any<FetchBeatmapMessage>());

            // Verify the service was called
            _mockBeatmapsetFetchService.Verify(
                x => x.FetchAndPersistBeatmapsetByBeatmapIdAsync(message.BeatmapId, It.IsAny<CancellationToken>()),
                Times.Once);
        }
        finally
        {
            await harness.Stop();
        }
    }

    [Fact]
    public async Task Consume_WhenBeatmapNotFound_ProcessesSuccessfully()
    {
        // Arrange
        var message = new FetchBeatmapMessage
        {
            BeatmapId = 123,
            CorrelationId = Guid.NewGuid()
        };

        _mockBeatmapsetFetchService
            .Setup(x => x.FetchAndPersistBeatmapsetByBeatmapIdAsync(message.BeatmapId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var harness = new InMemoryTestHarness();
        ConsumerTestHarness<BeatmapFetchConsumer>? consumerHarness = harness.Consumer(() => _consumer);

        await harness.Start();

        try
        {
            // Act
            await harness.InputQueueSendEndpoint.Send(message);

            // Assert
            Assert.True(await harness.Consumed.Any<FetchBeatmapMessage>());
            Assert.True(await consumerHarness.Consumed.Any<FetchBeatmapMessage>());

            // Verify the service was called
            _mockBeatmapsetFetchService.Verify(
                x => x.FetchAndPersistBeatmapsetByBeatmapIdAsync(message.BeatmapId, It.IsAny<CancellationToken>()),
                Times.Once);
        }
        finally
        {
            await harness.Stop();
        }
    }

    [Fact]
    public async Task Consume_WhenServiceThrowsException_PropagatesException()
    {
        // Arrange
        var message = new FetchBeatmapMessage
        {
            BeatmapId = 123,
            CorrelationId = Guid.NewGuid()
        };

        var expectedException = new InvalidOperationException("Test exception");

        _mockBeatmapsetFetchService
            .Setup(x => x.FetchAndPersistBeatmapsetByBeatmapIdAsync(message.BeatmapId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(expectedException);

        var harness = new InMemoryTestHarness();
        ConsumerTestHarness<BeatmapFetchConsumer>? consumerHarness = harness.Consumer(() => _consumer);

        await harness.Start();

        try
        {
            // Act
            await harness.InputQueueSendEndpoint.Send(message);

            // Wait for the message to be consumed
            await Task.Delay(500);

            // Assert - Check that the message was consumed but faulted
            Assert.True(await harness.Consumed.Any<FetchBeatmapMessage>());
            Assert.True(await consumerHarness.Consumed.Any<FetchBeatmapMessage>());

            // The consumer should have faulted
            IReceivedMessage<FetchBeatmapMessage>? consumedMessage = (await consumerHarness.Consumed.SelectAsync<FetchBeatmapMessage>().FirstOrDefault())!;
            Assert.NotNull(consumedMessage.Exception);

            // Verify the service was called
            _mockBeatmapsetFetchService.Verify(
                x => x.FetchAndPersistBeatmapsetByBeatmapIdAsync(message.BeatmapId, It.IsAny<CancellationToken>()),
                Times.Once);
        }
        finally
        {
            await harness.Stop();
        }
    }

    [Fact]
    public async Task Consume_LogsCorrelationIdProperly()
    {
        // Arrange
        var correlationId = Guid.NewGuid();
        var message = new FetchBeatmapMessage
        {
            BeatmapId = 123,
            CorrelationId = correlationId
        };

        _mockBeatmapsetFetchService
            .Setup(x => x.FetchAndPersistBeatmapsetByBeatmapIdAsync(message.BeatmapId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var context = new Mock<ConsumeContext<FetchBeatmapMessage>>();
        context.Setup(x => x.Message).Returns(message);
        context.Setup(x => x.CancellationToken).Returns(CancellationToken.None);

        // Act
        await _consumer.Consume(context.Object);

        // Assert
        _mockBeatmapsetFetchService.Verify(
            x => x.FetchAndPersistBeatmapsetByBeatmapIdAsync(message.BeatmapId, It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
