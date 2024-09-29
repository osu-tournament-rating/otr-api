using API.Authorization;
using API.DTOs;
using API.Services.Interfaces;
using API.Utilities;
using API.Utilities.Extensions;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[ApiVersion(1)]
[Route("api/v{version:apiVersion}/[controller]")]
public class FilteringController(IFilteringService filteringService) : Controller
{
    /// <summary>
    /// Filter a list of users based on the criteria as described in
    /// <see cref="FilteringResultDTO"/>
    /// </summary>
    /// <param name="filteringRequest">The filtering request</param>
    /// <response code="400">Errors encountered during validation</response>
    /// <response code="200">The filtering result</response>
    [HttpPost]
    [Authorize(Roles = $"{OtrJwtRoles.User}, {OtrJwtRoles.Client}")]
    [ProducesResponseType<string>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<FilteringResultDTO>(StatusCodes.Status200OK)]
    public async Task<IActionResult> FilterAsync([FromBody] FilteringRequestDTO filteringRequest)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState.ErrorMessage());
        }

        // Filter out duplicate ids
        filteringRequest.OsuPlayerIds = filteringRequest.OsuPlayerIds.Distinct();

        FilteringResultDTO filteringResult = await filteringService.FilterAsync(filteringRequest);
        return Ok(filteringResult);
    }
}
