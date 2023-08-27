using API.Entities;
using API.Services.Interfaces;

namespace API.Osu.Multiplayer;

public class MultiplayerLobbyDataWorker : IMultiplayerLobbyDataWorker
{
	private const int RATE_LIMIT_CAPACITY = 1000;
	private const int RATE_LIMIT_INTERVAL_SECONDS = 60;
	private const int INTERVAL_SECONDS = 1;
	private readonly OsuApiService _apiService;
	private readonly ILogger<MultiplayerLobbyDataWorker> _logger;
	private readonly IServiceProvider _serviceProvider;
	private int _rateLimitCounter;
	private DateTime _rateLimitResetTime = DateTime.UtcNow.AddSeconds(RATE_LIMIT_INTERVAL_SECONDS);

	public MultiplayerLobbyDataWorker(ILogger<MultiplayerLobbyDataWorker> logger, IServiceProvider serviceProvider, OsuApiService apiService)
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

				var link = await multiplayerLinkService.GetFirstPendingOrDefaultAsync();
				if (link == null)
				{
					await Task.Delay(INTERVAL_SECONDS * 1000, cancellationToken);
					continue;
				}
				
				try
				{
					var result = await _apiService.GetMatchAsync(link.MpLinkId);
					if (result == null)
					{
						_logger.LogWarning("Failed to fetch data for match {MatchId} (result from API was null)", link.MpLinkId);
						continue;
					}

					link.LobbyName = result.Match.Name;
					
					
					if (!LobbyNameChecker.IsNameValid(link.LobbyName))
					{
						await UpdateLinkStatusAsync(link, "REJECTED", multiplayerLinkService);
					}
					else
					{
						await UpdateLinkStatusAsync(link, "REVIEW", multiplayerLinkService);
					}
					
					_rateLimitCounter++;
				}
				catch (Exception e)
				{
					await UpdateLinkStatusAsync(link, "FAILED", multiplayerLinkService);

					_logger.LogWarning(e, "Failed to fetch data for match {MatchId} (exception was thrown)", link.MpLinkId);
				}
			}
		}
	}

	private async Task UpdateLinkStatusAsync(MultiplayerLink link, string status, IMultiplayerLinkService multiplayerLinkService)
	{
		link.Status = status;
		link.Updated = DateTime.Now;

		await multiplayerLinkService.UpdateAsync(link);
		_logger.LogDebug("Set status of MultiplayerLink {LinkId} to {Status}", link.Id, status);
	}
}