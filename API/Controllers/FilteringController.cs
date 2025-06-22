using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Text.Json;
using API.Authorization;
using API.DTOs;
using API.Services.Interfaces;
using API.Utilities.Extensions;
using Asp.Versioning;
using Database.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[ApiVersion(1)]
[Route("api/v{version:apiVersion}/[controller]")]
public class FilteringController(
    IFilteringService filteringService,
    IFilterReportsRepository filterReportsRepository) : Controller
{
    /// <summary>
    /// Filter a list of users based on the criteria as described in
    /// <see cref="FilteringResultDTO"/>
    /// </summary>
    /// <param name="filteringRequest">The filtering request</param>
    /// <response code="400">The request body is invalid</response>
    /// <response code="200">The filtering result</response>
    [HttpPost]
    [Authorize(Roles = $"{OtrClaims.Roles.User}, {OtrClaims.Roles.Client}")]
    [ProducesResponseType<string>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<FilteringResultDTO>(StatusCodes.Status200OK)]
    public async Task<IActionResult> FilterAsync([FromBody][Required] FilteringRequestDTO filteringRequest)
    {
        // Remove duplicate ids
        filteringRequest.OsuPlayerIds = filteringRequest.OsuPlayerIds.Distinct().ToList();

        // Get the current user ID from claims
        int userId = HttpContext.User.GetSubjectId();

        FilteringResultDTO filteringResult = await filteringService.FilterAsync(filteringRequest, userId);
        return Ok(filteringResult);
    }

    /// <summary>
    /// Get a stored filter report by ID
    /// </summary>
    /// <param name="id">The filter report ID</param>
    /// <response code="404">The filter report was not found</response>
    /// <response code="200">The filter report</response>
    [HttpGet("{id:int}")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType<FilteringResultDTO>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFilterReportAsync(int id)
    {
        var filterReport = await filterReportsRepository.GetAsync(id);
        if (filterReport == null)
        {
            return NotFound();
        }

        var filteringResult = JsonSerializer.Deserialize<FilteringResultDTO>(filterReport.ResponseJson);
        return Ok(filteringResult);
    }
}
