using API.Authorization;
using API.DTOs;
using API.Utilities.Extensions;
using Database.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public partial class TournamentsController
{
    /// <summary>
    /// Marks pre-rejected items as rejected, marks pre-verified
    /// items as verified. Applies for the tournament and all children.
    /// </summary>
    /// <param name="id">Tournament id</param>
    /// <response code="404">If a tournament matching the given id does not exist</response>
    /// <response code="200">All items were updated successfully</response>
    [HttpPost("{id:int}:accept-pre-verification-statuses")]
    [Authorize(Roles = OtrClaims.Roles.Admin)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType<TournamentDTO>(StatusCodes.Status200OK)]
    public async Task<IActionResult> AcceptPreVerificationStatusesAsync(int id)
    {
        if (!await tournamentsService.ExistsAsync(id))
        {
            return NotFound();
        }

        // Result will never be null here
        return Ok(await tournamentsService.AcceptPreVerificationStatusesAsync(id));
    }


    /// <summary>
    /// Creates an admin note for a tournament
    /// </summary>
    /// <param name="id">Tournament id</param>
    /// <response code="401">If the requester is not properly authorized</response>
    /// <response code="404">If a tournament matching the given id does not exist</response>
    /// <response code="400">If the authorized user does not exist</response>
    /// <response code="200">Returns the created admin note</response>
    [HttpPost("{id:int}/notes")]
    [Authorize(Roles = OtrClaims.Roles.Admin)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<AdminNoteDTO>(StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateAdminNoteAsync(int id, [FromBody] string note)
    {
        if (!await tournamentsService.ExistsAsync(id))
        {
            return NotFound();
        }

        AdminNoteDTO? result = await adminNoteService.CreateAsync<TournamentAdminNote>(id, User.GetSubjectId(), note);
        return result is not null
            ? Ok(result)
            : BadRequest();
    }

    /// <summary>
    /// List all admin notes from a tournament
    /// </summary>
    /// <param name="id">Tournament id</param>
    /// <response code="404">If a tournament matching the given id does not exist</response>
    /// <response code="200">Returns all admin notes from a tournament</response>
    [HttpGet("{id:int}/notes")]
    [Authorize(Roles = OtrClaims.Roles.Admin)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType<IEnumerable<AdminNoteDTO>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> ListAdminNotesAsync(int id)
    {
        if (!await tournamentsService.ExistsAsync(id))
        {
            return NotFound();
        }

        return Ok(await adminNoteService.ListAsync<TournamentAdminNote>(id));
    }

    /// <summary>
    /// Updates an admin note for a tournament
    /// </summary>
    /// <param name="id">Tournament id</param>
    /// <param name="noteId">Admin note id</param>
    /// <response code="401">If the requester is not properly authorized</response>
    /// <response code="404">
    /// If a tournament matching the given id does not exist.
    /// If an admin note matching the given noteId does not exist
    /// </response>
    /// <response code="403">If the requester did not create the admin note</response>
    /// <response code="200">Returns the updated admin note</response>
    [HttpPatch("{id:int}/notes/{noteId:int}")]
    [Authorize(Roles = OtrClaims.Roles.Admin)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<AdminNoteDTO>(StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateAdminNoteAsync(int id, int noteId, [FromBody] string note)
    {
        if (!await tournamentsService.ExistsAsync(id))
        {
            return NotFound();
        }

        AdminNoteDTO? existingNote = await adminNoteService.GetAsync<TournamentAdminNote>(noteId);
        if (existingNote is null)
        {
            return NotFound();
        }

        existingNote.Note = note;
        AdminNoteDTO? result = await adminNoteService.UpdateAsync<TournamentAdminNote>(existingNote);

        return result is not null
            ? Ok(result)
            : NotFound();
    }

    /// <summary>
    /// Deletes an admin note for a tournament
    /// </summary>
    /// <param name="id">Tournament id</param>
    /// <param name="noteId">Admin note id</param>
    /// <response code="401">If the requester is not properly authorized</response>
    /// <response code="404">
    /// If a tournament matching the given id does not exist.
    /// If an admin note matching the given noteId does not exist
    /// </response>
    /// <response code="200">Returns the updated admin note</response>
    [HttpDelete("{id:int}/notes/{noteId:int}")]
    [Authorize(Roles = OtrClaims.Roles.Admin)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<AdminNoteDTO>(StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteAdminNoteAsync(int id, int noteId)
    {
        if (!await tournamentsService.ExistsAsync(id))
        {
            return NotFound();
        }

        var result = await adminNoteService.DeleteAsync<TournamentAdminNote>(noteId);
        return result
            ? Ok()
            : NotFound();
    }

    /// <summary>
    /// Adds beatmaps to a tournament by osu! id
    /// </summary>
    /// <param name="id">Tournament id</param>
    /// <param name="osuBeatmapIds">A collection of osu! beatmap ids</param>
    /// <returns>The tournament's collection of pooled beatmaps</returns>
    /// <response code="404">The tournament does not exist</response>
    /// <response code="200">The beatmaps were added successfully</response>
    [HttpPost("{id:int}/beatmaps")]
    [Authorize(Roles = OtrClaims.Roles.Admin)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> InsertBeatmapsAsync(int id, [FromBody] ICollection<long> osuBeatmapIds)
    {
        if (!await tournamentsService.ExistsAsync(id))
        {
            return NotFound();
        }

        ICollection<BeatmapDTO> pooledBeatmaps = await tournamentsService.AddPooledBeatmapsAsync(id, osuBeatmapIds);
        return Ok(pooledBeatmaps);
    }

    /// <summary>
    /// Deletes all pooled beatmaps from a tournament. This does not alter the beatmaps table. This only
    /// deletes the mapping between a tournament and a pooled beatmap.
    /// </summary>
    /// <param name="id">Tournament id</param>
    /// <param name="beatmapIds">
    /// An optional collection of specific beatmap ids to remove from the pooled beatmaps collection
    /// </param>
    /// <response code="404">The tournament does not exist</response>
    /// <response code="204">
    /// All beatmaps were successfully removed from the list of pooled beatmaps for the tournament
    /// </response>
    [HttpDelete("{id:int}/beatmaps")]
    [Authorize(Roles = OtrClaims.Roles.Admin)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteBeatmapsAsync(int id, [FromBody] ICollection<int>? beatmapIds = null)
    {
        if (!await tournamentsService.ExistsAsync(id))
        {
            return NotFound();
        }

        if (beatmapIds is null || beatmapIds.Count == 0)
        {
            await tournamentsService.DeletePooledBeatmapsAsync(id);
        }
        else
        {
            await tournamentsService.DeletePooledBeatmapsAsync(id, beatmapIds);
        }

        return NoContent();
    }
}
