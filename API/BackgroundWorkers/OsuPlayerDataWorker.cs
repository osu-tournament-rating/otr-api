using API.Configurations;
using API.Entities;
using API.Osu.Enums;
using API.Osu.Multiplayer;
using API.Repositories.Interfaces;
using Microsoft.Extensions.Options;

namespace API.BackgroundWorkers;

/// <summary>
/// Worker tasked with updating <see cref="Player"/>s with current information from the osu! api
/// </summary>
public class OsuPlayerDataWorker(
    ILogger<OsuPlayerDataWorker> logger,
    IOsuApiService apiService,
    IServiceProvider serviceProvider,
    IOptions<OsuConfiguration> osuConfiguration
    ) : BackgroundService
{
    private const int UpdateIntervalSeconds = 5;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken) =>
        await BackgroundTask(stoppingToken);

    private async Task BackgroundTask(CancellationToken cancellationToken)
    {
        if (!osuConfiguration.Value.AutoUpdateUsers)
        {
            logger.LogInformation("Skipping osu! player data worker due to configuration");
            return;
        }

        logger.LogInformation("Initialized osu! player data worker");
        while (!cancellationToken.IsCancellationRequested)
        {
            /**
             * Players are marked out of date after a certain amount of time has passed
             * since their last update. This process automatically fetches updated
             * information from the osu! API. This updates the player's ranks, username, and ruleset.
             */
            using (IServiceScope scope = serviceProvider.CreateScope())
            {
                IPlayerRepository playerRepository = scope.ServiceProvider.GetRequiredService<IPlayerRepository>();
                IUserRepository userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();

                var playersToUpdate = (await playerRepository.GetOutdatedAsync()).ToList();

                if (playersToUpdate.Count == 0)
                {
                    await Task.Delay(UpdateIntervalSeconds * 1000, cancellationToken);
                    continue;
                }

                foreach (Player player in playersToUpdate)
                {
                    // Fetch data for all rulesets and update accordingly
                    var updatedOnce = false;
                    foreach (Ruleset ruleset in Enum.GetValues<Ruleset>())
                    {
                        OsuApiUser? apiResult = await apiService.GetUserAsync(
                            player.OsuId,
                            ruleset,
                            $"Identified player that needs to have ranks updated for mode {ruleset}"
                        );

                        if (apiResult is null)
                        {
                            logger.LogWarning(
                                "Failed to fetch data for player {PlayerId} in mode {GameMode}, skipping (user is likely restricted)",
                                player.OsuId,
                                ruleset
                            );
                            break;
                        }

                        if (!updatedOnce)
                        {
                            // Only need to be updated once
                            player.Country = apiResult.Country;
                            player.Username = apiResult.Username;
                            player.Ruleset = apiResult.Ruleset;
                            User? user = await userRepository.GetByPlayerIdAsync(player.Id, true);
                            if (user?.Settings is not null && !user.Settings.DefaultRulesetIsControlled)
                            {
                                user.Settings.DefaultRuleset = apiResult.Ruleset;
                                await userRepository.UpdateAsync(user);
                            }
                            updatedOnce = true;
                        }

                        // Suppression: Four ruleset values are the only possible cases
                        // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
                        switch (ruleset)
                        {
                            case Ruleset.Standard: player.RankStandard = apiResult.Rank; break;
                            case Ruleset.Taiko: player.RankTaiko = apiResult.Rank; break;
                            case Ruleset.Catch: player.RankCatch = apiResult.Rank; break;
                            case Ruleset.Mania: player.RankMania = apiResult.Rank; break;
                        }
                    }

                    await playerRepository.UpdateAsync(player);
                }
            }
        }
    }
}
