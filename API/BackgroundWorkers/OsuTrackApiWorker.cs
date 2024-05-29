using System.Text;
using API.Entities;
using API.Osu;
using API.Osu.Enums;
using API.Repositories.Interfaces;
using Newtonsoft.Json;

namespace API.BackgroundWorkers;

public class OsuTrackApiWorker(
    ILogger<OsuTrackApiWorker> logger,
    IServiceProvider serviceProvider,
    IConfiguration configuration
    ) : BackgroundService
{
    private const string URL_BASE = "https://osutrack-api.ameo.dev";
    private const int UPDATE_INTERVAL_MINUTES = 1; // Every minute, check if this call needs to be made.
    private const int API_RATELIMIT = 200; // 200 requests per minute.
    private readonly ILogger<OsuTrackApiWorker> _logger = logger;
    private readonly SemaphoreSlim _semaphore = new(1);
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    private DateTime _ratelimitReset = DateTime.UtcNow;
    private int _ratelimitTracker;
    private readonly bool _allowDataFetching = configuration.GetValue<bool>("Osu:AutoUpdateUsers");
    private readonly Ruleset[] _rulesets =
    [
        Ruleset.Standard,
        Ruleset.Taiko,
        Ruleset.Catch,
        Ruleset.Mania
    ];

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (_allowDataFetching)
        {
            await BackgroundTask(stoppingToken);
        }
        else
        {
            _logger.LogInformation("Skipping osu!Track API worker due to configuration");
        }
    }

    private async Task BackgroundTask(CancellationToken stoppingToken)
    {
        var client = new HttpClient();
        _logger.LogInformation("Initialized osu!track API worker");
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await _semaphore.WaitAsync(stoppingToken);
                using (IServiceScope scope = _serviceProvider.CreateScope())
                {
                    IPlayerRepository playerService = scope.ServiceProvider.GetRequiredService<IPlayerRepository>();
                    IMatchRatingStatsRepository ratingStatsRepository =
                        scope.ServiceProvider.GetRequiredService<IMatchRatingStatsRepository>();

                    var playersToUpdate = (await playerService.GetPlayersMissingRankAsync()).ToList();

                    if (playersToUpdate.Count == 0)
                    {
                        await Task.Delay(UPDATE_INTERVAL_MINUTES * 60 * 1000, stoppingToken);
                        continue;
                    }

                    _logger.LogInformation(
                        "Identified {PlayerCount} players to update earliest ranks for",
                        playersToUpdate.Count
                    );
                    foreach (Player? player in playersToUpdate)
                    {
                        // If the player doesn't have any history, we need to set their earliest ruleset rank to what we have now,
                        // then save what we end up with after we process all the rulesets.
                        SetDefaultEarliestKnownRanks(player);
                        await UpdateEarliestKnownRanksAsync(
                            ratingStatsRepository,
                            player,
                            client,
                            stoppingToken
                        );

                        _logger.LogInformation(
                            "Updated earliest known ranks for player {PlayerId}",
                            player.OsuId
                        );
                        await playerService.UpdateAsync(player);
                    }
                }
            }
            catch (TaskCanceledException)
            {
                // Ignore, application is probably shutting down
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

    private async Task UpdateEarliestKnownRanksAsync(
        IMatchRatingStatsRepository ratingStatsRepository,
        Player player,
        HttpClient client,
        CancellationToken stoppingToken
    )
    {
        foreach (Ruleset ruleset in _rulesets)
        {
            DateTime? oldestStatDate = await ratingStatsRepository.GetOldestForPlayerAsync(player.Id, (int)ruleset);

            if (!oldestStatDate.HasValue)
            {
                continue;
            }

            if (IsRatelimited())
            {
                _logger.LogDebug("osu!track API ratelimited, waiting...");
                await Task.Delay(_ratelimitReset - DateTime.UtcNow, stoppingToken);
            }

            var responseText = await FetchOsuTrackRankAsync(
                player,
                client,
                ruleset,
                oldestStatDate.Value,
                stoppingToken
            );

            if (string.IsNullOrEmpty(responseText) || responseText == "[]")
            {
                // Nothing found for this player in this ruleset.
                continue;
            }

            try
            {
                OsuTrackHistoryStats[]? stats = JsonConvert.DeserializeObject<OsuTrackHistoryStats[]>(responseText);

                if (stats == null)
                {
                    continue;
                }

                OsuTrackHistoryStats relevant = stats[0]; // The response is ordered by date.
                switch (ruleset)
                {
                    case Ruleset.Standard:
                        player.EarliestOsuGlobalRank = relevant.Rank;
                        player.EarliestOsuGlobalRankDate = relevant.Timestamp;
                        break;
                    case Ruleset.Taiko:
                        player.EarliestTaikoGlobalRank = relevant.Rank;
                        player.EarliestTaikoGlobalRankDate = relevant.Timestamp;
                        break;
                    case Ruleset.Catch:
                        player.EarliestCatchGlobalRank = relevant.Rank;
                        player.EarliestCatchGlobalRankDate = relevant.Timestamp;
                        break;
                    case Ruleset.Mania:
                        player.EarliestManiaGlobalRank = relevant.Rank;
                        player.EarliestManiaGlobalRankDate = relevant.Timestamp;
                        break;
                }

                _logger.LogInformation(
                    "Updated osu!track data for player {PlayerId} in ruleset {Ruleset}, ratelimit = {Ratelimit}: {@Data}",
                    player.OsuId,
                    ruleset,
                    _ratelimitTracker,
                    relevant
                );
            }
            catch (JsonSerializationException e)
            {
                _logger.LogError(
                    e,
                    "Failed to deserialize osu!track data for player {PlayerId} in ruleset {Ruleset}",
                    player.OsuId,
                    ruleset
                );
            }
        }
    }

    private async Task<string> FetchOsuTrackRankAsync(
        Player player,
        HttpClient client,
        Ruleset ruleset,
        DateTime oldestStatDate,
        CancellationToken stoppingToken
    )
    {
        var url = FormedUrl(player.OsuId, ruleset, oldestStatDate, oldestStatDate.AddYears(1));
        HttpResponseMessage response = await client.GetAsync(url, stoppingToken);
        _ratelimitTracker++;

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError(
                "Failed to fetch osu!Track history for player {PlayerId} in ruleset {Ruleset}, ratelimit = {Ratelimit}",
                player.OsuId,
                ruleset,
                _ratelimitTracker
            );

            return string.Empty;
        }

        var responseText = await response.Content.ReadAsStringAsync(stoppingToken);
        return responseText;
    }

    private static void SetDefaultEarliestKnownRanks(Player player)
    {
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
    }

    private bool IsRatelimited()
    {
        if (DateTime.UtcNow > _ratelimitReset)
        {
            _ratelimitTracker = 0;
            _ratelimitReset = DateTime.UtcNow.AddMinutes(UPDATE_INTERVAL_MINUTES);
            _logger.LogDebug("osu!Track ratelimiter reset");
        }

        return _ratelimitTracker >= API_RATELIMIT;
    }

    private static string FormedUrl(long osuPlayerId, Ruleset ruleset, DateTime from, DateTime? to = null) =>
        new StringBuilder(URL_BASE)
            .Append("/stats_history")
            .Append($"?user={osuPlayerId}")
            .Append($"&ruleset={(int)ruleset}")
            .Append($"&from={from:yyyy-MM-dd}")
            .Append(to != null ? $"&to={to:yyyy-MM-dd}" : "")
            .ToString();
}
