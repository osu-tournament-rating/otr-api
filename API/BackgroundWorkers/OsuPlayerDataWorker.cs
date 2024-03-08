using API.Osu;
using API.Osu.Multiplayer;
using API.Repositories.Interfaces;

namespace API.BackgroundWorkers;

public class OsuPlayerDataWorker(
    ILogger<OsuPlayerDataWorker> logger,
    IOsuApiService apiService,
    IServiceProvider serviceProvider,
    IConfiguration config
    ) : BackgroundService
{
    private readonly ILogger<OsuPlayerDataWorker> _logger = logger;
    private readonly IOsuApiService _apiService = apiService;
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    private readonly bool _shouldUpdateUsersAutomatically = config.GetValue<bool>("Osu:AutoUpdateUsers");
    private const int UPDATE_INTERVAL_SECONDS = 5;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken) =>
        await BackgroundTask(stoppingToken);

    private async Task BackgroundTask(CancellationToken cancellationToken)
    {
        if (!_shouldUpdateUsersAutomatically)
        {
            _logger.LogInformation("Skipping osu! player data worker due to configuration");
            return;
        }

        _logger.LogInformation("Initialized osu! player data worker");
        while (!cancellationToken.IsCancellationRequested)
        {
            /**
             * Create service scope, get the player service, and get the first pending player.
             *
             * Players are marked out of date after a certain amount of time has passed
             * since their last update. This process automatically fetches updated
             * information from the osu! API. This updates the player's rank and username.
             */
            using (IServiceScope scope = _serviceProvider.CreateScope())
            {
                IPlayerRepository playerService = scope.ServiceProvider.GetRequiredService<IPlayerRepository>();
                var playersToUpdate = (await playerService.GetOutdatedAsync()).ToList();

                if (playersToUpdate.Count == 0)
                {
                    await Task.Delay(UPDATE_INTERVAL_SECONDS * 1000, cancellationToken);
                    continue;
                }

                foreach (Entities.Player? player in playersToUpdate)
                {
                    // Fetch ranks for all 4 game modes and update accordingly.
                    foreach (OsuEnums.Mode gameModeEnum in Enum.GetValues<OsuEnums.Mode>())
                    {
                        OsuApiUser? apiResult = await _apiService.GetUserAsync(
                            player.OsuId,
                            gameModeEnum,
                            $"Identified player that needs to have ranks updated for mode {gameModeEnum}"
                        );
                        if (apiResult == null)
                        {
                            player.Updated = DateTime.UtcNow;
                            await playerService.UpdateAsync(player);
                            _logger.LogWarning(
                                "Failed to fetch data for player {PlayerId} in mode {GameMode}, skipping (user is likely restricted)",
                                player.OsuId,
                                gameModeEnum
                            );
                            break;
                        }

                        switch (gameModeEnum)
                        {
                            case OsuEnums.Mode.Standard:
                                player.RankStandard = apiResult.Rank;
                                player.Username = apiResult.Username; // Only needs an update once
                                break;
                            case OsuEnums.Mode.Taiko:
                                player.RankTaiko = apiResult.Rank;
                                break;
                            case OsuEnums.Mode.Catch:
                                player.RankCatch = apiResult.Rank;
                                break;
                            case OsuEnums.Mode.Mania:
                                player.RankMania = apiResult.Rank;
                                break;
                        }

                        player.Country = apiResult.Country;
                    }

                    await playerService.UpdateAsync(player);
                }
            }
        }
    }
}
