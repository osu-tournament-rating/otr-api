using System.ComponentModel.DataAnnotations;
using API.Authorization;
using API.DTOs;
using API.Utilities.Extensions;
using Database.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public partial class GameScoresController
{
    /// <summary>
    /// Create an admin note for a score
    /// </summary>
    /// <param name="id">Score id</param>
    /// <param name="note">Content of the admin note</param>
    /// <response code="404">A score matching the given id does not exist</response>
    /// <response code="400">The authorized user does not exist</response>
    /// <response code="200">Returns the created admin note</response>
    [HttpPost("{id:int}/notes")]
    [Authorize(Roles = OtrClaims.Roles.Admin)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<AdminNoteDTO>(StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateAdminNoteAsync(int id, [FromBody][Required] string note)
    {
        if (!await gameScoresService.ExistsAsync(id))
        {
            return NotFound();
        }

        AdminNoteDTO? result = await adminNoteService.CreateAsync<GameScoreAdminNote>(id, User.GetSubjectId(), note);
        return result is not null
            ? Ok(result)
            : BadRequest();
    }

    /// <summary>
    /// Update an admin note for a score
    /// </summary>
    /// <param name="id">Score id</param>
    /// <param name="noteId">Admin note id</param>
    /// <param name="note">New content of the admin note</param>
    /// <response code="404">
    /// A score matching the given id does not exist
    /// or an admin note matching the given noteId does not exist
    /// </response>
    /// <response code="200">Returns the updated admin note</response>
    [HttpPatch("{id:int}/notes/{noteId:int}")]
    [Authorize(Roles = OtrClaims.Roles.Admin)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType<AdminNoteDTO>(StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateAdminNoteAsync(
        int id,
        int noteId,
        [FromBody][Required] string note
    )
    {
        if (!await gameScoresService.ExistsAsync(id))
        {
            return NotFound();
        }

        AdminNoteDTO? existingNote = await adminNoteService.GetAsync<GameScoreAdminNote>(noteId);
        if (existingNote is null)
        {
            return NotFound();
        }

        existingNote.Note = note;
        AdminNoteDTO? result = await adminNoteService.UpdateAsync<GameScoreAdminNote>(existingNote);

        return result is not null
            ? Ok(result)
            : NotFound();
    }

    /// <summary>
    /// Delete an admin note for a score
    /// </summary>
    /// <param name="id">Score id</param>
    /// <param name="noteId">Admin note id</param>
    /// <response code="404">
    /// A score matching the given id does not exist
    /// or an admin note matching the given noteId does not exist
    /// </response>
    /// <response code="200">Returns the updated admin note</response>
    [HttpDelete("{id:int}/notes/{noteId:int}")]
    [Authorize(Roles = OtrClaims.Roles.Admin)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType<AdminNoteDTO>(StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteAdminNoteAsync(int id, int noteId)
    {
        if (!await gameScoresService.ExistsAsync(id))
        {
            return NotFound();
        }

        var result = await adminNoteService.DeleteAsync<GameScoreAdminNote>(noteId);
        return result
            ? Ok()
            : NotFound();
    }
}
