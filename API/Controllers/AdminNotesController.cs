using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using API.Authorization;
using API.DTOs;
using API.Services.Implementations;
using API.Services.Interfaces;
using API.Utilities.AdminNotes;
using API.Utilities.Extensions;
using Asp.Versioning;
using Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/*
 * This controller directly uses the context to check for existence of entities
 *
 * (AFAIK - myssto) Without extensive extra infrastructure or breaking the completely dynamic
 * design pattern of this tree, it would not be possible to access the services or repositories
 * of the parent entity types to satisfy our existing convention of never accessing the context
 * or repositories directly from the controller.
 *
 * Route template inspection suppressions
 *
 * These inspections are failing because the {entity} segment of the RouteAttribute defined
 * on the controller itself is not directly passed to each method. Instead of being passed to
 * each method, it is bound to a property of the class, which is implicitly accessible to each method.
 */

[ApiController]
[ApiVersion(1)]
[ValidateAdminNoteControllerRoute]
[Route("api/v{version:apiVersion}/{entity}")]
[SuppressMessage("ReSharper", "RouteTemplates.MethodMissingRouteParameters")]
[SuppressMessage("ReSharper", "RouteTemplates.ControllerRouteParameterIsNotPassedToMethods")]
public class AdminNotesController(IAdminNoteService adminNoteService, OtrContext context) : ControllerBase
{
    [FromRoute(Name = "entity")]
    // Property must be public and contain a setter in order to be bound from the route
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    [SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global")]
    public AdminNoteRouteTarget DynamicRouteTarget { get; set; } = null!;

    private DynamicAdminNoteService DynamicAdminNoteService
    {
        get => new(adminNoteService, DynamicRouteTarget);
    }

    /// <summary>
    /// Create an admin note for an entity
    /// </summary>
    /// <param name="entityId">Entity id</param>
    /// <param name="note">Content of the admin note</param>
    /// <response code="404">An entity matching the given id does not exist</response>
    /// <response code="400">The authorized user does not exist</response>
    /// <response code="201">Returns the created admin note</response>
    [HttpPost("{entityId:int}/notes")]
    [Authorize(Roles = OtrClaims.Roles.Admin)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<AdminNoteDTO>(StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateNoteAsync(
        int entityId,
        [FromBody][Required] string note
    )
    {
        // Check if target entity exists
        if (await context.FindAsync(DynamicRouteTarget.EntityType, entityId) is null)
        {
            return NotFound();
        }

        AdminNoteDTO? result = await DynamicAdminNoteService.CreateAsync(
            entityId,
            User.GetSubjectId(),
            note
        );

        return result is not null
            ? CreatedAtAction(
                "ListNotes",
                new { entity = DynamicRouteTarget.Original, entityId },
                result
            )
            : BadRequest();
    }

    /// <summary>
    /// List admin notes for an entity
    /// </summary>
    /// <param name="entityId">Entity id</param>
    /// <response code="404">An entity matching the given id does not exist</response>
    /// <response code="200">Returns all admin notes for the entity</response>
    [HttpGet("{entityId:int}/notes")]
    [Authorize(Policy = AuthorizationPolicies.ApiKeyAuthorization)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType<IEnumerable<AdminNoteDTO>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> ListNotesAsync(int entityId)
    {
        // Check if target entity exists
        if (await context.FindAsync(DynamicRouteTarget.EntityType, entityId) is null)
        {
            return NotFound();
        }

        IEnumerable<AdminNoteDTO> result = await DynamicAdminNoteService.ListAsync(entityId);

        return Ok(result);
    }

    /// <summary>
    /// Update an admin note
    /// </summary>
    /// <param name="noteId">Admin note id</param>
    /// <param name="note">New content of the admin note</param>
    /// <response code="404">An admin note matching the given noteId does not exist</response>
    /// <response code="403">User is attempting to update a note which they do not own</response>
    /// <response code="400">The update was not successful</response>
    /// <response code="200">Returns the updated admin note</response>
    [HttpPatch("notes/{noteId:int}")]
    [Authorize(Roles = OtrClaims.Roles.Admin)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<AdminNoteDTO>(StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateNoteAsync(
        int noteId,
        [FromBody][Required] string note
    )
    {
        AdminNoteDTO? existingNote = await DynamicAdminNoteService.GetAsync(noteId);

        if (existingNote is null)
        {
            return NotFound();
        }

        if (existingNote.AdminUser.Id != User.GetSubjectId())
        {
            return Forbid();
        }

        existingNote.Note = note;
        AdminNoteDTO? updatedNote = await DynamicAdminNoteService.UpdateAsync(existingNote);

        return updatedNote is not null ? Ok(updatedNote) : BadRequest();
    }

    /// <summary>
    /// Delete an admin note
    /// </summary>
    /// <param name="noteId">Admin note id</param>
    /// <response code="404">An admin note matching the given noteId does not exist </response>
    /// <response code="403">User is attempting to delete a note which they do not own</response>
    /// <response code="400">The deletion was not successful</response>
    /// <response code="204">The admin note was deleted</response>
    [Authorize(Roles = OtrClaims.Roles.Admin)]
    [HttpDelete("notes/{noteId:int}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteNoteAsync(
        int noteId
    )
    {
        if (!await DynamicAdminNoteService.ExistsAsync(noteId))
        {
            return NotFound();
        }

        if ((await DynamicAdminNoteService.GetAsync(noteId))?.AdminUser.Id != User.GetSubjectId())
        {
            return Forbid();
        }

        bool success = await DynamicAdminNoteService.DeleteAsync(noteId);

        return success ? NoContent() : BadRequest();
    }
}
