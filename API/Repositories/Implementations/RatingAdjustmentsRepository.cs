using API.DTOs;
using API.Entities;
using API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories.Implementations;

public class RatingAdjustmentsRepository(OtrContext context) : RepositoryBase<RatingAdjustment>(context), IRatingAdjustmentsRepository
{
    private readonly OtrContext _context = context;

    public async Task BatchInsertAsync(IEnumerable<RatingAdjustmentDTO> postBody)
    {
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

            _context.RatingAdjustments.Add(adjustment);
        }

        await _context.SaveChangesAsync();
    }

    public async Task TruncateAsync() =>
        await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE rating_adjustments RESTART IDENTITY");
}
