using API.Services.Interfaces;
using Newtonsoft.Json;
using System.Text;

namespace API.Osu;

public class OsuTrackHistoryStats
{
	[JsonProperty("pp_rank")]
	public int Rank { get; set; }
	[JsonProperty("timestamp")]
	public DateTime Timestamp { get; set; }
}

public class OsuTrackApiWorker : BackgroundService
{
	private const string URL_BASE = "https://osutrack-api.ameo.dev";
	private const int UPDATE_INTERVAL_MINUTES = 1; // Every minute, check if this call needs to be made.
	private const int API_RATELIMIT = 200;         // 200 requests per minute.
	private readonly ILogger<OsuTrackApiWorker> _logger;
	private readonly IServiceProvider _serviceProvider;
	private readonly SemaphoreSlim _semaphore = new(1);

	private DateTime _ratelimitReset = DateTime.UtcNow;
	private int _tracker;

	public OsuTrackApiWorker(ILogger<OsuTrackApiWorker> logger, IServiceProvider serviceProvider)
	{
		_logger = logger;
		_serviceProvider = serviceProvider;
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken) => await BackgroundTask(stoppingToken);

	private async Task BackgroundTask(CancellationToken stoppingToken)
	{
		var client = new HttpClient();
		_logger.LogInformation("Initializes osu!track API worker");
		while (!stoppingToken.IsCancellationRequested)
		{
			try
			{
				await _semaphore.WaitAsync(stoppingToken);
				using (var scope = _serviceProvider.CreateScope())
				{
					var playerService = scope.ServiceProvider.GetRequiredService<IPlayerService>();
					var ratingHistoryService = scope.ServiceProvider.GetRequiredService<IRatingHistoryService>();

					var playersToUpdate = (await playerService.GetPlayersWhereMissingGlobalRankAsync()).ToList();
					_logger.LogInformation("Identified {PlayerCount} players to update earliest ranks for", playersToUpdate.Count);

					if (!playersToUpdate.Any())
					{
						await Task.Delay(UPDATE_INTERVAL_MINUTES * 60 * 1000, stoppingToken);
						continue;
					}

					var modes = new[] { OsuEnums.Mode.Standard, OsuEnums.Mode.Taiko, OsuEnums.Mode.Catch, OsuEnums.Mode.Mania };
					foreach (var player in playersToUpdate)
					{
						// If the player doesn't have any history, we need to set their earliest mode rank to what we have now,
						// then save what we end up with after we process all the modes.
						if (player.EarliestOsuGlobalRank == null)
						{
							player.EarliestOsuGlobalRank = player.RankStandard;
							player.EarliestOsuGlobalRankDate = DateTime.UtcNow;
						}
						
						if (player.EarliestTaikoGlobalRank == null)
						{
							player.EarliestTaikoGlobalRank = player.RankTaiko;
							player.EarliestTaikoGlobalRankDate = DateTime.UtcNow;
						}
						
						if (player.EarliestCatchGlobalRank == null)
						{
							player.EarliestCatchGlobalRank = player.RankCatch;
							player.EarliestCatchGlobalRankDate = DateTime.UtcNow;
						}
						
						if (player.EarliestManiaGlobalRank == null)
						{
							player.EarliestManiaGlobalRank = player.RankMania;
							player.EarliestManiaGlobalRankDate = DateTime.UtcNow;
						}

						foreach (var mode in modes)
						{
							var oldestHistory = await ratingHistoryService.GetOldestForPlayerAsync(player.OsuId, (int)mode);

							if (oldestHistory == null)
							{
								continue;
							}

							if(IsRatelimited())
							{
								_logger.LogDebug("osu!track API ratelimited, waiting...");
								await Task.Delay(_ratelimitReset - DateTime.UtcNow, stoppingToken);
							}

							string url = FormedUrl(player.OsuId, mode, oldestHistory.Created, oldestHistory.Created.AddYears(1));
							var response = await client.GetAsync(url, stoppingToken);
							_tracker++;
							
							if (!response.IsSuccessStatusCode)
							{
								_logger.LogError("Failed to fetch osu!Track history for player {PlayerId} in mode {GameMode}, ratelimit = {Ratelimit}", player.OsuId, mode, _tracker);
								continue;
							}

							string responseText = await response.Content.ReadAsStringAsync(stoppingToken);
							
							if (string.IsNullOrEmpty(responseText) || responseText == "[]")
							{
								// Nothing found for this player in this mode.
								continue;
							}

							try
							{
								var stats = JsonConvert.DeserializeObject<OsuTrackHistoryStats[]>(responseText);

								if (stats == null)
								{
									continue;
								}

								var relevant = stats[0]; // The response is ordered by date.
								switch (mode)
								{
									case OsuEnums.Mode.Standard:
										player.EarliestOsuGlobalRank = relevant.Rank;
										player.EarliestOsuGlobalRankDate = relevant.Timestamp;
										break;
									case OsuEnums.Mode.Taiko:
										player.EarliestTaikoGlobalRank = relevant.Rank;
										player.EarliestTaikoGlobalRankDate = relevant.Timestamp;
										break;
									case OsuEnums.Mode.Catch:
										player.EarliestCatchGlobalRank = relevant.Rank;
										player.EarliestCatchGlobalRankDate = relevant.Timestamp;
										break;
									case OsuEnums.Mode.Mania:
										player.EarliestManiaGlobalRank = relevant.Rank;
										player.EarliestManiaGlobalRankDate = relevant.Timestamp;
										break;
								}
								
								_logger.LogInformation("Updated osu!track data for player {PlayerId} in mode {GameMode}, ratelimit = {Ratelimit}: {@Data}", player.OsuId, mode, _tracker, relevant);
							}
							catch (JsonSerializationException e)
							{
								_logger.LogError(e, "Failed to deserialize osu!track data for player {PlayerId} in mode {GameMode}", player.OsuId, mode);
							}
						}

						await playerService.UpdateAsync(player);
					}
				}
			}
			catch (Exception e)
			{
				_logger.LogError(e, "Error occurred while fetching osu!track data");
			}
			finally
			{
				_semaphore.Release();
			}
		}
	}

	private bool IsRatelimited()
	{
		if (DateTime.UtcNow > _ratelimitReset)
		{
			_tracker = 0;
			_ratelimitReset = DateTime.UtcNow.AddMinutes(UPDATE_INTERVAL_MINUTES);
			_logger.LogDebug("osu!Track ratelimiter reset");
		}

		return _tracker >= API_RATELIMIT;
	}

	private string FormedUrl(long osuPlayerId, OsuEnums.Mode mode, DateTime from, DateTime? to = null) => new StringBuilder(URL_BASE)
	                                                                                                      .Append("/stats_history")
	                                                                                                      .Append($"?user={osuPlayerId}")
	                                                                                                      .Append($"&mode={(int)mode}")
	                                                                                                      .Append($"&from={from:yyyy-MM-dd}")
	                                                                                                      .Append(to != null ? $"&to={to:yyyy-MM-dd}" : "")
	                                                                                                      .ToString();
}