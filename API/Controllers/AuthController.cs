using Asp.Versioning;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[ApiVersion(1)]
[Route("api/v{version:apiVersion}/[controller]")]
public class AuthController : ControllerBase
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
    [HttpGet("logout")]
    public async Task<IActionResult> LogoutAsync()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Ok();
    }
}
