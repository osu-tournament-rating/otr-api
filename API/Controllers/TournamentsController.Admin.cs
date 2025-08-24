using API.Authorization;
using API.DTOs;
using API.Utilities.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public partial class TournamentsController
{
    /// <summary>
    /// Delete a tournament
    /// </summary>
    /// <param name="id">Tournament id</param>
    /// <response code="404">A tournament matching the given id does not exist</response>
    /// <response code="204">The tournament was deleted successfully</response>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = OtrClaims.Roles.Admin)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        if (!await tournamentsService.ExistsAsync(id))
        {
            return NotFound();
        }

        await tournamentsService.DeleteAsync(id);
        return NoContent();
    }

    /// <summary>
    /// Mark pre-rejected items as rejected, marks pre-verified
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
        return Ok(await tournamentsService.AcceptPreVerificationStatusesAsync(id, User.GetSubjectId()));
    }

    /// <summary>
    /// Rerun automation checks for a tournament
    /// </summary>
    /// <param name="id">Tournament id</param>
    /// <param name="overrideVerifiedState">Whether to override existing human-verified or rejected states</param>
    /// <response code="404">If a tournament matching the given id does not exist</response>
    /// <response code="200">The entities were updated successfully</response>
    [HttpPost("{id:int}:reset-automation-statuses")]
    [Authorize(Roles = OtrClaims.Roles.Admin)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> RerunAutomationChecksAsync(int id, [FromQuery] bool overrideVerifiedState = false)
    {
        if (!await tournamentsService.ExistsAsync(id))
        {
            return NotFound();
        }

        await tournamentsService.RerunAutomationChecksAsync(id, overrideVerifiedState);
        return Ok();
    }

    /// <summary>
    /// Add beatmaps to a tournament by osu! id
    /// </summary>
    /// <param name="id">Tournament id</param>
    /// <param name="osuBeatmapIds">A collection of osu! beatmap ids</param>
    /// <returns>The tournament's collection of pooled beatmaps</returns>
    /// <response code="404">A tournament matching the given id does not exist</response>
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
    /// Delete all pooled beatmaps from a tournament. This does not alter the beatmaps table. This only
    /// deletes the mapping between a tournament and a pooled beatmap.
    /// </summary>
    /// <param name="id">Tournament id</param>
    /// <param name="beatmapIds">
    /// An optional collection of specific beatmap ids to remove from the pooled beatmaps collection
    /// </param>
    /// <response code="404">A tournament matching the given id does not exist</response>
    /// <response code="204">All beatmaps were successfully removed</response>
    [HttpDelete("{id:int}/beatmaps")]
    [Authorize(Roles = OtrClaims.Roles.Admin)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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

    /// <summary>
    /// Enqueues data for match fetching 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpPost("{id:int}:fetch-match-data")]
    [Authorize(Roles = OtrClaims.Roles.Admin)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> FetchMatchDataAsync(int id)
    {
        if (!await tournamentsService.ExistsAsync(id))
        {
            return NotFound();
        }

        await tournamentsService.FetchMatchDataAsync(id);

        return Ok();
    }
}
