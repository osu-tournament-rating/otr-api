using API.Messages;
using DWS.Services;
using JetBrains.Annotations;
using MassTransit;

namespace DWS.Consumers;

[UsedImplicitly]
public class BeatmapFetchConsumer(
    ILogger<BeatmapFetchConsumer> logger,
    IBeatmapFetchService beatmapFetchService)
    : IConsumer<FetchBeatmapMessage>
{
    public async Task Consume(ConsumeContext<FetchBeatmapMessage> context)
    {
        FetchBeatmapMessage message = context.Message;

        logger.LogInformation(
            "Received beatmap fetch request for beatmap {BeatmapId} with correlation ID {CorrelationId}",
            message.BeatmapId,
            message.CorrelationId);

        try
        {
            bool success = await beatmapFetchService.FetchAndPersistBeatmapAsync(
                message.BeatmapId,
                context.CancellationToken);

            if (!success)
            {
                logger.LogWarning(
                    "Beatmap {BeatmapId} was not found in osu! API but was processed (Correlation: {CorrelationId})",
                    message.BeatmapId,
                    message.CorrelationId);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "Failed to process beatmap {BeatmapId} (Correlation: {CorrelationId})",
                message.BeatmapId,
                message.CorrelationId);

            // Re-throw to let MassTransit handle retry logic
            throw;
        }
    }
}
