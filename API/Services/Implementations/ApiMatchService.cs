using API.Entities;
using API.Osu.Multiplayer;
using API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Services.Implementations;

public class ApiMatchService : IApiMatchService
{
	private readonly OtrContext _context;
	private readonly IPlayerService _playerService;
	private readonly ILogger<ApiMatchService> _logger;

	public ApiMatchService(ILogger<ApiMatchService> logger, OtrContext context, IPlayerService playerService)
	{
		_logger = logger;
		_context = context;
		_playerService = playerService;
	}

	public async Task CreateFromApiMatchAsync(OsuApiMatchData apiMatch)
	{
		_logger.LogInformation("Processing match {MatchId}", apiMatch.Match.MatchId);

		await CreatePlayersAsync(apiMatch);
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
	/// <param name="apiMatch"></param>
	private async Task SaveBeatmapsAsync(OsuApiMatchData apiMatch)
	{
		var beatmapIds = GetBeatmapIds(apiMatch);
		
		// Create them if they don't exist
		await _context.Beatmaps.AddRangeAsync(beatmapIds.Select(x => new Beatmap { BeatmapId = x }));
	}

	private IEnumerable<long> GetBeatmapIds(OsuApiMatchData apiMatch)
	{
		return apiMatch.Games.Select(x => x.BeatmapId);
	}
}