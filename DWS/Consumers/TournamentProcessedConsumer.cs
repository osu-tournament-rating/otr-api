using Database.Entities;
using Database.Repositories.Interfaces;
using DWS.Messages;
using DWS.Services;
using DWS.Services.Interfaces;
using JetBrains.Annotations;
using MassTransit;

namespace DWS.Consumers;

/// <summary>
/// Consumer for handling tournament processed messages from the otr-processor.
/// Validates the tournament and triggers stats generation when a tournament has been processed.
/// </summary>
[UsedImplicitly]
public class TournamentProcessedConsumer(
    ILogger<TournamentProcessedConsumer> logger,
    ITournamentsRepository tournamentsRepository,
    ITournamentStatsService tournamentStatsService)
    : IConsumer<TournamentProcessedMessage>
{
    /// <inheritdoc />
    public async Task Consume(ConsumeContext<TournamentProcessedMessage> context)
    {
        TournamentProcessedMessage message = context.Message;

        using (logger.BeginScope(new Dictionary<string, object>
        {
            ["TournamentId"] = message.TournamentId,
            ["CorrelationId"] = context.CorrelationId ?? message.CorrelationId,
            ["Action"] = message.Action,
            ["ProcessedAt"] = message.ProcessedAt
        }))
        {
            logger.LogInformation(
                "Processing tournament {TournamentId} [Action: {Action}]",
                message.TournamentId, message.Action);

            try
            {
                // Validate the tournament exists
                Tournament? tournament = await tournamentsRepository.GetAsync(message.TournamentId);
                if (tournament == null)
                {
                    logger.LogWarning(
                        "Tournament {TournamentId} not found. Skipping stats generation [Action: {Action}]",
                        message.TournamentId, message.Action);
                    return;
                }

                // Trigger stats generation for the tournament
                bool success = await tournamentStatsService.ProcessTournamentStatsAsync(message.TournamentId);

                if (!success)
                {
                    logger.LogWarning(
                        "Tournament {TournamentId} statistics processing returned false [Action: {Action}]",
                        message.TournamentId, message.Action);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex,
                    "Failed to process statistics for tournament {TournamentId} [Action: {Action} | Processed At: {ProcessedAt}]",
                    message.TournamentId, message.Action, message.ProcessedAt);

                // Re-throw to let MassTransit handle retry logic
                throw;
            }
        }
    }
}
