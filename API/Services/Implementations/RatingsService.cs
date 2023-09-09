using API.Models;
using API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Services.Implementations;

public class RatingsService : ServiceBase<Rating>, IRatingsService
{
	private readonly ILogger<RatingsService> _logger;
	public RatingsService(ILogger<RatingsService> logger) : base(logger) { _logger = logger; }

	public async Task<IEnumerable<Rating>> GetForPlayerAsync(int playerId)
	{
		using (var context = new OtrContext())
		{
			return await context.Ratings.Where(x => x.PlayerId == playerId).ToListAsync();
		}
	}

	public override async Task<int> UpdateAsync(Rating entity)
	{
		using (var context = new OtrContext())
		{
			// First, copy the current state of the entity to the history table.
			var history = new RatingHistory
			{
				PlayerId = entity.PlayerId,
				Mu = entity.Mu,
				Sigma = entity.Sigma,
				Created = DateTime.UtcNow,
				Mode = entity.Mode
			};

			await context.RatingHistories.AddAsync(history);
			return await base.UpdateAsync(entity);
		}
	}

	public async Task<int> InsertOrUpdateForPlayerAsync(int playerId, Rating rating)
	{
		using (var context = new OtrContext())
		{
			var existingRating = await context.Ratings
			                                  .Where(r => r.PlayerId == rating.PlayerId && r.Mode == rating.Mode)
			                                  .FirstOrDefaultAsync();

			if (existingRating != null)
			{
				existingRating.Mu = rating.Mu;
				existingRating.Sigma = rating.Sigma;
				existingRating.Updated = rating.Updated;
			}
			else
			{
				context.Ratings.Add(rating);
			}

			return await context.SaveChangesAsync();
		}
	}

	public async Task<int> BatchInsertOrUpdateAsync(IEnumerable<Rating> ratings)
	{
		using (var context = new OtrContext())
		{
			foreach (var rating in ratings)
			{
				var existingRating = await context.Ratings
				                                  .Where(r => r.PlayerId == rating.PlayerId && r.Mode == rating.Mode)
				                                  .FirstOrDefaultAsync();

				if (existingRating != null)
				{
					existingRating.Mu = rating.Mu;
					existingRating.Sigma = rating.Sigma;
					existingRating.Updated = rating.Updated;
				}
				else
				{
					context.Ratings.Add(rating);
				}
			}

			return await context.SaveChangesAsync();
		}
	}

	public async Task<IEnumerable<Rating>> GetAllAsync()
	{
		using (var context = new OtrContext())
		{
			return await context.Ratings.ToListAsync();
		}
	}

	public async Task TruncateAsync()
	{
		using (var context = new OtrContext())
		{
			await context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE ratings RESTART IDENTITY;");
		}
	}
}