using API.DTOs;
using API.Services.Interfaces;
using API.Utilities;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace API.Controllers;

[ApiController]
[ApiVersion(1)]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize(Roles = "user")]
[Authorize(Roles = "whitelist")]
public class TournamentsController(ITournamentsService tournamentsService, IMatchesService matchesService) : Controller
{
    private readonly ITournamentsService _tournamentsService = tournamentsService;
    private readonly IMatchesService _matchesService = matchesService;

    /// <summary>
    /// List all tournaments
    /// </summary>
    /// <remarks>Will not include match data</remarks>
    /// <response code="200">Returns all tournaments</response>
    [HttpGet]
    [Authorize(Roles = "admin, system")]
    [ProducesResponseType<IEnumerable<TournamentDTO>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> ListAsync()
    {
        IEnumerable<TournamentDTO> result = await _tournamentsService.ListAsync();
        return Ok(result);
    }

    /// <summary>
    /// Submit a tournament
    /// </summary>
    /// <remarks>
    /// The user submits a tournament (containing metadata and links) to the front-end. They are looking to add new data
    /// to our database that will eventually count towards ratings.
    ///
    /// This post endpoint takes these links, validates them (i.e. checks for duplicates,
    /// whether the match titles align with osu! tournament naming conventions,
    /// amount of matches being submitted, etc.).
    ///
    /// Assuming we have a good batch, we will mark all of the new items as "PENDING".
    /// If verify is true, they will be marked as "VERIFIED" immediately.
    /// The MultiplayerLobbyDataWorker service checks the database for pending links
    /// periodically and processes them automatically.
    /// </remarks>
    /// <param name="tournamentSubmission">Tournament submission data</param>
    /// <param name="verify">Optionally verify all included matches, assuming the user has permission to do so</param>
    /// <response code="401">If verify is true and the User does not have match verification privileges</response>
    /// <response code="400">If a tournament matching the given name and mode already exists, or the given <see cref="tournamentSubmission"/> is malformed</response>
    /// <response code="201">Returns location information for the created tournament</response>
    [HttpPost]
    [ProducesResponseType<ModelStateDictionary>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<TournamentCreatedResultDTO>(StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateAsync([FromBody] TournamentWebSubmissionDTO tournamentSubmission,
        [FromQuery] bool verify = false)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (verify && !User.IsMatchVerifier())
        {
            return Unauthorized();
        }

        if (await _tournamentsService.ExistsAsync(tournamentSubmission.TournamentName, tournamentSubmission.Mode))
        {
            return BadRequest(
                $"A tournament with name {tournamentSubmission.TournamentName} for mode {tournamentSubmission.Mode} already exists");
        }

        // Create tournament
        TournamentCreatedResultDTO result =
            await _tournamentsService.CreateAsync(tournamentSubmission, verify, (int?)User.VerificationSource());
        return CreatedAtAction("Get", new { id = result.Id }, result);
    }

    /// <summary>
    /// Get a tournament
    /// </summary>
    /// <param name="id">Tournament id</param>
    /// <response code="404">If a tournament matching the given id does not exist</response>
    /// <response code="200">Returns the tournament</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType<TournamentDTO>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAsync(int id)
    {
        TournamentDTO? result = await _tournamentsService.GetAsync(id);
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
    /// <response code="404">If a tournament matching the given id does not exist</response>
    /// <response code="400">If JsonPatch data is malformed</response>
    /// <response code="200">Returns the patched tournament</response>
    [HttpPatch("{id:int}")]
    [Authorize(Roles = "admin")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ModelStateDictionary>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<TournamentDTO>(StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateAsync(
        int id,
        [FromBody] JsonPatchDocument<TournamentDTO> patch
    )
    {
        // Ensure target tournament exists
        TournamentDTO? tournament = await _tournamentsService.GetAsync(id);
        if (tournament is null)
        {
            return NotFound();
        }

        // Ensure request is only attempting to perform a replace operation.
        if (!patch.IsReplaceOnly())
        {
            return BadRequest();
        }

        // Patch and validate
        patch.ApplyTo(tournament, ModelState);
        if (!TryValidateModel(tournament))
        {
            return BadRequest(ModelState);
        }

        // Apply patched values to entity
        TournamentDTO? updatedTournament = await _tournamentsService.UpdateAsync(id, tournament);
        return Ok(updatedTournament!);
    }

    /// <summary>
    /// Submit matches to an existing tournament
    /// </summary>
    /// <param name="id">Tournament id</param>
    /// <param name="matchesSubmission">Match submission data</param>
    /// <param name="verify">Optionally verify all included matches, assuming the user has permission to do so</param>
    /// <response code="401">If verify is true and the User does not have match verification privileges</response>
    /// <response code="404">If a tournament matching the given id does not exist</response>
    /// <response code="201">Returns location information of the created matches</response>
    [HttpPost("{id:int}/matches")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType<IEnumerable<MatchCreatedResultDTO>>(StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateMatchesAsync(int id,
        [FromBody] MatchesWebSubmissionDTO matchesSubmission,
        [FromQuery] bool verify = false)
    {
        if (verify && !User.IsMatchVerifier())
        {
            return Unauthorized();
        }

        IEnumerable<MatchCreatedResultDTO>? result =
            await _matchesService.CreateAsync(
                id,
                matchesSubmission.SubmitterId,
                matchesSubmission.Ids, verify,
                (int?)User.VerificationSource()
            );

        if (result is null)
        {
            return NotFound();
        }

        // Use no location header for multiple creation
        return Created((string?)null, result);
    }

    /// <summary>
    /// List all matches from a tournament
    /// </summary>
    /// <param name="id">Tournament id</param>
    /// <response code="404">If a tournament matching the given id does not exist</response>
    /// <response code="200">Returns all matches from a tournament</response>
    [HttpGet("{id:int}/matches")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType<IEnumerable<MatchDTO>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> ListMatchesAsync(int id)
    {
        TournamentDTO? result = await _tournamentsService.GetAsync(id);
        if (result is null)
        {
            return NotFound();
        }

        return Ok(result.Matches);
    }
}
