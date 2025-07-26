using DWS.Messages;
using DWS.Services;
using JetBrains.Annotations;
using MassTransit;

namespace DWS.Consumers;

/// <summary>
/// Consumer for processing tournament automation check messages.
/// </summary>
[UsedImplicitly]
public class TournamentAutomationCheckConsumer(
    ILogger<TournamentAutomationCheckConsumer> logger,
    ITournamentAutomationCheckService tournamentAutomationCheckService)
    : IConsumer<ProcessTournamentAutomationCheckMessage>
{
    /// <inheritdoc />
    public async Task Consume(ConsumeContext<ProcessTournamentAutomationCheckMessage> context)
    {
        ProcessTournamentAutomationCheckMessage message = context.Message;

        using (logger.BeginScope(new Dictionary<string, object>
        {
            ["TournamentId"] = message.TournamentId,
            ["CorrelationId"] = context.CorrelationId ?? message.CorrelationId
        }))
        {
            logger.LogInformation("Processing tournament automation check request [Tournament ID: {TournamentId} | Correlation ID: {CorrelationId}]",
                message.TournamentId, context.CorrelationId ?? message.CorrelationId);

            try
            {
                bool success = await tournamentAutomationCheckService.ProcessAutomationChecksAsync(message.TournamentId, message.OverrideVerifiedState);

                if (success)
                {
                    logger.LogInformation("Tournament {TournamentId} passed automation checks", message.TournamentId);
                }
                else
                {
                    logger.LogInformation("Tournament {TournamentId} failed automation checks", message.TournamentId);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to process automation checks for tournament {TournamentId}", message.TournamentId);

                // Re-throw to let MassTransit handle retry logic
                throw;
            }
        }
    }
}
