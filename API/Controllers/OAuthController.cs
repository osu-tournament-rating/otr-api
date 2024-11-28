using System.ComponentModel.DataAnnotations;
using API.Authorization;
using API.DTOs;
using API.Handlers.Interfaces;
using API.Services.Interfaces;
using API.Utilities.Extensions;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[ApiVersion(1)]
[Route("api/v{version:apiVersion}/[controller]")]
public class OAuthController(IOAuthHandler oAuthHandler, IOAuthClientService oAuthClientService) : Controller
{
    /// <summary>
    /// Authorize using an osu! authorization code
    /// </summary>
    /// <param name="code">osu! authorization code</param>
    /// <response code="401">There was an error during authorization</response>
    /// <response code="200">Returns user access credentials</response>
    [HttpPost("authorize")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<AccessCredentialsDTO>(StatusCodes.Status200OK)]
    public async Task<IActionResult> AuthorizeAsync([FromQuery][Required] string code)
    {
        AccessCredentialsDTO? result = await oAuthHandler.AuthorizeAsync(code);

        return result is not null
            ? Ok(result)
            : Unauthorized();
    }

    /// <summary>
    /// Authorize using client credentials
    /// </summary>
    /// <param name="clientId">Client id</param>
    /// <param name="clientSecret">Client secret</param>
    /// <response code="400">There was an error during authorization</response>
    /// <response code="200">Returns client access credentials</response>
    [HttpPost("token")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<AccessCredentialsDTO>(StatusCodes.Status200OK)]
    public async Task<IActionResult> AuthorizeClientAsync(
        [FromQuery][Required] int clientId,
        [FromQuery][Required] string clientSecret
    )
    {
        DetailedResponseDTO<AccessCredentialsDTO> result = await oAuthHandler.AuthorizeAsync(clientId, clientSecret);

        if (!string.IsNullOrEmpty(result.ErrorDetail))
        {
            return BadRequest(result.ErrorDetail);
        }

        return result.Response is not null
            ? Ok(result.Response)
            : Problem(
                detail: "Unknown error: Authorization attempt was not successful and did not include error detail.",
                statusCode: StatusCodes.Status500InternalServerError
            );
    }

    /// <summary>
    /// Create a new OAuth client
    /// </summary>
    /// <remarks>
    /// Client secret is only returned from creation.
    /// The user will have to reset the secret if they lose access.
    /// </remarks>
    /// <response code="200">Returns created client credentials</response>
    [HttpPost("client")]
    [Authorize(Roles = OtrClaims.Roles.User)]
    [ProducesResponseType<OAuthClientCreatedDTO>(StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateClientAsync() =>
        Ok(await oAuthClientService.CreateAsync(User.GetSubjectId()));

    /// <summary>
    /// Generate new access credentials from a valid refresh token
    /// </summary>
    /// <remarks>Generated access credentials will contain only a new access token</remarks>
    /// <param name="refreshToken">Refresh token</param>
    /// <response code="400">The refresh token is invalid or there was an error during authorization</response>
    /// <response code="200">Returns access credentials containing a new access token</response>
    [HttpPost("refresh")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<AccessCredentialsDTO>(StatusCodes.Status200OK)]
    public async Task<IActionResult> RefreshAsync([FromQuery][Required] string refreshToken)
    {
        DetailedResponseDTO<AccessCredentialsDTO> result = await oAuthHandler.RefreshAsync(refreshToken);

        if (!string.IsNullOrEmpty(result.ErrorDetail))
        {
            return BadRequest(result.ErrorDetail);
        }

        return result.Response is not null
            ? Ok(result.Response)
            : Problem(
                detail: "Unknown error: Authorization attempt was not successful and did not include error detail.",
                statusCode: StatusCodes.Status500InternalServerError
            );
    }
}
