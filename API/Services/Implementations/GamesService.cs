using API.DTOs;
using API.Entities;
using API.Osu;
using API.Services.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace API.Services.Implementations;

public class GamesService : ServiceBase<Game>, IGamesService
{
	private readonly OtrContext _context;
	private readonly ILogger<GamesService> _logger;
	private readonly IGameSrCalculator _gameSrCalculator;
	private readonly IMapper _mapper;

	public GamesService(ILogger<GamesService> logger, IGameSrCalculator gameSrCalculator, IMapper mapper, OtrContext context) : base(logger, context)
	{
		_logger = logger;
		_gameSrCalculator = gameSrCalculator;
		_mapper = mapper;
		_context = context;
	}

	public async Task<int> CreateIfNotExistsAsync(Game dbGame)
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

	public async Task<GameDTO?> GetByOsuGameIdAsync(long osuGameId) => _mapper.Map<GameDTO?>(await _context.Games.FirstOrDefaultAsync(g => g.GameId == osuGameId));

	public async Task<IEnumerable<GameDTO>> GetByMatchIdAsync(long matchId)
	{
		int id = await _context.Matches.Where(match => match.MatchId == matchId).Select(match => match.Id).FirstOrDefaultAsync();
		return _mapper.Map<IEnumerable<GameDTO>>(await _context.Games.Where(game => game.MatchId == id).ToListAsync());
	}

	public async Task<IEnumerable<Game>> GetAllAsync()
	{
		return await _context.Games
		                     .Include(b => b.Beatmap)
		                     .Include(g => g.MatchScores)
		                     .ToListAsync();
	}

	// ReSharper disable PossibleMultipleEnumeration
	public async Task UpdateAllPostModSrsAsync()
	{
		_logger.LogInformation("Beginning batch update of post-mod SRs");
		var all = await GetAllAsync();
		_logger.LogInformation("Identified {Count} games to update (all games in database)", all.Count());
		
		foreach (var game in all)
		{
			var beatmap = game.Beatmap;

			if (beatmap == null)
			{
				_logger.LogWarning("Could not find beatmap for game {GameId}", game.GameId);
				continue;
			}
			
			var mods = game.ModsEnum;
			var playerMods = game.MatchScores.Select(x => x.EnabledModsEnum);

			game.PostModSr = await _gameSrCalculator.Calculate(beatmap.Sr, beatmap.Id, mods, playerMods);
			_context.Games.Update(game);
		}

		await _context.SaveChangesAsync();
		_logger.LogInformation("Successfully updated {Count} games", all.Count());
	}
	// ReSharper enable PossibleMultipleEnumeration
}