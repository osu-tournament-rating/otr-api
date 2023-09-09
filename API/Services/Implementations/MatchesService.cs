using API.DTOs;
using API.Enums;
using API.Models;
using API.Osu;
using API.Osu.Multiplayer;
using API.Services.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace API.Services.Implementations;

public class MatchesService : ServiceBase<Models.Match>, IMatchesService
{
	private readonly ILogger<MatchesService> _logger;
	private readonly IPlayerService _playerService;
	private readonly IMapper _mapper;
	private readonly OtrContext _context;

	public MatchesService(ILogger<MatchesService> logger, IPlayerService playerService, IMapper mapper, OtrContext context) : base(logger, context)
	{
		_logger = logger;
		_playerService = playerService;
		_mapper = mapper;
		_context = context;
	}

	public async Task<IEnumerable<MatchDTO>> GetAllAsync(bool onlyIncludeFiltered)
	{
		using (_context)
		{
			var query = _context.Matches
			                    .Include(m => m.Games)
			                    .ThenInclude(g => g.Scores)
			                    .Include(m => m.Games)
			                    .ThenInclude(g => g.Beatmap).AsQueryable();

			if (onlyIncludeFiltered)
			{
				query = query.Where(m => (m.VerificationStatus == (int) VerificationStatus.Verified || m.VerificationStatus == (int)VerificationStatus.PreVerified) &&
				                         m.Games.Any(g => g.Scores.Any(ms => ms.Score > 10000) &&
				                                          g.Beatmap!.DrainTime > 20 &&
				                                          g.Beatmap!.Sr < 12 &&
				                                          g.ScoringType == (int) OsuEnums.ScoringType.ScoreV2)); // Score v2 only
			}

			var matches = await query.ToListAsync();

			if (onlyIncludeFiltered)
			{
				var matchesToRemove = new List<Models.Match>();
				foreach (var match in matches)
				{
					var gamesToRemove = new List<Models.Game>();
					foreach (var game in match.Games)
					{
						if ((game.Scores.Count % 2) != 0 || game.Scores.Count == 0 || !IsValidModCombination(game.ModsEnum))
						{
							gamesToRemove.Add(game);
							continue;
						}

						foreach (var score in game.Scores)
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
	}

	public async Task<MatchDTO?> GetByOsuMatchIdAsync(long osuMatchId)
	{
		using (_context)
		{
			return _mapper.Map<MatchDTO?>(await _context.Matches
			                                            .Include(x => x.Games)
			                                            .ThenInclude(x => x.Beatmap)
			                                            .Include(x => x.Games)
			                                            .ThenInclude(x => x.Scores)
			                                            .FirstOrDefaultAsync(x => x.MatchId == osuMatchId));
		}
	}

	public async Task<Models.Match?> GetFirstPendingOrDefaultAsync()
	{
		using (_context)
		{
			return await _context.Matches.FirstOrDefaultAsync(x => x.VerificationStatus == (int)VerificationStatus.PendingVerification);
		}
	}

	public async Task<IEnumerable<long>> CheckExistingAsync(IEnumerable<long> matchIds)
	{
		using (_context)
		{
			return await _context.Matches.Where(x => matchIds.Contains(x.MatchId)).Select(x => x.MatchId).ToListAsync();
		}
	}

	public async Task<int> InsertFromIdBatchAsync(IEnumerable<Models.Match> matches)
	{
		using (_context)
		{
			await _context.Matches.AddRangeAsync(matches);
			return await _context.SaveChangesAsync();
		}
	}

	public async Task<bool> CreateFromApiMatchAsync(OsuApiMatchData osuMatch)
	{
		using (_context)
		{
			try
			{
				// Step 1.
				var osuPlayerIds = osuMatch.Games.SelectMany(x => x.Scores).Select(x => x.UserId).Distinct().ToList();
				var existingPlayers = await _context.Players.Where(p => osuPlayerIds.Contains(p.OsuId)).ToListAsync();
				var playerIdMapping = existingPlayers.ToDictionary(player => player.OsuId, player => player.Id);

				// Create the players that don't exist
				var playersToCreate = osuPlayerIds.Except(existingPlayers.Select(x => x.OsuId)).ToList();
				foreach (long playerId in playersToCreate)
				{
					var newPlayer = new Player { OsuId = playerId };
					var player = await _playerService.CreateAsync(newPlayer);
					playerIdMapping.Add(player.OsuId, player.Id);
				}

				// Step 2.
				var osuBeatmapIds = osuMatch.Games.Select(x => x.BeatmapId).Distinct().ToList();
				var existingBeatmaps = await _context.Beatmaps.Where(b => osuBeatmapIds.Contains(b.BeatmapId)).ToListAsync();
				var beatmapIdMapping = existingBeatmaps.ToDictionary(beatmap => beatmap.BeatmapId, beatmap => beatmap.Id);

				// Step 3.
				var existingMatch = await _context.Matches.FirstOrDefaultAsync(m => m.MatchId == osuMatch.Match.MatchId);
				if (existingMatch == null)
				{
					var dbMatch = new Models.Match
					{
						MatchId = osuMatch.Match.MatchId,
						Name = osuMatch.Match.Name,
						StartTime = osuMatch.Match.StartTime,
						EndTime = osuMatch.Match.EndTime,
						VerificationStatus = (int)VerificationStatus.PendingVerification
					};

					_context.Matches.Add(dbMatch);
					await _context.SaveChangesAsync();

					existingMatch = await _context.Matches.FirstAsync(m => m.MatchId == osuMatch.Match.MatchId);
				}

				// Step 4.
				foreach (var game in osuMatch.Games)
				{
					var existingGame = await _context.Games.FirstOrDefaultAsync(g => g.GameId == game.GameId);
					if (existingGame == null)
					{
						var dbGame = new Models.Game
						{
							MatchId = existingMatch.Id,
							GameId = game.GameId,
							StartTime = game.StartTime,
							EndTime = game.EndTime,
							BeatmapId = beatmapIdMapping[game.BeatmapId],
							PlayMode = (int)game.PlayMode,
							MatchType = (int)game.MatchType,
							ScoringType = (int)game.ScoringType,
							TeamType = (int)game.TeamType,
							Mods = (int)game.Mods
						};

						_context.Games.Add(dbGame);
						await _context.SaveChangesAsync();
					}
				}

				// Step 5.
				foreach (var game in osuMatch.Games)
				{
					var dbGame = await _context.Games.FirstOrDefaultAsync(g => g.GameId == game.GameId);
					if (dbGame == null)
					{
						_logger.LogError("Failed to fetch game {GameId}", game.GameId);
						continue;
					}

					foreach (var score in game.Scores)
					{
						var existingMatchScore = await _context.MatchScores.FirstOrDefaultAsync(s => s.PlayerId == playerIdMapping[score.UserId] && s.GameId == dbGame.Id);
						if (existingMatchScore == null)
						{
							var dbMatchScore = new MatchScore
							{
								PlayerId = playerIdMapping[score.UserId],
								GameId = dbGame.Id,
								Team = (int)score.Team,
								Score = score.PlayerScore,
								MaxCombo = score.MaxCombo,
								Count50 = score.Count50,
								Count100 = score.Count100,
								Count300 = score.Count300,
								CountMiss = score.CountMiss,
								CountKatu = score.CountKatu,
								CountGeki = score.CountGeki,
								Perfect = score.Perfect == 1,
								Pass = score.Pass == 1,
								EnabledMods = (int?)score.EnabledMods
							};

							_context.MatchScores.Add(dbMatchScore);
							await _context.SaveChangesAsync();
						}
					}
				}

				return true;
			}
			catch (Exception e)
			{
				_logger.LogError(e, "An unhandled exception occurred while processing match {MatchId}", osuMatch.Match.MatchId);
				return false;
			}
		}
	}

	public async Task<int> UpdateVerificationStatusAsync(long matchId, VerificationStatus status, MatchVerificationSource source, string? info = null)
	{
		using (_context)
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
			return await _context.SaveChangesAsync();
		}
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