using API.DTOs;
using API.Enums;
using API.Services.Interfaces;
using API.Utilities;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace API.Controllers;

[ApiController]
[ApiVersion(1)]
[Route("api/v{version:apiVersion}/[controller]")]
public class MatchesController(IMatchesService matchesService) : Controller
{
    [HttpPost("checks/refresh")]
    [Authorize(Roles = $"{OtrClaims.Admin}, {OtrClaims.System}")]
    [EndpointSummary(
        "Sets all matches as requiring automation checks. Should be run if " + "automation checks are altered."
    )]
    public async Task<IActionResult> RefreshAutomationChecksAsync()
    {
        // Marks all matches as needing automation checks
        await matchesService.RefreshAutomationChecks(false);
        return Ok();
    }

    [HttpGet("{id:int}")]
    [Authorize(Roles = $"{OtrClaims.User}, {OtrClaims.Client}")]
    public async Task<ActionResult<MatchDTO>> GetByIdAsync(int id)
    {
        MatchDTO? match = await matchesService.GetAsync(id);

        if (match == null)
        {
            return NotFound($"Failed to locate match {id}");
        }

        return Ok(match);
    }

    [HttpGet("duplicates")]
    [Authorize(Roles = OtrClaims.Admin)]
    [EndpointSummary("Retrieves all known duplicate groups")]
    public async Task<IActionResult> GetDuplicatesAsync() => Ok(await matchesService.GetAllDuplicatesAsync());

    [HttpPost("duplicate")]
    [Authorize(Roles = OtrClaims.Admin)]
    [EndpointSummary("Mark a match as a confirmed or denied duplicate of the root")]
    public async Task<IActionResult> MarkDuplicatesAsync([FromQuery] int rootId,
        [FromQuery]
        bool confirmedDuplicate)
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

        await matchesService.VerifyDuplicatesAsync(loggedInUser.Value, rootId, confirmedDuplicate);
        return Ok();
    }

    // TODO: Should be /player/{osuId}/matches instead.
    [HttpGet("player/{osuId:long}")]
    [Authorize(Roles = $"{OtrClaims.Admin}, {OtrClaims.System}")]
    public async Task<ActionResult<IEnumerable<MatchDTO>>> GetMatchesAsync(long osuId, int mode) =>
        Ok(await matchesService.GetAllForPlayerAsync(osuId, mode, DateTime.MinValue, DateTime.MaxValue));

    [HttpGet("{id:int}/osuid")]
    [Authorize(Roles = OtrClaims.System)]
    public async Task<ActionResult<long>> GetOsuMatchIdByIdAsync(int id)
    {
        MatchDTO? match = await matchesService.GetByOsuIdAsync(id);
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

    /// <summary>
    /// Update the verification status of a match
    /// </summary>
    /// <param name="id">Match id</param>
    /// <param name="patch">JsonPatch data</param>
    /// <response code="404">If a match matching the given id does not exist</response>
    /// <response code="400">If JsonPatch data is malformed</response>
    /// <response code="200">Returns the updated match</response>
    [HttpPatch("{id:int}/verification-status")]
    [Authorize(Roles = OtrClaims.Admin)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ModelStateDictionary>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<MatchDTO>(StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateVerificationStatusAsync(int id, [FromBody] JsonPatchDocument<MatchDTO> patch)
    {
        MatchDTO? match = await matchesService.GetAsync(id, false);
        if (match is null)
        {
            return NotFound();
        }

        if (!patch.IsReplaceOnly())
        {
            return BadRequest();
        }

        patch.ApplyTo(match, ModelState);
        if (!TryValidateModel(match))
        {
            return BadRequest(ModelState);
        }

        if (match.VerificationStatus is null)
        {
            return BadRequest();
        }

        var verifierId = HttpContext.AuthorizedUserIdentity();
        MatchDTO? updatedMatch = await matchesService.UpdateVerificationStatusAsync(
            id,
            (MatchVerificationStatus)match.VerificationStatus,
            MatchVerificationSource.MatchVerifier,
            "Updated manually by an admin",
            verifierId
            );
        return Ok(updatedMatch);
    }
}
