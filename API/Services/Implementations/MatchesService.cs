using API.DTOs;
using API.Entities;
using API.Enums;
using API.Osu;
using API.Services.Interfaces;
using API.Utilities;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace API.Services.Implementations;

public class MatchesService : ServiceBase<Match>, IMatchesService
{
	private readonly IBeatmapService _beatmapService;
	private readonly OtrContext _context;
	private readonly IGameSrCalculator _gameSrCalculator;
	private readonly ILogger<MatchesService> _logger;
	private readonly IMapper _mapper;
	private readonly IPlayerService _playerService;
	private readonly OsuEnums.Mods[] _unallowedMods =
	{
		OsuEnums.Mods.TouchDevice,
		OsuEnums.Mods.SuddenDeath,
		OsuEnums.Mods.Relax,
		OsuEnums.Mods.Autoplay,
		OsuEnums.Mods.SpunOut,
		OsuEnums.Mods.Relax2, // Autopilot
		OsuEnums.Mods.Perfect,
		OsuEnums.Mods.Random,
		OsuEnums.Mods.Cinema,
		OsuEnums.Mods.Target
	};

	public MatchesService(ILogger<MatchesService> logger, IPlayerService playerService,
		IBeatmapService beatmapService, IMapper mapper, OtrContext context, IGameSrCalculator gameSrCalculator) : base(logger, context)
	{
		_logger = logger;
		_playerService = playerService;
		_beatmapService = beatmapService;
		_mapper = mapper;
		_context = context;
		_gameSrCalculator = gameSrCalculator;
	}

	public override async Task<Match?> GetAsync(int id) =>
		// Get the match with all associated data
		await _context.Matches
		              .Include(x => x.Games)
		              .ThenInclude(x => x.MatchScores)
		              .Include(x => x.Games)
		              .ThenInclude(x => x.Beatmap)
		              .FirstOrDefaultAsync(x => x.Id == id);

	public override async Task<int> UpdateAsync(Match entity)
	{
		entity.Updated = DateTime.UtcNow;
		_context.Matches.Update(entity);
		return await _context.SaveChangesAsync();
	}

	public async Task RefreshAllVerifiedAsync()
	{
		_logger.LogWarning("Refreshing all verified matches");
		await _context.Matches.Where(x => x.VerificationStatus == (int)MatchVerificationStatus.Verified)
		              .ExecuteUpdateAsync(x => x.SetProperty(y => y.IsApiProcessed, false).SetProperty(y => y.NeedsAutoCheck, true));

		_logger.LogWarning("Refreshed all verified matches");
	}

	public async Task RefreshAutomationChecks(bool invalidOnly = true)
	{
		var query = _context.Matches
		                      .Where(x => x.NeedsAutoCheck == false && x.IsApiProcessed == true);

		if (invalidOnly)
		{
			query = query.Where(x => x.VerificationStatus == (int)MatchVerificationStatus.Rejected);
		}
		
		await query.ExecuteUpdateAsync(x => x.SetProperty(y => y.NeedsAutoCheck, true));
		_logger.LogInformation("Refreshed automation checks for {Count} matches", await query.CountAsync());
	}

	public async Task<IEnumerable<MatchDTO>> GetAllAsync(bool onlyIncludeFiltered)
	{
		var query = _context.Matches
		                    .Include(m => m.Games)
		                    .ThenInclude(g => g.MatchScores)
		                    .Include(m => m.Games)
		                    .ThenInclude(g => g.Beatmap)
		                    .OrderBy(m => m.StartTime)
		                    .AsQueryable();

		if (onlyIncludeFiltered)
		{
			query = query.Where(m => m.VerificationStatus == (int)MatchVerificationStatus.Verified &&
			                         m.Games.Any(g => g.MatchScores.Any(ms => ms.Score > 10000) &&
			                                          g.Beatmap!.DrainTime > 20 &&
			                                          g.Beatmap!.Sr < 10 &&
			                                          g.ScoringType == (int)OsuEnums.ScoringType.ScoreV2)); // Score v2 only
		}

		var matches = await query.ToListAsync();

		if (onlyIncludeFiltered)
		{
			var matchesToRemove = new List<Match>();
			foreach (var match in matches)
			{
				var gamesToRemove = new List<Game>();
				foreach (var game in match.Games)
				{
					if ((game.MatchScores.Count % 2) != 0 || game.MatchScores.Count == 0 || !IsValidModCombination(game.ModsEnum))
					{
						gamesToRemove.Add(game);
						continue;
					}

					foreach (var score in game.MatchScores)
					{
						if (!IsValidModCombination(score.EnabledModsEnum ?? OsuEnums.Mods.None))
						{
							gamesToRemove.Add(game);
							break;
						}
					}
				}

				foreach (var game in gamesToRemove)
				{
					match.Games.Remove(game);
				}

				if (!match.Games.Any())
				{
					matchesToRemove.Add(match);
				}
			}

			foreach (var match in matchesToRemove)
			{
				matches.Remove(match);
			}
		}

		return _mapper.Map<IEnumerable<MatchDTO>>(matches);
	}

