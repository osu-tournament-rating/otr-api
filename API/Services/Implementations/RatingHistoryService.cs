using API.Models;
using API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Services.Implementations;

public class RatingHistoryService : ServiceBase<RatingHistory>, IRatingHistoryService
{
	private readonly ILogger<RatingHistoryService> _logger;
	public RatingHistoryService(ILogger<RatingHistoryService> logger) : base(logger) { _logger = logger; }

	public async Task<IEnumerable<RatingHistory>> GetForPlayerAsync(int playerId)
	{
		using (var context = new OtrContext())
		{
			return await context.RatingHistories.Where(x => x.PlayerId == playerId).ToListAsync();
		}
	}

	public async Task ReplaceBatchAsync(IEnumerable<RatingHistory> histories)
	{
		histories = histories.ToList();
		using (var context = new OtrContext())
		{
			foreach (var history in histories.OrderBy(x => x.Created))
			{
				var existingHistory = await context.RatingHistories
				                                   .FirstOrDefaultAsync(h => h.PlayerId == history.PlayerId && h.GameId == history.GameId);

				if (existingHistory == null)
				{
					context.RatingHistories.Add(history);
				}
				else
				{
					existingHistory.Mu = history.Mu;
					existingHistory.Sigma = history.Sigma;
					existingHistory.Mode = history.Mode;
					existingHistory.Created = history.Created;
					existingHistory.Updated = DateTime.UtcNow; // Assuming you have an Updated property

					context.RatingHistories.Update(existingHistory);
				}
			}

			await context.SaveChangesAsync();
			_logger.LogInformation("Batch inserted {Count} rating histories", histories.Count());
		}
	}

	public async Task TruncateAsync()
	{
		using (var context = new OtrContext())
		{
			await context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE ratinghistories RESTART IDENTITY");
		}
	}
}