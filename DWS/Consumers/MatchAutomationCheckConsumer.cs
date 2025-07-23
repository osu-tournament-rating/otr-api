using DWS.Messages;
using DWS.Services;
using JetBrains.Annotations;
using MassTransit;

namespace DWS.Consumers;

/// <summary>
/// Consumer for processing match automation check messages.
/// </summary>
[UsedImplicitly]
public class MatchAutomationCheckConsumer(
    ILogger<MatchAutomationCheckConsumer> logger,
    IMatchAutomationCheckService matchAutomationCheckService)
    : IConsumer<ProcessMatchAutomationCheckMessage>
{
    /// <inheritdoc />
    public async Task Consume(ConsumeContext<ProcessMatchAutomationCheckMessage> context)
    {
        ProcessMatchAutomationCheckMessage message = context.Message;

        using (logger.BeginScope(new Dictionary<string, object>
        {
            ["MatchId"] = message.MatchId,
            ["CorrelationId"] = message.CorrelationId
        }))
        {
            logger.LogInformation("Processing match automation check request [Match ID: {MatchId} | Correlation ID: {CorrelationId}]",
                message.MatchId, message.CorrelationId);

            try
            {
                bool success = await matchAutomationCheckService.ProcessAutomationChecksAsync(message.MatchId);

                if (success)
                {
                    logger.LogInformation("Match {MatchId} passed automation checks", message.MatchId);
                }
                else
                {
                    logger.LogInformation("Match {MatchId} failed automation checks", message.MatchId);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to process automation checks for match {MatchId}", message.MatchId);

                // Re-throw to let MassTransit handle retry logic
                throw;
            }
        }
    }
}
