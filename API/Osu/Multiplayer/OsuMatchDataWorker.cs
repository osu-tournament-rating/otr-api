using API.Entities;
using API.Enums;
using API.Osu.AutomationChecks;
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
				
				var matchesToCheck = await matchesService.GetNeedApiProcessingAsync();
				if (!matchesToCheck.Any())
				{
					await Task.Delay(INTERVAL_SECONDS * 1000, cancellationToken);
					_logger.LogTrace("No matches needing auto checks, waiting for {Interval} seconds", INTERVAL_SECONDS);
					continue;
				}

				foreach (var match in matchesToCheck)
				{
					try
					{
						// Matches at this point should only contain data posted from the API.
						// We need to call the osu! API on these matches and persist them.
					
						// After, we do automation checks to filter out invalid matches based solely
						// on what is in the database.
						var updatedEntity = await ProcessMatchAsync(match.MatchId, apiMatchService);
						
						// Now that we've updated the entity, and stored all of it's matches and games,
						// we need to perform automation checks and update the verification statuses accordingly.

						if (updatedEntity == null)
						{
							_logger.LogWarning("Failed to process match {MatchId} because result from ApiMatchService processing was null", match.MatchId);
							continue;
						}
						
						// Match verification checks
						if (!MatchAutomationChecks.PassesAllChecks(updatedEntity))
						{
							updatedEntity.VerificationStatus = (int)MatchVerificationStatus.Rejected;
							updatedEntity.VerificationSource = (int)MatchVerificationSource.System;
							updatedEntity.VerificationInfo = "Failed automated checks";

							await matchesService.UpdateAsync(updatedEntity);
							continue;
						}
						
						// Game verification checks
						foreach (var game in updatedEntity.Games)
						{
							if (!GameAutomationChecks.PassesAutomationChecks(game))
							{
								game.VerificationStatus = (int)GameVerificationStatus.Rejected;
								game.RejectionReason = (int)GameRejectionReason.FailedAutomationChecks;
								
								await gamesService.UpdateAsync(game);
							}
						}
						
						// Score verification checks
					}
					catch (Exception e)
					{
						_logger.LogError("Failed to automatically process match {MatchId}: {Message}", match.MatchId, e.Message);
					}
				}
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