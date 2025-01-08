using API.Authorization;
using API.DTOs;
using API.Services.Interfaces;
using API.Utilities.Extensions;
using Asp.Versioning;
using Database.Entities;
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
    /// <param name="verified">Whether all games and subsequent child navigations must be verified</param>
    /// <response code="404">A match matching the given id does not exist</response>
    /// <response code="200">Returns a match</response>
    [HttpGet("{id:int}")]
    [Authorize(Roles = $"{OtrClaims.Roles.User}, {OtrClaims.Roles.Client}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType<MatchDTO>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAsync(int id, [FromQuery] bool verified = true)
    {
        MatchDTO? match = await matchesService.GetAsync(id, verified);

        return match is null
            ? NotFound()
            : Ok(match);
    }

    /// <summary>
    /// Amend match data
    /// </summary>
    /// <param name="id">Match id</param>
    /// <param name="patch">JsonPatch data</param>
    /// <response code="404">A match matching the given id does not exist</response>
    /// <response code="400">The JsonPatch data is malformed</response>
    /// <response code="200">Returns the updated match</response>
    [HttpPatch("{id:int}")]
    [Authorize(Roles = OtrClaims.Roles.Admin)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<MatchDTO>(StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateAsync(int id, [FromBody] JsonPatchDocument<MatchDTO> patch)
    {
        MatchDTO? match = await matchesService.GetAsync(id, false);
        if (match is null)
        {
            return NotFound();
        }

        // Ensure request is only attempting to perform a replace operation.
        if (patch.Operations.Count == 0 || !patch.IsReplaceOnly())
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
    /// <response code="404">A match matching the given id does not exist</response>
    /// <response code="204">The match was deleted successfully</response>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = OtrClaims.Roles.Admin)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        MatchDTO? result = await matchesService.GetAsync(id, false);
        if (result is null)
        {
            return NotFound();
        }

        await matchesService.DeleteAsync(id);
        return NoContent();
    }

    /// <summary>
    /// List all admin notes from a match
    /// </summary>
    /// <param name="id">Match id</param>
    /// <response code="404">A match matching the given id does not exist</response>
    /// <response code="200">Returns all admin notes from a match</response>
    [HttpGet("{id:int}/notes")]
    [Authorize(Roles = $"{OtrClaims.Roles.User}, {OtrClaims.Roles.Client}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType<IEnumerable<AdminNoteDTO>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> ListAdminNotesAsync(int id)
    {
        if (!await matchesService.ExistsAsync(id))
        {
            return NotFound();
        }

        return Ok(await adminNoteService.ListAsync<MatchAdminNote>(id));
    }
}
