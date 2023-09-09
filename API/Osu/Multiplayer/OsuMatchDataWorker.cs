using API.Services.Interfaces;

namespace API.Osu.Multiplayer;

public class OsuMatchDataWorker : BackgroundService
{
	private const int INTERVAL_SECONDS = 5;

	private readonly OsuApiService _apiService;
	private readonly ILogger<OsuMatchDataWorker> _logger;
	private readonly IServiceProvider _serviceProvider;
	
	public OsuMatchDataWorker(ILogger<OsuMatchDataWorker> logger, IServiceProvider serviceProvider, OsuApiService apiService)
	{
		_logger = logger;
		_serviceProvider = serviceProvider;
		_apiService = apiService;
	}

	/// <summary>
	///  This background service constantly checks the database for pending multiplayer links and processes them.
	///  The osu! API rate limit is taken into account.
	/// </summary>
	/// <param name="cancellationToken"></param>
	protected override async Task ExecuteAsync(CancellationToken stoppingToken) => await BackgroundTask(stoppingToken);

	private async Task BackgroundTask(CancellationToken cancellationToken = default)
	{
		_logger.LogInformation("Initialized osu! match data worker");

		while (!cancellationToken.IsCancellationRequested)
		{
			using (var scope = _serviceProvider.CreateScope())
			{
				var matchesService = scope.ServiceProvider.GetRequiredService<IMatchesService>();
				var beatmapsService = scope.ServiceProvider.GetRequiredService<IBeatmapService>();
				
				var osuMatch = await matchesService.GetFirstPendingOrDefaultAsync();
				if (osuMatch == null)
				{
					await Task.Delay(INTERVAL_SECONDS * 1000, cancellationToken);
					_logger.LogTrace("Nothing to process, waiting for {Interval} seconds", INTERVAL_SECONDS);
					continue;
				}
				
				try
				{
					var result = await _apiService.GetMatchAsync(osuMatch.MatchId);
					if (result == null)
					{
						await UpdateLinkStatusAsync(osuMatch.MatchId, VerificationStatus.Failure, matchesService);
						continue;
					}
					
					await ProcessOsuMatch(result, matchesService, beatmapsService);
				}
				catch (Exception e)
				{
					await UpdateLinkStatusAsync(osuMatch.MatchId, VerificationStatus.Failure, matchesService);

					_logger.LogWarning(e, "Failed to fetch data for match {MatchId} (exception was thrown)", osuMatch.MatchId);
				}
			}
		}
	}

	private async Task UpdateLinkStatusAsync(long matchId, VerificationStatus status, IMatchesService matchesService)
	{
		await matchesService.UpdateVerificationStatusAsync(matchId, status, MatchVerificationSource.System);
		_logger.LogDebug("Set status of MultiplayerLink {LinkId} to {Status}", matchId, status);
	}

	/// <summary>
	/// Steps:
	///
	/// 1. Process the beatmaps and ensure we are not duplicating this data
	/// 2. Process the match, the games, and the scores
	/// 3. Update the match verification status accordingly
	/// </summary>
	/// <param name="osuMatch"></param>
	/// <param name="matchesService"></param>
	/// <param name="gamesService"></param>
	/// <param name="matchScoresService"></param>
	/// <param name="beatmapService"></param>
	/// <param name="playerService"></param>
	/// <exception cref="NullReferenceException"></exception>
	private async Task ProcessOsuMatch(OsuApiMatchData osuMatch, IMatchesService matchesService, IBeatmapService beatmapService)
	{
		await ProcessBeatmapsAsync(osuMatch, beatmapService);
		await matchesService.CreateFromApiMatchAsync(osuMatch);
		
		if (!LobbyNameChecker.IsNameValid(osuMatch.Match.Name))
		{
			await UpdateLinkStatusAsync(osuMatch.Match.MatchId, VerificationStatus.Rejected, matchesService);
			_logger.LogDebug("Match {MatchId} was rejected", osuMatch.Match.MatchId);
		}
		else
		{
			await UpdateLinkStatusAsync(osuMatch.Match.MatchId, VerificationStatus.PreVerified, matchesService);
		}

		_logger.LogInformation("Match with id {MatchId} was processed", osuMatch.Match.MatchId);
	}

	private async Task ProcessBeatmapsAsync(OsuApiMatchData osuMatch, IBeatmapService beatmapService)
	{
		// Processes and inserts beatmaps into the database if they don't already exist.
		var distinctBeatmapIds = osuMatch.Games.Select(x => x.BeatmapId).Distinct().ToList();
		var existing = await beatmapService.GetByBeatmapIdsAsync(distinctBeatmapIds);
		var idsToProcess = distinctBeatmapIds.Except(existing.Select(x => x.BeatmapId)).ToList();
		var beatmaps = new List<Beatmap>();
		
		foreach (long beatmapId in idsToProcess)
		{
			// Fetch the api and store
			var beatmap = await _apiService.GetBeatmapAsync(beatmapId);
			if (beatmap == null)
			{
				_logger.LogWarning("Failed to fetch beatmap {BeatmapId} (result from API was null)", beatmapId);
				continue;
			}

			beatmaps.Add(beatmap);
		}

		await beatmapService.BulkInsertAsync(beatmaps);
	}
}