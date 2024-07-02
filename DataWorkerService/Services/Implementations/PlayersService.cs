using Database;
using Database.Entities;
using Database.Enums;
using Database.Extensions;
using Database.Repositories.Interfaces;
using DataWorkerService.Configurations;
using DataWorkerService.Services.Interfaces;
using OsuApiClient;
using OsuApiClient.Domain.Osu.Users;
using OsuApiClient.Domain.Osu.Users.Attributes;

namespace DataWorkerService.Services.Implementations;

public class PlayersService(
    ILogger<PlayersService> logger,
    IPlayersRepository playersRepository,
    IOsuClient osuClient,
    OtrContext context
    ) : IPlayersService
{
    public async Task SetAllOutdatedOsuApiAsync(PlayerFetchPlatformConfiguration config)
    {
        await playersRepository.SetOutdatedOsuAsync(TimeSpan.FromDays(config.PlayerOutdatedAfterDays));
    }

    public async Task UpdateOutdatedFromOsuApiAsync(PlayerFetchPlatformConfiguration config)
    {
        IEnumerable<Player> outdatedPlayers = (await playersRepository.GetOutdatedOsuAsync(
            TimeSpan.FromDays(config.PlayerOutdatedAfterDays),
            config.BatchSize
        )).ToList();

        foreach (Player player in outdatedPlayers)
        {
            await UpdateFromOsuApiAsync(player);
        }

        await context.SaveChangesAsync();

        logger.LogDebug(
            "Updated Players with outdated osu! API data [Count: {Count}]",
            outdatedPlayers.Count()
        );
    }

    public async Task UpdateFromOsuApiAsync(Player player)
    {
        logger.LogDebug(
            "Preparing to update Player osu! API data [Id: {Id} | osu! Id: {OsuId} | Last Update: {LastUpdate:u}]",
            player.Id,
            player.OsuId,
            player.OsuLastFetch
        );

        var once = false;
        foreach (Ruleset ruleset in Enum.GetValues<Ruleset>().Where(r => r.IsFetchable()))
        {
            UserExtended? result = await osuClient.GetUserAsync(player.OsuId, ruleset);

            if (result is null)
            {
                logger.LogWarning(
                    "Failed to fetch Player osu! API data. Cancelling update, Player is likely restricted " +
                    "[Id: {Id} | osu! Id: {OsuId}]",
                    player.Id,
                    player.OsuId
                );
                break;
            }

            // Player data that only requires updating once, doesn't change across responses
            if (!once)
            {
                player.Username = result.Username;
                player.Country = result.CountryCode;
                player.Ruleset = result.Ruleset;

                if (player.User is not null && !player.User.Settings.DefaultRulesetIsControlled)
                {
                    player.User.Settings.DefaultRuleset = result.Ruleset;
                }

                once = true;
            }

            if (result.Statistics is null || !result.Statistics.IsRanked)
            {
                logger.LogDebug(
                    "Skipping ruleset, Player is not ranked [Id: {Id} | osu! Id: {OsuId} | Ruleset: {Ruleset}]",
                    player.Id,
                    player.OsuId,
                    ruleset
                );
                continue;
            }

            // Update ruleset specific data
            PlayerOsuRulesetData? rulesetData = player.RulesetData.FirstOrDefault(rd => rd.Ruleset == ruleset);

            if (rulesetData is null)
            {
                rulesetData = new PlayerOsuRulesetData { Ruleset = ruleset };
                player.RulesetData.Add(rulesetData);
            }

            rulesetData.Pp = result.Statistics.Pp;
            // Safe when IsRanked is true
            rulesetData.GlobalRank = result.Statistics.GlobalRank!.Value;

            // Update any ruleset variant data
            foreach (UserStatisticsVariant variant in result.Statistics.Variants.Where(v => v.IsRanked))
            {
                PlayerOsuRulesetData? variantData = player.RulesetData.FirstOrDefault(rd => rd.Ruleset == variant.Ruleset);

                if (variantData is null)
                {
                    variantData = new PlayerOsuRulesetData { Ruleset = variant.Ruleset };
                    player.RulesetData.Add(variantData);
                }

                variantData.Pp = variant.Pp;
                // Safe when IsRanked is true
                variantData.GlobalRank = variant.GlobalRank!.Value;
            }
        }

        player.OsuLastFetch = DateTime.UtcNow;
    }

    public async Task SetAllOutdatedOsuTrackApiAsync(PlayerFetchPlatformConfiguration config) => throw new NotImplementedException();

    public async Task UpdateOutdatedFromOsuTrackApiAsync(PlayerFetchPlatformConfiguration config) => throw new NotImplementedException();

    public async Task UpdateFromOsuTrackApiAsync(Player player) => throw new NotImplementedException();
}
