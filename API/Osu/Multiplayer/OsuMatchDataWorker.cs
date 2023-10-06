using API.Entities;
using API.Enums;
using API.Services.Interfaces;

namespace API.Osu.Multiplayer;

public class OsuMatchDataWorker : BackgroundService
{
	private const int INTERVAL_SECONDS = 5;
	private readonly IOsuApiService _apiService;
	private readonly ILogger<OsuMatchDataWorker> _logger;
	private readonly IServiceProvider _serviceProvider;

	public OsuMatchDataWorker(ILogger<OsuMatchDataWorker> logger, IServiceProvider serviceProvider, IOsuApiService apiService)
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
				var apiMatchService = scope.ServiceProvider.GetRequiredService<IApiMatchService>();

				var osuMatch = await matchesService.GetFirstUnprocessedOrIncompleteMatchAsync();
				if (osuMatch == null)
				{
					await Task.Delay(INTERVAL_SECONDS * 1000, cancellationToken);
					_logger.LogTrace("Nothing to process, waiting for {Interval} seconds", INTERVAL_SECONDS);
					continue;
				}

				try
				{
					var result = await _apiService.GetMatchAsync(osuMatch.MatchId, "Identified osu! match that needs to be processed (matchesService.GetFirstUnprocessedOrIncompleteMatchAsync() returned a value)");
					if (result == null)
					{
						await UpdateLinkStatusAsync(osuMatch.MatchId, MatchVerificationStatus.Failure, matchesService);
						continue;
					}

					bool isVerified = osuMatch.VerificationStatus == (int)MatchVerificationStatus.Verified;
					await ProcessOsuMatch(result, matchesService, beatmapsService, apiMatchService, isVerified);
				}
				catch (Exception e)
				{
					await UpdateLinkStatusAsync(osuMatch.MatchId, MatchVerificationStatus.Failure, matchesService);

					_logger.LogWarning(e, "Failed to fetch data for match {MatchId} (exception was thrown)", osuMatch.MatchId);
				}
			}
		}
	}

	private async Task UpdateLinkStatusAsync(long matchId, MatchVerificationStatus status, IMatchesService matchesService, string? verificationInfo = null)
	{
		await matchesService.UpdateVerificationStatusAsync(matchId, status, MatchVerificationSource.System, verificationInfo);
	}

	/// <summary>
	///  Steps:
	///  1. Process the beatmaps and ensure we are not duplicating this data
	///  2. Process the match, the games, and the scores
	///  3. Update the match verification status accordingly
	/// </summary>
	/// <param name="osuMatch"></param>
	/// <param name="matchesService"></param>
	/// <param name="gamesService"></param>
	/// <param name="matchScoresService"></param>
	/// <param name="beatmapService"></param>
	/// <param name="playerService"></param>
	/// <exception cref="NullReferenceException"></exception>
	private async Task ProcessOsuMatch(OsuApiMatchData osuMatch, IMatchesService matchesService, IBeatmapService beatmapService, IApiMatchService apiMatchService, bool verified)
	{
		await ProcessBeatmapsAsync(osuMatch, beatmapService);
		await apiMatchService.CreateFromApiMatchAsync(osuMatch);

		string? abbreviation = await matchesService.GetMatchAbbreviationAsync(osuMatch.Match.MatchId);

		string? verificationInfo = null;
		MatchVerificationStatus verificationStatus;
		if (verified)
		{
			verificationStatus = MatchVerificationStatus.Verified;
		}
		else
		{
			if (!LobbyNameChecker.IsNameValid(osuMatch.Match.Name, abbreviation ?? string.Empty))
			{
				verificationStatus = MatchVerificationStatus.Rejected;
				verificationInfo = $"Failed to validate lobby name against regex patterns. " +
				                   $"Expected result like '{abbreviation}: (Team A) vs. (Team B)' but received '{osuMatch.Match.Name}' instead.";
			}
			else
			{
				verificationStatus = MatchVerificationStatus.PreVerified;
			}
		}

		await UpdateLinkStatusAsync(osuMatch.Match.MatchId, verificationStatus, matchesService, verificationInfo);
		_logger.LogInformation("Match with id {MatchId} was processed as {Status}", osuMatch.Match.MatchId, verificationStatus);
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
			var beatmap = await _apiService.GetBeatmapAsync(beatmapId, $"Identified beatmap from match {osuMatch.Match.MatchId} that needs to be processed");
			if (beatmap == null)
			{
				_logger.LogWarning("Failed to fetch beatmap {BeatmapId} (result from API was null)", beatmapId);
				continue;
			}
			
			var existingBeatmap = await beatmapService.GetBeatmapByBeatmapIdAsync(beatmapId);
			if (existingBeatmap == null)
			{
				// Only insert new beatmaps
				beatmaps.Add(beatmap);
			}
		}

		await beatmapService.BulkInsertAsync(beatmaps);
	}
}