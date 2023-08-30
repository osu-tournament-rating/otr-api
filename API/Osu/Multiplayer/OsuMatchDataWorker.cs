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
		while (!cancellationToken.IsCancellationRequested)
		{
			if (_rateLimitCounter >= RATE_LIMIT_CAPACITY && DateTime.UtcNow <= _rateLimitResetTime)
			{
				_logger.LogDebug("Rate limit reached, waiting for reset...");
				await Task.Delay(1000, cancellationToken);
				continue;
			}

			if (DateTime.UtcNow > _rateLimitResetTime)
			{
				_rateLimitCounter = 0; // Reset the counter when the reset time has passed
				_rateLimitResetTime = DateTime.UtcNow.AddSeconds(RATE_LIMIT_INTERVAL_SECONDS);
			}

			using (var scope = _serviceProvider.CreateScope())
			{
				var multiplayerLinkService = scope.ServiceProvider.GetRequiredService<IMultiplayerLinkService>();
				var matchDataService = scope.ServiceProvider.GetRequiredService<IMatchDataService>();

				var osuMatch = await multiplayerLinkService.GetFirstPendingOrDefaultAsync();
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

					osuMatch.Name = result.Match.Name;
					
					if (!LobbyNameChecker.IsNameValid(osuMatch.Name))
					{
						await UpdateLinkStatusAsync(osuMatch, VerificationStatus.Rejected, multiplayerLinkService);
						_logger.LogDebug("Match {MatchId} was rejected (ratelimit currently at {Ratelimit})", osuMatch.MatchId, _rateLimitCounter + 1);
					}
					else
					{
						await UpdateLinkStatusAsync(osuMatch, VerificationStatus.PreVerified, multiplayerLinkService);
					}
					
					_rateLimitCounter++;
				}
				catch (Exception e)
				{
					await UpdateLinkStatusAsync(osuMatch, VerificationStatus.Failure, multiplayerLinkService);

					_logger.LogWarning(e, "Failed to fetch data for match {MatchId} (exception was thrown)", osuMatch.MatchId);
				}
			}
		}
	}

	private async Task UpdateLinkStatusAsync(OsuMatch link, VerificationStatus status, IMultiplayerLinkService multiplayerLinkService)
	{
		link.VerificationStatus = status;
		link.Updated = DateTime.Now;

		await multiplayerLinkService.UpdateAsync(link);
		_logger.LogDebug("Set status of MultiplayerLink {LinkId} to {Status}", link.Id, status);
	}
}