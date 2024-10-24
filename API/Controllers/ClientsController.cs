using System.Diagnostics.CodeAnalysis;
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
public class ClientsController(IOAuthClientService clientService) : Controller
{
    /// <summary>
    /// Set the rate limit for a client
    /// </summary>
    /// <param name="id">The client id</param>
    /// <param name="rateLimitOverride">The new rate limit for the client</param>
    /// <response code="404">If the provided id does not belong to a client</response>
    /// <response code="200">Returns the patched client</response>
    [Authorize(Roles = OtrClaims.Roles.Admin)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType<OAuthClientDTO>(StatusCodes.Status200OK)]
    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    [HttpPost("{id:int}/ratelimit")]
    public async Task<IActionResult> PatchRateLimitAsync(int id, int rateLimitOverride)
    {
        OAuthClientDTO? client = await clientService.GetAsync(id);

        if (client == null)
        {
            return NotFound();
        }

        await clientService.SetRateLimitOverrideAsync(id, rateLimitOverride);
        return Ok(client);
    }
}
