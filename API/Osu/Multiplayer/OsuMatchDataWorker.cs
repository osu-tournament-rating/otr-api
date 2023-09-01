using API.Entities;
using API.Services.Interfaces;

namespace API.Osu.Multiplayer;

public class OsuMatchDataWorker : IOsuMatchDataWorker
{
	private const int RATE_LIMIT_CAPACITY = 1000;
	private const int RATE_LIMIT_INTERVAL_SECONDS = 60;
	private const int INTERVAL_SECONDS = 10;
	private readonly OsuApiService _apiService;
	private readonly ILogger<OsuMatchDataWorker> _logger;
	private readonly IServiceProvider _serviceProvider;
	private int _rateLimitCounter;
	private DateTime _rateLimitResetTime = DateTime.UtcNow.AddSeconds(RATE_LIMIT_INTERVAL_SECONDS);

	public OsuMatchDataWorker(ILogger<OsuMatchDataWorker> logger, IServiceProvider serviceProvider, OsuApiService apiService)
	{
		_logger = logger;
		_serviceProvider = serviceProvider;
		_apiService = apiService;
	}

	/// <summary>
	///  This method constantly checks the database for pending multiplayer links and processes them.
	///  The osu! API rate limit is taken into account.
	/// </summary>
	/// <param name="cancellationToken"></param>
	public Task StartAsync(CancellationToken cancellationToken = default)
	{
		_ = BackgroundTask(cancellationToken);
		return Task.CompletedTask;
	}

	public async Task StopAsync(CancellationToken cancellationToken) => await Task.CompletedTask;

