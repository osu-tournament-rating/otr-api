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
[Route("api/v{version:apiVersion}/[controller]/{id:int}")]
public class GameScoresController(IGameScoresService gameScoresService) : Controller
{
    /// <summary>
    /// Get a score
    /// </summary>
    /// <response code="404">A score matching the given id does not exist</response>
    /// <response code="200">Returns the score</response>
    [HttpGet]
    [Authorize(Roles = OtrClaims.Roles.Admin)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType<GameScoreDTO>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAsync(int id)
    {
        GameScoreDTO? score = await gameScoresService.GetAsync(id);
        if (score is null)
        {
            return NotFound();
        }

        return Ok(score);
    }

    /// <summary>
    /// Amend score data
    /// </summary>
    /// <param name="id">Score id</param>
    /// <param name="patch">JsonPatch data</param>
    /// <response code="404">A score matching the given id does not exist</response>
    /// <response code="400">The JsonPatch data is malformed</response>
    /// <response code="200">Returns the updated score</response>
    [HttpPatch]
    [Authorize(Roles = OtrClaims.Roles.Admin)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<GameScoreDTO>(StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateAsync(int id, [FromBody] JsonPatchDocument<GameScoreDTO> patch)
    {
        // Ensure target game score exists
        GameScoreDTO? score = await gameScoresService.GetAsync(id);
        if (score is null)
        {
            return NotFound();
        }

        // Ensure request is only attempting to perform a replace operation.
        if (patch.Operations.Count == 0 || !patch.IsReplaceOnly())
        {
            return BadRequest();
        }

        // Patch and validate
        patch.ApplyTo(score, ModelState);
        if (!TryValidateModel(score))
        {
            return ValidationProblem(ModelState);
        }

        // Apply patched values to entity
        GameScoreDTO? updatedScore = await gameScoresService.UpdateAsync(id, score);
        return Ok(updatedScore!);
    }

    /// <summary>
    /// Delete a score
    /// </summary>
    /// <param name="id">Score id</param>
    /// <response code="404">A score matching the given id does not exist</response>
    /// <response code="204">The score was deleted successfully</response>
    [HttpDelete]
    [Authorize(Roles = OtrClaims.Roles.Admin)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        GameScoreDTO? result = await gameScoresService.GetAsync(id);
        if (result is null)
        {
            return NotFound();
        }

        await gameScoresService.DeleteAsync(id);
        return NoContent();
    }
}
