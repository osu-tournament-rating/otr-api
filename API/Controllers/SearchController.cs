using API.DTOs;
using API.Services.Interfaces;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[ApiVersion(1)]
[EnableCors]
[Authorize(Roles = "system")] // Internal access only at this time
[Route("api/v{version:apiVersion}/[controller]")]
public class SearchController(ISearchService service) : Controller
{
    private readonly ISearchService _searchService = service;

    [HttpGet]
    [EndpointSummary("Allows for partial or full searching on the names of tournaments, matches and usernames.")]
    [ProducesResponseType<SearchResponseDTO>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SearchByNames([FromQuery] string? tournamentName, [FromQuery] string? matchName, [FromQuery] string? username)
    {
        List<SearchResponseDTO>? response = await _searchService.SearchByNameAsync(tournamentName, matchName, username);
        return response is null ? NotFound("Requested search returned nothing.") : Ok(response);
    }
}
