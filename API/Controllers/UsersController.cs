using System.Diagnostics.CodeAnalysis;
using API.Authorization;
using API.DTOs;
using API.Enums;
using API.Osu.Enums;
using API.Services.Interfaces;
using API.Utilities;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[ApiVersion(1)]
[Route("api/v{version:apiVersion}/[controller]")]
[SuppressMessage("ReSharper", "RouteTemplates.ActionRoutePrefixCanBeExtractedToControllerRoute")]
public class UsersController(IUserService userService, IOAuthClientService clientService, IUserSettingsService userSettingsService) : Controller
{
    /// <summary>
    /// Get a user
    /// </summary>
    /// <param name="id">Id of the user</param>
    /// <response code="404">If a user does not exist</response>
    /// <response code="200">Returns a user</response>
    [HttpGet("{id:int}")]
    [Authorize(Policy = AuthorizationPolicies.AccessUserResources)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType<UserDTO>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAsync(int id)
    {
        UserDTO? result = await userService.GetAsync(id);

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
    /// <response code="400">If any of the given scopes are invalid, or the update was not successful</response>
    /// <response code="200">Returns an updated user</response>
    [HttpPatch("{id:int}/scopes")]
    [Authorize(Roles = OtrClaims.Admin)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<UserDTO>(StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateScopesAsync(int id, [FromBody] List<string> scopes)
    {
        scopes = scopes.Select(s => s.ToLower()).ToList();
        foreach (var scope in scopes)
        {
            if (!OtrClaims.IsUserAssignableClaim(scope))
            {
                return BadRequest($"Given scope \"{scope}\" is invalid");
            }
        }

        if (!await userService.ExistsAsync(id))
        {
            return NotFound();
        }

        UserDTO? result = await userService.UpdateScopesAsync(id, scopes);

        return result == null
            ? BadRequest()
            : Ok(result);
    }

    /// <summary>
    /// Get a user's match submissions
    /// </summary>
    /// <param name="id">Id of the user</param>
    /// <response code="404">If a user does not exist</response>
    /// <response code="200">Returns a list of submissions</response>
    [HttpGet("{id:int}/submissions")]
    [Authorize(Policy = AuthorizationPolicies.AccessUserResources)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType<IEnumerable<MatchSubmissionStatusDTO>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSubmissionsAsync(int id)
    {
        if (!await userService.ExistsAsync(id))
        {
            return NotFound();
        }

        return Ok(await userService.GetSubmissionsAsync(id) ?? new List<MatchSubmissionStatusDTO>());
    }

    /// <summary>
    /// Rejects a user's match submissions
    /// </summary>
    /// <param name="id">Id of the user</param>
    /// <response code="404">If a user does not exist</response>
    /// <response code="400">If the operation was not successful</response>
    /// <response code="200">Denotes the operation was successful</response>
    [HttpPost("{id:int}/submissions:reject")]
    [Authorize(Roles = OtrClaims.Admin)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> RejectSubmissionsAsync(int id)
    {
        if (!await userService.ExistsAsync(id))
        {
            return NotFound();
        }

        return await userService.RejectSubmissionsAsync(id, User.AuthorizedIdentity(), MatchVerificationSource.Admin)
            ? Ok()
            : BadRequest();
    }

    /// <summary>
    /// Get a user's OAuth clients
    /// </summary>
    /// <remarks>
    /// All users have access to clients that they own. Admin users have access to clients from any user.
    /// </remarks>
    /// <param name="id">Id of the user</param>
    /// <response code="404">If a user does not exist</response>
    /// <response code="200">Returns a list of OAuth clients</response>
    [HttpGet("{id:int}/clients")]
    [Authorize(Policy = AuthorizationPolicies.AccessUserResources)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType<IEnumerable<OAuthClientDTO>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetClientsAsync(int id)
    {
        if (!await userService.ExistsAsync(id))
        {
            return NotFound();
        }

        return Ok(await userService.GetClientsAsync(id) ?? new List<OAuthClientDTO>());
    }

    /// <summary>
    /// Delete a user's OAuth client
    /// </summary>
    /// <remarks>
    /// All users have access to delete clients that they own. Admin users have access
    /// to delete clients from any user.
    /// </remarks>
    /// <param name="id">Id of the user</param>
    /// <param name="clientId">Id of the OAuth client</param>
    /// <response code="404">If a user or client does not exist</response>
    /// <response code="400">If the deletion was not successful</response>
    /// <response code="200">Denotes the deletion was successful</response>
    [HttpDelete("{id:int}/clients/{clientId:int}")]
    [Authorize(Policy = AuthorizationPolicies.AccessUserResources)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteClientAsync(int id, int clientId)
    {
        if (!await clientService.ExistsAsync(clientId, id))
        {
            return NotFound();
        }

        return await clientService.DeleteAsync(clientId)
            ? Ok()
            : BadRequest();
    }

    /// <summary>
    /// Update the ruleset for a user
    /// </summary>
    /// <response code="404">If a user does not exist</response>
    /// <response code="200">If the operation was successful</response>
    [HttpPatch("{id:int}/settings/ruleset")]
    [Authorize(Policy = AuthorizationPolicies.AccessUserResources)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateRulesetAsync(int id, [FromBody] Ruleset ruleset)
    {
        if (!await userService.ExistsAsync(id))
        {
            return NotFound();
        }

        return await userSettingsService.UpdateRulesetAsync(id, ruleset)
            ? Ok()
            : BadRequest();
    }

    /// <summary>
    /// Sync the ruleset of a user with their osu! ruleset
    /// </summary>
    /// <response code="404">If a user does not exist</response>
    /// <response code="200">If the operation was successful</response>
    [HttpPost("{id:int}/settings/ruleset:sync")]
    [Authorize(Policy = AuthorizationPolicies.AccessUserResources)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> SyncRulesetAsync(int id)
    {
        if (!await userService.ExistsAsync(id))
        {
            return NotFound();
        }

        return await userSettingsService.SyncRulesetAsync(id)
            ? Ok()
            : BadRequest();
    }
}
