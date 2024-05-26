using API.DTOs;
using API.Repositories.Interfaces;
using API.Services.Interfaces;
using Database.Entities;

namespace API.Services.Implementations;

public class RatingAdjustmentsesService(IRatingAdjustmentsRepository ratingAdjustmentsRepository) : IRatingAdjustmentsService
{
    public async Task BatchInsertAsync(IEnumerable<RatingAdjustmentDTO> postBody)
    {
        var adjustments = new List<RatingAdjustment>();
        foreach (RatingAdjustmentDTO item in postBody)
        {
            var adjustment = new RatingAdjustment
            {
                PlayerId = item.PlayerId,
                Mode = item.Mode,
                RatingAdjustmentAmount = item.RatingAdjustmentAmount,
                VolatilityAdjustmentAmount = item.VolatilityAdjustmentAmount,
                RatingBefore = item.RatingBefore,
                RatingAfter = item.RatingAfter,
                VolatilityBefore = item.VolatilityBefore,
                VolatilityAfter = item.VolatilityAfter,
                RatingAdjustmentType = item.RatingAdjustmentType,
                Timestamp = item.Timestamp
            };

            adjustments.Add(adjustment);
        }

        await ratingAdjustmentsRepository.BulkInsertAsync(adjustments);
    }

    public async Task TruncateAsync() => await ratingAdjustmentsRepository.TruncateAsync();
}
