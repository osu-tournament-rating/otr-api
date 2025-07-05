using System.ComponentModel.DataAnnotations;
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
public partial class TournamentsController(ITournamentsService tournamentsService) : Controller
{
    /// <summary>
    /// Get all tournaments which fit an optional request query
    /// </summary>
    /// <remarks>Results will not include match data</remarks>
    /// <response code="200">Returns all tournaments which fit the request query</response>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType<IEnumerable<TournamentDTO>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> ListAsync([FromQuery] TournamentRequestQueryDTO requestQuery) =>
        Ok(await tournamentsService.GetAsync(requestQuery));

    /// <summary>
    /// Submit a tournament
    /// </summary>
    /// <param name="tournamentSubmission">Tournament submission data</param>
    /// <response code="400">
    /// The tournament submission is malformed or
    /// a tournament matching the given name and ruleset already exists
    /// </response>
    /// <response code="201">Returns location information for the created tournament</response>
    [HttpPost]
    [Authorize(Roles = OtrClaims.Roles.User)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<TournamentCreatedResultDTO>(StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateAsync([FromBody][Required] TournamentSubmissionDTO tournamentSubmission)
    {
        // Sanitize id collections
        tournamentSubmission.Ids = tournamentSubmission.Ids.Distinct().Where(id => id >= 1).ToList();
        tournamentSubmission.BeatmapIds = tournamentSubmission.BeatmapIds.Distinct().Where(id => id >= 1);

        #region Model Validation

        // Check for unique submissions
        if (await tournamentsService.ExistsAsync(tournamentSubmission.Name, tournamentSubmission.Ruleset))
        {
            ModelState.AddModelError(
                nameof(TournamentSubmissionDTO.Name),
                $"A tournament with the name '{tournamentSubmission.Name}' and ruleset " +
                $"'{tournamentSubmission.Ruleset}' already exists."
            );
            ModelState.AddModelError(
                nameof(TournamentSubmissionDTO.Ruleset),
                $"A tournament with the ruleset '{tournamentSubmission.Ruleset}' and name " +
                $"'{tournamentSubmission.Name}' already exists."
            );
        }

        // Check for existing match IDs
        var existingMatchIds = (await tournamentsService.GetExistingMatchIdsAsync(tournamentSubmission.Ids)).ToList();
        if (existingMatchIds.Count != 0)
        {
            string matchIdsString = string.Join(", ", existingMatchIds.OrderBy(id => id));
            ModelState.AddModelError(
                nameof(TournamentSubmissionDTO.Ids),
                $"osu! match IDs {matchIdsString} already exist"
            );
        }

        // Some submission data requires admin permission
        if (!User.IsAdmin())
        {
            // Check reject on submit
            if (tournamentSubmission.RejectionReason.HasValue)
            {
                ModelState.AddModelError(
                    nameof(TournamentSubmissionDTO.RejectionReason),
                    $"The field {nameof(TournamentSubmissionDTO.RejectionReason)} may only be supplied by admin users."
                );
            }

            // We never want unprivileged users submitting empty tournaments
            if (!tournamentSubmission.Ids.Any())
            {
                ModelState.AddModelError(
                    nameof(TournamentSubmissionDTO.Ids),
                    $"The field {nameof(TournamentSubmissionDTO.Ids)} cannot be empty."
                );
            }
        }

        // Check for valid forum post url
        if (!Uri.TryCreate(tournamentSubmission.ForumUrl, UriKind.Absolute, out Uri? sanitizedUri))
        {
            ModelState.AddModelError(
                nameof(TournamentSubmissionDTO.ForumUrl),
                $"The field {nameof(TournamentSubmissionDTO.ForumUrl)} is invalid."
            );
        }

        if (!ModelState.IsValid)
        {
            return ValidationProblem();
        }

        #endregion

        // Sanitize forum url
        tournamentSubmission.ForumUrl = sanitizedUri!.GetLeftPart(UriPartial.Path);

        // Create tournament
        TournamentCreatedResultDTO result = await tournamentsService.CreateAsync(
            tournamentSubmission,
            User.GetSubjectId(),
            User.IsAdmin()
        );

        return CreatedAtAction("Get", new { id = result.Id }, result);
    }

    /// <summary>
    /// Get a tournament
    /// </summary>
    /// <param name="id">Tournament id</param>
    /// <response code="404">A tournament matching the given id does not exist</response>
    /// <response code="200">Returns a tournament</response>
    [HttpGet("{id:int}")]
    [Authorize(Roles = $"{OtrClaims.Roles.User}, {OtrClaims.Roles.Client}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType<TournamentDTO>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAsync(int id)
    {
        TournamentDTO? result = await tournamentsService.GetAsync(id);

        if (result is null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    /// <summary>
    /// Amend tournament data
    /// </summary>
    /// <param name="id">Tournament id</param>
    /// <param name="patch">JsonPatch data</param>
    /// <response code="404">A tournament matching the given id does not exist</response>
    /// <response code="400">JsonPatch data is malformed</response>
    /// <response code="200">Returns the updated tournament</response>
    [HttpPatch("{id:int}")]
    [Authorize(Roles = OtrClaims.Roles.Admin)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<string>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<TournamentCompactDTO>(StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateAsync(
        int id,
        [FromBody] JsonPatchDocument<TournamentCompactDTO> patch
    )
    {
        // Ensure target tournament exists
        TournamentCompactDTO? tournament = await tournamentsService.GetAsync(id, false);
        if (tournament is null)
        {
            return NotFound();
        }

        // Ensure request is only attempting to perform a replace operation.
        if (patch.Operations.Count == 0 || !patch.IsReplaceOnly())
        {
            return BadRequest();
        }

        // Patch and validate
        patch.ApplyTo(tournament, ModelState);
        if (!TryValidateModel(tournament))
        {
            return ValidationProblem(ModelState);
        }

        // Apply patched values to entity
        TournamentCompactDTO? updatedTournament = await tournamentsService.UpdateAsync(id, tournament);
        return Ok(updatedTournament!);
    }

    /// <summary>
    /// List all matches from a tournament
    /// </summary>
    /// <param name="id">Tournament id</param>
    /// <response code="404">A tournament matching the given id does not exist</response>
    /// <response code="200">Returns all matches from a tournament</response>
    [HttpGet("{id:int}/matches")]
    [Authorize(Roles = $"{OtrClaims.Roles.User}, {OtrClaims.Roles.Client}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType<IEnumerable<MatchDTO>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> ListMatchesAsync(int id)
    {
        TournamentDTO? result = await tournamentsService.GetAsync(id);
        if (result is null)
        {
            return NotFound();
        }

        return Ok(result.Matches);
    }

    /// <summary>
    /// Get all beatmaps pooled by a tournament
    /// </summary>
    /// <param name="id">Tournament id</param>
    /// <response code="404">A tournament matching the given id does not exist</response>
    /// <response code="200">Returns a collection of pooled beatmaps</response>
    [HttpGet("{id:int}/beatmaps")]
    [Authorize(Roles = $"{OtrClaims.Roles.User}, {OtrClaims.Roles.Client}")]
    [ProducesResponseType<IEnumerable<BeatmapDTO>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetBeatmapsAsync(int id)
    {
        if (!await tournamentsService.ExistsAsync(id))
        {
            return NotFound();
        }

        return Ok(await tournamentsService.GetPooledBeatmapsAsync(id));
    }
}
