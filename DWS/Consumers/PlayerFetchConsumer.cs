using DWS.Messages;
using DWS.Services;
using JetBrains.Annotations;
using MassTransit;

namespace DWS.Consumers;

[UsedImplicitly]
public class PlayerFetchConsumer(ILogger<PlayerFetchConsumer> logger,
    IPlayerFetchService playerFetchService) : IConsumer<FetchPlayerMessage>
{
    public async Task Consume(ConsumeContext<FetchPlayerMessage> context)
    {
        FetchPlayerMessage message = context.Message;

        using (logger.BeginScope(new Dictionary<string, object>
        {
            ["OsuPlayerId"] = message.OsuPlayerId,
            ["CorrelationId"] = context.CorrelationId ?? message.CorrelationId
        }))
        {
            logger.LogInformation("Processing player fetch request [osu! Player ID: {OsuPlayerId} | Correlation ID: {CorrelationId}]",
                message.OsuPlayerId, context.CorrelationId ?? message.CorrelationId);

            try
            {
                bool success = await playerFetchService.FetchAndPersistAsync(message.OsuPlayerId, context.CancellationToken);

                if (success)
                {
                    logger.LogInformation("Successfully processed player {OsuPlayerId}", message.OsuPlayerId);
                }
                else
                {
                    logger.LogInformation("Failed to process player {OsuPlayerId} - likely missing from osu! API", message.OsuPlayerId);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to process player {OsuPlayerId}", message.OsuPlayerId);

                // Re-throw to let MassTransit handle retry logic
                throw;
            }
        }
    }
}
