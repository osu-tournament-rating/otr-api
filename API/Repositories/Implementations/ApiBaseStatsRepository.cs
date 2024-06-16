using System.Diagnostics.CodeAnalysis;
using API.DTOs;
using API.Enums;
using API.Repositories.Interfaces;
using API.Utilities;
using API.Utilities.Extensions;
using Database;
using Database.Entities;
using Database.Enums;
using Database.Extensions;
using Database.Repositories.Implementations;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories.Implementations;

[SuppressMessage("Performance", "CA1862:Use the \'StringComparison\' method overloads to perform case-insensitive string comparisons")]
[SuppressMessage("ReSharper", "SpecifyStringComparison")]
public class ApiBaseStatsRepository(
    OtrContext context,
    IApiPlayersRepository playerRepository
    ) : BaseStatsRepository(context, playerRepository), IApiBaseStatsRepository
{
    private readonly OtrContext _context = context;

    public async Task<IEnumerable<BaseStats>> GetLeaderboardAsync(
        int page,
        int pageSize,
        int mode,
        LeaderboardChartType chartType,
        LeaderboardFilterDTO? filter,
        int? playerId
    )
    {
        IQueryable<BaseStats> query = await LeaderboardQuery(mode, chartType, filter, playerId);

        return await query
            .OrderByRatingDescending()
            .Skip(page * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<int> LeaderboardCountAsync(
        int mode,
        LeaderboardChartType chartType,
        LeaderboardFilterDTO filter,
        int? playerId
    )
    {
        IQueryable<BaseStats> query = await LeaderboardQuery(mode, chartType, filter, playerId);

        return await query.CountAsync();
    }

    private async Task<IQueryable<BaseStats>> LeaderboardQuery(
        int mode,
        LeaderboardChartType chartType,
        LeaderboardFilterDTO? filter,
        int? playerId
    )
    {
        IQueryable<BaseStats> baseQuery = _context.BaseStats.WhereRuleset((Ruleset)mode);

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

        baseQuery = FilterByRank(mode, baseQuery, filter.MinRank, filter.MaxRank);
        baseQuery = FilterByRating(baseQuery, filter.MinRating, filter.MaxRating);
        baseQuery = FilterByMatchesPlayed(baseQuery, filter.MinMatches, filter.MaxMatches);

        if (filter.TierFilters.IsEngaged())
        {
            baseQuery = FilterByTier(baseQuery, filter.TierFilters!);
        }

        return baseQuery;
    }

    private static IQueryable<BaseStats> FilterByRank(
        int mode,
        IQueryable<BaseStats> query,
        int? minRank,
        int? maxRank
    )
    {
        if (minRank.HasValue)
        {
            query = query.Where(x =>
                mode == 0
                    ? x.Player.RankStandard >= minRank.Value
                    : mode == 1
                        ? x.Player.RankTaiko >= minRank.Value
                        : mode == 2
                            ? x.Player.RankCatch >= minRank.Value
                            : x.Player.RankMania >= minRank.Value
            );
        }

        if (maxRank.HasValue)
        {
            query = query.Where(x =>
                mode == 0
                    ? x.Player.RankStandard <= maxRank.Value
                    : mode == 1
                        ? x.Player.RankTaiko <= maxRank.Value
                        : mode == 2
                            ? x.Player.RankCatch <= maxRank.Value
                            : x.Player.RankMania <= maxRank.Value
            );
        }

        return query;
    }

    private static IQueryable<BaseStats> FilterByRating(IQueryable<BaseStats> query, int? minRating, int? maxRating)
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

    private static IQueryable<BaseStats> FilterByMatchesPlayed(
        IQueryable<BaseStats> query,
        int? minMatches,
        int? maxMatches
    )
    {
        // This is required to count the number of matches played.
        // In the future this should be a stat tied to BaseStats, not calculated.

        if (minMatches.HasValue || maxMatches.HasValue)
        {
            query = query.Include(x => x.Player).ThenInclude(x => x.MatchStats);
        }

        if (minMatches.HasValue)
        {
            query = query.Where(x => x.Player.MatchStats.Count() >= minMatches.Value);
        }

        if (maxMatches.HasValue)
        {
            query = query.Where(x => x.Player.MatchStats.Count() <= maxMatches.Value);
        }

        return query;
    }

    private static IQueryable<BaseStats> FilterByTier(
        IQueryable<BaseStats> query,
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
            || (includeGrandmaster && x.Rating >= RatingUtils.RatingGrandmasterIII && x.Rating < RatingUtils.RatingEliteGrandmaster)
            || (includeEliteGrandmaster && x.Rating >= RatingUtils.RatingEliteGrandmaster)
        );
    }
}