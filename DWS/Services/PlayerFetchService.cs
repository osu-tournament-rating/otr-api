using Common.Constants;
using Common.Enums;
using Database;
using Database.Entities;
using Database.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using OsuApiClient;
using OsuApiClient.Domain.Osu.Users;
using OsuApiClient.Domain.Osu.Users.Attributes;

namespace DWS.Services;

public class PlayerFetchService(ILogger<PlayerFetchService> logger, OtrContext context,
    IOsuClient osuClient, IPlayersRepository playersRepository) : IPlayerFetchService
{
    public async Task<bool> FetchAndPersistAsync(long osuPlayerId, CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Fetching player {OsuPlayerId} from osu! API", osuPlayerId);

        bool processedOnce = false;
        foreach (Ruleset ruleset in EnumConstants.FetchableRulesets)
        {
            try
            {
                UserExtended? osuUser = await osuClient.GetUserAsync(osuPlayerId, ruleset, cancellationToken);

                if (osuUser is null)
                {
                    logger.LogWarning("No data found for player, aborting further fetching of ruleset data [osu! ID: {OsuPlayerId} | Ruleset: {Ruleset}]",
                        osuPlayerId, ruleset);
                    return false;
                }

                if (!processedOnce)
                {
                    await ProcessPlayerAsync(osuUser, cancellationToken);
                    processedOnce = true;
                }

                if (osuUser.Statistics is null)
                {
                    continue;
                }

                await ProcessPlayerRulesetDataAsync(osuUser, ruleset, cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error processing player [osu! ID: {OsuPlayerId} | Ruleset: {Ruleset}]", osuPlayerId, ruleset);
                return false;
            }
        }

        logger.LogDebug("Finished fetch & storage of ruleset data for player {OsuPlayerId}", osuPlayerId);

        return true;
    }

    private async Task ProcessPlayerAsync(UserExtended osuUser, CancellationToken cancellationToken)
    {
        // Get reference to existing player if available
        Player? player = await context.Players.FirstOrDefaultAsync(p => p.OsuId == osuUser.Id, cancellationToken);
        bool exists = player is not null;

        player ??= new Player { OsuId = osuUser.Id };

        // Update basic information
        player.Country = osuUser.CountryCode;
        player.Username = osuUser.Username;
        player.DefaultRuleset = osuUser.Ruleset;
        player.OsuLastFetch = DateTime.UtcNow;

        if (exists)
        {
            await playersRepository.UpdateAsync(player);
        }
        else
        {
            await playersRepository.CreateAsync(player);
        }
    }

    private async Task ProcessPlayerRulesetDataAsync(UserExtended osuUser, Ruleset ruleset, CancellationToken cancellationToken)
    {
        if (osuUser.Statistics is null)
        {
            throw new ArgumentNullException(nameof(osuUser), "Statistics cannot be null here");
        }

        // Player is guaranteed to exist here assuming this is called after ProcessPlayerAsync
        Player? player = await context.Players.Include(p => p.RulesetData).FirstOrDefaultAsync(cancellationToken: cancellationToken);

        if (player is null)
        {
            logger.LogError("Expected to receive a valid player for ruleset data processing, " +
                            "received null instead [osu! ID: {OsuPlayerId} | Ruleset: {Ruleset}", osuUser.Id, ruleset);
            return;
        }

        if (ruleset == Ruleset.ManiaOther)
        {
            HandleVariants(player, osuUser);
        }
        else
        {
            PlayerOsuRulesetData? rulesetData = player.RulesetData.FirstOrDefault(rd => rd.Ruleset == ruleset);
            bool exists = rulesetData is not null;

            rulesetData ??= new PlayerOsuRulesetData { Ruleset = ruleset };

            if (!exists)
            {
                player.RulesetData.Add(rulesetData);
            }

            rulesetData.Pp = osuUser.Statistics.Pp;

            if (osuUser.Statistics.GlobalRank is null)
            {
                logger.LogDebug("Found null global rank while processing ruleset data, aborting " +
                                "[osu! ID: {OsuPlayerId} | Ruleset: {Ruleset} | Statistics: {@Statistics}]", osuUser.Id, ruleset, osuUser.Statistics);
                return;
            }
            else
            {
                rulesetData.GlobalRank = osuUser.Statistics.GlobalRank.Value;
            }
        }

        await playersRepository.UpdateAsync(player);
    }

    private void HandleVariants(Player player, UserExtended osuUser)
    {
        if (osuUser.Statistics is null || osuUser.Statistics.Variants.Length == 0)
        {
            return;
        }

        foreach (UserStatisticsVariant variant in osuUser.Statistics.Variants.Where(v => v.IsRanked))
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

            logger.LogInformation("Handled variant {VariantRuleset} for player {OsuPlayerId}", variant.Ruleset, osuUser.Id);
        }
    }
}
