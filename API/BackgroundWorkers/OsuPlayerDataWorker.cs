using API.Configurations;
using API.Entities;
using API.Osu;
using API.Osu.Multiplayer;
using API.Repositories.Interfaces;
using Microsoft.Extensions.Options;

namespace API.BackgroundWorkers;

public class OsuPlayerDataWorker(
    ILogger<OsuPlayerDataWorker> logger,
    IOsuApiService apiService,
    IServiceProvider serviceProvider,
    IOptions<OsuConfiguration> osuConfiguration
    ) : BackgroundService
{
    private readonly bool _shouldUpdateUsersAutomatically = osuConfiguration.Value.AutoUpdateUsers;
    private const int UpdateIntervalSeconds = 5;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken) =>
        await BackgroundTask(stoppingToken);

    private async Task BackgroundTask(CancellationToken cancellationToken)
    {
        if (!_shouldUpdateUsersAutomatically)
        {
            logger.LogInformation("Skipping osu! player data worker due to configuration");
            return;
        }

        logger.LogInformation("Initialized osu! player data worker");
        while (!cancellationToken.IsCancellationRequested)
        {
            /**
             * Create service scope, get the player service, and get the first pending player.
             *
             * Players are marked out of date after a certain amount of time has passed
             * since their last update. This process automatically fetches updated
             * information from the osu! API. This updates the player's rank and username.
             */
            using (IServiceScope scope = serviceProvider.CreateScope())
            {
                IPlayerRepository playerService = scope.ServiceProvider.GetRequiredService<IPlayerRepository>();
                var playersToUpdate = (await playerService.GetOutdatedAsync()).ToList();

                if (playersToUpdate.Count == 0)
                {
                    await Task.Delay(UpdateIntervalSeconds * 1000, cancellationToken);
                    continue;
                }

                foreach (Player? player in playersToUpdate)
                {
                    // Fetch data for all game modes and update accordingly
                    var updatedOnce = false;
                    foreach (OsuEnums.Ruleset gameModeEnum in Enum.GetValues<OsuEnums.Ruleset>())
                    {
                        OsuApiUser? apiResult = await apiService.GetUserAsync(
                            player.OsuId,
                            gameModeEnum,
                            $"Identified player that needs to have ranks updated for mode {gameModeEnum}"
                        );
                        if (apiResult is null)
                        {
                            await playerService.UpdateAsync(player);
                            logger.LogWarning(
                                "Failed to fetch data for player {PlayerId} in mode {GameMode}, skipping (user is likely restricted)",
                                player.OsuId,
                                gameModeEnum
                            );
                            break;
                        }

                        if (!updatedOnce)
                        {
                            // Only need to be updated once
                            player.Country = apiResult.Country;
                            player.Username = apiResult.Username;
                            player.Ruleset = (OsuEnums.Ruleset)(int)apiResult.Ruleset;
                            updatedOnce = true;
                        }

                        switch (gameModeEnum)
                        {
                            case OsuEnums.Ruleset.Standard:
                                player.RankStandard = apiResult.Rank;
                                break;
                            case OsuEnums.Ruleset.Taiko:
                                player.RankTaiko = apiResult.Rank;
                                break;
                            case OsuEnums.Ruleset.Catch:
                                player.RankCatch = apiResult.Rank;
                                break;
                            case OsuEnums.Ruleset.Mania:
                                player.RankMania = apiResult.Rank;
                                break;
                        }
                    }

                    await playerService.UpdateAsync(player);
                }
            }
        }
    }
}
