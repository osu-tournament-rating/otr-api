using API.Entities;
using API.Repositories.Interfaces;

namespace API.BackgroundWorkers;

/// <summary>
///  Scans and fixes any match duplicates.
/// </summary>
public class MatchDuplicateDataWorker : BackgroundService
{
	private readonly ILogger<MatchDuplicateDataWorker> _logger;
	private readonly IServiceProvider _serviceProvider;
	private readonly TimeSpan _interval = TimeSpan.FromSeconds(20); // TODO: Change to longer interval

	public MatchDuplicateDataWorker(ILogger<MatchDuplicateDataWorker> logger, IServiceProvider serviceProvider)
	{
		_logger = logger;
		_serviceProvider = serviceProvider;
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken) => await BackgroundTask(stoppingToken);

	private async Task BackgroundTask(CancellationToken stoppingToken)
	{
		// Periodically checks the database for duplicate matches
		// Any duplicates found will be marked for manual review
		while (!stoppingToken.IsCancellationRequested)
		{
			using var scope = _serviceProvider.CreateScope();
			var matchesRepository = scope.ServiceProvider.GetRequiredService<IMatchesRepository>();
			var duplicateRepository = scope.ServiceProvider.GetRequiredService<IMatchDuplicateXRefRepository>();
			var duplicateGroups = (await matchesRepository.GetDuplicateGroupsAsync()).ToList();
			int created = 0;

			if (duplicateGroups.Count > 0)
			{
				_logger.LogInformation("Identified {Count} duplicate groups", duplicateGroups.Count);

				foreach (var duplicateGroup in duplicateGroups)
				{
					var root = duplicateGroup.OrderBy(x => x.StartTime).First();
					duplicateGroup.Remove(root);

					foreach (var remainingDuplicate in duplicateGroup)
					{
						var duplicate = new MatchDuplicateXRef
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