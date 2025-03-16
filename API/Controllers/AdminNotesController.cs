using System.ComponentModel.DataAnnotations;
using API.Authorization;
using API.DTOs;
using API.Services.Interfaces;
using API.Utilities.AdminNotes;
using API.Utilities.Extensions;
using Asp.Versioning;
using Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/*
 * This controller directly uses the context to check for existence of entities.
 *
 * (AFAIK - myssto) Without extensive extra infrastructure or breaking the completely dynamic
 * design pattern of this tree, it would not be possible to access the services or repositories
 * of the parent entity types to satisfy our existing convention of never accessing the context
 * or repositories directly from the controller.
 */

[ApiController]
[ApiVersion(1)]
[ValidateAdminNoteControllerRoute]
[Route("api/v{version:apiVersion}/{entity}")]
public class AdminNotesController(IAdminNoteService adminNoteService, OtrContext context) : ControllerBase
{
    /// <summary>
    /// Create an admin note for an entity
    /// </summary>
    /// <param name="entity">Entity type</param>
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
        AdminNoteRouteTarget entity,
        int entityId,
        [FromBody][Required] string note
    )
    {
        // Check if target entity exists
        if (await context.FindAsync(entity.EntityType, entityId) is null)
        {
            return NotFound();
        }

        AdminNoteDTO? result = await adminNoteService.CreateAsync(
            entity.AdminNoteType,
            entityId,
            User.GetSubjectId(),
            note
        );

        return result is not null
            ? CreatedAtAction(
                "ListNotes",
                new { entity = entity.Original, entityId },
                result
            )
            : BadRequest();
    }

    /// <summary>
    /// List admin notes for an entity
    /// </summary>
    /// <param name="entity">Entity type</param>
    /// <param name="entityId">Entity id</param>
    /// <response code="404">An entity matching the given id does not exist</response>
    /// <response code="200">Returns all admin notes for the entity</response>
    [HttpGet("{entityId:int}/notes")]
    [Authorize(Roles = $"{OtrClaims.Roles.User}, {OtrClaims.Roles.Client}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType<IEnumerable<AdminNoteDTO>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> ListNotesAsync(
        AdminNoteRouteTarget entity,
        int entityId
    )
    {
        // Check if target entity exists
        if (await context.FindAsync(entity.EntityType, entityId) is null)
        {
            return NotFound();
        }

        IEnumerable<AdminNoteDTO> result = await adminNoteService.ListAsync(
            entity.AdminNoteType,
            entityId
        );

        return Ok(result);
    }

    /// <summary>
    /// Update an admin note
    /// </summary>
    /// <param name="entity">Entity type</param>
    /// <param name="noteId">Admin note id</param>
    /// <param name="note">New content of the admin note</param>
    /// <response code="404">An admin note matching the given noteId does not exist </response>
    /// <response code="400">The update was not successful</response>
    /// <response code="200">Returns the updated admin note</response>
    [HttpPatch("notes/{noteId:int}")]
    [Authorize(Roles = OtrClaims.Roles.Admin)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<AdminNoteDTO>(StatusCodes.Status201Created)]
    public async Task<IActionResult> UpdateNoteAsync(
        AdminNoteRouteTarget entity,
        int noteId,
        [FromBody][Required] string note
    )
    {
        AdminNoteDTO? existingNote = await adminNoteService.GetAsync(entity.AdminNoteType, noteId);

        if (existingNote is null)
        {
            return NotFound();
        }

        existingNote.Note = note;
        AdminNoteDTO? updatedNote = await adminNoteService.UpdateAsync(entity.AdminNoteType, existingNote);

        return updatedNote is not null ? Ok(updatedNote) : BadRequest();
    }

    /// <summary>
    /// Delete an admin note
    /// </summary>
    /// <param name="entity">Entity type</param>
    /// <param name="noteId">Admin note id</param>
    /// <response code="404">An admin note matching the given noteId does not exist </response>
    /// <response code="400">The deletion was not successful</response>
    /// <response code="204">The admin note was deleted</response>
    [Authorize(Roles = OtrClaims.Roles.Admin)]
    [HttpDelete("notes/{noteId:int}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteNoteAsync(
        AdminNoteRouteTarget entity,
        int noteId
    )
    {
        if (!await adminNoteService.ExistsAsync(entity.AdminNoteType, noteId))
        {
            return NotFound();
        }

        var success = await adminNoteService.DeleteAsync(entity.AdminNoteType, noteId);

        return success ? NoContent() : BadRequest();
    }
}
