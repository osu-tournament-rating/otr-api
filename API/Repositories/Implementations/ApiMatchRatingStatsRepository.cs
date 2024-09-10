using API.DTOs;
using API.Repositories.Interfaces;
using Database;
using Database.Enums;
using Database.Repositories.Implementations;

namespace API.Repositories.Implementations;

public class ApiMatchRatingStatsRepository(OtrContext context) : MatchRatingStatsRepository(context), IApiMatchRatingStatsRepository
{
    public Task<PlayerRatingChartDTO> GetRatingChartAsync(
        int playerId,
        Ruleset ruleset,
        DateTime? dateMin = null,
        DateTime? dateMax = null
    )
    {
        // TODO: Rewrite this
        // // Default date range to min and max values if not provided
        // dateMin ??= DateTime.MinValue;
        // dateMax ??= DateTime.MaxValue;
        //
        // // Fetch Match Rating Stats and group by Match.StartTime.Date
        // var matchRatingStats = await _context
        //     .MatchRatingStats.Where(mrs =>
        //         mrs.PlayerId == playerId
        //         && mrs.Match.Tournament.Ruleset == (Ruleset)ruleset
        //         && mrs.Match.StartTime >= dateMin
        //         && mrs.Match.StartTime <= dateMax
        //     )
        //     .Select(mrs => new
        //     {
        //         mrs.Match.StartTime,
        //         DataPoint = new PlayerRatingChartDataPointDTO
        //         {
        //             Name = mrs.Match.Name ?? "<Unknown>",
        //             MatchId = mrs.MatchId,
        //             MatchOsuId = mrs.Match.OsuId,
        //             // MatchCost = mrs.MatchCost,
        //             RatingBefore = mrs.RatingBefore,
        //             RatingAfter = mrs.RatingAfter,
        //             VolatilityBefore = mrs.VolatilityBefore,
        //             VolatilityAfter = mrs.VolatilityAfter,
        //             IsAdjustment = false,
        //             Timestamp = mrs.Match.StartTime
        //         }
        //     })
        //     .ToListAsync();
        //
        // // Assuming RatingAdjustments should be grouped by their own timestamp since they may not have a Match.StartTime
        // var ratingAdjustments = await _context
        //     .RatingAdjustments
        //     .Where(ra =>
        //         ra.PlayerId == playerId
        //         && ra.Ruleset == (Ruleset)ruleset
        //         && ra.Timestamp >= dateMin
        //         && ra.Timestamp <= dateMax
        //     )
        //     .Select(ra => new
        //     {
        //         ra.Timestamp, // Use Timestamp for grouping as fallback
        //         DataPoint = new PlayerRatingChartDataPointDTO
        //         {
        //             Name = ra.RatingAdjustmentType == 0 ? "Decay" : "Adjustment",
        //             RatingBefore = ra.RatingBefore,
        //             RatingAfter = ra.RatingAfter,
        //             VolatilityBefore = ra.VolatilityBefore,
        //             VolatilityAfter = ra.VolatilityAfter,
        //             IsAdjustment = true,
        //             Timestamp = ra.Timestamp
        //         }
        //     })
        //     .ToListAsync();
        //
        // // Combine data points, converting Match.StartTime and RatingAdjustment.Timestamp to Date for grouping
        // var combinedDataPoints = matchRatingStats
        //     .Select(mrs => new { mrs.StartTime.Date, mrs.DataPoint })
        //     .Concat(ratingAdjustments.Select(ra => new { ra.Timestamp.Date, ra.DataPoint }))
        //     .GroupBy(x => x.Date)
        //     .OrderBy(g => g.Key)
        //     .Select(g => g.Select(x => x.DataPoint).ToList())
        //     .ToList();

        // Prepare and return the DTO
        return Task.FromResult(new PlayerRatingChartDTO { ChartData = new List<List<PlayerRatingChartDataPointDTO>>() });
    }
}
