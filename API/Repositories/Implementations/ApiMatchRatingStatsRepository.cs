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
        var adjustments = await _context
            .RatingAdjustments
            .Where(ra =>
                ra.PlayerId == playerId
                && ra.AdjustmentType == RatingAdjustmentType.Match
                && ra.Match!.Tournament.Ruleset == ruleset
                && ra.Timestamp >= dateMin
                && ra.Timestamp <= dateMax
            )
            .Select(ra => new
            {
                ra.Timestamp,
                DataPoint = new PlayerRatingChartDataPointDTO
                {
                    Name = ra.Match!.Name ?? "<Unknown>",
                    MatchId = ra.MatchId,
                    MatchOsuId = ra.Match.OsuId,
                    MatchCost = _context.PlayerMatchStats
                        .Where(pms => pms.PlayerId == ra.PlayerId &&
                                      pms.MatchId == ra.MatchId)
                        .Select(pms => pms.MatchCost)
                        .FirstOrDefault(),
                    RatingBefore = ra.RatingBefore,
                    RatingAfter = ra.RatingAfter,
                    VolatilityBefore = ra.VolatilityBefore,
                    VolatilityAfter = ra.VolatilityAfter,
                    IsAdjustment = false,
                    Timestamp = ra.Match.StartTime
                }
            }).ToListAsync();

        var ratingAdjustments = await _context
            .RatingAdjustments
            .Where(ra =>
                ra.PlayerId == playerId
                && ra.Ruleset == ruleset
                && ra.Timestamp >= dateMin
                && ra.Timestamp <= dateMax
            )
            .Select(ra => new
            {
                ra.Timestamp, // Use Timestamp for grouping as fallback
                DataPoint = new PlayerRatingChartDataPointDTO
                {
                    Name = ra.AdjustmentType == RatingAdjustmentType.Decay ? "Decay" : "Adjustment",
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
        var combinedDataPoints = adjustments
            .Select(mrs => new { mrs.Timestamp.Date, mrs.DataPoint })
            .Concat(ratingAdjustments.Select(ra => new { ra.Timestamp.Date, ra.DataPoint }))
            .GroupBy(x => x.Date)
            .OrderBy(g => g.Key)
            .Select(g => g.Select(x => x.DataPoint).ToList())
            .ToList();

        // Prepare and return the DTO
        return new PlayerRatingChartDTO { ChartData = combinedDataPoints };
    }
}
