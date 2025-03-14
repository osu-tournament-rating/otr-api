using Common.Enums;
using Database;
using Database.Entities;
using Database.Repositories.Interfaces;
using Database.Utilities.Extensions;
using DataWorkerService.Configurations;
using DataWorkerService.Services.Interfaces;
using OsuApiClient;
using OsuApiClient.Domain.Osu.Users;
using OsuApiClient.Domain.Osu.Users.Attributes;
using OsuApiClient.Domain.OsuTrack;

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
        var outdatedPlayers = (await playersRepository.GetOutdatedOsuAsync(
            TimeSpan.FromDays(config.PlayerOutdatedAfterDays),
            config.BatchSize
        )).ToList();

        foreach (Player player in outdatedPlayers)
        {
            await UpdateFromOsuApiAsync(player);
        }

        await context.SaveChangesAsync();

        logger.LogInformation(
            "Updated players with outdated osu! API data [Count: {Count}]",
            outdatedPlayers.Count
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
                player.DefaultRuleset = result.Ruleset;

                if (player.User?.Settings is { DefaultRulesetIsControlled: false })
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
                PlayerOsuRulesetData? variantData =
                    player.RulesetData.FirstOrDefault(rd => rd.Ruleset == variant.Ruleset);

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

    public async Task SetAllOutdatedOsuTrackApiAsync(PlayerFetchPlatformConfiguration config)
    {
        await playersRepository.SetOutdatedOsuTrackAsync(TimeSpan.FromDays(config.PlayerOutdatedAfterDays));
    }

    public async Task UpdateOutdatedFromOsuTrackApiAsync(PlayerFetchPlatformConfiguration config)
    {
        var outdatedPlayers = (await playersRepository.GetOutdatedOsuTrackAsync(
            TimeSpan.FromDays(config.PlayerOutdatedAfterDays),
            config.BatchSize
        )).ToList();

        foreach (Player player in outdatedPlayers)
        {
            await UpdateFromOsuTrackApiAsync(player);
        }

        await playersRepository.UpdateAsync(outdatedPlayers);

        logger.LogInformation(
            "Updated players with outdated osu!track API data [Count: {Count}]",
            outdatedPlayers.Count
        );
    }

    public async Task UpdateFromOsuTrackApiAsync(Player player)
    {
        logger.LogDebug(
            "Preparing to update osu!track data for player [Id: {Id} | osu! Id: {OsuId} | Last Update: {LastUpdate:u}]",
            player.Id,
            player.OsuId,
            player.OsuLastFetch
        );

        foreach (Ruleset r in Enum.GetValues<Ruleset>().Where(r => r.IsFetchable()))
        {
            var result = (await osuClient.GetUserStatsHistoryAsync(player.OsuId, r) ?? [])
                .ToList();

            if (result.Count == 0)
            {
                logger.LogTrace(
                    "Failed to fetch Player osu!track API data. Result has no elements. [Id: {Id} | Ruleset: {Ruleset}]",
                    player.Id, r);
            }

            PlayerOsuRulesetData? rulesetData = player.RulesetData.FirstOrDefault(x => x.Ruleset == r);

            if (rulesetData is null)
            {
                continue;
            }

            DateTime? earliestMatchDate = player.MatchStats
                .DefaultIfEmpty()
                .Min(pms => pms?.Match.StartTime);

            UserStatUpdate? statUpdate = earliestMatchDate is not null
                ? result.OrderBy(s => Math.Abs((s.Timestamp - earliestMatchDate.Value).Ticks)).FirstOrDefault()
                : result.FirstOrDefault();

            if (statUpdate is null)
            {
                logger.LogTrace("No osu!track stats found for Player [Id: {Id} | Ruleset: {Ruleset}]", player.Id, r);
                continue;
            }

            rulesetData.EarliestGlobalRank = statUpdate.Rank;
            rulesetData.EarliestGlobalRankDate = statUpdate.Timestamp;

            logger.LogDebug("Updated Player osu!track API data [Id: {Id} | Ruleset: {Ruleset}]", player.Id, r);
        }

        player.OsuTrackLastFetch = DateTime.UtcNow;
    }
}
