using Common.Enums;
using DWS.Consumers;
using DWS.Messages;
using DWS.Services;
using DWS.Services.Interfaces;
using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;

namespace DWS.Tests.Consumers;

public class MatchFetchConsumerTests
{
    private readonly Mock<ILogger<MatchFetchConsumer>> _loggerMock;
    private readonly Mock<IMatchFetchService> _matchFetchServiceMock;
    private readonly MatchFetchConsumer _consumer;

    public MatchFetchConsumerTests()
    {
        _loggerMock = new Mock<ILogger<MatchFetchConsumer>>();
        _matchFetchServiceMock = new Mock<IMatchFetchService>();
        _consumer = new MatchFetchConsumer(_loggerMock.Object, _matchFetchServiceMock.Object);
    }

    [Fact]
    public async Task Consume_ShouldProcessMatchSuccessfully_WhenMatchExists()
    {
        // Arrange
        var message = new FetchMatchMessage
        {
            OsuMatchId = 123456,
            CorrelationId = Guid.NewGuid(),
            Priority = MessagePriority.Normal
        };

        var context = Mock.Of<ConsumeContext<FetchMatchMessage>>(ctx =>
            ctx.Message == message &&
            ctx.CancellationToken == CancellationToken.None);

        _matchFetchServiceMock
            .Setup(x => x.FetchAndPersistMatchAsync(message.OsuMatchId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        await _consumer.Consume(context);

        // Assert
        _matchFetchServiceMock.Verify(x => x.FetchAndPersistMatchAsync(
            message.OsuMatchId,
            It.IsAny<CancellationToken>()), Times.Once);

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Successfully processed match")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task Consume_ShouldLogWarning_WhenMatchNotFoundInApi()
    {
        // Arrange
        var message = new FetchMatchMessage
        {
            OsuMatchId = 999999,
            CorrelationId = Guid.NewGuid(),
            Priority = MessagePriority.High
        };

        var context = Mock.Of<ConsumeContext<FetchMatchMessage>>(ctx =>
            ctx.Message == message &&
            ctx.CancellationToken == CancellationToken.None);

        _matchFetchServiceMock
            .Setup(x => x.FetchAndPersistMatchAsync(message.OsuMatchId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        await _consumer.Consume(context);

        // Assert
        _matchFetchServiceMock.Verify(x => x.FetchAndPersistMatchAsync(
            message.OsuMatchId,
            It.IsAny<CancellationToken>()), Times.Once);

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("likely missing from osu! API")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task Consume_ShouldRethrowException_WhenServiceThrows()
    {
        // Arrange
        var message = new FetchMatchMessage
        {
            OsuMatchId = 123456,
            CorrelationId = Guid.NewGuid(),
            Priority = MessagePriority.Low
        };

        var context = Mock.Of<ConsumeContext<FetchMatchMessage>>(ctx =>
            ctx.Message == message &&
            ctx.CancellationToken == CancellationToken.None);

        var expectedException = new InvalidOperationException("API error");

        _matchFetchServiceMock
            .Setup(x => x.FetchAndPersistMatchAsync(message.OsuMatchId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(expectedException);

        // Act & Assert
        var actualException = await Assert.ThrowsAsync<InvalidOperationException>(() => _consumer.Consume(context));
        Assert.Equal(expectedException.Message, actualException.Message);

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Failed to process match")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task Consume_ShouldRespectCancellationToken()
    {
        // Arrange
        var message = new FetchMatchMessage
        {
            OsuMatchId = 123456,
            CorrelationId = Guid.NewGuid()
        };

        var cancellationTokenSource = new CancellationTokenSource();
        var context = Mock.Of<ConsumeContext<FetchMatchMessage>>(ctx =>
            ctx.Message == message &&
            ctx.CancellationToken == cancellationTokenSource.Token);

        _matchFetchServiceMock
            .Setup(x => x.FetchAndPersistMatchAsync(message.OsuMatchId, cancellationTokenSource.Token))
            .ReturnsAsync(true);

        // Act
        await _consumer.Consume(context);

        // Assert
        _matchFetchServiceMock.Verify(x => x.FetchAndPersistMatchAsync(
            message.OsuMatchId,
            cancellationTokenSource.Token), Times.Once);
    }
}
