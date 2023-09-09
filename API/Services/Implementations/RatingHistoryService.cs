using API.DTOs;
using API.Models;
using API.Services.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace API.Services.Implementations;

public class RatingHistoryService : ServiceBase<RatingHistory>, IRatingHistoryService
{
	private readonly ILogger<RatingHistoryService> _logger;
	private readonly IMapper _mapper;
	private readonly OtrContext _context;

	public RatingHistoryService(ILogger<RatingHistoryService> logger, IMapper mapper, OtrContext context) : base(logger, context)
	{
		_logger = logger;
		_mapper = mapper;
		_context = context;
	}

	public async Task<IEnumerable<RatingHistoryDTO>> GetForPlayerAsync(long osuPlayerId)
	{
		using (_context)
		{
			int dbId = await _context.Players.Where(x => x.OsuId == osuPlayerId).Select(x => x.Id).FirstOrDefaultAsync();
			
			if (dbId == 0)
			{
				return new List<RatingHistoryDTO>();
			}
			
			return _mapper.Map<IEnumerable<RatingHistoryDTO>>(await _context.RatingHistories.Where(x => x.PlayerId == dbId).ToListAsync());
		}
	}

	public async Task ReplaceBatchAsync(IEnumerable<RatingHistory> histories)
	{
		histories = histories.ToList();
		using (_context)
		{
			foreach (var history in histories.OrderBy(x => x.Created))
			{
				var existingHistory = await _context.RatingHistories
				                                    .FirstOrDefaultAsync(h => h.PlayerId == history.PlayerId && h.MatchId == history.MatchId);

				if (existingHistory == null)
				{
					_context.RatingHistories.Add(history);
				}
				else
				{
					existingHistory.Mu = history.Mu;
					existingHistory.Sigma = history.Sigma;
					existingHistory.Mode = history.Mode;
					existingHistory.Created = history.Created;
					existingHistory.Updated = DateTime.UtcNow; // Assuming you have an Updated property

					_context.RatingHistories.Update(existingHistory);
				}
			}

			await _context.SaveChangesAsync();
			_logger.LogInformation("Batch inserted {Count} rating histories", histories.Count());
		}
	}

	public async Task TruncateAsync()
	{
		using (_context)
		{
			await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE ratinghistories RESTART IDENTITY");
		}
	}
}