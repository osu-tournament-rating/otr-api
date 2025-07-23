using DWS.Messages;
using DWS.Services;
using JetBrains.Annotations;
using MassTransit;

namespace DWS.Consumers;

/// <summary>
/// Consumer for processing score automation check messages.
/// </summary>
[UsedImplicitly]
public class ScoreAutomationCheckConsumer(
    ILogger<ScoreAutomationCheckConsumer> logger,
    IScoreAutomationCheckService scoreAutomationCheckService)
    : IConsumer<ProcessScoreAutomationCheckMessage>
{
    /// <inheritdoc />
    public async Task Consume(ConsumeContext<ProcessScoreAutomationCheckMessage> context)
    {
        ProcessScoreAutomationCheckMessage message = context.Message;

        using (logger.BeginScope(new Dictionary<string, object>
        {
            ["ScoreId"] = message.ScoreId,
            ["CorrelationId"] = message.CorrelationId
        }))
        {
            logger.LogInformation("Processing score automation check request [Score ID: {ScoreId} | Correlation ID: {CorrelationId}]",
                message.ScoreId, message.CorrelationId);

            try
            {
                bool success = await scoreAutomationCheckService.ProcessAutomationChecksAsync(
                    message.ScoreId,
                    context.CancellationToken);

                if (success)
                {
                    logger.LogInformation("Score {ScoreId} passed automation checks", message.ScoreId);
                }
                else
                {
                    logger.LogInformation("Score {ScoreId} failed automation checks", message.ScoreId);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to process automation checks for score {ScoreId}", message.ScoreId);

                // Re-throw to let MassTransit handle retry logic
                throw;
            }
        }
    }
}
