using DWS.Messages;
using DWS.Services;
using JetBrains.Annotations;
using MassTransit;

namespace DWS.Consumers;

[UsedImplicitly]
public class MatchFetchConsumer(
    ILogger<MatchFetchConsumer> logger,
    IMatchFetchService matchFetchService)
    : IConsumer<FetchMatchMessage>
{
    public async Task Consume(ConsumeContext<FetchMatchMessage> context)
    {
        FetchMatchMessage message = context.Message;

        using (logger.BeginScope(new Dictionary<string, object>
        {
            ["OsuMatchId"] = message.OsuMatchId,
            ["CorrelationId"] = context.CorrelationId ?? message.CorrelationId
        }))
        {
            logger.LogInformation("Processing match fetch request [osu! Match ID: {OsuMatchId} | Correlation ID: {CorrelationId}]",
                message.OsuMatchId, context.CorrelationId ?? message.CorrelationId);

            try
            {
                bool success = await matchFetchService.FetchAndPersistMatchAsync(
                    message.OsuMatchId,
                    context.CancellationToken);

                if (success)
                {
                    logger.LogInformation("Successfully processed match {OsuMatchId}", message.OsuMatchId);
                }
                else
                {
                    logger.LogInformation("Failed to process match {OsuMatchId} - likely missing from osu! API", message.OsuMatchId);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to process match {OsuMatchId}", message.OsuMatchId);

                // Re-throw to let MassTransit handle retry logic
                throw;
            }
        }
    }
}
