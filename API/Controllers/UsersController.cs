using System.Diagnostics.CodeAnalysis;
using API.DTOs;
using API.Services.Interfaces;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[ApiVersion(1)]
[Authorize(Roles = "system, admin")] // Internal access only
[Route("api/v{version:apiVersion}/[controller]")]
[SuppressMessage("ReSharper", "RouteTemplates.ActionRoutePrefixCanBeExtractedToControllerRoute")]
public class UsersController(IUserService userService, IOAuthClientService clientService) : Controller
{
    private readonly IUserService _userService = userService;
    private readonly IOAuthClientService _clientService = clientService;

    /// <summary>
    /// Get a user
    /// </summary>
    /// <param name="id">Id of the user</param>
    /// <response code="404">If a user does not exist</response>
    /// <response code="200">Returns a user</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType<UserDTO>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAsync(int id)
    {
        UserDTO? result = await _userService.GetAsync(id);

        return result == null
            ? NotFound()
            : Ok(result);
    }

    /// <summary>
    /// Update a user's scopes
    /// </summary>
    /// <param name="id">Id of the user</param>
    /// <param name="scopes">List of scopes to assign to the user</param>
    /// <response code="404">If a user does not exist</response>
    /// <response code="400">If the update was not successful</response>
    /// <response code="200">Returns an updated user</response>
    [HttpPatch("{id:int}/scopes")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<UserDTO>(StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateAsync(int id, [FromBody] List<string> scopes)
    {
        if (!await _userService.ExistsAsync(id))
        {
            return NotFound();
        }

        UserDTO? result = await _userService.UpdateScopesAsync(id, scopes);

        return result == null
            ? BadRequest()
            : Ok(result);
    }

    /// <summary>
    /// Get a user's OAuth clients
    /// </summary>
    /// <param name="id">Id of the user</param>
    /// <response code="404">If a user does not exist</response>
    /// <response code="200">Returns a list of OAuth clients</response>
    [HttpGet("{id:int}/clients")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType<IEnumerable<OAuthClientDTO>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetClientsAsync(int id)
    {
        IEnumerable<OAuthClientDTO>? result = await _userService.GetClientsAsync(id);

        return result == null
            ? NotFound()
            : Ok(result);
    }

    /// <summary>
    /// Delete a user's OAuth client
    /// </summary>
    /// <param name="id">Id of the user</param>
    /// <param name="clientId">Id of the OAuth client</param>
    /// <response code="404">If a user or client does not exist</response>
    /// <response code="400">If the deletion was not successful</response>
    /// <response code="200">Denotes the deletion was successful</response>
    [HttpDelete("{id:int}/clients/{clientId:int}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteClientAsync(int id, int clientId)
    {
        if (!await _clientService.ExistsAsync(clientId, id))
        {
            return NotFound();
        }

        return await _clientService.DeleteAsync(clientId)
            ? BadRequest()
            : Ok();
    }
}