	public async Task<MatchDTO?> GetDTOByOsuMatchIdAsync(long osuMatchId) => _mapper.Map<MatchDTO?>(await _context.Matches
	                                                                                                           .Include(x => x.Games)
	                                                                                                           .ThenInclude(g => g.Beatmap)
	                                                                                                           .Include(x => x.Games)
	                                                                                                           .ThenInclude(x => x.MatchScores)
	                                                                                                           .FirstOrDefaultAsync(x => x.MatchId == osuMatchId));

	public async Task<Entities.Match?> GetByMatchIdAsync(long matchId) => await _context.Matches.FirstOrDefaultAsync(x => x.MatchId == matchId);

	public async Task<IList<Match>> GetMatchesNeedingAutoCheckAsync() =>
		// We only want api processed matches because the verification checks require the data from the API
		await _context.Matches
		              .Include(x => x.Games)
		              .ThenInclude(x => x.MatchScores)
		              .Where(x => x.NeedsAutoCheck == true && x.IsApiProcessed == true)
		              .ToListAsync();

	public async Task<Match?> GetFirstMatchNeedingApiProcessingAsync() => await _context.Matches
	                                                                                    .Include(x => x.Games)
	                                                                                    .ThenInclude(x => x.MatchScores)
	                                                                                    .Where(x => x.IsApiProcessed == false)
	                                                                                    .FirstOrDefaultAsync();

	public async Task<Match?> GetFirstMatchNeedingAutoCheckAsync() => await _context.Matches
	                                                                                .Include(x => x.Games)
	                                                                                .ThenInclude(x => x.MatchScores)
	                                                                                .Where(x => x.NeedsAutoCheck == true && x.IsApiProcessed == true)
	                                                                                .FirstOrDefaultAsync();

	public async Task<IList<Match>> GetNeedApiProcessingAsync() => await _context.Matches.Where(x => x.IsApiProcessed == false).ToListAsync();
	public async Task<IEnumerable<Match>> CheckExistingAsync(IEnumerable<long> matchIds) => await _context.Matches.Where(x => matchIds.Contains(x.MatchId)).ToListAsync();

	public async Task<int> BatchInsertAsync(IEnumerable<Match> matches)
	{
		await _context.Matches.AddRangeAsync(matches);
		return await _context.SaveChangesAsync();
	}

	public async Task<int> UpdateVerificationStatusAsync(long matchId, MatchVerificationStatus status, MatchVerificationSource source, string? info = null)
	{
		var match = await _context.Matches.FirstOrDefaultAsync(x => x.MatchId == matchId);
		if (match == null)
		{
			_logger.LogWarning("Match {MatchId} not found (failed to update verification status)", matchId);
			return 0;
		}

		match.VerificationStatus = (int)status;
		match.VerificationSource = (int)source;
		match.VerificationInfo = info;

		_logger.LogInformation("Updated verification status of match {MatchId} to {Status} (source: {Source}, info: {Info})", matchId, status, source, info);
		return await _context.SaveChangesAsync();
	}

	public async Task<Unmapped_PlayerMatchesDTO> GetPlayerMatchesAsync(long osuId, DateTime fromTime)
	{
		var matches = await _context.Matches
		                            .Include(x => x.Games)
		                            .After(fromTime)
		                            .Where(x => x.Games.Any(y => y.MatchScores.Any(z => z.Player.OsuId == osuId)))
		                            .ToListAsync();

		return new Unmapped_PlayerMatchesDTO
		{
			OsuId = osuId,
			Matches = _mapper.Map<ICollection<MatchDTO>>(matches)
		};
	}

	public async Task<int> CountMatchWinsAsync(long osuPlayerId, int mode, DateTime fromTime)
	{
		int wins = 0;
		var matches = await _context.Matches
		                            .WhereVerified()
		                            .After(fromTime)
		                            .Include(x => x.Games)
		                            .ThenInclude(x => x.MatchScores)
		                            .ThenInclude(x => x.Player)
		                            .Where(x => x.Games.Any(y => y.PlayMode == mode && y.MatchScores.Any(z => z.Player.OsuId == osuPlayerId)))
		                            .ToListAsync();

		foreach (var match in matches)
		{
			// For head to head (lobby size 2), calculate the winner based on score
			int pointsPlayer = 0;
			int pointsOpponent = 0;
			int team = 0;
			foreach (var game in match.Games)
			{
				if (!game.MatchScores.Any(x => x.Player.OsuId == osuPlayerId))
				{
					continue;
				}

				team = game.MatchScores.First(x => x.Player.OsuId == osuPlayerId).Team;
			}

			foreach (var game in match.Games)
			{
				try
				{
					if (game.MatchScores.Count == 2)
					{
						// Assuming this is a 1v1...
						if (!game.MatchScores.Any(x => x.Player.OsuId == osuPlayerId))
						{
							continue;
						}

						long playerScore = game.MatchScores.First(x => x.Player.OsuId == osuPlayerId).Score;
						long opponentScore = game.MatchScores.First(x => x.Player.OsuId != osuPlayerId).Score;

						if (playerScore > opponentScore)
						{
							pointsPlayer++;
						}
						else
						{
							pointsOpponent++;
						}
					}
					else if (game.MatchScores.Count >= 4)
					{
						// Identify player team, sum the scores, then add points this way
						int playerTeam = team;
						int? opponentTeam = game.MatchScores.FirstOrDefault(x => x.Team != playerTeam)?.Team;

						long playerTeamScores = game.MatchScores.Where(x => x.Team == playerTeam).Sum(x => x.Score);
						long opponentTeamScores = game.MatchScores.Where(x => x.Team == opponentTeam).Sum(x => x.Score);

						if (playerTeamScores > opponentTeamScores)
						{
							pointsPlayer++;
						}
						else
						{
							pointsOpponent++;
						}
					}
				}
				catch (Exception e)
				{
					_logger.LogWarning(e, "Error occurred while calculating match wins for player {OsuId}", osuPlayerId);
				}
			}

			if (pointsPlayer > pointsOpponent)
			{
				wins++;
			}
		}

		return wins;
	}

