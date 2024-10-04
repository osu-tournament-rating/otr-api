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
public class TournamentsController(ITournamentsService tournamentsService) : Controller
{
    /// <summary>
    /// Get all tournaments which fit an optional request query
    /// </summary>
    /// <param name="requestQuery">The optional request query filter</param>
    /// <remarks>Will not include match data</remarks>
    /// <response code="200">Returns all tournaments which fit the request query</response>
    /// <returns>
    /// A page of tournaments
    /// </returns>
    [HttpGet]
    [ProducesResponseType<IEnumerable<TournamentDTO>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ListAsync([FromQuery] TournamentRequestQueryDTO? requestQuery)
    {
        requestQuery ??= new TournamentRequestQueryDTO();

        ICollection<TournamentDTO> tournaments = await tournamentsService.GetAsync(requestQuery);
        return Ok(tournaments);
    }

    /// <summary>
    /// Submit a tournament
    /// </summary>
    /// <param name="tournamentSubmission">Tournament submission data</param>
    /// <response code="400">
    /// If the given <see cref="tournamentSubmission"/> is malformed
    /// If a tournament matching the given name and ruleset already exists
    /// </response>
    /// <response code="201">Returns location information for the created tournament</response>
    [HttpPost]
    [Authorize(Roles = OtrClaims.Roles.User)]
    [ProducesResponseType<string>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<TournamentCreatedResultDTO>(StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateAsync([FromBody] TournamentSubmissionDTO tournamentSubmission)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState.ErrorMessage());
        }

        if (await tournamentsService.ExistsAsync(tournamentSubmission.Name, tournamentSubmission.Ruleset))
        {
            return BadRequest(
                $"A tournament with name {tournamentSubmission.Name} for ruleset {tournamentSubmission.Ruleset} already exists");
        }

        tournamentSubmission.ForumUrl = new Uri(tournamentSubmission.ForumUrl).GetLeftPart(UriPartial.Path); // Remove query string

        // Create tournament
        TournamentCreatedResultDTO result = await tournamentsService.CreateAsync(
            tournamentSubmission,
            User.GetSubjectId(),
            User.IsAdmin() || User.IsSystem()
        );

        return CreatedAtAction("Get", new { id = result.Id }, result);
    }

    /// <summary>
    /// Get a tournament
    /// </summary>
    /// <param name="id">Tournament id</param>
    /// <param name="verified">If true, specifically includes verified match data. If false,
    /// includes all data, regardless of verification status.
    /// Also includes all child navigations if false.
    /// Default true (strictly verified data with limited navigation properties)
    /// </param>
    /// <response code="404">If a tournament matching the given id does not exist</response>
    /// <response code="200">Returns the tournament</response>
    [HttpGet("{id:int}")]
    [Authorize(Roles = $"{OtrClaims.Roles.User}, {OtrClaims.Roles.Client}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType<TournamentDTO>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAsync(int id, [FromQuery] bool verified = true)
    {
        TournamentDTO? result = verified
            ? await tournamentsService.GetVerifiedAsync(id)
            : await tournamentsService.GetAsync(id);

        if (result is null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    /// <summary>
    /// Amend tournament data
    /// </summary>
    /// <param name="id">The tournament id</param>
    /// <param name="patch">JsonPatch data</param>
    /// <response code="404">If the provided id does not belong to a tournament</response>
    /// <response code="400">If JsonPatch data is malformed</response>
    /// <response code="200">Returns the patched tournament</response>
    [HttpPatch("{id:int}")]
    [Authorize(Roles = OtrClaims.Roles.Admin)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<string>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<TournamentDTO>(StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateAsync(
        int id,
        [FromBody] JsonPatchDocument<TournamentDTO> patch
    )
    {
        // Ensure target tournament exists
        TournamentDTO? tournament = await tournamentsService.GetAsync(id);
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
            return BadRequest(ModelState.ErrorMessage());
        }

        // Apply patched values to entity
        TournamentDTO? updatedTournament = await tournamentsService.UpdateAsync(id, tournament);
        return Ok(updatedTournament!);
    }

    /// <summary>
    /// List all matches from a tournament
    /// </summary>
    /// <param name="id">Tournament id</param>
    /// <response code="404">If a tournament matching the given id does not exist</response>
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
}
