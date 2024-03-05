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
public class OAuthController : Controller
{
    private readonly IOAuthHandler _oAuthHandler;

    public OAuthController(IOAuthHandler oAuthHandler)
    {
        _oAuthHandler = oAuthHandler;
    }

    [HttpPost("authorize")]
    public async Task<IActionResult> AuthorizeAsync([FromQuery] string code)
    {
        var result = await _oAuthHandler.AuthorizeAsync(code);

        if (result == null)
        {
            return Unauthorized();
        }

        return Ok(result);
    }

    [HttpPost("token")]
    public async Task<IActionResult> AuthorizeAsync([FromQuery] int clientId, [FromQuery] string clientSecret)
    {
        var authorizationResponse = await _oAuthHandler.AuthorizeAsync(clientId, clientSecret);

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

        var result = await _oAuthHandler.CreateClientAsync(userId!.Value);

        if (result == null)
        {
            return BadRequest();
        }

        return Ok(result);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshAsync([FromQuery] string refreshToken)
    {
        var result = await _oAuthHandler.RefreshAsync(refreshToken);

        if (result == null)
        {
            return Unauthorized();
        }

        return Ok(result);
    }
}
