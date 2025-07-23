using Common.Enums;
using DWS.Consumers;
using DWS.Messages;
using DWS.Services;
using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;

namespace DWS.Tests.Consumers;

public class ScoreAutomationCheckConsumerTests
{
    private readonly Mock<ILogger<ScoreAutomationCheckConsumer>> _loggerMock;
    private readonly Mock<IScoreAutomationCheckService> _scoreAutomationCheckServiceMock;
    private readonly ScoreAutomationCheckConsumer _consumer;

    public ScoreAutomationCheckConsumerTests()
    {
        _loggerMock = new Mock<ILogger<ScoreAutomationCheckConsumer>>();
        _scoreAutomationCheckServiceMock = new Mock<IScoreAutomationCheckService>();
        _consumer = new ScoreAutomationCheckConsumer(_loggerMock.Object, _scoreAutomationCheckServiceMock.Object);
    }

    [Fact]
    public async Task Consume_ShouldProcessScoreSuccessfully_WhenScorePassesAutomationChecks()
    {
        // Arrange
        var message = new ProcessScoreAutomationCheckMessage
        {
            ScoreId = 123456,
            CorrelationId = Guid.NewGuid(),
            Priority = MessagePriority.Normal
        };

        ConsumeContext<ProcessScoreAutomationCheckMessage> context = Mock.Of<ConsumeContext<ProcessScoreAutomationCheckMessage>>(ctx =>
            ctx.Message == message &&
            ctx.CancellationToken == CancellationToken.None);

        _scoreAutomationCheckServiceMock
            .Setup(x => x.ProcessAutomationChecksAsync(message.ScoreId, It.IsAny<bool>()))
            .ReturnsAsync(true);

        // Act
        await _consumer.Consume(context);

        // Assert
        _scoreAutomationCheckServiceMock.Verify(x => x.ProcessAutomationChecksAsync(
            message.ScoreId, It.IsAny<bool>()), Times.Once);

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("passed automation checks")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task Consume_ShouldLogFailure_WhenScoreFailsAutomationChecks()
    {
        // Arrange
        var message = new ProcessScoreAutomationCheckMessage
        {
            ScoreId = 999999,
            CorrelationId = Guid.NewGuid(),
            Priority = MessagePriority.High
        };

        ConsumeContext<ProcessScoreAutomationCheckMessage> context = Mock.Of<ConsumeContext<ProcessScoreAutomationCheckMessage>>(ctx =>
            ctx.Message == message &&
            ctx.CancellationToken == CancellationToken.None);

        _scoreAutomationCheckServiceMock
            .Setup(x => x.ProcessAutomationChecksAsync(message.ScoreId, It.IsAny<bool>()))
            .ReturnsAsync(false);

        // Act
        await _consumer.Consume(context);

        // Assert
        _scoreAutomationCheckServiceMock.Verify(x => x.ProcessAutomationChecksAsync(
            message.ScoreId, It.IsAny<bool>()), Times.Once);

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("failed automation checks")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task Consume_ShouldRethrowException_WhenServiceThrows()
    {
        // Arrange
        var message = new ProcessScoreAutomationCheckMessage
        {
            ScoreId = 123456,
            CorrelationId = Guid.NewGuid(),
            Priority = MessagePriority.Low
        };

        ConsumeContext<ProcessScoreAutomationCheckMessage> context = Mock.Of<ConsumeContext<ProcessScoreAutomationCheckMessage>>(ctx =>
            ctx.Message == message &&
            ctx.CancellationToken == CancellationToken.None);

        var expectedException = new InvalidOperationException("Database error");

        _scoreAutomationCheckServiceMock
            .Setup(x => x.ProcessAutomationChecksAsync(message.ScoreId, It.IsAny<bool>()))
            .ThrowsAsync(expectedException);

        // Act & Assert
        InvalidOperationException actualException = await Assert.ThrowsAsync<InvalidOperationException>(() => _consumer.Consume(context));
        Assert.Equal(expectedException.Message, actualException.Message);

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Failed to process automation checks for score")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task Consume_ShouldRespectCancellationToken()
    {
        // Arrange
        var message = new ProcessScoreAutomationCheckMessage
        {
            ScoreId = 123456,
            CorrelationId = Guid.NewGuid()
        };

        var cancellationTokenSource = new CancellationTokenSource();
        ConsumeContext<ProcessScoreAutomationCheckMessage> context = Mock.Of<ConsumeContext<ProcessScoreAutomationCheckMessage>>(ctx =>
            ctx.Message == message &&
            ctx.CancellationToken == cancellationTokenSource.Token);

        _scoreAutomationCheckServiceMock
            .Setup(x => x.ProcessAutomationChecksAsync(message.ScoreId, It.IsAny<bool>()))
            .ReturnsAsync(true);

        // Act
        await _consumer.Consume(context);

        // Assert
        _scoreAutomationCheckServiceMock.Verify(x => x.ProcessAutomationChecksAsync(
            message.ScoreId, It.IsAny<bool>()), Times.Once);
    }

    [Fact]
    public async Task Consume_ShouldLogWithCorrelationId()
    {
        // Arrange
        var correlationId = Guid.NewGuid();
        var message = new ProcessScoreAutomationCheckMessage
        {
            ScoreId = 123456,
            CorrelationId = correlationId,
            Priority = MessagePriority.Normal
        };

        ConsumeContext<ProcessScoreAutomationCheckMessage> context = Mock.Of<ConsumeContext<ProcessScoreAutomationCheckMessage>>(ctx =>
            ctx.Message == message &&
            ctx.CancellationToken == CancellationToken.None);

        _scoreAutomationCheckServiceMock
            .Setup(x => x.ProcessAutomationChecksAsync(message.ScoreId, It.IsAny<bool>()))
            .ReturnsAsync(true);

        // Act
        await _consumer.Consume(context);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains(correlationId.ToString())),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.AtLeastOnce);
    }
}
