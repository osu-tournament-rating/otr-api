using API.Authorization;
using API.DTOs;
using API.Handlers.Interfaces;
using API.Services.Interfaces;
using API.Utilities;
using API.Utilities.Extensions;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[ApiVersion(1)]
[AllowAnonymous]
[Route("api/v{version:apiVersion}/[controller]")]
public class OAuthController(IOAuthHandler oAuthHandler, IOAuthClientService oAuthClientService) : Controller
{
    /// <summary>
    /// Authorize using an osu! authorization code
    /// </summary>
    /// <param name="code">The osu! authorization code</param>
    /// <response code="201">If there was an error during authorization</response>
    /// <response code="200">Returns user access credentials</response>
    [HttpPost("authorize")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<OAuthResponseDTO>(StatusCodes.Status200OK)]
    public async Task<IActionResult> AuthorizeAsync([FromQuery] string code)
    {
        OAuthResponseDTO? result = await oAuthHandler.AuthorizeAsync(code);

        return result is not null
            ? Ok(result)
            : Unauthorized();
    }

    /// <summary>
    /// Authorize using client credentials
    /// </summary>
    /// <param name="clientId">The id of the client</param>
    /// <param name="clientSecret">The secret of the client</param>
    /// <response code="201">If there was an error during authorization</response>
    /// <response code="200">Returns client access credentials</response>
    [HttpPost("token")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<OAuthResponseDTO>(StatusCodes.Status200OK)]
    public async Task<IActionResult> AuthorizeClientAsync([FromQuery] int clientId, [FromQuery] string clientSecret)
    {
        OAuthResponseDTO? result = await oAuthHandler.AuthorizeAsync(clientId, clientSecret);

        return result is not null
            ? Ok(result)
            : Unauthorized();
    }

    /// <summary>
    /// Create a new OAuth client
    /// </summary>
    /// <remarks>
    /// Client secret is only returned from creation.
    /// The user will have to reset the secret if they lose access.
    /// </remarks>
    /// <response code="401">If the user is not properly authenticated</response>
    /// <response code="200">Returns created client credentials</response>
    [HttpPost("client")]
    [Authorize(Roles = OtrJwtRoles.User)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<OAuthClientCreatedDTO>(StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateClientAsync()
    {
        var userId = User.AuthorizedIdentity();
        if (!userId.HasValue)
        {
            return Unauthorized();
        }

        OAuthClientCreatedDTO result = await oAuthClientService.CreateAsync(userId.Value);

        return Ok(result);
    }

    /// <summary>
    /// Generate new access credentials from a valid refresh token
    /// </summary>
    /// <remarks>
    /// Generated access credentials will contain only a new access token,
    /// and the given refresh token is returned with it
    /// </remarks>
    /// <param name="refreshToken"></param>
    /// <response code="201">If the given refresh token is invalid, or there was an error during authorization</response>
    /// <response code="200">Returns access credentials containing a new access token</response>
    [HttpPost("refresh")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<OAuthResponseDTO>(StatusCodes.Status200OK)]
    public async Task<IActionResult> RefreshAsync([FromQuery] string refreshToken)
    {
        OAuthResponseDTO? result = await oAuthHandler.RefreshAsync(refreshToken);

        return result is not null
            ? Ok(result)
            : Unauthorized();
    }
}
