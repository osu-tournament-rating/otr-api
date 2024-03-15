using API.DTOs;
using API.Services.Interfaces;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[ApiVersion(1)]
[EnableCors]
[Authorize(Roles = "system")] // Internal access only at this time
[Route("api/v{version:apiVersion}/[controller]")]
public class SearchController(ITournamentsService tournamentsService, IMatchesService matchesService, IPlayerService playerService) : Controller
{
    [HttpGet]
    [EndpointSummary("Allows for partial or full searching on the names of tournaments, matches and usernames.")]
    [ProducesResponseType<SearchResponseDTO>(StatusCodes.Status200OK)]
    public async Task<IActionResult> SearchByNames([FromQuery] string? tournamentName, [FromQuery] string? matchName, [FromQuery] string? username)
    {
        if (!string.IsNullOrEmpty(tournamentName))
        {
            TournamentDTO? tournamentDto = await tournamentsService.SearchAsync(tournamentName);

            return tournamentDto is null ? Ok(null) : Ok(new SearchResponseDTO()
            {
                Text = tournamentDto.Name,
                Url = $"/tournaments/{tournamentDto.Name}"
            });
        }

        if (!string.IsNullOrEmpty(matchName))
        {
            MatchDTO? matchDto = await matchesService.SearchAsync(matchName);

            return matchDto is null ? Ok(null) : Ok(new SearchResponseDTO()
            {
                Text = matchDto.Id.ToString(),
                Url = $"/matches/{matchDto.MatchId}"
            });
        }

        //Since this is the last check, checking the inverse so code looks a bit cleaner.
        if (string.IsNullOrEmpty(username))
        {
            return Ok(null);
        }

        PlayerDTO? playerDto = await playerService.SearchAsync(username);

        return playerDto is null ? Ok(null) : Ok(new SearchResponseDTO()
        {
            Text = playerDto.Username ?? "<Unknown>",
            Url = $"/users/{playerDto.Id}",
            Thumbnail = $"a.ppy.sh/{playerDto.OsuId}"
        });
    }
}
