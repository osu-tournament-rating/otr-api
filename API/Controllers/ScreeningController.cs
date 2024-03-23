using API.DTOs;
using API.Services.Interfaces;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[ApiVersion(1)]
[EnableCors]
[Route("api/v{version:apiVersion}/[controller]")]
public class ScreeningController(IScreeningService screeningService) : Controller
{
    [HttpPost]
    [Authorize(Roles = "user, client")]
    [EndpointSummary("Allows authorized users & clients to submit a collection of " +
                     "users to be screened based on a set of criteria")]
    public async Task<Results<Ok<IEnumerable<ScreeningResultDTO>>, BadRequest<string>>> ScreenAsync([FromBody] ScreeningDTO screeningRequest)
    {
        if (!screeningRequest.OsuPlayerIds.Any())
        {
            return TypedResults.BadRequest("Screening list cannot be empty");
        }

        if (screeningRequest.OsuPlayerIds.Count() > 1000)
        {
            return TypedResults.BadRequest("Screening list cannot exceed 1000 players");
        }

        if (screeningRequest.Ruleset is < 0 or > 3)
        {
            return TypedResults.BadRequest("Invalid ruleset (must be 0-3)");
        }

        if (screeningRequest.MatchesPlayed < 0)
        {
            return TypedResults.BadRequest("Matches played must be greater than or equal to 0");
        }

        if (screeningRequest.MinRating < 100)
        {
            return TypedResults.BadRequest("Minimum rating must be greater than or equal to 100");
        }

        if (screeningRequest.MaxRating < 100)
        {
            return TypedResults.BadRequest("Maximum rating must be greater than or equal to 100");
        }

        if (screeningRequest.MinRating >= screeningRequest.MaxRating)
        {
            return TypedResults.BadRequest("Minimum rating must be less than maximum rating");
        }

        IEnumerable<ScreeningResultDTO> results = await screeningService.ScreenAsync(screeningRequest);
        return TypedResults.Ok(results);
    }
}
