using API.Authorization;
using API.DTOs;
using API.Services.Interfaces;
using API.Utilities.Extensions;
using Asp.Versioning;
using Database.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[ApiVersion(1)]
[Route("api/v{version:apiVersion}/[controller]")]
public partial class MatchesController(IMatchesService matchesService, IAdminNoteService adminNoteService) : Controller
{
    /// <summary>
    /// Get all matches which fit an optional request query
    /// </summary>
    /// <remarks>Will not include game data</remarks>
    /// <response code="200">Returns all matches which fit the request query</response>
    [HttpGet]
    [Authorize(Roles = $"{OtrClaims.Roles.User}, {OtrClaims.Roles.Client}")]
    [ProducesResponseType<IEnumerable<MatchDTO>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> ListAsync([FromQuery] MatchRequestQueryDTO requestQuery) =>
        Ok(await matchesService.GetAsync(requestQuery));

    /// <summary>
    /// Get a match
    /// </summary>
    /// <param name="id">Match id</param>
    /// <response code="404">If a match does not exist for the given id</response>
    /// <response code="200">Returns a match</response>
    [HttpGet("{id:int}")]
    [Authorize(Roles = $"{OtrClaims.Roles.User}, {OtrClaims.Roles.Client}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType<MatchDTO>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAsync(int id)
    {
        MatchDTO? match = await matchesService.GetAsync(id);

        return match is null
            ? NotFound()
            : Ok(match);
    }

    // TODO: Should be /player/{osuId}/matches instead.
    [HttpGet("player/{osuId:long}")]
    [Authorize(Roles = OtrClaims.Roles.Admin)]
    public async Task<IActionResult> GetMatchesAsync(long osuId, Ruleset ruleset) =>
        Ok(await matchesService.GetAllForPlayerAsync(osuId, ruleset, DateTime.MinValue, DateTime.MaxValue));

    /// <summary>
    ///  Amend match data
    /// </summary>
    /// <param name="id">The match id</param>
    /// <param name="patch">JsonPatch data</param>
    /// <response code="404">If the provided id does not belong to a match</response>
    /// <response code="400">If JsonPatch data is malformed</response>
    /// <response code="200">Returns the patched match</response>
    /// <returns></returns>
    [HttpPatch("{id:int}")]
    [Authorize(Roles = OtrClaims.Roles.Admin)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<string>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<MatchDTO>(StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateAsync(int id, [FromBody] JsonPatchDocument<MatchDTO> patch)
    {
        MatchDTO? match = await matchesService.GetAsync(id);
        if (match is null)
        {
            return NotFound();
        }

        // Ensure request is only attempting to perform a replace operation.
        if (!patch.IsReplaceOnly())
        {
            return BadRequest();
        }

        patch.ApplyTo(match, ModelState);
        if (!TryValidateModel(match))
        {
            return BadRequest(ModelState.ErrorMessage());
        }

        MatchDTO? updatedMatch = await matchesService.UpdateAsync(id, match);
        return Ok(updatedMatch!);
    }

    /// <summary>
    /// Delete a match
    /// </summary>
    /// <param name="id">Match id</param>
    /// <response code="404">The match does not exist</response>
    /// <response code="204">The match was deleted successfully</response>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = OtrClaims.Roles.Admin)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        MatchDTO? result = await matchesService.GetAsync(id);
        if (result is null)
        {
            return NotFound();
        }

        await matchesService.DeleteAsync(id);
        return NoContent();
    }
}
