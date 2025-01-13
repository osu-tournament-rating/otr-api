using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[ApiVersion(1)]
[Route("api/v{version:apiVersion}/[controller]")]
public class DiagnosticsController : Controller
{
    /// <summary>
    /// Allows clients to determine if the server is running
    /// </summary>
    /// <response code="200">The server is running</response>
    [HttpGet("ping")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public Task<IActionResult> PingAsync() => Task.FromResult<IActionResult>(Ok());
}
