using System.ComponentModel.DataAnnotations;
using API.DTOs;
using API.Services.Interfaces;
using API.Utilities;
using API.Utilities.Extensions;
using Asp.Versioning;
using Database.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[ApiVersion(1)]
[Route("api/v{version:apiVersion}/[controller]")]
public class MatchesController(IMatchesService matchesService) : Controller
{
    /// <summary>
    /// Gets all matches
    /// </summary>
    /// <remarks>
    /// Results are ordered by id and support pagination. All match data is included.
    /// </remarks>
    /// <param name="limit">
    /// Controls the number of matches to return. Functions as a "page size".
    /// Default: 100 Constraints: Minimum 1, Maximum 5000
    /// </param>
    /// <param name="page">
    /// Controls which block of size <paramref name="limit"/> to return.
    /// Default: 1, Constraints: Minimum 1
    /// </param>
    /// <response code="200">Returns the desired page of matches</response>
    [HttpGet]
    [Authorize(Roles = $"{OtrClaims.User}, {OtrClaims.Client}")]
    [ProducesResponseType<PagedResultDTO<MatchDTO>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> ListAsync(
        [FromQuery] MatchesFilterDTO filter,
        [FromQuery][Range(1, int.MaxValue)] int limit = 100,
        [FromQuery][Range(1, int.MaxValue)] int page = 1
    )
    {
        // Clamp page size for non-admin users
        if (limit > 5000 && !(User.IsAdmin() || User.IsSystem()))
        {
            limit = 5000;
        }

        return Ok(await matchesService.GetAsync(limit, page, filter));
    }

    /// <summary>
    /// Get a match
    /// </summary>
    /// <param name="id">Match id</param>
    /// <response code="404">If a match does not exist for the given id</response>
    /// <response code="200">Returns a match</response>
    [HttpGet("{id:int}")]
    [Authorize(Roles = $"{OtrClaims.User}, {OtrClaims.Client}")]
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
    [Authorize(Roles = $"{OtrClaims.Admin}, {OtrClaims.System}")]
    public async Task<ActionResult<IEnumerable<MatchDTO>>> GetMatchesAsync(long osuId, Ruleset ruleset) =>
        Ok(await matchesService.GetAllForPlayerAsync(osuId, ruleset, DateTime.MinValue, DateTime.MaxValue));
}
