using Common.Enums.Enums.Verification;
using Database.Entities;
using DataWorkerService.Processors.Resolvers.Interfaces;
using DataWorkerService.Services.Interfaces;

namespace DataWorkerService.Processors.Tournaments;

/// <summary>
/// Processor tasked with fetching data from outside sources for a <see cref="Tournament"/>
/// </summary>
public class TournamentDataProcessor(
    ILogger<TournamentDataProcessor> logger,
    IMatchProcessorResolver matchProcessorResolver,
    IOsuApiDataParserService osuApiDataParserService
) : ProcessorBase<Tournament>(logger)
{
    protected override async Task OnProcessingAsync(Tournament entity, CancellationToken cancellationToken)
    {
        if (entity.ProcessingStatus is not TournamentProcessingStatus.NeedsMatchData)
        {
            logger.LogDebug(
                "Tournament does not require processing [Id: {Id} | Processing Status: {Status}]",
                entity.Id,
                entity.ProcessingStatus
            );

            return;
        }

        await osuApiDataParserService.ProcessBeatmapsAsync(entity.PooledBeatmaps.Select(b => b.OsuId));

        IProcessor<Match> matchDataProcessor = matchProcessorResolver.GetDataProcessor();
        foreach (Match match in entity.Matches)
        {
            await matchDataProcessor.ProcessAsync(match, cancellationToken);
        }

        logger.LogInformation(
            "Tournament data processing summary " +
            "[Matches: {MCnt} | Games: {GCnt} | Beatmaps: {BCnt} | Scores: {SCnt} | Players: {PCnt}]",
            entity.Matches.Count,
            entity.Matches.SelectMany(m => m.Games).Count(),
            entity.PooledBeatmaps.Count,
            entity.Matches.SelectMany(m => m.Games.SelectMany(g => g.Scores)).Count(),
            entity.Matches.SelectMany(m => m.Games.SelectMany(g => g.Scores).Select(s => s.Player)).Distinct().Count()
        );

        // If all matches completed data processing, advance processing status
        if (entity.Matches.All(m => m.ProcessingStatus > MatchProcessingStatus.NeedsData))
        {
            entity.StartTime ??= entity.Matches.DefaultIfEmpty().Min(x => x?.StartTime);
            entity.EndTime ??= entity.Matches.DefaultIfEmpty().Max(x => x?.EndTime);

            entity.ProcessingStatus = TournamentProcessingStatus.NeedsAutomationChecks;
        }
    }
}
