using API.Entities;
using API.Repositories.Interfaces;

namespace API.BackgroundWorkers;

/// <summary>
///  Scans and fixes any match duplicates.
/// </summary>
public class MatchDuplicateDataWorker(
    ILogger<MatchDuplicateDataWorker> logger,
    IServiceProvider serviceProvider
    ) : BackgroundService
{
    private readonly ILogger<MatchDuplicateDataWorker> _logger = logger;
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    private readonly TimeSpan _interval = TimeSpan.FromMinutes(10);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken) =>
        await BackgroundTask(stoppingToken);

    private async Task BackgroundTask(CancellationToken stoppingToken)
    {
        // Periodically checks the database for duplicate matches
        // Any duplicates found will be marked for manual review
        while (!stoppingToken.IsCancellationRequested)
        {
            using IServiceScope scope = _serviceProvider.CreateScope();
            IMatchesRepository matchesRepository = scope.ServiceProvider.GetRequiredService<IMatchesRepository>();
            IMatchDuplicateRepository duplicateRepository = scope.ServiceProvider.GetRequiredService<IMatchDuplicateRepository>();
            var duplicateGroups = (await matchesRepository.GetDuplicateGroupsAsync()).ToList();
            var created = 0;

            if (duplicateGroups.Count > 0)
            {
                _logger.LogInformation("Identified {Count} duplicate groups", duplicateGroups.Count);

                foreach (IList<Match>? duplicateGroup in duplicateGroups)
                {
                    Match root = duplicateGroup.OrderBy(x => x.StartTime).First();
                    duplicateGroup.Remove(root);

                    foreach (Match? remainingDuplicate in duplicateGroup)
                    {
                        var duplicate = new MatchDuplicate
                        {
                            MatchId = remainingDuplicate.Id,
                            OsuMatchId = remainingDuplicate.MatchId,
                            SuspectedDuplicateOf = root.Id
                        };

                        await duplicateRepository.CreateAsync(duplicate);
                        created++;
                    }
                }
            }

            if (created > 0)
            {
                _logger.LogInformation("Created {Count} duplicate matches (awaiting manual review)", created);
            }

            await Task.Delay(_interval, stoppingToken);
        }
    }
}
