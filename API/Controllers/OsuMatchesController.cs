using API.Entities;
using API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OsuMatchesController : Controller
{
	private readonly ILogger<OsuMatchesController> _logger;
	private readonly IMatchesService _service;

	public OsuMatchesController(ILogger<OsuMatchesController> logger, IMatchesService service)
	{
		_logger = logger;
		_service = service;
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
		var matches = await _service.GetAllAsync();
		return Ok(matches);
	}
}