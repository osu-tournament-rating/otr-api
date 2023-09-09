using API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OsuMatchesController : Controller
{
	private readonly ILogger<OsuMatchesController> _logger;
	private readonly IMatchesService _service;
	private readonly IGamesService _gamesService;
	private readonly IMatchScoresService _scoresService;
	
	public OsuMatchesController(ILogger<OsuMatchesController> logger, 
		IMatchesService service, IGamesService gamesService, IMatchScoresService scoresService)
	{
		_logger = logger;
		_service = service;
		_gamesService = gamesService;
		_scoresService = scoresService;
	}

	[HttpPost("batch")]
	public async Task<ActionResult<int>> PostAsync([FromBody] IEnumerable<long> ids)
	{
		/**
		 * FLOW:
		 *
		 * The user submits a batch of links to the front-end. They are looking to add new data
		 * to our database that will eventually count towards ratings.
		 *
		 * This post endpoint takes these links, validates them (i.e. checks for duplicates,
		 * whether the match titles align with osu! tournament naming conventions,
		 * amount of matches being submitted, etc.).
		 *
		 * Assuming we have a good batch, we will mark all of the new items as "PENDING".
		 * The API.Osu.Multiplayer.MultiplayerLobbyDataWorker service checks the database for pending links
		 * periodically and processes them automatically.
		 */

		// Check if any of the links already exist in the database
		ids = ids.ToList();
		var existingMatches = await _service.CheckExistingAsync(ids);
		var stripped = ids.Except(existingMatches).ToList();
		
		var matches = stripped.Select(id => new Match { MatchId = id, VerificationStatus = VerificationStatus.PendingVerification });
		int? result = await _service.InsertFromIdBatchAsync(matches);
		if (result > 0)
		{
			_logger.LogInformation("Successfully marked {Matches} matches as {Status}", result, VerificationStatus.PendingVerification);
			return Ok();
		}
		
		return StatusCode(500, $"Failed to insert matches");
	}
	
	[HttpGet("all")]
	public async Task<ActionResult<IEnumerable<Match>?>> GetAllAsync()
	{
		var matches = (await _service.GetAllAsync(true)).ToList();
		return Ok(matches);
	}
	
	[HttpGet("{matchId:long}")]
	public async Task<ActionResult<Match>> GetByOsuMatchIdAsync(long matchId)
	{
		var match = await _service.GetByOsuGameIdAsync(matchId);

		if (match == null)
		{
			return NotFound($"Failed to locate match {matchId}");
		}
		
		var games = (await _gamesService.GetByMatchIdAsync(match.Id)).ToList();
		match.Games = games;
		
		foreach (var game in games)
		{
			var scores = (await _scoresService.GetForGameAsync(game.Id)).ToList();
			game.Scores = scores;
		}
		
		return Ok(match);
	}
}