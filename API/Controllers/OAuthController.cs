using API.Handlers.Interfaces;
using API.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[AllowAnonymous]
[Route("api/[controller]")]
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
        
        Console.WriteLine(result.AccessToken);
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
}