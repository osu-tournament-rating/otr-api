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
public class ScreeningController(IScreeningService screeningService) : Controller
{
    /// <summary>
    /// Screen a list of users based on the criteria as described in
    /// <see cref="ScreeningResultDTO"/>
    /// </summary>
    /// <param name="screeningRequest">The screening request</param>
    /// <response code="400">Errors encountered during validation</response>
    /// <response code="200">The screening result</response>
    [HttpPost]
    [Authorize(Roles = $"{OtrClaims.User}, {OtrClaims.Client}")]
    [ProducesResponseType<string>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ScreeningResultDTO>(StatusCodes.Status200OK)]
    public async Task<IActionResult> ScreenAsync([FromBody] ScreeningRequestDTO screeningRequest)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState.ErrorMessage());
        }

        // Filter out duplicate ids
        screeningRequest.OsuPlayerIds = screeningRequest.OsuPlayerIds.Distinct();

        ScreeningResultDTO screeningResult = await screeningService.ScreenAsync(screeningRequest);
        return Ok(screeningResult);
    }
}