	private async Task BackgroundTask(CancellationToken cancellationToken = default)
	{
		while (!cancellationToken.IsCancellationRequested && !await IsRateLimited())
		{
			using (var scope = _serviceProvider.CreateScope())
			{
				var matchesService = scope.ServiceProvider.GetRequiredService<IMatchesService>();
				var gamesService = scope.ServiceProvider.GetRequiredService<IGamesService>();
				var matchScoresService = scope.ServiceProvider.GetRequiredService<IMatchScoresService>();
				var beatmapService = scope.ServiceProvider.GetRequiredService<IBeatmapService>();
				var playerService = scope.ServiceProvider.GetRequiredService<IPlayerService>();

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
						_logger.LogWarning("Failed to fetch data for match {MatchId} (result from API was null)", osuMatch.MatchId);
						continue;
					}

					await ProcessOsuMatch(result, matchesService, gamesService, matchScoresService, beatmapService, playerService);
					
					_rateLimitCounter++;
				}
				catch (Exception e)
				{
					await UpdateLinkStatusAsync(osuMatch, VerificationStatus.Failure, matchesService);

					_logger.LogWarning(e, "Failed to fetch data for match {MatchId} (exception was thrown)", osuMatch.MatchId);
				}
			}
		}
	}

	private void CheckRatelimitReset()
	{
		if (DateTime.UtcNow > _rateLimitResetTime)
		{
			_rateLimitCounter = 0; // Reset the counter when the reset time has passed
			_rateLimitResetTime = DateTime.UtcNow.AddSeconds(RATE_LIMIT_INTERVAL_SECONDS);
		}
	}

	private async Task<bool> IsRateLimited()
	{
		CheckRatelimitReset();

		if (_rateLimitCounter >= RATE_LIMIT_CAPACITY && DateTime.UtcNow <= _rateLimitResetTime)
		{
			_logger.LogDebug("Rate limit reached, waiting for reset...");
			await Task.Delay(1000);
			return true;
		}

		return false;
	}

	private async Task UpdateLinkStatusAsync(Entities.Match link, VerificationStatus status, IMatchesService matchesService)
	{
		link.VerificationStatus = status;
		link.VerificationSource = MatchVerificationSource.System;
		link.Updated = DateTime.Now;

		await matchesService.UpdateAsync(link);
		_logger.LogDebug("Set status of MultiplayerLink {LinkId} to {Status}", link.Id, status);
	}

	private async Task ProcessOsuMatch(OsuApiMatchData osuMatch, IMatchesService matchesService,
		IGamesService gamesService, IMatchScoresService matchScoresService, IBeatmapService beatmapService,
		IPlayerService playerService)
	{
		var match = new Entities.Match();
		var games = new List<Entities.Game>();
		var matchScores = new List<MatchScore>();
		
		// TODO: Verify that we are not inserting games twice

		Dictionary<long, int>? beatmapIdMapping = null;
		var osuBeatmapIds = osuMatch.Games.Select(x => x.BeatmapId).Distinct().ToList();
		var existingBeatmaps = await beatmapService.GetByBeatmapIdsAsync(osuBeatmapIds);
		osuBeatmapIds.RemoveAll(x => existingBeatmaps.Any(y => y.BeatmapId == x));
		osuBeatmapIds = osuBeatmapIds.Distinct().ToList();
		
		// Insert all of the beatmaps first
		if (osuBeatmapIds.Count > 0)
		{
			var beatmaps = new List<Beatmap>();
			foreach (long beatmapId in osuBeatmapIds)
			{
				var beatmap = await ProcessBeatmap(beatmapId);

				if (beatmap != null)
				{
					beatmaps.Add(beatmap);
				}
			}
			
			await beatmapService.BulkInsertAsync(beatmaps);
			existingBeatmaps = await beatmapService.GetByBeatmapIdsAsync(osuBeatmapIds);
			
			beatmapIdMapping = existingBeatmaps.ToDictionary(x => x.BeatmapId, x => x.Id);
		}

		if (beatmapIdMapping == null)
		{
			_logger.LogError("Something went very wrong, there are not beatmaps to map to");
			return;
		}
		
		match.MatchId = osuMatch.Match.MatchId;
		match.Name = osuMatch.Match.Name;
		match.StartTime = osuMatch.Match.StartTime;
		match.EndTime = osuMatch.Match.EndTime;

		// TODO: Instead, this needs to be a CreateIfNotExistsAsync method (currently throws an exception if the match already exists)
		int? matchId = await matchesService.CreateAsync(match);
		if (matchId == null)
		{
			_logger.LogError("Failed to create match {MatchId}", osuMatch.Match.MatchId);
			return;
		}

		foreach (var game in osuMatch.Games)
		{
			var g = new Entities.Game();

			g.GameId = game.GameId;
			g.MatchId = matchId.Value;
			g.StartTime = game.StartTime;
			g.EndTime = game.EndTime;
			g.Mods = game.Mods;
			g.MatchType = game.MatchType;
			g.PlayMode = game.PlayMode;
			g.ScoringType = game.ScoringType;
			g.TeamType = game.TeamType;
			g.BeatmapId = beatmapIdMapping[game.BeatmapId];

			games.Add(g);
		}

		var gameIdMapping = await gamesService.GetGameIdMappingAsync(osuMatch.Games.Select(x => x.GameId));
		
		// Get player ids
		var playerOsuIds = osuMatch.Games.SelectMany(x => x.Scores).Select(y => y.UserId).Distinct().ToList();
		// TODO: Verify that we are not inserting players twice
		var players = await playerService.GetByOsuIdAsync(playerOsuIds);
		var playerIdMapping = players.ToDictionary(x => x.OsuId, x => x.Id);
		
		// TODO: Create player if not exists
		
		foreach (var game in osuMatch.Games)
		{
			foreach (var score in game.Scores)
			{
				var matchScore = new MatchScore(); // TODO: needs playerid, gameid

				matchScore.PlayerId = playerIdMapping[score.UserId];
				matchScore.GameId = gameIdMapping[game.GameId];
			
				matchScore.Score = score.PlayerScore;
				matchScore.MaxCombo = score.MaxCombo;
				matchScore.Count50 = score.Count50;
				matchScore.Count100 = score.Count100;
				matchScore.Count300 = score.Count300;
				matchScore.CountMiss = score.CountMiss;
				matchScore.CountKatu = score.CountKatu;
				matchScore.CountGeki = score.CountGeki;
				matchScore.Perfect = score.Perfect == 1;
				matchScore.Pass = score.Pass == 1;
				matchScore.EnabledMods = score.EnabledMods;
				matchScore.Team = score.Team;
			
				matchScores.Add(matchScore);
			}
		}
		
		int matchInsertions = await matchScoresService.BulkInsertAsync(matchScores);
		_logger.LogInformation("Inserted {MatchInsertions} match scores", matchInsertions);
		
		if (!LobbyNameChecker.IsNameValid(osuMatch.Match.Name))
		{
			await UpdateLinkStatusAsync(match, VerificationStatus.Rejected, matchesService);
			_logger.LogDebug("Match {MatchId} was rejected (ratelimit currently at {Ratelimit})", match.MatchId, _rateLimitCounter + 1);
		}
		else
		{
			await UpdateLinkStatusAsync(match, VerificationStatus.PreVerified, matchesService);
		}
	}

	private async Task<Beatmap?> ProcessBeatmap(long beatmapId)
	{
		while (await IsRateLimited()) {} // Wait until the rate limit is reset

		var beatmap = await _apiService.GetBeatmapAsync(beatmapId);
		if (beatmap == null)
		{
			_logger.LogError("Failed to fetch data for beatmap {BeatmapId} (result from API was null)", beatmapId);
			return beatmap;
		}

		return beatmap;
	}
}