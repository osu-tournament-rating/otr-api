using API.DTOs;
using API.Handlers.Interfaces;
using API.Utilities;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
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
    [EndpointDescription("Authorize using an osu! OAuth code")]
    public async Task<Results<UnauthorizedHttpResult, Ok<OAuthResponseDTO>>> AuthorizeAsync([FromQuery] string code)
    {
        OAuthResponseDTO? result = await _oAuthHandler.AuthorizeAsync(code);

        if (result == null)
        {
            return TypedResults.Unauthorized();
        }

        return TypedResults.Ok(result);
    }

    [HttpPost("token")]
    [EndpointDescription("Obtain an access and refresh token using OAuth credentials")]
    public async Task<Results<UnauthorizedHttpResult, Ok<OAuthResponseDTO>>> AuthorizeAsync([FromQuery] int clientId, [FromQuery] string clientSecret)
    {
        OAuthResponseDTO? authorizationResponse = await _oAuthHandler.AuthorizeAsync(clientId, clientSecret);

        if (authorizationResponse == null)
        {
            return TypedResults.Unauthorized();
        }

        return TypedResults.Ok(authorizationResponse);
    }

    [HttpPost("client")]
    [Authorize(Roles = "user")]
    [EndpointDescription("Create a new OAuth client")]
    public async Task<Results<UnauthorizedHttpResult, Ok<OAuthClientDTO>>> CreateClientAsync()
    {
        var userId = HttpContext.AuthorizedUserIdentity();

        if (!userId.HasValue)
        {
            return TypedResults.Unauthorized();
        }

        OAuthClientDTO result = await _oAuthHandler.CreateClientAsync(userId.Value);

        return TypedResults.Ok(result);
    }

    [HttpPost("refresh")]
    [EndpointDescription("Refresh an access token")]
    public async Task<Results<UnauthorizedHttpResult, Ok<OAuthResponseDTO>>> RefreshAsync([FromQuery] string refreshToken)
    {
        OAuthResponseDTO? result = await _oAuthHandler.RefreshAsync(refreshToken);

        if (result == null)
        {
            return TypedResults.Unauthorized();
        }

        return TypedResults.Ok(result);
    }
}
