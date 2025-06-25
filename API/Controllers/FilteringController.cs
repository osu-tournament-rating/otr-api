using System.ComponentModel.DataAnnotations;
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
    [ProducesResponseType<FilterReportDTO>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFilterReportAsync(int id)
    {
        var filterReport = await filterReportsRepository.GetAsync(id);
        if (filterReport == null)
        {
            return NotFound();
        }

        FilteringRequestDTO? request = null;
        FilteringResultDTO? response = null;

        try
        {
            if (!string.IsNullOrEmpty(filterReport.RequestJson))
            {
                request = JsonSerializer.Deserialize<FilteringRequestDTO>(filterReport.RequestJson);
            }

            if (!string.IsNullOrEmpty(filterReport.ResponseJson))
            {
                response = JsonSerializer.Deserialize<FilteringResultDTO>(filterReport.ResponseJson);
            }
        }
        catch (JsonException)
        {
            // If deserialization fails, we'll return the DTO with null values
            // This provides type safety even if the JSON structure changes
        }

        var filterReportDto = new FilterReportDTO
        {
            Id = filterReport.Id,
            Created = filterReport.Created,
            UserId = filterReport.UserId,
            Request = request,
            Response = response
        };

        return Ok(filterReportDto);
    }
}