	public async Task<IEnumerable<Unmapped_VerifiedTournamentDTO>> GetAllVerifiedTournamentsAsync() => await _context.Matches
	                                                                                                                 .WhereVerified()
	                                                                                                                 .GroupBy(x => x.TournamentName)
	                                                                                                                 .Where(x => x.Key != null)
	                                                                                                                 .OrderBy(x => x.Key)
	                                                                                                                 .Select(x => new Unmapped_VerifiedTournamentDTO
	                                                                                                                 {
		                                                                                                                 TournamentName = x.Key,
		                                                                                                                 Abbreviation = x.First().Abbreviation,
		                                                                                                                 ForumPost = x.First().Forum
	                                                                                                                 })
	                                                                                                                 .ToListAsync();

	public async Task<int> CountMatchesPlayedAsync(long osuPlayerId, int mode, DateTime fromTime) => await _context.MatchScores
	                                                                                                               .WhereVerified()
	                                                                                                               .WherePlayer(osuPlayerId)
	                                                                                                               .WhereMode(mode)
	                                                                                                               .After(fromTime)
	                                                                                                               .Include(x => x.Game)
	                                                                                                               .GroupBy(x => x.Game.MatchId)
	                                                                                                               .CountAsync();

	public async Task<double> GetWinRateAsync(long osuPlayerId, int mode, DateTime fromTime)
	{
		int played = await CountMatchesPlayedAsync(osuPlayerId, mode, fromTime);
		int won = await CountMatchWinsAsync(osuPlayerId, mode, fromTime);

		if (played == 0)
		{
			return 0;
		}

		return (double)won / played;
	}

	public async Task<string?> GetMatchAbbreviationAsync(long osuMatchId) =>
		await _context.Matches.Where(x => x.MatchId == osuMatchId).Select(x => x.Abbreviation).FirstOrDefaultAsync();

	public async Task UpdateAsApiProcessed(Match match)
	{
		match.IsApiProcessed = true;
		await UpdateAsync(match);
	}
	public async Task UpdateAsAutoChecked(Match match)
	{
		match.NeedsAutoCheck = false;
		await UpdateAsync(match);
	}

	/// <summary>
	///  Calculates the effective "post-mod SR" of a given game.
	///  This is used by the rating algorithm specifically.
	/// </summary>
	/// <param name="game"></param>
	/// <param name="baseSr">The default nomod SR of the beatmap</param>
	/// <returns>SR after mods are applied</returns>
	private async Task<double> CalculatePostModSr(Osu.Multiplayer.Game game, int beatmapDbId, double baseSr)
	{
		var baseMods = game.Mods;
		var appliedMods = game.Scores.Select(x => x.EnabledMods).Where(x => x != null);

		return await _gameSrCalculator.Calculate(baseSr, beatmapDbId, baseMods, appliedMods);
	}

	private bool IsValidModCombination(OsuEnums.Mods modCombination)
	{
		List<OsuEnums.Mods> validMods = new()
		{
			OsuEnums.Mods.None,
			OsuEnums.Mods.Hidden,
			OsuEnums.Mods.HardRock,
			OsuEnums.Mods.DoubleTime,
			OsuEnums.Mods.Nightcore,
			OsuEnums.Mods.Flashlight,
			OsuEnums.Mods.Easy,
			OsuEnums.Mods.NoFail,
			OsuEnums.Mods.HalfTime,
			OsuEnums.Mods.Mirror,
			OsuEnums.Mods.Key1,
			OsuEnums.Mods.Key2,
			OsuEnums.Mods.Key3,
			OsuEnums.Mods.Key4,
			OsuEnums.Mods.Key5,
			OsuEnums.Mods.Key6,
			OsuEnums.Mods.Key7,
			OsuEnums.Mods.Key8,
			OsuEnums.Mods.Key9
		};

		foreach (var validMod in validMods)
		{
			// Remove the valid mod from the combination
			modCombination &= ~validMod;
		}

		// If the result is not Mods.None, then there was an invalid mod in the combination
		return modCombination == OsuEnums.Mods.None;
	}
}