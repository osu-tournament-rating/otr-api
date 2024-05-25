using System.Diagnostics.CodeAnalysis;
using API.DTOs;
using API.Enums;
using API.Repositories.Interfaces;
using Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories.Implementations;

[SuppressMessage("Performance", "CA1862:Use the \'StringComparison\' method overloads to perform case-insensitive string comparisons")]
[SuppressMessage("ReSharper", "SpecifyStringComparison")]
public class MatchRatingStatsRepository(OtrContext context) : IMatchRatingStatsRepository
{
    private readonly OtrContext _context = context;

    public async Task<IEnumerable<IEnumerable<MatchRatingStats>>> GetForPlayerAsync(
        int playerId,
        int mode,
        DateTime? dateMin = null,
        DateTime? dateMax = null
    )
    {
        dateMin ??= DateTime.MinValue;
        dateMax ??= DateTime.MaxValue;

        return await _context
            .MatchRatingStats.Where(x =>
                x.PlayerId == playerId
                && x.Match.Tournament.Mode == mode
                && x.Match.StartTime >= dateMin
                && x.Match.StartTime <= dateMax
            )
            .Include(x => x.Match)
            .ThenInclude(x => x.Tournament)
            .GroupBy(x => x.Match.StartTime!.Value.Date)
            .ToListAsync();
    }

    public async Task<PlayerRatingChartDTO> GetRatingChartAsync(
        int playerId,
        int mode,
        DateTime? dateMin = null,
        DateTime? dateMax = null
    )
    {
        // Default date range to min and max values if not provided
        dateMin ??= DateTime.MinValue;
        dateMax ??= DateTime.MaxValue;

        // Fetch Match Rating Stats and group by Match.StartTime.Date
        var matchRatingStats = await _context
            .MatchRatingStats.Where(mrs =>
                mrs.PlayerId == playerId
                && mrs.Match.Tournament.Mode == mode
                && mrs.Match.StartTime >= dateMin
                && mrs.Match.StartTime <= dateMax
            )
            .Select(mrs => new
            {
                mrs.Match.StartTime,
                DataPoint = new PlayerRatingChartDataPointDTO
                {
                    Name = mrs.Match.Name ?? "<Unknown>",
                    MatchId = mrs.MatchId,
                    MatchOsuId = mrs.Match.MatchId,
                    MatchCost = mrs.MatchCost,
                    RatingBefore = mrs.RatingBefore,
                    RatingAfter = mrs.RatingAfter,
                    VolatilityBefore = mrs.VolatilityBefore,
                    VolatilityAfter = mrs.VolatilityAfter,
                    IsAdjustment = false,
                    Timestamp = mrs.Match.StartTime
                }
            })
            .ToListAsync();

        // Assuming RatingAdjustments should be grouped by their own timestamp since they may not have a Match.StartTime
        var ratingAdjustments = await _context
            .RatingAdjustments.Where(ra =>
                ra.PlayerId == playerId
                && ra.Mode == mode
                && ra.Timestamp >= dateMin
                && ra.Timestamp <= dateMax
            )
            .Select(ra => new
            {
                ra.Timestamp, // Use Timestamp for grouping as fallback
                DataPoint = new PlayerRatingChartDataPointDTO
                {
                    Name = ra.RatingAdjustmentType == 0 ? "Decay" : "Adjustment",
                    RatingBefore = ra.RatingBefore,
                    RatingAfter = ra.RatingAfter,
                    VolatilityBefore = ra.VolatilityBefore,
                    VolatilityAfter = ra.VolatilityAfter,
                    IsAdjustment = true,
                    Timestamp = ra.Timestamp
                }
            })
            .ToListAsync();

        // Combine data points, converting Match.StartTime and RatingAdjustment.Timestamp to Date for grouping
        var combinedDataPoints = matchRatingStats
            .Select(mrs => new { mrs.StartTime!.Value.Date, mrs.DataPoint })
            .Concat(ratingAdjustments.Select(ra => new { ra.Timestamp.Date, ra.DataPoint }))
            .GroupBy(x => x.Date)
            .OrderBy(g => g.Key)
            .Select(g => g.Select(x => x.DataPoint).ToList())
            .ToList();

        // Prepare and return the DTO
        return new PlayerRatingChartDTO { ChartData = combinedDataPoints };
    }

    public async Task InsertAsync(MatchRatingStats item)
    {
        await _context.MatchRatingStats.AddAsync(item);
        await _context.SaveChangesAsync();
    }

