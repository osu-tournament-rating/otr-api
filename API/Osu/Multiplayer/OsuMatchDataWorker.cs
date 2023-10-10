using API.Entities;
using API.Enums;
using API.Osu.AutomationChecks;
using API.Services.Implementations;
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
	///  This background service constantly checks the database for pending matches and processes them.
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
				var apiMatchService = scope.ServiceProvider.GetRequiredService<IApiMatchService>();
				var gamesService = scope.ServiceProvider.GetRequiredService<IGamesService>();
				var matchScoresService = scope.ServiceProvider.GetRequiredService<IMatchScoresService>();
				
				await ProcessMatchesOsuApiAsync(cancellationToken, matchesService, apiMatchService, gamesService);
				await ProcessMatchesNeedingAutomatedChecksAsync(cancellationToken, matchesService, gamesService, matchScoresService);
			}
		}
	}
	
	private async Task ProcessMatchesNeedingAutomatedChecksAsync(CancellationToken cancellationToken, IMatchesService matchesService, IGamesService gamesService,
		IMatchScoresService matchScoresService)
	{
		if (cancellationToken.IsCancellationRequested)
		{
			return;
		}
		
		var matchesToCheck = await matchesService.GetMatchesNeedingAutoCheckAsync();

		foreach (var match in matchesToCheck)
		{
			// Match verification checks
			if (!MatchAutomationChecks.PassesAllChecks(match))
			{
				match.VerificationStatus = (int)MatchVerificationStatus.Rejected;
				match.VerificationSource = (int)MatchVerificationSource.System;
				match.VerificationInfo = "Failed automated checks";

				match.NeedsAutoCheck = false;

				await matchesService.UpdateAsync(match);
				
				_logger.LogInformation("Match {Match} failed automated checks", match.MatchId);
				continue;
			}

			// Game verification checks
			foreach (var game in match.Games)
			{
				if (!GameAutomationChecks.PassesAutomationChecks(game))
				{
					game.VerificationStatus = (int)GameVerificationStatus.Rejected;
					game.RejectionReason = (int)GameRejectionReason.FailedAutomationChecks;
					_logger.LogInformation("Game {Game} failed automation checks", game.GameId);

					await gamesService.UpdateAsync(game);
				}
				else
				{
					// Game has passed automation checks
					game.VerificationStatus = (int)GameVerificationStatus.PreVerified;
					if (match.VerificationStatus == (int)MatchVerificationStatus.Verified)
					{
						game.VerificationStatus = (int)GameVerificationStatus.Verified;
					}
					
					_logger.LogDebug("Game {Game} passed automation checks and is marked as {Status}", game.GameId, (GameVerificationStatus)game.VerificationStatus);
				}
				
				// Score verification checks
				foreach (var score in game.MatchScores)
				{
					if (!ScoreAutomationChecks.PassesAutomationChecks(score))
					{
						score.IsValid = false;
						await matchScoresService.UpdateAsync(score);
						
						_logger.LogDebug("Score [Player: {Player} | Game: {Game}] failed automation checks", score.PlayerId, game.GameId);
					}
					else
					{
						score.IsValid = true;
						await matchScoresService.UpdateAsync(score);
						
						_logger.LogTrace("Score [Player: {Player} | Game: {Game}] passed automation checks", score.PlayerId, game.GameId);
					}
				}
			}
			
			match.NeedsAutoCheck = false;
			await matchesService.UpdateAsync(match);
			
			_logger.LogInformation("Match {Match} has passed automated checks", match.MatchId);
		}
	}

	private async Task ProcessMatchesOsuApiAsync(CancellationToken cancellationToken, IMatchesService matchesService, IApiMatchService apiMatchService, IGamesService gamesService)
	{
		var matchesToCheck = await matchesService.GetNeedApiProcessingAsync();
		if (!matchesToCheck.Any())
		{
			await Task.Delay(INTERVAL_SECONDS * 1000, cancellationToken);
			_logger.LogTrace("No matches needing auto checks, waiting for {Interval} seconds", INTERVAL_SECONDS);
			return;
		}

		foreach (var match in matchesToCheck)
		{
			try
			{
				// Matches at this point should only contain data posted from the web interface.
				// We need to call the osu! API on these matches and persist them.
				var updatedEntity = await ProcessMatchAsync(match.MatchId, apiMatchService);

				if (updatedEntity == null)
				{
					_logger.LogWarning("Failed to process match {MatchId} because result from ApiMatchService processing was null", match.MatchId);
				}
			}
			catch (Exception e)
			{
				_logger.LogError("Failed to automatically process match {MatchId}: {Message}", match.MatchId, e.Message);
			}
		}
	}
	
	private async Task<Entities.Match?> ProcessMatchAsync(long osuMatchId, IApiMatchService apiMatchService)
	{
		var osuMatch = await _apiService.GetMatchAsync(osuMatchId, $"{osuMatchId} was identified as a match that needs to be processed");
		if (osuMatch == null)
		{
			return null;
		}
		
		return await apiMatchService.CreateFromApiMatchAsync(osuMatch);
	}
}