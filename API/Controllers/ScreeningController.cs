using API.DTOs;
using API.Services.Interfaces;
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
    ///
    /// <remarks>
    ///
    /// This is a test remark
    ///
    /// </remarks>
    /// </summary>
    /// <param name="screeningRequest"></param>
    /// <returns></returns>
    [HttpPost]
    [Authorize(Roles = "user, client")]
    [ProducesResponseType(typeof(ScreeningResultDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ScreenAsync([FromBody] ScreeningRequestDTO screeningRequest)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Filter out duplicate ids
        screeningRequest.OsuPlayerIds = screeningRequest.OsuPlayerIds.Distinct();

        IEnumerable<ScreeningResultDTO> results = await screeningService.ScreenAsync(screeningRequest);
        return Ok(results);
    }
}
