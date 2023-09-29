using API.DTOs;
using API.Entities;
using API.Enums;
using API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// ReSharper disable PossibleMultipleEnumeration
namespace API.Controllers;

public class BatchWrapper
{
	public string? TournamentName { get; set; }
	public string? Abbreviation { get; set; }
	public string ForumPost { get; set; } = null!;
	public int RankRangeLowerBound { get; set; }
	public int TeamSize { get; set; }
	public int Mode { get; set; }
	public int SubmitterId { get; set; }
	public IEnumerable<long> Ids { get; set; } = new List<long>();
}

[ApiController]
[Authorize(Roles = "Admin, System")]
[Route("api/[controller]")]
public class OsuMatchesController : Controller
{
	private readonly IGamesService _gamesService;
	private readonly ILogger<OsuMatchesController> _logger;
	private readonly IMatchScoresService _scoresService;
	private readonly IPlayerService _playerService;
	private readonly IMatchesService _service;

	public OsuMatchesController(ILogger<OsuMatchesController> logger,
		IMatchesService service, IGamesService gamesService, IMatchScoresService scoresService, IPlayerService playerService)
	{
		_logger = logger;
		_service = service;
		_gamesService = gamesService;
		_scoresService = scoresService;
		_playerService = playerService;
	}

	[HttpPost("force-update")]
	public async Task<ActionResult> ForceUpdateAsync([FromBody] long id) =>
		Ok(await _service.UpdateVerificationStatusAsync(id, MatchVerificationStatus.PendingVerification, MatchVerificationSource.Admin));

	[HttpPost("batch")]
	public async Task<ActionResult<int>> PostAsync([FromBody] BatchWrapper wrapper, [FromQuery] bool verified = false)
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
		var ids = wrapper.Ids;
		ids = ids.ToList();
		var existingMatches = await _service.CheckExistingAsync(ids);

		// If we are verifying a match that already exists, we need to update the verification status
		if (verified)
		{
			// Check authorization
			if (!User.IsInRole("Admin"))
			{
				return Unauthorized("You are not authorized to verify matches");
			}
			
			foreach (var verifiedMatch in existingMatches)
			{
				verifiedMatch.VerificationStatus = (int)MatchVerificationStatus.Verified;
				verifiedMatch.VerificationSource = (int)MatchVerificationSource.Admin;
				verifiedMatch.TournamentName = wrapper.TournamentName;
				verifiedMatch.Abbreviation = wrapper.Abbreviation;
				verifiedMatch.Forum = wrapper.ForumPost;
				verifiedMatch.SubmitterUserId = wrapper.SubmitterId;
				verifiedMatch.RankRangeLowerBound = wrapper.RankRangeLowerBound;
				verifiedMatch.TeamSize = wrapper.TeamSize;
				verifiedMatch.Mode = wrapper.Mode;
				verifiedMatch.Updated = DateTime.UtcNow;
				await _service.UpdateAsync(verifiedMatch);
				_logger.LogInformation("Updated {@Match}", verifiedMatch);
			}
		}

		// Continue processing the rest of the links

		var stripped = ids.Except(existingMatches.Select(x => x.MatchId)).ToList();

		var verification = MatchVerificationStatus.PendingVerification;
		if (verified)
		{
			verification = MatchVerificationStatus.Verified;
		}

		var matches = stripped.Select(id => new Match
		{
			MatchId = id, VerificationStatus = (int)verification,
			TournamentName = wrapper.TournamentName,
			Abbreviation = wrapper.Abbreviation,
			Forum = wrapper.ForumPost,
			SubmitterUserId = wrapper.SubmitterId,
			RankRangeLowerBound = wrapper.RankRangeLowerBound,
			TeamSize = wrapper.TeamSize,
			Mode = wrapper.Mode
		});

		int? result = await _service.InsertFromIdBatchAsync(matches);
		if (result > 0)
		{
			_logger.LogInformation("Successfully marked {Matches} matches as {Status}", result, MatchVerificationStatus.PendingVerification);
		}

		return Ok();
	}

	[HttpGet("all")]
	public async Task<ActionResult<IEnumerable<Match>?>> GetAllAsync()
	{
		var matches = (await _service.GetAllAsync(true)).ToList();
		return Ok(matches);
	}

	[HttpGet("{osuMatchId:long}")]
	public async Task<ActionResult<Match>> GetByOsuMatchIdAsync(long osuMatchId)
	{
		var match = await _service.GetByOsuMatchIdAsync(osuMatchId);

		if (match == null)
		{
			return NotFound($"Failed to locate match {osuMatchId}");
		}

		return Ok(match);
	}

	[HttpGet("player/{osuId:long}")]
	public async Task<ActionResult<IEnumerable<Unmapped_PlayerMatchesDTO>>> GetMatchesAsync(long osuId) => Ok(await _service.GetPlayerMatchesAsync(osuId, DateTime.MinValue));

	[HttpGet("{id:int}/osuid")]
	public async Task<ActionResult<long>> GetOsuMatchIdByIdAsync(int id)
	{
		var match = await _service.GetAsync(id);
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
}