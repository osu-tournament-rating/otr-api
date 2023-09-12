using API.DTOs;
using API.Entities;
using API.Enums;
using API.Osu;
using API.Osu.Multiplayer;
using API.Services.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace API.Services.Implementations;

public class MatchesService : ServiceBase<Entities.Match>, IMatchesService
{
	private readonly OtrContext _context;
	private readonly ILogger<MatchesService> _logger;
	private readonly IMapper _mapper;
	private readonly IPlayerService _playerService;

	public MatchesService(ILogger<MatchesService> logger, IPlayerService playerService, IMapper mapper, OtrContext context) : base(logger, context)
	{
		_logger = logger;
		_playerService = playerService;
		_mapper = mapper;
		_context = context;
	}

	public async Task<IEnumerable<MatchDTO>> GetAllAsync(bool onlyIncludeFiltered)
	{
		var query = _context.Matches
		                    .Include(m => m.Games)
		                    .ThenInclude(g => g.MatchScores)
		                    .Include(m => m.Games)
		                    .ThenInclude(g => g.Beatmap)
		                    .AsQueryable();

		if (onlyIncludeFiltered)
		{
			query = query.Where(m => (m.VerificationStatus == (int)MatchVerificationStatus.Verified || m.VerificationStatus == (int)MatchVerificationStatus.PreVerified) &&
			                         m.Games.Any(g => g.MatchScores.Any(ms => ms.Score > 10000) &&
			                                          g.Beatmap!.DrainTime > 20 &&
			                                          g.Beatmap!.Sr < 12 &&
			                                          g.ScoringType == (int)OsuEnums.ScoringType.ScoreV2)); // Score v2 only
		}

		var matches = await query.ToListAsync();

		if (onlyIncludeFiltered)
		{
			var matchesToRemove = new List<Entities.Match>();
			foreach (var match in matches)
			{
				var gamesToRemove = new List<Entities.Game>();
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

	public async Task<MatchDTO?> GetByOsuMatchIdAsync(long osuMatchId) => _mapper.Map<MatchDTO?>(await _context.Matches
	                                                                                                           .Include(x => x.Games)
	                                                                                                           .ThenInclude(x => x.Beatmap)
	                                                                                                           .Include(x => x.Games)
	                                                                                                           .ThenInclude(x => x.MatchScores)
	                                                                                                           .FirstOrDefaultAsync(x => x.MatchId == osuMatchId));

	public async Task<Entities.Match?> GetFirstPendingOrDefaultAsync() =>
		await _context.Matches.FirstOrDefaultAsync(x => x.VerificationStatus == (int)MatchVerificationStatus.PendingVerification);

	public async Task<IEnumerable<long>> CheckExistingAsync(IEnumerable<long> matchIds) =>
		await _context.Matches.Where(x => matchIds.Contains(x.MatchId)).Select(x => x.MatchId).ToListAsync();

	public async Task<int> InsertFromIdBatchAsync(IEnumerable<Entities.Match> matches)
	{
		await _context.Matches.AddRangeAsync(matches);
		return await _context.SaveChangesAsync();
	}

	public async Task<bool> CreateFromApiMatchAsync(OsuApiMatchData osuMatch)
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
				var dbMatch = new Entities.Match
				{
					MatchId = osuMatch.Match.MatchId,
					Name = osuMatch.Match.Name,
					StartTime = osuMatch.Match.StartTime,
					EndTime = osuMatch.Match.EndTime,
					VerificationStatus = (int)MatchVerificationStatus.PendingVerification
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
					var dbGame = new Entities.Game
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
		return await _context.SaveChangesAsync();
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