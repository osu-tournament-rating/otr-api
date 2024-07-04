using Database.Entities;
using Database.Enums.Verification;

namespace DataWorkerService.Processors.Scores;

/// <summary>
/// Processor tasked with fetching data from outside sources for a <see cref="Tournament"/>
/// </summary>
public class ScoreAutomationChecksProcessor(
    ILogger<ScoreAutomationChecksProcessor> logger
) : ProcessorBase<GameScore>(logger)
{
    protected override async Task OnProcessingAsync(GameScore entity, CancellationToken cancellationToken)
    {
        if (entity.ProcessingStatus is not ScoreProcessingStatus.NeedsAutomationChecks)
        {
            logger.LogDebug(
                "Score does not require processing [Id: {Id} | Processing Status: {Status}]",
                entity.Id,
                entity.ProcessingStatus
            );

            return;
        }


    }
}
