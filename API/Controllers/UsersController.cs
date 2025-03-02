using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using API.Authorization;
using API.DTOs;
using API.Services.Interfaces;
using API.Utilities.Extensions;
using Asp.Versioning;
using Common.Enums.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[ApiVersion(1)]
[Route("api/v{version:apiVersion}/[controller]")]
[SuppressMessage("ReSharper", "RouteTemplates.ActionRoutePrefixCanBeExtractedToControllerRoute")]
public class UsersController(
    IUserService userService,
    IOAuthClientService clientService,
    IUserSettingsService userSettingsService
) : Controller
{
    /// <summary>
    /// Get a user
    /// </summary>
    /// <param name="id">User id</param>
    /// <response code="404">A user matching the given id does not exist</response>
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
    /// <param name="id">User id</param>
    /// <param name="scopes">List of scopes to assign to the user</param>
    /// <response code="404">A user matching the given id does not exist</response>
    /// <response code="400">A given scope is invalid, or the update was not successful</response>
    /// <response code="200">Returns the updated user</response>
    [HttpPatch("{id:int}/scopes")]
    [Authorize(Roles = OtrClaims.Roles.Admin)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<UserDTO>(StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateScopesAsync(int id, [FromBody][Required] List<string> scopes)
    {
        scopes = [.. scopes.Select(s => s.ToLower())];
        foreach (var scope in scopes)
        {
            if (!OtrClaims.Roles.IsUserAssignableRole(scope))
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
    /// <param name="id">User id</param>
    /// <response code="404">A user matching the given id does not exist</response>
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

        return Ok(await userService.GetSubmissionsAsync(id) ?? []);
    }

    /// <summary>
    /// Reject a user's match submissions
    /// </summary>
    /// <param name="id">User id</param>
    /// <response code="404">A user matching the given id does not exist</response>
    /// <response code="400">The operation was not successful</response>
    /// <response code="200">The operation was successful</response>
    [HttpPost("{id:int}/submissions:reject")]
    [Authorize(Roles = OtrClaims.Roles.Admin)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> RejectSubmissionsAsync(int id)
    {
        if (!await userService.ExistsAsync(id))
        {
            return NotFound();
        }

        return await userService.RejectSubmissionsAsync(id, User.GetSubjectId())
            ? Ok()
            : BadRequest();
    }

    /// <summary>
    /// Get a user's OAuth clients
    /// </summary>
    /// <param name="id">User id</param>
    /// <response code="404">A user matching the given id does not exist</response>
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

        return Ok(await userService.GetClientsAsync(id) ?? []);
    }

    /// <summary>
    /// Delete a user's OAuth client
    /// </summary>
    /// <param name="id">User id</param>
    /// <param name="clientId">OAuth client id</param>
    /// <response code="404">
    /// A user matching the given id does not exist
    /// or an OAuth client matching the given id is not owned by the user
    /// </response>
    /// <response code="400">The deletion was not successful</response>
    /// <response code="200">The deletion was successful</response>
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
    /// Reset the secret of a user's OAuth client
    /// </summary>
    /// <param name="id">User id</param>
    /// <param name="clientId">OAuth client id</param>
    /// <response code="404">
    /// A user matching the given id does not exist
    /// or an OAuth client matching the given id is not owned by the user
    /// </response>
    /// <response code="200">Returns new client credentials</response>
    [HttpPost("{id:int}/clients/{clientId:int}/secret:reset")]
    [Authorize(Policy = AuthorizationPolicies.AccessUserResources)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType<OAuthClientCreatedDTO>(StatusCodes.Status200OK)]
    public async Task<IActionResult> ResetClientSecretAsync(int id, int clientId)
    {
        if (!await clientService.ExistsAsync(clientId, id))
        {
            return NotFound();
        }

        OAuthClientCreatedDTO? result = await clientService.ResetSecretAsync(clientId);

        return result is not null
            ? Ok(result)
            : NotFound();
    }

    /// <summary>
    /// Update the ruleset of a user
    /// </summary>
    /// <remarks>
    /// If a user's preferred ruleset was previously being synced with the one selected on their osu! profile,
    /// updating it will stop their preferred ruleset from being synced in the future unless it is requested
    /// to be synced again
    /// </remarks>
    /// <param name="id">User id</param>
    /// <param name="ruleset">The new ruleset</param>
    /// <response code="404">A user matching the given id does not exist</response>
    /// <response code="400">The operation was not successful</response>
    /// <response code="200">The operation was successful</response>
    [HttpPatch("{id:int}/settings/ruleset")]
    [Authorize(Policy = AuthorizationPolicies.AccessUserResources)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateRulesetAsync(int id, [FromBody][Required] Ruleset ruleset)
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
    /// <remarks>
    /// Sets the user's preferred ruleset to the one currently selected on their osu! profile
    /// and in the future will continuously update if the ruleset selected on their osu! profile changes
    /// </remarks>
    /// <param name="id">User id</param>
    /// <response code="404">A user matching the given id does not exist</response>
    /// <response code="400">The operation was not successful</response>
    /// <response code="200">The operation was successful</response>
    [HttpPost("{id:int}/settings/ruleset:sync")]
    [Authorize(Policy = AuthorizationPolicies.AccessUserResources)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
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
