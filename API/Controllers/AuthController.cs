using System.ComponentModel.DataAnnotations;
using API.DTOs;
using API.Services.Interfaces;
using Asp.Versioning;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[ApiVersion(1)]
[AllowAnonymous]
[Route("api/v{version:apiVersion}/[controller]")]
public class AuthController(IOAuthClientService oAuthClientService) : ControllerBase
{
    /// <summary>
    /// Logs in to o!TR
    /// </summary>
    /// <param name="redirectUri">Redirects the client to the given uri after login</param>
    [HttpGet("login")]
    public IActionResult Login([FromQuery] string? redirectUri = null)
    {
        return Challenge(new AuthenticationProperties
        {
            RedirectUri = redirectUri
        }, "osu");
    }

    /// <summary>
    /// Logs out from o!TR
    /// </summary>
    /// <param name="redirectUri">Redirects the client to the given uri after logout</param>
    [HttpGet("logout")]
    public async Task<IActionResult> LogoutAsync([FromQuery] string? redirectUri = null)
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        return !string.IsNullOrEmpty(redirectUri)
            ? Redirect(redirectUri)
            : Ok();
    }

    /// <summary>
    /// Authenticate using client credentials
    /// </summary>
    /// <param name="clientId">Client id</param>
    /// <param name="clientSecret">Client secret</param>
    /// <response code="401">Could not authenticate</response>
    /// <response code="200">Returns client access credentials</response>
    [HttpPost("token")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<AccessCredentialsDTO>(StatusCodes.Status200OK)]
    public async Task<IActionResult> AuthenticateClientAsync(
        [FromQuery][Required] int clientId,
        [FromQuery][Required] string clientSecret
    )
    {
        AccessCredentialsDTO? result = await oAuthClientService.AuthenticateAsync(clientId, clientSecret);

        return result is not null
            ? Ok(result)
            : Unauthorized();
    }
}
