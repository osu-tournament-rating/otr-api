using API.Handlers.Interfaces;
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

    [HttpPost]
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
}