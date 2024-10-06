using API.Authorization;
using API.DTOs;
using API.Utilities.Extensions;
using Database.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public partial class PlayersController
{
    /// <summary>
    /// Creates an admin note for a player
    /// </summary>
    /// <param name="id">Player id</param>
    /// <response code="404">If a player matching the given id does not exist</response>
    /// <response code="400">If the authorized user does not exist</response>
    /// <response code="200">Returns the created admin note</response>
    [HttpPost("{id:int}/notes")]
    [Authorize(Roles = $"{OtrClaims.Roles.Admin}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<AdminNoteDTO>(StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateAdminNoteAsync(int id, [FromBody] string note)
    {
        if (!await playerService.ExistsAsync(id))
        {
            return NotFound();
        }

        AdminNoteDTO? result = await adminNoteService.CreateAsync<PlayerAdminNote>(id, User.GetSubjectId(), note);
        return result is not null
            ? Ok(result)
            : BadRequest();
    }

    /// <summary>
    /// List all admin notes for a player
    /// </summary>
    /// <param name="id">Player id</param>
    /// <response code="404">If a player matching the given id does not exist</response>
    /// <response code="200">Returns all admin notes from a player</response>
    [HttpGet("{id:int}/notes")]
    [Authorize(Roles = $"{OtrClaims.Roles.Admin}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType<IEnumerable<AdminNoteDTO>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> ListAdminNotesAsync(int id)
    {
        if (!await playerService.ExistsAsync(id))
        {
            return NotFound();
        }

        return Ok(await adminNoteService.ListAsync<PlayerAdminNote>(id));
    }

    /// <summary>
    /// Updates an admin note for a player
    /// </summary>
    /// <param name="id">Player id</param>
    /// <param name="noteId">Admin note id</param>
    /// <response code="404">
    /// If a player matching the given id does not exist.
    /// If an admin note matching the given noteId does not exist
    /// </response>
    /// <response code="403">If the requester did not create the admin note</response>
    /// <response code="200">Returns the updated admin note</response>
    [HttpPatch("{id:int}/notes/{noteId:int}")]
    [Authorize(Roles = $"{OtrClaims.Roles.Admin}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<AdminNoteDTO>(StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateAdminNoteAsync(int id, int noteId, [FromBody] string note)
    {
        if (!await playerService.ExistsAsync(id))
        {
            return NotFound();
        }

        AdminNoteDTO? existingNote = await adminNoteService.GetAsync<PlayerAdminNote>(noteId);
        if (existingNote is null)
        {
            return NotFound();
        }

        existingNote.Note = note;
        AdminNoteDTO? result = await adminNoteService.UpdateAsync<PlayerAdminNote>(existingNote);

        return result is not null
            ? Ok(result)
            : NotFound();
    }

    /// <summary>
    /// Deletes an admin note for a player
    /// </summary>
    /// <param name="id">Player id</param>
    /// <param name="noteId">Admin note id</param>
    /// <response code="404">
    /// If a player matching the given id does not exist.
    /// If an admin note matching the given noteId does not exist
    /// </response>
    /// <response code="200">Returns the updated admin note</response>
    [HttpDelete("{id:int}/notes/{noteId:int}")]
    [Authorize(Roles = $"{OtrClaims.Roles.Admin}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<AdminNoteDTO>(StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteAdminNoteAsync(int id, int noteId)
    {
        if (!await playerService.ExistsAsync(id))
        {
            return NotFound();
        }

        var result = await adminNoteService.DeleteAsync<PlayerAdminNote>(noteId);
        return result
            ? Ok()
            : NotFound();
    }
}
