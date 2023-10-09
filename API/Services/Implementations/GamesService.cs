using API.DTOs;
using API.Entities;
using API.Osu;
using API.Services.Interfaces;
using API.Utilities;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace API.Services.Implementations;

public class GamesService : ServiceBase<Game>, IGamesService
{
	private readonly OtrContext _context;
	private readonly IGameSrCalculator _gameSrCalculator;
	private readonly ILogger<GamesService> _logger;
	private readonly IMapper _mapper;

	public GamesService(ILogger<GamesService> logger, IGameSrCalculator gameSrCalculator, IMapper mapper, OtrContext context) : base(logger, context)
	{
		_logger = logger;
		_gameSrCalculator = gameSrCalculator;
		_mapper = mapper;
		_context = context;
	}

	public override async Task<int> UpdateAsync(Game game)
	{
		game.Updated = DateTime.UtcNow;
		return await base.UpdateAsync(game);
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

	public async Task<IEnumerable<Game>> GetAllAsync() => await _context.Games
	                                                                    .Include(b => b.Beatmap)
	                                                                    .Include(g => g.MatchScores)
	                                                                    .ToListAsync(); // ReSharper disable PossibleMultipleEnumeration
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

	public async Task<int> CountGameWinsAsync(long osuPlayerId, int mode, DateTime fromTime)
	{
		/**
		 * This one's a little tricky. For purely head-to-head matches, which are ideally 1v1,
		 * we simply return the number of matches where the player's score is higher than the opponent's.
		 *
		 * For team matches, we need to do a little more work. We need to find the player's team, and then
		 * find the opposing team. Then, we need to sum both team's scores. If the sum of the player's team
		 * is greater than the opponent, then this is counted as a win.
		 *
		 * TODO: Implement accuracy win condition
		 */
		int wins = 0;
		var games = await _context.Games
		                          .WhereVerified()
		                          .Include(x => x.MatchScores)
		                          .ThenInclude(x => x.Player)
		                          .After(fromTime)
		                          .Where(x => x.MatchScores.Any(y => y.Player.OsuId == osuPlayerId) && x.PlayMode == mode)
		                          .ToListAsync();

		// Process HeadToHead (includes 1v1 team games)
		var headToHeadGames = games
		                      .Where(x => x.MatchScores.Count == 2)
		                      .ToList();

		foreach (var game in headToHeadGames)
		{
			try
			{
				var playerScore = game.MatchScores.First(x => x.Player.OsuId == osuPlayerId);
				var opponentScore = game.MatchScores.First(x => x.Player.OsuId != osuPlayerId);
				if (playerScore.Score > opponentScore.Score)
				{
					wins++;
				}
			}
			catch (Exception)
			{
				//
			}
		}

		// Team games
		var teamGames = games.Where(x => x.TeamTypeEnum != OsuEnums.TeamType.HeadToHead && x.MatchScores.Count >= 4).ToList();
		foreach (var game in teamGames)
		{
			try
			{
				int playerTeam = game.MatchScores.First(x => x.Player.OsuId == osuPlayerId).Team;
				int opponentTeam = game.MatchScores.First(x => x.Player.OsuId != osuPlayerId && x.Team != playerTeam).Team;

				long playerTeamScores = game.MatchScores.Where(x => x.Team == playerTeam).Sum(x => x.Score);
				long opponentTeamScores = game.MatchScores.Where(x => x.Team == opponentTeam).Sum(x => x.Score);

				if (playerTeamScores > opponentTeamScores)
				{
					wins++;
				}
			}
			catch (Exception)
			{
				//
			}
		}

		return wins;
	}

	public async Task<string?> MostPlayedTeammateNameAsync(long osuPlayerId, int mode, DateTime fromDate)
	{
		var verifiedMatchScores = _context.MatchScores.WhereVerified();
		var teammateMatchScores = verifiedMatchScores.WhereTeammate(osuPlayerId)
		                                             .WhereMode(mode)
		                                             .After(fromDate);
    
		var teammateNames = await (from oms in teammateMatchScores
		                           join p in _context.Players on oms.PlayerId equals p.Id
		                           where p.Username != null
		                           select p.Username).ToListAsync();

		var mostPlayedTeammate = teammateNames.GroupBy(x => x).MaxBy(g => g.Count());

		return mostPlayedTeammate?.Key;
	}

	public async Task<string?> MostPlayedOpponentNameAsync(long osuPlayerId, int mode, DateTime fromDate)
	{
		var verifiedMatchScores = _context.MatchScores.WhereVerified();
		var opponentMatchScores = verifiedMatchScores.WhereOpponent(osuPlayerId)
		                                             .WhereMode(mode)
		                                             .After(fromDate);
    
		var opponentNames = await (from oms in opponentMatchScores
		                           join p in _context.Players on oms.PlayerId equals p.Id
		                           where p.Username != null
		                           select p.Username).ToListAsync();

		var mostPlayedOpponent = opponentNames.GroupBy(x => x).MaxBy(g => g.Count());

		return mostPlayedOpponent?.Key;
	}
}