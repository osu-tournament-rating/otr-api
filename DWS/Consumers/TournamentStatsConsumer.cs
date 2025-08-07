using DWS.Messages;
using DWS.Services;
using DWS.Services.Interfaces;
using JetBrains.Annotations;
using MassTransit;

namespace DWS.Consumers;

/// <summary>
/// Consumer for processing tournament statistics messages.
/// </summary>
[UsedImplicitly]
public class TournamentStatsConsumer(
    ILogger<TournamentStatsConsumer> logger,
    ITournamentStatsService tournamentStatsService)
    : IConsumer<ProcessTournamentStatsMessage>
{
    /// <inheritdoc />
    public async Task Consume(ConsumeContext<ProcessTournamentStatsMessage> context)
    {
        ProcessTournamentStatsMessage message = context.Message;

        using (logger.BeginScope(new Dictionary<string, object>
        {
            ["TournamentId"] = message.TournamentId,
            ["CorrelationId"] = context.CorrelationId ?? message.CorrelationId
        }))
        {
            logger.LogInformation("Processing tournament statistics request [Tournament ID: {TournamentId} | Correlation ID: {CorrelationId}]",
                message.TournamentId, context.CorrelationId ?? message.CorrelationId);

            try
            {
                bool success = await tournamentStatsService.ProcessTournamentStatsAsync(message.TournamentId);

                if (success)
                {
                    logger.LogInformation("Tournament {TournamentId} statistics processed successfully", message.TournamentId);
                }
                else
                {
                    logger.LogWarning("Tournament {TournamentId} statistics processing failed", message.TournamentId);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to process statistics for tournament {TournamentId}", message.TournamentId);

                // Re-throw to let MassTransit handle retry logic
                throw;
            }
        }
    }
}
