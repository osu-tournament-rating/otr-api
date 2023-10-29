using API.DTOs;
using API.Entities;
using API.Enums;
using API.Services.Interfaces;
using API.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

// ReSharper disable PossibleMultipleEnumeration
namespace API.Controllers;

public class BatchWrapper
{
	public string TournamentName { get; set; } = null!;
	public string Abbreviation { get; set; } = null!;
	public string ForumPost { get; set; } = null!;
	public int RankRangeLowerBound { get; set; }
	public int TeamSize { get; set; }
	public int Mode { get; set; }
	public int SubmitterId { get; set; }
	public IEnumerable<long> Ids { get; set; } = new List<long>();
}

[ApiController]
[EnableCors]
[Authorize]
[Route("api/[controller]")]
public class MatchesController : Controller
{
	private readonly ILogger<MatchesController> _logger;
	private readonly IMatchesService _matchesService;
	private readonly ITournamentsService _tournamentsService;

	public MatchesController(ILogger<MatchesController> logger, IMatchesService matchesService, ITournamentsService tournamentsService)
	{
		_logger = logger;
		_matchesService = matchesService;
		_tournamentsService = tournamentsService;
	}

	[HttpPost("batch")]
	public async Task<IActionResult> PostAsync([FromBody] BatchWrapper wrapper, [FromQuery] bool verified = false)
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
		int verifier = (int) MatchVerificationSource.MatchVerifier;

		if (User.IsAdmin())
		{
			verifier = (int) MatchVerificationSource.Admin;
		}

		if (User.IsSystem())
		{
			verifier = (int) MatchVerificationSource.System;
		}

		return verifier;
	}

	[HttpPost("refresh/automations")]
	[Authorize(Roles = "Admin, System")]
	public async Task<IActionResult> RefreshAutomationChecksAsync()
	{
		// Marks invalid matches as needing automation checks
		await _matchesService.RefreshAutomationChecks(false);
		return Ok();
	}
	
	[HttpGet("all")]
	[Authorize(Roles = "Admin, System")]
	public async Task<ActionResult<IEnumerable<MatchDTO>>> GetAllAsync()
	{
		var matches = await _matchesService.GetAllAsync(true);
		return Ok(matches);
	}

	[HttpGet("{osuMatchId:long}")]
	[Authorize(Roles = "Admin, System")]
	public async Task<ActionResult<Match>> GetByOsuMatchIdAsync(long osuMatchId)
	{
		var match = await _matchesService.GetAsync(osuMatchId);

		if (match == null)
		{
			return NotFound($"Failed to locate match {osuMatchId}");
		}

		return Ok(match);
	}

	[HttpGet("player/{osuId:long}")]
	[Authorize(Roles = "Admin, System")]
	public async Task<ActionResult<IEnumerable<MatchDTO>>> GetMatchesAsync(long osuId, int mode) => Ok(await _matchesService.GetAllForPlayerAsync(osuId, mode, DateTime.MinValue, DateTime.MaxValue));

	[HttpGet("{id:int}/osuid")]
	[Authorize(Roles = "Admin, System")]
	public async Task<ActionResult<long>> GetOsuMatchIdByIdAsync(int id)
	{
		var match = await _matchesService.GetAsync(id);
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
	public async Task<ActionResult<Dictionary<long, int>>> GetIdMappingAsync()
	{
		var mapping = await _matchesService.GetIdMappingAsync();
		return Ok(mapping);
	}
}