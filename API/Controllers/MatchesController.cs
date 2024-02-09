using API.DTOs;
using API.Enums;
using API.Services.Interfaces;
using API.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

// ReSharper disable PossibleMultipleEnumeration
namespace API.Controllers;

[ApiController]
[EnableCors]
[Authorize]
[Route("api/[controller]")]
public class MatchesController : Controller
{
	private readonly IMatchesService _matchesService;
	private readonly ITournamentsService _tournamentsService;

	public MatchesController(IMatchesService matchesService, ITournamentsService tournamentsService)
	{
		_matchesService = matchesService;
		_tournamentsService = tournamentsService;
	}

	[HttpPost("batch")]
	public async Task<IActionResult> PostAsync([FromBody] MatchWebSubmissionDTO wrapper, [FromQuery] bool verified = false)
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

		if (verified && !User.IsMatchVerifier())
		{
			return Unauthorized("You are not authorized to verify matches");
		}

		// Gather tournament information
		if (!verified && await _tournamentsService.ExistsAsync(wrapper.TournamentName, wrapper.Mode))
		{
			return BadRequest($"Tournament {wrapper.TournamentName} already exists for this mode");
		}

		await _matchesService.BatchInsertOrUpdateAsync(wrapper, verified, IdentifyVerifier(verified));

		return Ok();
	}

	private int? IdentifyVerifier(bool verified)
	{
		if (!verified)
		{
			return null;
		}

		// We need to know what entity verified the match
		int verifier = (int)MatchVerificationSource.MatchVerifier;

		if (User.IsAdmin())
		{
			verifier = (int)MatchVerificationSource.Admin;
		}

		if (User.IsSystem())
		{
			verifier = (int)MatchVerificationSource.System;
		}

		return verifier;
	}

	[HttpPost("refresh/automations")]
	[Authorize(Roles = "Admin, System")]
	public async Task<IActionResult> RefreshAutomationChecksAsync()
	{
		// Marks all matches as needing automation checks
		await _matchesService.RefreshAutomationChecks(false);
		return Ok();
	}

	[HttpGet("ids")]
	[Authorize(Roles = "Admin, System")]
	[EndpointSummary("Returns all verified match ids")]
	public async Task<ActionResult<IEnumerable<int>>> GetAllAsync()
	{
		var matches = await _matchesService.GetAllIdsAsync(true);
		return Ok(matches);
	}

	[HttpGet("{id:int}")]
	[Authorize(Roles = "Admin, System")]
	public async Task<ActionResult<MatchDTO>> GetByIdAsync(int id)
	{
		var match = await _matchesService.GetAsync(id);

		if (match == null)
		{
			return NotFound($"Failed to locate match {id}");
		}

		return Ok(match);
	}

	[HttpPost("convert")]
	[EndpointSummary("Converts a list of match ids to MatchDTO objects")]
	[EndpointDescription("This is a useful way to fetch a list of matches without starving the " +
	                     "program of memory. This is used by the rating processor to fetch matches")]
	public async Task<ActionResult<IEnumerable<MatchDTO>>> ConvertAsync([FromBody] IEnumerable<int> ids)
	{
		var matches = await _matchesService.ConvertAsync(ids);
		return Ok(matches);
	}

	[Authorize(Roles = "Admin")]
	[HttpGet("duplicates")]
	[EndpointSummary("Retreives all known duplicate groups")]
	public async Task<IActionResult> GetDuplicatesAsync() => Ok(await _matchesService.GetAllDuplicatesAsync());

	[Authorize(Roles = "Admin")]
	[HttpPost("duplicate")]
	[EndpointSummary("Mark a match as a confirmed or denied duplicate of the root")]
	public async Task<IActionResult> MarkDuplicatesAsync([FromQuery] int rootId, [FromQuery] bool confirmedDuplicate)
	{
		int? loggedInUser = HttpContext.AuthorizedUserIdentity();
		if (!loggedInUser.HasValue)
		{
			return Unauthorized("You must be logged in to perform this action.");
		}

		if (!HttpContext.User.IsAdmin())
		{
			return Unauthorized("You lack permissions to perform this action.");
		}

		await _matchesService.VerifyDuplicatesAsync(loggedInUser.Value, rootId, confirmedDuplicate);
		return Ok();
	}

	// [HttpGet("{osuMatchId:long}")]
	// [Authorize(Roles = "Admin, System")]
	// public async Task<ActionResult<Match>> GetByOsuMatchIdAsync(long osuMatchId)
	// {
	// 	var match = await _matchesService.GetAsync(osuMatchId);
	//
	// 	if (match == null)
	// 	{
	// 		return NotFound($"Failed to locate match {osuMatchId}");
	// 	}
	//
	// 	return Ok(match);
	// }

	[HttpGet("player/{osuId:long}")]
	[Authorize(Roles = "Admin, System")]
	public async Task<ActionResult<IEnumerable<MatchDTO>>> GetMatchesAsync(long osuId, int mode) =>
		Ok(await _matchesService.GetAllForPlayerAsync(osuId, mode, DateTime.MinValue, DateTime.MaxValue));

	[HttpGet("{id:int}/osuid")]
	[Authorize(Roles = "Admin, System")]
	public async Task<ActionResult<long>> GetOsuMatchIdByIdAsync(int id)
	{
		var match = await _matchesService.GetByOsuIdAsync(id);
		if (match == null)
		{
			return NotFound($"Match with id {id} does not exist");
		}

		long osuMatchId = match.MatchId;
		if (osuMatchId != 0)
		{
			return Ok(osuMatchId);
		}

		return NotFound($"Match with id {id} does not exist");
	}

	[HttpGet("id-mapping")]
	public async Task<IActionResult> GetIdMappingAsync()
	{
		var mapping = await _matchesService.GetIdMappingAsync();
		return Ok(mapping);
	}
}