using API.Authorization;
using API.DTOs;
using API.Services.Interfaces;
using API.Utilities.Extensions;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[ApiVersion(1)]
[Route("api/v{version:apiVersion}/[controller]")]
public class GamesController(IGamesService gamesService) : Controller
{
    /// <summary>
    /// Get a game
    /// </summary>
    /// <param name="verified">Whether the game's scores must be verified</param>
    /// <response code="404">A game matching the given id does not exist</response>
    /// <response code="200">Returns a game</response>
    [HttpGet("{id:int}")]
    [Authorize(Roles = $"{OtrClaims.Roles.User}, {OtrClaims.Roles.Client}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType<GameDTO>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAsync(int id, [FromQuery] bool verified = true)
    {
        GameDTO? game = await gamesService.GetAsync(id, verified);
        if (game is null)
        {
            return NotFound();
        }

        return Ok(game);
    }

    /// <summary>
    /// Amend game data
    /// </summary>
    /// <param name="id">Game id</param>
    /// <param name="patch">JsonPatch data</param>
    /// <response code="404">A game matching the given id does not exist</response>
    /// <response code="400">The JsonPatch data is malformed</response>
    /// <response code="200">Returns the updated game</response>
    [HttpPatch("{id:int}")]
    [Authorize(Roles = OtrClaims.Roles.Admin)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<GameDTO>(StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateAsync(int id, [FromBody] JsonPatchDocument<GameDTO> patch)
    {
        // Ensure target game exists
        GameDTO? game = await gamesService.GetAsync(id, false);
        if (game is null)
        {
            return NotFound();
        }

        // Ensure request is only attempting to perform a replace operation.
        if (patch.Operations.Count == 0 || !patch.IsReplaceOnly())
        {
            return BadRequest();
        }

        // Patch and validate
        patch.ApplyTo(game, ModelState);
        if (!TryValidateModel(game))
        {
            return ValidationProblem(ModelState);
        }

        // Apply patched values to entity
        GameDTO? updatedGame = await gamesService.UpdateAsync(id, game);
        return Ok(updatedGame);
    }

    /// <summary>
    /// Delete a game
    /// </summary>
    /// <param name="id">Game id</param>
    /// <response code="404">A game matching the given id does not exist</response>
    /// <response code="204">The game was deleted successfully</response>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = OtrClaims.Roles.Admin)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        GameDTO? result = await gamesService.GetAsync(id, false);
        if (result is null)
        {
            return NotFound();
        }

        await gamesService.DeleteAsync(id);
        return NoContent();
    }

    /// <summary>
    /// Merge scores from source games into a target game. The source games must be from the same match
    /// and have the same beatmap as the target game. After successful merging, the source games are deleted.
    /// </summary>
    /// <param name="id">Id of the game to merge scores into</param>
    /// <param name="sourceGameIds">Game ids whose scores will be merged into the target game</param>
    /// <response code="400">Merge failed</response>
    /// <response code="200">State of the game after merging</response>
    [HttpPost("{id:int}:merge")]
    [Authorize(Roles = OtrClaims.Roles.Admin)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<GameDTO>(StatusCodes.Status200OK)]
    public async Task<IActionResult> MergeScoresAsync(int id, [FromBody] IEnumerable<int> sourceGameIds)
    {
        GameDTO? result = await gamesService.MergeScoresAsync(id, sourceGameIds);

        if (result is not null)
        {
            return Ok(result);
        }

        return BadRequest();
    }
}
