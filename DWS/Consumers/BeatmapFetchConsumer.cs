using DWS.Messages;
using DWS.Services;
using JetBrains.Annotations;
using MassTransit;

namespace DWS.Consumers;

[UsedImplicitly]
public class BeatmapFetchConsumer(
    ILogger<BeatmapFetchConsumer> logger,
    IBeatmapsetFetchService beatmapsetFetchService)
    : IConsumer<FetchBeatmapMessage>
{
    public async Task Consume(ConsumeContext<FetchBeatmapMessage> context)
    {
        FetchBeatmapMessage message = context.Message;

        using (logger.BeginScope(new Dictionary<string, object>
        {
            ["BeatmapId"] = message.BeatmapId,
            ["CorrelationId"] = message.CorrelationId
        }))
        {
            logger.LogInformation("Processing beatmap fetch request [osu! ID: {OsuId} | Correlation ID: {CorrelationID}]",
                message.BeatmapId, message.CorrelationId);

            try
            {
                bool success = await beatmapsetFetchService.FetchAndPersistBeatmapsetAsync(
                    message.BeatmapId,
                    context.CancellationToken);

                if (success)
                {
                    logger.LogInformation("Successfully processed beatmap {BeatmapId}", message.BeatmapId);
                }
                else
                {
                    logger.LogInformation("Beatmap {BeatmapId} not found in osu! API", message.BeatmapId);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to process beatmap {BeatmapId}", message.BeatmapId);

                // Re-throw to let MassTransit handle retry logic
                throw;
            }
        }
    }
}
