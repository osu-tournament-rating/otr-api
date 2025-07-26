using DWS.Messages;
using DWS.Services;
using JetBrains.Annotations;
using MassTransit;

namespace DWS.Consumers;

[UsedImplicitly]
public class PlayerOsuTrackFetchConsumer(ILogger<PlayerOsuTrackFetchConsumer> logger,
    IPlayerOsuTrackFetchService playerOsuTrackFetchService) : IConsumer<FetchPlayerOsuTrackMessage>
{
    public async Task Consume(ConsumeContext<FetchPlayerOsuTrackMessage> context)
    {
        FetchPlayerOsuTrackMessage message = context.Message;

        using (logger.BeginScope(new Dictionary<string, object>
        {
            ["OsuPlayerId"] = message.OsuPlayerId,
            ["CorrelationId"] = context.CorrelationId ?? message.CorrelationId
        }))
        {
            logger.LogInformation("Processing osu!track fetch request [osu! Player ID: {OsuPlayerId} | Correlation ID: {CorrelationId}]",
                message.OsuPlayerId, context.CorrelationId ?? message.CorrelationId);

            try
            {
                bool success = await playerOsuTrackFetchService.FetchAndPersistAsync(
                    message.OsuPlayerId,
                    context.CancellationToken);

                if (success)
                {
                    logger.LogInformation("Successfully processed osu!track data for player {OsuPlayerId}",
                        message.OsuPlayerId);
                }
                else
                {
                    logger.LogInformation("Failed to process osu!track data for player {OsuPlayerId} - no data available or player not found",
                        message.OsuPlayerId);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to process osu!track data for player {OsuPlayerId}", message.OsuPlayerId);

                // Re-throw to let MassTransit handle retry logic
                throw;
            }
        }
    }
}
