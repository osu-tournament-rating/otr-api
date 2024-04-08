using API.DTOs;
using API.Services.Interfaces;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[ApiVersion(1)]
[Authorize(Roles = "user")]
[Authorize(Roles = "whitelist")]
[Route("api/v{version:apiVersion}/[controller]")]
public class SearchController(ISearchService service) : Controller
{
    /// <summary>
    /// Search for tournaments, matches, and users
    /// </summary>
    /// <remarks>
    /// Allows for partial or full searching on the names of tournaments, matches, and usernames
    /// </remarks>
    /// <param name="searchKey">The string to match against names of tournaments, matches, and usernames</param>
    /// <response code="404">If no data matches the given search key</response>
    /// <response code="200">Returns a list of all possible tournaments, matches, and usernames for the given search key</response>
    [HttpGet]
    [EndpointSummary("Allows for partial or full searching on the names of tournaments, matches and usernames.")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType<IEnumerable<SearchResponseDTO>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> SearchByNamesAsync([FromQuery] string searchKey)
    {
        List<SearchResponseDTO> response = await service.SearchByNameAsync(searchKey);
        return response.Count == 0 ? NotFound() : Ok(response);
    }
}
