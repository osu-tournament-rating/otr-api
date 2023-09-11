using API.DTOs;
using API.Entities;
using API.Services.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace API.Services.Implementations;

public class RatingHistoryService : ServiceBase<RatingHistory>, IRatingHistoryService
{
	private readonly OtrContext _context;
	private readonly ILogger<RatingHistoryService> _logger;
	private readonly IMapper _mapper;

	public RatingHistoryService(ILogger<RatingHistoryService> logger, IMapper mapper, OtrContext context) : base(logger, context)
	{
		_logger = logger;
		_mapper = mapper;
		_context = context;
	}

	public async Task<IEnumerable<RatingHistoryDTO>> GetForPlayerAsync(long osuPlayerId)
	{
		int dbId = await _context.Players.Where(x => x.OsuId == osuPlayerId).Select(x => x.Id).FirstOrDefaultAsync();

		if (dbId == 0)
		{
			return new List<RatingHistoryDTO>();
		}

		return _mapper.Map<IEnumerable<RatingHistoryDTO>>(await _context.RatingHistories.Where(x => x.PlayerId == dbId).ToListAsync());
	}

	public async Task<int> BatchInsertAsync(IEnumerable<RatingHistoryDTO> histories)
	{
		var ls = new List<RatingHistory>();
		foreach (var history in histories)
		{
			ls.Add(new RatingHistory
			{
				PlayerId = history.PlayerId,
				Mu = history.Mu,
				Sigma = history.Sigma,
				Created = history.Created,
				Mode = history.Mode,
				MatchId = history.MatchId
			});
		}

		await _context.RatingHistories.AddRangeAsync(ls);
		return await _context.SaveChangesAsync();
	}

	public async Task TruncateAsync() => await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE ratinghistories RESTART IDENTITY");
}