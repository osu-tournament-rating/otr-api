using DWS.Messages;
using DWS.Services;
using JetBrains.Annotations;
using MassTransit;

namespace DWS.Consumers;

/// <summary>
/// Consumer for processing game automation check messages.
/// </summary>
[UsedImplicitly]
public class GameAutomationCheckConsumer(
    ILogger<GameAutomationCheckConsumer> logger,
    IGameAutomationCheckService gameAutomationCheckService)
    : IConsumer<ProcessGameAutomationCheckMessage>
{
    /// <inheritdoc />
    public async Task Consume(ConsumeContext<ProcessGameAutomationCheckMessage> context)
    {
        ProcessGameAutomationCheckMessage message = context.Message;

        using (logger.BeginScope(new Dictionary<string, object>
        {
            ["GameId"] = message.GameId,
            ["CorrelationId"] = message.CorrelationId
        }))
        {
            logger.LogInformation("Processing game automation check request [Game ID: {GameId} | Correlation ID: {CorrelationId}]",
                message.GameId, message.CorrelationId);

            try
            {
                bool success = await gameAutomationCheckService.ProcessAutomationChecksAsync(message.GameId, message.OverrideVerifiedState);

                if (success)
                {
                    logger.LogInformation("Game {GameId} passed automation checks", message.GameId);
                }
                else
                {
                    logger.LogInformation("Game {GameId} failed automation checks", message.GameId);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to process automation checks for game {GameId}", message.GameId);

                // Re-throw to let MassTransit handle retry logic
                throw;
            }
        }
    }
}
