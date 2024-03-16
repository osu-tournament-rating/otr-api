using API.DTOs;
using API.Handlers.Interfaces;
using API.Utilities;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[ApiVersion(1)]
[AllowAnonymous]
[Route("api/v{version:apiVersion}/[controller]")]
public class OAuthController(IOAuthHandler oAuthHandler) : Controller
{
    private readonly IOAuthHandler _oAuthHandler = oAuthHandler;

    [HttpPost("authorize")]
    public async Task<IActionResult> AuthorizeAsync([FromQuery] string code)
    {
        OAuthResponseDTO? result = await _oAuthHandler.AuthorizeAsync(code);

        if (result == null)
        {
            return Unauthorized();
        }

        return Ok(result);
    }

    [HttpPost("token")]
    public async Task<IActionResult> AuthorizeAsync([FromQuery] int clientId, [FromQuery] string clientSecret)
    {
        OAuthResponseDTO? authorizationResponse = await _oAuthHandler.AuthorizeAsync(clientId, clientSecret);

        if (authorizationResponse == null)
        {
            return Unauthorized();
        }

        return Ok(authorizationResponse);
    }

    [HttpPost("client")]
    [Authorize(Roles = "user")]
    public async Task<IActionResult> CreateClientAsync()
    {
        var userId = HttpContext.AuthorizedUserIdentity();

        if (!userId.HasValue)
        {
            return Unauthorized();
        }

        OAuthClientDTO result = await _oAuthHandler.CreateClientAsync(userId!.Value);

        return Ok(result);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshAsync([FromQuery] string refreshToken)
    {
        OAuthResponseDTO? result = await _oAuthHandler.RefreshAsync(refreshToken);

        if (result == null)
        {
            return Unauthorized();
        }

        return Ok(result);
    }
}
