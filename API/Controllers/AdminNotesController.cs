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

[ApiController]
[ApiVersion(1)]
[ValidateAdminNoteControllerRoute]
[Route("api/v{version:apiVersion}/{entity}")]
public class AdminNotesController(IAdminNoteService adminNoteService, OtrContext context) : ControllerBase
{
    [Authorize(Roles = OtrClaims.Roles.Admin)]
    [HttpPost("{entityId:int}/notes")]
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

    [Authorize(Roles = OtrClaims.Roles.Admin)]
    [HttpGet("{entityId:int}/notes")]
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

    [Authorize(Roles = OtrClaims.Roles.Admin)]
    [HttpPatch("notes/{noteId:int}")]
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

    [Authorize(Roles = OtrClaims.Roles.Admin)]
    [HttpDelete("notes/{noteId:int}")]
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