    public async Task InsertAsync(IEnumerable<MatchRatingStats> items)
    {
        await _context.MatchRatingStats.AddRangeAsync(items);
        await _context.SaveChangesAsync();
    }

    public async Task TruncateAsync() =>
        await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE match_rating_stats RESTART IDENTITY");

    public async Task<int> HighestGlobalRankAsync(
        int playerId,
        int mode,
        DateTime? dateMin,
        DateTime? dateMax
    )
    {
        dateMin ??= DateTime.MinValue;
        dateMax ??= DateTime.MaxValue;

        return await _context
            .MatchRatingStats.Where(x =>
                x.PlayerId == playerId
                && x.Match.Tournament.Mode == mode
                && x.Match.StartTime != null
                && x.Match.StartTime >= dateMin
                && x.Match.StartTime <= dateMax
            )
            .Select(x => x.GlobalRankAfter)
            .DefaultIfEmpty()
            .MaxAsync();
    }

    public async Task<int> HighestCountryRankAsync(
        int playerId,
        int mode,
        DateTime? dateMin = null,
        DateTime? dateMax = null
    )
    {
        dateMin ??= DateTime.MinValue;
        dateMax ??= DateTime.MaxValue;

        return await _context
            .MatchRatingStats.Where(x =>
                x.PlayerId == playerId
                && x.Match.Tournament.Mode == mode
                && x.Match.StartTime != null
                && x.Match.StartTime >= dateMin
                && x.Match.StartTime <= dateMax
            )
            .Select(x => x.CountryRankAfter)
            .DefaultIfEmpty()
            .MaxAsync();
    }

    public async Task<DateTime?> GetOldestForPlayerAsync(int playerId, int mode) =>
        await _context
            .MatchRatingStats.Where(x =>
                x.PlayerId == playerId && x.Match.Tournament.Mode == mode && x.Match.StartTime != null
            )
            .Select(x => x.Match.StartTime)
            .MinAsync();

    public async Task<IEnumerable<MatchRatingStats>> TeammateRatingStatsAsync(
        int playerId,
        int teammateId,
        int mode,
        DateTime dateMin,
        DateTime dateMax
    ) =>
        await _context
            .MatchRatingStats.Where(mrs => mrs.PlayerId == playerId)
            .Where(mrs =>
                _context.PlayerMatchStats.Any(pms =>
                    pms.PlayerId == mrs.PlayerId
                    && pms.TeammateIds.Contains(teammateId)
                    && pms.Match.Tournament.Mode == mode
                    && pms.Match.StartTime >= dateMin
                    && pms.Match.StartTime <= dateMax
                )
            )
            .Distinct()
            .ToListAsync();

    public async Task<IEnumerable<MatchRatingStats>> OpponentRatingStatsAsync(
        int playerId,
        int opponentId,
        int mode,
        DateTime dateMin,
        DateTime dateMax
    ) =>
        await _context
            .MatchRatingStats.Where(mrs => mrs.PlayerId == playerId)
            .Where(mrs =>
                _context.PlayerMatchStats.Any(pms =>
                    pms.PlayerId == mrs.PlayerId
                    && pms.OpponentIds.Contains(opponentId)
                    && pms.Match.Tournament.Mode == mode
                    && pms.Match.StartTime >= dateMin
                    && pms.Match.StartTime <= dateMax
                )
            )
            .Distinct()
            .ToListAsync();

    public async Task<PlayerRankChartDTO> GetRankChartAsync(
        int playerId,
        int mode,
        LeaderboardChartType chartType,
        DateTime? dateMin = null,
        DateTime? dateMax = null
    )
    {
        dateMin ??= DateTime.MinValue;
        dateMax ??= DateTime.MaxValue;

        List<RankChartDataPointDTO> chartData = await _context
            .MatchRatingStats.Include(x => x.Match)
            .ThenInclude(x => x.Tournament)
            .Where(x =>
                x.PlayerId == playerId
                && x.Match.Tournament.Mode == mode
                && x.Match.StartTime != null
                && x.Match.StartTime >= dateMin
                && x.Match.StartTime <= dateMax
            )
            .Select(x => new RankChartDataPointDTO
            {
                TournamentName = x.Match.Tournament.Name,
                MatchName = x.Match.Name ?? "Undefined",
                Rank = chartType == LeaderboardChartType.Global ? x.GlobalRankAfter : x.CountryRankAfter,
                RankChange =
                    chartType == LeaderboardChartType.Global ? x.GlobalRankChange : x.CountryRankChange
            })
            .ToListAsync();

        return new PlayerRankChartDTO { ChartData = chartData };
    }
}
