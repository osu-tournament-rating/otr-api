using System.Diagnostics.CodeAnalysis;
using API.DTOs;
using API.Enums;
using API.Repositories.Interfaces;
using API.Utilities;
using API.Utilities.Extensions;
using Database;
using Database.Entities.Processor;
using Database.Enums;
using Database.Repositories.Implementations;
using Database.Repositories.Interfaces;
using Database.Utilities.Extensions;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories.Implementations;

[SuppressMessage("Performance",
    "CA1862:Use the \'StringComparison\' method overloads to perform case-insensitive string comparisons")]
[SuppressMessage("ReSharper", "SpecifyStringComparison")]
public class ApiPlayerRatingsRepository(
    OtrContext context,
    IPlayersRepository playerRepository
) : PlayerRatingsRepository(context, playerRepository), IApiPlayerRatingsRepository
{
    private readonly OtrContext _context = context;

    public async Task<IEnumerable<PlayerRating>> GetLeaderboardAsync(
        int page,
        int pageSize,
        Ruleset ruleset,
        LeaderboardChartType chartType,
        LeaderboardFilterDTO? filter,
        int? playerId
    )
    {
        IQueryable<PlayerRating> query = await LeaderboardQuery(ruleset, chartType, filter, playerId);

        return await query
            .OrderByRatingDescending()
            .Page(pageSize, page - 1)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<int> LeaderboardCountAsync(
        Ruleset requestQueryRuleset,
        LeaderboardChartType chartType,
        LeaderboardFilterDTO filter,
        int? playerId
    )
    {
        IQueryable<PlayerRating> query = await LeaderboardQuery(requestQueryRuleset, chartType, filter, playerId);

        return await query.CountAsync();
    }

    private async Task<IQueryable<PlayerRating>> LeaderboardQuery(
        Ruleset ruleset,
        LeaderboardChartType chartType,
        LeaderboardFilterDTO? filter,
        int? playerId
    )
    {
        IQueryable<PlayerRating> baseQuery = _context.PlayerRatings
            .WhereRuleset(ruleset)
            // Filter out players who only have the initial adjustment
            .Where(pr => pr.Adjustments.Count > 1);

        if (chartType == LeaderboardChartType.Country && playerId.HasValue)
        {
            var playerCountry = await _context
                .Players.Where(x => x.Id == playerId)
                .Select(x => x.Country)
                .FirstOrDefaultAsync();

            // Addresses players in dependent territories having a *very* small country leaderboard.
            if (
                playerCountry != null
                && LeaderboardUtils.DependentTerritoriesMapping.TryGetValue(
                    playerCountry,
                    out var mappedCountry
                )
            )
            {
                playerCountry = mappedCountry;
            }

            baseQuery = baseQuery.Where(x => x.Player.Country == playerCountry);
        }

        if (filter == null)
        {
            return baseQuery;
        }

        baseQuery = FilterByRank(ruleset, baseQuery, filter.MinRank, filter.MaxRank);
        baseQuery = FilterByRating(baseQuery, filter.MinRating, filter.MaxRating);
        baseQuery = FilterByMatchesPlayed(baseQuery, filter.MinMatches, filter.MaxMatches);

        if (filter.TierFilters.IsEngaged())
        {
            baseQuery = FilterByTier(baseQuery, filter.TierFilters!);
        }

        return baseQuery;
    }

    private static IQueryable<PlayerRating> FilterByRank(
        Ruleset ruleset,
        IQueryable<PlayerRating> query,
        int? minRank,
        int? maxRank
    )
    {
        if (minRank.HasValue)
        {
            query = query.Where(x =>
                x.Player.RulesetData.Any(rd => rd.Ruleset == ruleset)
                && x.Player.RulesetData.FirstOrDefault(rd => rd.Ruleset == ruleset)!.GlobalRank >= minRank
            );
        }

        if (maxRank.HasValue)
        {
            query = query.Where(x =>
                x.Player.RulesetData.Any(rd => rd.Ruleset == ruleset)
                && x.Player.RulesetData.FirstOrDefault(rd => rd.Ruleset == ruleset)!.GlobalRank <= maxRank
            );
        }

        return query;
    }

    private static IQueryable<PlayerRating> FilterByRating(IQueryable<PlayerRating> query, int? minRating,
        int? maxRating)
    {
        if (minRating.HasValue)
        {
            query = query.Where(x => x.Rating >= minRating.Value);
        }

        if (maxRating.HasValue)
        {
            query = query.Where(x => x.Rating <= maxRating.Value);
        }

        return query;
    }

    private static IQueryable<PlayerRating> FilterByMatchesPlayed(
        IQueryable<PlayerRating> query,
        int? minMatches,
        int? maxMatches
    )
    {
        if (minMatches.HasValue || maxMatches.HasValue)
        {
            query = query.Include(x => x.Player).ThenInclude(x => x.MatchStats);
        }

        if (minMatches.HasValue)
        {
            query = query.Where(x => x.Player.MatchStats.Count >= minMatches.Value);
        }

        if (maxMatches.HasValue)
        {
            query = query.Where(x => x.Player.MatchStats.Count <= maxMatches.Value);
        }

        return query;
    }

    private static IQueryable<PlayerRating> FilterByTier(
        IQueryable<PlayerRating> query,
        LeaderboardTierFilterDTO tierFilter
    )
    {
        var includeBronze = tierFilter.FilterBronze;
        var includeSilver = tierFilter.FilterSilver;
        var includeGold = tierFilter.FilterGold;
        var includePlatinum = tierFilter.FilterPlatinum;
        var includeEmerald = tierFilter.FilterEmerald;
        var includeDiamond = tierFilter.FilterDiamond;
        var includeMaster = tierFilter.FilterMaster;
        var includeGrandmaster = tierFilter.FilterGrandmaster;
        var includeEliteGrandmaster = tierFilter.FilterEliteGrandmaster;

        return query.Where(x =>
            (includeBronze && x.Rating < RatingUtils.RatingSilverIII)
            || (includeSilver && x.Rating >= RatingUtils.RatingSilverIII && x.Rating < RatingUtils.RatingGoldIII)
            || (includeGold && x.Rating >= RatingUtils.RatingGoldIII && x.Rating < RatingUtils.RatingPlatinumIII)
            || (includePlatinum && x.Rating >= RatingUtils.RatingPlatinumIII && x.Rating < RatingUtils.RatingEmeraldIII)
            || (includeEmerald && x.Rating >= RatingUtils.RatingEmeraldIII && x.Rating < RatingUtils.RatingDiamondIII)
            || (includeDiamond && x.Rating >= RatingUtils.RatingDiamondIII && x.Rating < RatingUtils.RatingMasterIII)
            || (includeMaster && x.Rating >= RatingUtils.RatingMasterIII && x.Rating < RatingUtils.RatingGrandmasterIII)
            || (includeGrandmaster && x.Rating >= RatingUtils.RatingGrandmasterIII &&
                x.Rating < RatingUtils.RatingEliteGrandmaster)
            || (includeEliteGrandmaster && x.Rating >= RatingUtils.RatingEliteGrandmaster)
        );
    }
}
