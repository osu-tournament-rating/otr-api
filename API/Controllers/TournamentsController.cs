using API.DTOs;
using API.Enums;
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
    /// The API.Osu.Multiplayer.MultiplayerLobbyDataWorker service checks the database for pending links
    /// periodically and processes them automatically.
    /// </remarks>
    /// <param name="wrapper">Tournament submission data</param>
    /// <param name="verify">Optionally verify all included matches</param>
    /// <response code="401">If verify is true and the User does not have match verification privileges</response>
    /// <response code="400">If a tournament matching the given name and mode already exists</response>
    /// <response code="201">Returns location information for the created tournament</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<TournamentCreatedResultDTO>(StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateAsync(
        [FromBody] TournamentWebSubmissionDTO wrapper,
        [FromQuery] bool verify = false
    )
    {
        // Determine verification source
        MatchVerificationSource? verificationSource = null;
        if (verify)
        {
            if (!User.CanVerifyMatches())
            {
                return Unauthorized();
            }

            verificationSource = User.VerificationSource();
        }

        if (await _tournamentsService.ExistsAsync(wrapper.TournamentName, wrapper.Mode))
        {
            return BadRequest(
                $"A tournament with name {wrapper.TournamentName} for mode {wrapper.Mode} already exists");
        }

        // Create tournament
        TournamentCreatedResultDTO result =
            await _tournamentsService.CreateAsync(wrapper, verify, (int?)verificationSource);
        return Created(result.Location, result);
    }

    /// <summary>
    /// Get a tournament
    /// </summary>
    /// <param name="id">Tournament id</param>
    /// <response code="404">If a tournament matching the given id does not exist</response>
    /// <response code="200">Returns the tournament</response>
    [HttpGet("{id:int}")]
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
    /// <param name="wrapper">Match submission data</param>
    /// <param name="verify">Optionally verify all included matches</param>
    /// <response code="401">If verify is true and the User does not have match verification privileges</response>
    /// <response code="404">If a tournament matching the given id does not exist</response>
    /// <response code="201">Returns location information of the created matches</response>
    [HttpPost("{id:int}/matches")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType<IEnumerable<MatchCreatedResultDTO>>(StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateMatchesAsync(
        int id,
        [FromBody] MatchesWebSubmissionDTO wrapper,
        [FromQuery] bool verify = false
    )
    {
        MatchVerificationSource? verificationSource = null;
        if (verify)
        {
            if (!User.CanVerifyMatches())
            {
                return Unauthorized();
            }

            verificationSource = User.VerificationSource();
        }

        IEnumerable<MatchCreatedResultDTO>? result =
            await _matchesService.CreateAsync(
                id,
                wrapper.SubmitterId,
                wrapper.Ids, verify,
                (int?)verificationSource
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
