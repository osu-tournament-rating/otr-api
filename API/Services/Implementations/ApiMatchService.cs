using API.Entities;
using API.Enums;
using API.Osu.Multiplayer;
using API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Services.Implementations;

public class ApiMatchService : IApiMatchService
{
	private readonly OtrContext _context;
	private readonly IPlayerService _playerService;
	private readonly IBeatmapService _beatmapService;
	private readonly ILogger<ApiMatchService> _logger;

	public ApiMatchService(ILogger<ApiMatchService> logger, OtrContext context, IPlayerService playerService, IBeatmapService beatmapService)
	{
		_logger = logger;
		_context = context;
		_playerService = playerService;
		_beatmapService = beatmapService;
	}

	public async Task CreateFromApiMatchAsync(OsuApiMatchData apiMatch)
	{
		_logger.LogInformation("Processing match {MatchId}", apiMatch.Match.MatchId);

		await CreatePlayersAsync(apiMatch);
		await CreateBeatmapsAsync(apiMatch);
		await CreateMatchAsync(apiMatch);
	}

	private async Task CreatePlayersAsync(OsuApiMatchData apiMatch)
	{
		var existingPlayersMapping = await ExistingPlayersMapping(apiMatch);

		if (existingPlayersMapping == null || !existingPlayersMapping.Any())
		{
			return;
		}

		foreach (long osuId in GetUserIdsFromMatch(apiMatch)!)
		{
			var newPlayer = new Player { OsuId = osuId };
			var player = await _playerService.CreateAsync(newPlayer);
			
			_logger.LogInformation("Saved new player: {PlayerId} (osuId: {OsuId})", player.Id, osuId);
		}
	}

	/// <summary>
	/// Gets all players from the database that are in the match.
	/// </summary>
	/// <param name="apiMatch"></param>
	/// <returns></returns>
	private async Task<Dictionary<long, int>?> ExistingPlayersMapping(OsuApiMatchData apiMatch)
	{
		// Select all osu! user ids from the match's scores
		var osuPlayerIds = GetUserIdsFromMatch(apiMatch);

		if (!osuPlayerIds?.Any() ?? true)
		{
			_logger.LogError("No players found in match {MatchId}", apiMatch.Match.MatchId);
			return null;
		}

		// Select all players from the database that are in the match
		var existingPlayers = await _context.Players.Where(p => osuPlayerIds.Contains(p.OsuId)).ToListAsync();

		_logger.LogTrace("Identified {Count} existing players to add for match {MatchId}", existingPlayers.Count, apiMatch.Match.MatchId);

		// Map these osu! ids to their database ids
		return existingPlayers.ToDictionary(player => player.OsuId, player => player.Id);
	}

	private List<long>? GetUserIdsFromMatch(OsuApiMatchData apiMatch) => apiMatch.Games.SelectMany(x => x.Scores).Select(x => x.UserId).Distinct().ToList();
	
	// Beatmaps
	
	/// <summary>
	/// Saves the beatmaps identified in the match to the database.
	/// </summary>
	private async Task CreateBeatmapsAsync(OsuApiMatchData apiMatch)
	{
		var beatmapIds = GetBeatmapIds(apiMatch);
		await _beatmapService.CreateIfNotExistsAsync(beatmapIds);
	}

	private IEnumerable<long> GetBeatmapIds(OsuApiMatchData apiMatch)
	{
		return apiMatch.Games.Select(x => x.BeatmapId);
	}
	
	// Match
	private async Task CreateMatchAsync(OsuApiMatchData apiMatch)
	{
		var existingMatch = await ExistingMatch(apiMatch);
		
		if (existingMatch == null || MatchNeedsPopulation(apiMatch, existingMatch))
		{
			await CreatePendingMatchAsync(apiMatch);
			existingMatch = await ExistingMatch(apiMatch);
		}
	}
	
	private async Task<Entities.Match?> ExistingMatch(OsuApiMatchData apiMatch) => await _context.Matches.FirstOrDefaultAsync(x => x.MatchId == apiMatch.Match.MatchId);

	private bool MatchNeedsPopulation(OsuApiMatchData apiMatch, Entities.Match existingMatch)
	{
		if (apiMatch.Match.MatchId != existingMatch.MatchId)
		{
			_logger.LogError("Match ID mismatch during processing: {ApiMatchId} vs {ExistingMatchId}", apiMatch.Match.MatchId, existingMatch.MatchId);
			return false;
		}

		return existingMatch.Name != apiMatch.Match.Name;
	}

	private async Task CreatePendingMatchAsync(OsuApiMatchData apiMatch)
	{
		var match = new Entities.Match
		{
			MatchId = apiMatch.Match.MatchId,
			Name = apiMatch.Match.Name,
			StartTime = apiMatch.Match.StartTime,
			EndTime = apiMatch.Match.EndTime,
			VerificationStatus = (int) MatchVerificationStatus.PendingVerification
		};

		await _context.Matches.AddAsync(match);
		await _context.SaveChangesAsync();
		
		_logger.LogInformation("Saved new match: {MatchId} (name: {MatchName})", match.Id, match.Name);
	}
}