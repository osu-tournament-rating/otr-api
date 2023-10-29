using API.DTOs;
using API.Entities;
using API.Enums;
using API.Osu;
using API.Repositories.Interfaces;
using API.Utilities;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories.Implementations;

public class MatchesRepository : RepositoryBase<Match>, IMatchesRepository
{
	private readonly OtrContext _context;
	private readonly ILogger<MatchesRepository> _logger;
	private readonly IMapper _mapper;

	public MatchesRepository(ILogger<MatchesRepository> logger, IMapper mapper, OtrContext context) : base(context)
	{
		_logger = logger;
		_mapper = mapper;
		_context = context;
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

	public async Task<IEnumerable<Match>> GetAllAsync(bool onlyIncludeFiltered)
	{
		IQueryable<Match>? query;
		
		query = _context.Matches
		                    .Include(m => m.Games)
		                    .ThenInclude(g => g.MatchScores)
		                    .Include(m => m.Games)
		                    .ThenInclude(g => g.Beatmap)
		                    .OrderBy(m => m.StartTime)
		                    .AsQueryable();

		if (onlyIncludeFiltered)
		{
			query = _context.Matches
			                .Include(m => m.Games.Where(x => x.VerificationStatus == (int)GameVerificationStatus.Verified))
			                .ThenInclude(g => g.MatchScores.Where(x => x.IsValid == true))
			                // Unsure if repeating the filter is necessary for subsequent .ThenInclude
			                .Include(m => m.Games.Where(x => x.VerificationStatus == (int)GameVerificationStatus.Verified)) 
			                .ThenInclude(g => g.Beatmap)
			                .OrderBy(m => m.StartTime)
			                .WhereVerified()
			                .AsQueryable();
		}

		var matches = await query
		                    .AsNoTracking()
		                    .ToListAsync();

		matches.RemoveAll(x => x.Games.Count == 0);

		// TODO: Remove - we have game verification for this reason
		// if (onlyIncludeFiltered)
		// {
		// 	var matchesToRemove = new List<Match>();
		// 	foreach (var match in matches)
		// 	{
		// 		var gamesToRemove = new List<Game>();
		// 		foreach (var game in match.Games)
		// 		{
		// 			if ((game.MatchScores.Count % 2) != 0 || game.MatchScores.Count == 0 || !IsValidModCombination(game.ModsEnum))
		// 			{
		// 				gamesToRemove.Add(game);
		// 				continue;
		// 			}
		//
		// 			foreach (var score in game.MatchScores)
		// 			{
		// 				if (!IsValidModCombination(score.EnabledModsEnum ?? OsuEnums.Mods.None))
		// 				{
		// 					gamesToRemove.Add(game);
		// 					break;
		// 				}
		// 			}
		// 		}
		//
		// 		foreach (var game in gamesToRemove)
		// 		{
		// 			match.Games.Remove(game);
		// 		}
		//
		// 		if (!match.Games.Any())
		// 		{
		// 			matchesToRemove.Add(match);
		// 		}
		// 	}
		//
		// 	foreach (var match in matchesToRemove)
		// 	{
		// 		matches.Remove(match);
		// 	}
		// }

		return matches;
	}

	public async Task<MatchDTO?> GetDTOByOsuMatchIdAsync(long osuMatchId) => _mapper.Map<MatchDTO?>(await _context.Matches
	                                                                                                           .Include(x => x.Games)
	                                                                                                           .ThenInclude(g => g.Beatmap)
	                                                                                                           .Include(x => x.Games)
	                                                                                                           .ThenInclude(x => x.MatchScores)
	                                                                                                           .FirstOrDefaultAsync(x => x.MatchId == osuMatchId));

	public async Task<Match?> GetByMatchIdAsync(long matchId) => await _context.Matches
	                                                                           .Include(x => x.Games)
	                                                                           .ThenInclude(x => x.MatchScores)
	                                                                           .Include(x => x.Tournament)
	                                                                           .FirstOrDefaultAsync(x => x.MatchId == matchId);

	public async Task<IList<Match>> GetMatchesNeedingAutoCheckAsync() =>
		// We only want api processed matches because the verification checks require the data from the API
		await _context.Matches
		              .Include(x => x.Games)
		              .ThenInclude(x => x.MatchScores)
		              .Include(x => x.Tournament)
		              .Where(x => x.NeedsAutoCheck == true && x.IsApiProcessed == true)
		              .ToListAsync();

	public async Task<Match?> GetFirstMatchNeedingApiProcessingAsync() => await _context.Matches
	                                                                                    .Include(x => x.Games)
	                                                                                    .ThenInclude(x => x.MatchScores)
	                                                                                    .Where(x => x.IsApiProcessed == false)
	                                                                                    .FirstOrDefaultAsync();

	public async Task<Match?> GetFirstMatchNeedingAutoCheckAsync() => await _context.Matches
	                                                                                .Include(x => x.Tournament)
	                                                                                .Include(x => x.Games)
	                                                                                .ThenInclude(x => x.MatchScores)
	                                                                                .Where(x => x.NeedsAutoCheck == true && x.IsApiProcessed == true)
	                                                                                .FirstOrDefaultAsync();

	public async Task<IList<Match>> GetNeedApiProcessingAsync() => await _context.Matches.Where(x => x.IsApiProcessed == false).ToListAsync();
	public async Task<IEnumerable<Match>> GetByMatchIdsAsync(IEnumerable<long> matchIds) => await _context.Matches.Where(x => matchIds.Contains(x.MatchId)).ToListAsync();

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

	public async Task<IEnumerable<Match>> GetPlayerMatchesAsync(long osuId, int mode, DateTime before, DateTime after)
	{
		return await _context.Matches
		                     .IncludeAllChildren()
		                     .WherePlayerParticipated(osuId)
		                     .WhereMode(mode)
		                     .Before(before)
		                     .After(after)
		                     .ToListAsync();
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

	public async Task<int> CountMatchesPlayedAsync(long osuPlayerId, int mode, DateTime fromTime) => await _context.MatchScores
	                                                                                                               .WhereVerified()
	                                                                                                               .WhereOsuPlayerId(osuPlayerId)
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

	public async Task SetRequireAutoCheckAsync(bool invalidOnly = true)
	{
		if (invalidOnly)
		{
			await _context.Matches.Where(x => x.VerificationStatus != (int)MatchVerificationStatus.Verified && x.VerificationStatus != (int)MatchVerificationStatus.PreVerified)
			              .ExecuteUpdateAsync(x => x.SetProperty(y => y.NeedsAutoCheck, true));
		}
		else
		{
			// Applies to all matches
			await _context.Matches.ExecuteUpdateAsync(x => x.SetProperty(y => y.NeedsAutoCheck, true));
		}
	}

	public async Task<Dictionary<long, int>> GetIdMappingAsync() => await _context.Matches.AsNoTracking().ToDictionaryAsync(x => x.MatchId, x => x.Id);

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