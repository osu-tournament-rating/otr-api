using API.DTOs;
using API.DTOs.Processor;
using API.Utilities;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[ApiVersion(1)]
[Route("api/v{version:apiVersion}/[controller]")]
public class ProcessorController : Controller
{
    [HttpGet("/matches")]
    [Authorize(Roles = OtrClaims.System)]
    [ProducesResponseType<PagedResultDTO<ProcessorMatchDTO>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> ListMatchesAsync(
        [FromQuery] int limit = 100,
        [FromQuery] int page = 1
    )
    {
        if (limit < 1 || limit > 5000 || page < 1)
        {
            return BadRequest();
        }

        return Ok();
    }
}
