using API.DTOs;
using API.Models;
using API.Services.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace API.Services.Implementations;

public class GamesService : ServiceBase<Game>, IGamesService
{
	private readonly IMapper _mapper;
	private readonly OtrContext _context;
	public GamesService(ILogger<GamesService> logger, IMapper mapper, OtrContext context) : base(logger, context)
	{
		_mapper = mapper;
		_context = context;
	}

	public async Task<int> CreateIfNotExistsAsync(Game dbGame)
	{
		using (_context)
		{
			var existingGame = await _context.Games.FirstOrDefaultAsync(g => g.MatchId == dbGame.MatchId && g.BeatmapId == dbGame.BeatmapId);
			if (existingGame != null)
			{
				return existingGame.Id;
			}

			await _context.Games.AddAsync(dbGame);
			await _context.SaveChangesAsync();
			return dbGame.Id;
		}
	}

	public async Task<GameDTO?> GetByOsuGameIdAsync(long osuGameId)
	{
		using (_context)
		{
			return _mapper.Map<GameDTO?>(await _context.Games.FirstOrDefaultAsync(g => g.GameId == osuGameId));
		}
	}

	public async Task<IEnumerable<GameDTO>> GetByMatchIdAsync(long matchId)
	{
		using (_context)
		{
			int id = await _context.Matches.Where(match => match.MatchId == matchId).Select(match => match.Id).FirstOrDefaultAsync();
			return _mapper.Map<IEnumerable<GameDTO>>(await _context.Games.Where(game => game.MatchId == id).ToListAsync());
		}
	}
}