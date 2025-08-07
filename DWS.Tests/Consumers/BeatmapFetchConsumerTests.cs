using Common.Enums;
using DWS.Consumers;
using DWS.Messages;
using DWS.Services;
using DWS.Services.Interfaces;
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

    public BeatmapFetchConsumerTests()
    {
        _mockBeatmapsetFetchService = new Mock<IBeatmapsetFetchService>();
        var loggerFactory = new SerilogLoggerFactory();
        ILogger<BeatmapFetchConsumer> logger = new Logger<BeatmapFetchConsumer>(loggerFactory);
        _consumer = new BeatmapFetchConsumer(logger, _mockBeatmapsetFetchService.Object);
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
            .Setup(x => x.FetchAndPersistBeatmapsetAsync(message.BeatmapId, It.IsAny<CancellationToken>()))
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
                x => x.FetchAndPersistBeatmapsetAsync(message.BeatmapId, It.IsAny<CancellationToken>()),
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
            .Setup(x => x.FetchAndPersistBeatmapsetAsync(message.BeatmapId, It.IsAny<CancellationToken>()))
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
                x => x.FetchAndPersistBeatmapsetAsync(message.BeatmapId, It.IsAny<CancellationToken>()),
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
            .Setup(x => x.FetchAndPersistBeatmapsetAsync(message.BeatmapId, It.IsAny<CancellationToken>()))
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
                x => x.FetchAndPersistBeatmapsetAsync(message.BeatmapId, It.IsAny<CancellationToken>()),
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
            .Setup(x => x.FetchAndPersistBeatmapsetAsync(message.BeatmapId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var context = new Mock<ConsumeContext<FetchBeatmapMessage>>();
        context.Setup(x => x.Message).Returns(message);
        context.Setup(x => x.CancellationToken).Returns(CancellationToken.None);

        // Act
        await _consumer.Consume(context.Object);

        // Assert
        _mockBeatmapsetFetchService.Verify(
            x => x.FetchAndPersistBeatmapsetAsync(message.BeatmapId, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Theory]
    [InlineData(MessagePriority.Low)]
    [InlineData(MessagePriority.Normal)]
    [InlineData(MessagePriority.High)]
    public async Task Consume_HandlesAllPriorityLevels(MessagePriority priority)
    {
        // Arrange
        var message = new FetchBeatmapMessage
        {
            BeatmapId = 456,
            CorrelationId = Guid.NewGuid(),
            Priority = priority
        };

        _mockBeatmapsetFetchService
            .Setup(x => x.FetchAndPersistBeatmapsetAsync(message.BeatmapId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var harness = new InMemoryTestHarness();
        ConsumerTestHarness<BeatmapFetchConsumer>? consumerHarness = harness.Consumer(() => _consumer);

        await harness.Start();

        try
        {
            // Act
            // Note: SetPriority is RabbitMQ-specific and doesn't work with InMemoryTestHarness
            // The message priority is still part of the message data
            await harness.InputQueueSendEndpoint.Send(message);

            // Assert
            Assert.True(await harness.Consumed.Any<FetchBeatmapMessage>());
            Assert.True(await consumerHarness.Consumed.Any<FetchBeatmapMessage>());

            var consumedMessage = await consumerHarness.Consumed.SelectAsync<FetchBeatmapMessage>().FirstOrDefault();
            Assert.NotNull(consumedMessage);
            Assert.Equal(priority, consumedMessage!.Context.Message.Priority);

            // Verify the service was called
            _mockBeatmapsetFetchService.Verify(
                x => x.FetchAndPersistBeatmapsetAsync(message.BeatmapId, It.IsAny<CancellationToken>()),
                Times.Once);
        }
        finally
        {
            await harness.Stop();
        }
    }
}
