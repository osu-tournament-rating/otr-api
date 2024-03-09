using API.DTOs;
using API.Enums;
using API.Services.Interfaces;
using API.Utilities;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

// ReSharper disable PossibleMultipleEnumeration
namespace API.Controllers;

[ApiController]
[ApiVersion(1)]
[EnableCors]
[Authorize(Roles = "user")]
[Authorize(Roles = "whitelist")]
[Route("api/v{version:apiVersion}/[controller]")]
public class MatchesController(IMatchesService matchesService, ITournamentsService tournamentsService) : Controller
{
    private readonly IMatchesService _matchesService = matchesService;
    private readonly ITournamentsService _tournamentsService = tournamentsService;

    [HttpPost("batch")]
    [Authorize(Roles = "submit")]
    public async Task<IActionResult> PostAsync(
        [FromBody] TournamentWebSubmissionDTO wrapper,
        [FromQuery] bool verified = false
    )
    {
        /*
         * FLOW:
         *
         * The user submits a tournament (containing metadata and links) to the front-end. They are looking to add new data
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

        await _matchesService.BatchInsertOrUpdateAsync(
            wrapper,
            verified,
            (int)MatchVerificationSource.MatchVerifier
        );

        return Ok();
    }

    [HttpPost("checks/refresh")]
    [Authorize(Roles = "admin, system")]
    [EndpointSummary(
        "Sets all matches as requiring automation checks. Should be run if "
            + "automation checks are altered."
    )]
    public async Task<IActionResult> RefreshAutomationChecksAsync()
    {
        // Marks all matches as needing automation checks
        await _matchesService.RefreshAutomationChecks(false);
        return Ok();
    }

    [HttpGet("ids")]
    [Authorize(Roles = "system")]
    [EndpointSummary("Returns all verified match ids")]
    public async Task<ActionResult<IEnumerable<int>>> GetAllAsync()
    {
        IEnumerable<int> matches = await _matchesService.GetAllIdsAsync(true);
        return Ok(matches);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<MatchDTO>> GetByIdAsync(int id)
    {
        MatchDTO? match = await _matchesService.GetAsync(id);

        if (match == null)
        {
            return NotFound($"Failed to locate match {id}");
        }

        return Ok(match);
    }

    [HttpPost("convert")]
    [Authorize(Roles = "system")]
    [EndpointSummary("Converts a list of match ids to MatchDTO objects")]
    [EndpointDescription(
        "This is a useful way to fetch a list of matches without starving the "
            + "program of memory. This is used by the rating processor to fetch matches"
    )]
    public async Task<ActionResult<IEnumerable<MatchDTO>>> ConvertAsync([FromBody] IEnumerable<int> ids)
    {
        IEnumerable<MatchDTO> matches = await _matchesService.ConvertAsync(ids);
        return Ok(matches);
    }

    [Authorize(Roles = "admin")]
    [HttpGet("duplicates")]
    [EndpointSummary("Retrieves all known duplicate groups")]
    public async Task<IActionResult> GetDuplicatesAsync() =>
        Ok(await _matchesService.GetAllDuplicatesAsync());

    [Authorize(Roles = "admin")]
    [HttpPost("duplicate")]
    [EndpointSummary("Mark a match as a confirmed or denied duplicate of the root")]
    public async Task<IActionResult> MarkDuplicatesAsync(
        [FromQuery] int rootId,
        [FromQuery] bool confirmedDuplicate
    )
    {
        var loggedInUser = HttpContext.AuthorizedUserIdentity();
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

    // TODO: Should be /player/{osuId}/matches instead.
    [HttpGet("player/{osuId:long}")]
    [Authorize(Roles = "admin, system")]
    public async Task<ActionResult<IEnumerable<MatchDTO>>> GetMatchesAsync(long osuId, int mode) =>
        Ok(await _matchesService.GetAllForPlayerAsync(osuId, mode, DateTime.MinValue, DateTime.MaxValue));

    [HttpGet("{id:int}/osuid")]
    [Authorize(Roles = "system")]
    public async Task<ActionResult<long>> GetOsuMatchIdByIdAsync(int id)
    {
        MatchDTO? match = await _matchesService.GetByOsuIdAsync(id);
        if (match == null)
        {
            return NotFound($"Match with id {id} does not exist");
        }

        var osuMatchId = match.MatchId;
        if (osuMatchId != 0)
        {
            return Ok(osuMatchId);
        }

        return NotFound($"Match with id {id} does not exist");
    }

    [HttpGet("id-mapping")]
    [Authorize(Roles = "system")]
    public async Task<IActionResult> GetIdMappingAsync()
    {
        IEnumerable<MatchIdMappingDTO> mapping = await _matchesService.GetIdMappingAsync();
        return Ok(mapping);
    }

    [HttpPatch("verification-status/{id:int}")]
    [Authorize(Roles = "admin")]
    [EndpointSummary(
        "Takes in json patch for verification status, and returns the patched object. The object being patched is a MatchDTO."
    )]
    [ProducesResponseType<MatchDTO>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> EditVerificationStatus(
        int id,
        [FromBody] JsonPatchDocument<MatchDTO> patch
    )
    {
        MatchDTO? match = await _matchesService.GetAsync(id, false);

        if (match is null)
        {
            return NotFound($"Match with id {id} does not exist");
        }

        //Ensure request is only attempting to perform a replace operation.
        if (patch.Operations.Any(op => op.op != "replace"))
        {
            return BadRequest("This endpoint can only perform replace operations.");
        }

        patch.ApplyTo(match, ModelState);

        if (!TryValidateModel(match))
        {
            return BadRequest(ModelState);
        }

        MatchDTO updatedMatch = await _matchesService.UpdateVerificationStatus(id, match.VerificationStatus);

        return Ok(updatedMatch);
    }
}
