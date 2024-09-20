using API.DTOs;
using API.Services.Interfaces;
using API.Utilities;
using API.Utilities.Extensions;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[ApiVersion(1)]
[Route("api/v{version:apiVersion}/[controller]")]
public class GameScoresController(IGameScoresService gameScoresService) : Controller
{
    /// <summary>
    ///  Amend score data
    /// </summary>
    /// <param name="id">The score id</param>
    /// <param name="patch">JsonPatch data</param>
    /// <response code="404">If the provided id does not belong to a score</response>
    /// <response code="400">If JsonPatch data is malformed</response>
    /// <response code="200">Returns the patched score</response>
    [Authorize(Roles = OtrClaims.Admin)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<string>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<GameScoreDTO>(StatusCodes.Status200OK)]
    [HttpPatch("{id:int}")]
    public async Task<IActionResult> UpdateAsync(int id, [FromBody] JsonPatchDocument<GameScoreDTO> patch)
    {
        // Ensure target game score exists
        GameScoreDTO? score = await gameScoresService.GetAsync(id);
        if (score is null)
        {
            return NotFound();
        }

        // Ensure request is only attempting to perform a replace operation.
        if (!patch.IsReplaceOnly())
        {
            return BadRequest();
        }

        // Patch and validate
        patch.ApplyTo(score, ModelState);
        if (!TryValidateModel(score))
        {
            return BadRequest(ModelState.ErrorMessage());
        }

        // Apply patched values to entity
        GameScoreDTO? updatedScore = await gameScoresService.UpdateAsync(id, score);
        return Ok(updatedScore!);
    }
}
