using API.DTOs;
using API.Enums;
using API.Services.Interfaces;
using API.Utilities;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[ApiVersion(1)]
[EnableCors]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize(Roles = "user")]
[Authorize(Roles = "whitelist")]
public class TournamentsController(ITournamentsService tournamentsService, IMatchesService matchesService) : Controller
{
    private readonly ITournamentsService _tournamentsService = tournamentsService;
    private readonly IMatchesService _matchesService = matchesService;

    /*
     * FLOW:
     *
     * The user submits a tournament (containing metadata and links) to the front-end. They are looking to add new data
     * to our database that will eventually count towards ratings.
     *
     * This post endpoint takes these links, validates them (i.e. checks for duplicates,
     * whether the match titles align with osu! tournament naming conventions,
     * amount of matches being submitted, etc.).
     *
     * Assuming we have a good batch, we will mark all of the new items as "PENDING".
     * The API.Osu.Multiplayer.MultiplayerLobbyDataWorker service checks the database for pending links
     * periodically and processes them automatically.
     */
    [HttpPost]
    [Authorize(Roles = "user")]
    [Authorize(Roles = "whitelist")]
    [Authorize(Roles = "submit")]
    [EndpointSummary("Create a tournament")]
    [EndpointDescription("Submit a tournament and associated matches")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<TournamentDTO>> CreateAsync(
        [FromBody] TournamentWebSubmissionDTO wrapper,
        [FromQuery] bool verify = false
    )
    {
        MatchVerificationSource? verificationSource = null;
        if (verify)
        {
            if (!User.CanVerifyMatches())
            {
                return Unauthorized("You are not authorized to verify matches");
            }
            verificationSource = User.VerificationSource();
        }

        if (await _tournamentsService.ExistsAsync(wrapper.TournamentName, wrapper.Mode))
        {
            return BadRequest($"A tournament with name {wrapper.TournamentName} for mode {wrapper.Mode} already exists");
        }

        TournamentDTO result = await _tournamentsService.CreateAsync(wrapper);
        // Create matches, add them to the DTO
        result.Matches = (await _matchesService.CreateAsync(
            result.Id,
            wrapper.SubmitterId,
            wrapper.Ids, verify,
            (int?)verificationSource
        )).ToList();

        return Ok(result);
    }

    [HttpPost("{id:int}/matches")]
    [Authorize(Roles = "user")]
    [Authorize(Roles = "whitelist")]
    [Authorize(Roles = "submit")]
    [EndpointSummary("Create matches associated with a tournament")]
    [EndpointDescription("Append tournament matches to an existing tournament")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IEnumerable<MatchDTO>>> CreateMatchesAsync(
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
                return Unauthorized("You are not authorized to verify matches");
            }
            verificationSource = User.VerificationSource();
        }

        if (!await _tournamentsService.ExistsAsync(id))
        {
            return NotFound($"A tournament with id {id} does not exist");
        }

        IEnumerable<MatchDTO> result =
            await _matchesService.CreateAsync(
                id,
                wrapper.SubmitterId,
                wrapper.Ids, verify,
                (int?)verificationSource
            );
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    [EndpointSummary("Get a tournament")]
    public async Task<ActionResult<TournamentDTO>> GetAsync(int id)
    {
        TournamentDTO? result = await _tournamentsService.GetAsync(id);
        if (result is null)
        {
            return NotFound($"A tournament with id {id} does not exist");
        }
        return Ok(result);
    }

    [HttpGet]
    [Authorize(Roles = "admin, system")]
    [EndpointSummary("List all tournaments")]
    public async Task<ActionResult<IEnumerable<TournamentDTO>>> ListAsync()
    {
        IEnumerable<TournamentDTO> result = await _tournamentsService.GetAllAsync();
        return Ok(result);
    }

    [HttpPatch("{id:int}")]
    [Authorize(Roles = "admin, system")]
    [EndpointSummary("Amend tournament data")]
    [EndpointDescription("Amend tournament data (Excluded fields: id, matches)")]
    public async Task<ActionResult<TournamentDTO>> UpdateAsync(
        int id,
        [FromBody] JsonPatchDocument<TournamentDTO> patch
    )
    {
        // Ensure target tournament exists
        TournamentDTO? tournament = await _tournamentsService.GetAsync(id);
        if (tournament is null)
        {
            return NotFound($"A tournament with id {id} does not exist");
        }

        // Ensure request is only attempting to perform a replace operation.
        if (!patch.IsReplaceOnly())
        {
            return BadRequest("This endpoint can only perform replace operations.");
        }

        // Ensure request is not editing any restricted fields
        IEnumerable<string> invalidFields = patch.IncludesFields(["Id", "Matches"]).ToList();
        if (invalidFields.Any())
        {
            return BadRequest($"The following fields cannot be replaced: [{string.Join(", ", invalidFields)}]");
        }

        // Patch and validate
        patch.ApplyTo(tournament, ModelState);
        if (!TryValidateModel(tournament))
        {
            return BadRequest(ModelState);
        }

        // Apply patched values to entity
        TournamentDTO updatedTournament = await _tournamentsService.UpdateAsync(id, tournament);
        return Ok(updatedTournament);
    }
}
