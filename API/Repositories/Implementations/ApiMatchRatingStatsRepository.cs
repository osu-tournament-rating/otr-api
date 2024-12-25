using API.DTOs;
using API.Repositories.Interfaces;
using Database;
using Database.Enums;
using Database.Repositories.Implementations;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories.Implementations;

public class ApiMatchRatingStatsRepository(OtrContext context)
    : MatchRatingStatsRepository(context), IApiMatchRatingStatsRepository
{
    private readonly OtrContext _context = context;

    public async Task<PlayerRatingChartDTO> GetRatingChartAsync(
        int playerId,
        Ruleset ruleset,
        DateTime? dateMin = null,
        DateTime? dateMax = null
    )
    {
        // Default date range to min and max values if not provided
        dateMin ??= DateTime.MinValue;
        dateMax ??= DateTime.MaxValue;

        // Fetch Match Rating Stats and group by Match.StartTime.Date
        List<IGrouping<DateTime?, PlayerRatingChartDataPointDTO>> chartPoints = await _context
            .RatingAdjustments
            .Include(ra => ra.Match)
            .Where(ra =>
                ra.PlayerId == playerId
                && ra.Ruleset == ruleset
                && ra.Timestamp >= dateMin
                && ra.Timestamp <= dateMax
            )
            .OrderBy(ra => ra.Timestamp)
            .Select(ra =>
            new PlayerRatingChartDataPointDTO
            {
                Name = ra.Match == null
                    ? null
                    : ra.Match.Name,
                MatchId = ra.MatchId,
                MatchOsuId = ra.Match == null
                    ? null
                    : ra.Match.OsuId,
                MatchCost = _context.PlayerMatchStats
                    .Where(pms => pms.PlayerId == ra.PlayerId &&
                                  pms.MatchId == ra.MatchId)
                    .Select(pms => pms.MatchCost)
                    .FirstOrDefault(),
                RatingBefore = ra.RatingBefore,
                RatingAfter = ra.RatingAfter,
                VolatilityBefore = ra.VolatilityBefore,
                VolatilityAfter = ra.VolatilityAfter,
                Ruleset = ra.Ruleset,
                RatingAdjustmentType = ra.AdjustmentType,
                Timestamp = ra.Timestamp
            })
            .OrderBy(ra => ra.Timestamp)
            .GroupBy(ra => ra.Timestamp)
            .ToListAsync();

        // Combine data points, converting Match.StartTime and RatingAdjustment.Timestamp to Date for grouping
        return new PlayerRatingChartDTO { ChartData = chartPoints };
    }
}
