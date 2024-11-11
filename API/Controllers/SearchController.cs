using System.ComponentModel.DataAnnotations;
using API.Authorization;
using API.DTOs;
using API.Services.Interfaces;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[ApiVersion(1)]
[Route("api/v{version:apiVersion}/[controller]")]
public class SearchController(ISearchService service) : Controller
{
    /// <summary>
    /// Search for tournaments, matches, and users
    /// </summary>
    /// <remarks>
    /// Search uses partial matching on: tournament name and abbreviation, match name, and player name
    /// </remarks>
    /// <param name="searchKey">Search key</param>
    /// <response code="200">
    /// Returns a list of tournaments, matches, and usernames matching the given search key
    /// </response>
    [HttpGet]
    [Authorize(Roles = $"{OtrClaims.Roles.User}, {OtrClaims.Roles.Client}")]
    [ProducesResponseType<SearchResponseCollectionDTO>(StatusCodes.Status200OK)]
    public async Task<IActionResult> SearchAsync([FromQuery][Required] string searchKey)
    {
        SearchResponseCollectionDTO response = await service.SearchByNameAsync(searchKey);
        return Ok(response);
    }
}
