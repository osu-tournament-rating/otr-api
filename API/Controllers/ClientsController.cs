using System.ComponentModel.DataAnnotations;
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
    /// <param name="id">Client id</param>
    /// <param name="rateLimitOverride">The new rate limit for the client</param>
    /// <response code="404">A client matching the given id does not exist</response>
    /// <response code="200">Returns the updated client</response>
    [HttpPost("{id:int}/ratelimit")]
    [Authorize(Roles = OtrClaims.Roles.Admin)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType<OAuthClientDTO>(StatusCodes.Status200OK)]
    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    public async Task<IActionResult> PatchRateLimitAsync(int id, [FromBody][Required] int rateLimitOverride)
    {
        OAuthClientDTO? client = await clientService.GetAsync(id);

        if (client == null)
        {
            return NotFound();
        }

        client = await clientService.SetRateLimitOverrideAsync(id, rateLimitOverride);
        return Ok(client);
    }
}
