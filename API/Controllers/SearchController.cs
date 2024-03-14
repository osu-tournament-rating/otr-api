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
[AllowAnonymous]
[Route("api/v{version:apiVersion}/[controller]")]
public class SearchController(ITournamentsService tournamentsService, IMatchesService matchesService, IPlayerService playerService) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Test()
    {
        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> SearchByNames([FromQuery(Name = "tournamentName")] string? tournamentName, [FromQuery(Name = "matchName")] string? matchName, [FromQuery(Name = "username")] string? username)
    {
        if (tournamentName is not null)
        {
            TournamentDTO? tournamentDto = await tournamentsService.GetByName(tournamentName);

            return tournamentDto is null ? Ok(null) : Ok(new SearchResponseDTO()
            {
                Text = tournamentDto.Name,
                Url = $"/tournaments/{tournamentDto.Name}"
            });
        }

        if (matchName is not null)
        {
            MatchDTO? matchDto = await matchesService.GetByNameAsync(matchName);

            return matchDto is null ? Ok(null) : Ok(new SearchResponseDTO()
            {
                Text = matchDto.Id.ToString(),
                Url = $"/matches/{matchDto.MatchId}"
            });
        }

        //Since this is the last check, checking the inverse so code looks a bit cleaner.
        if (username is null)
        {
            return Ok(null);
        }

        PlayerDTO? playerDto = await playerService.GetByUsernameAsync(username);

        return playerDto is null ? Ok(null) : Ok(new SearchResponseDTO()
        {
            Text = playerDto.Username ?? "Restricted",
            Url = $"/players/{playerDto.OsuId}",
            Thumbnail = $"a.ppy.sh/{playerDto.OsuId}"
        });

    }
}
