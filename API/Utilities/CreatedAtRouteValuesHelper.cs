using API.Controllers;
using API.DTOs;

namespace API.Utilities;

/// <summary>
/// Helper class to dynamically generate resource route access information
/// </summary>
public static class CreatedAtRouteValuesHelper
{
    /// <summary>
    /// Creates route values for accessing a tournament
    /// </summary>
    public static CreatedAtRouteValues GetTournament(int id) =>
        new()
        {
            Action = nameof(TournamentsController.GetAsync),
            Controller = nameof(TournamentsController),
            RouteValues = new { id }
        };

    /// <summary>
    /// Creates route values for accessing a match
    /// </summary>
    public static CreatedAtRouteValues GetMatch(int id) =>
        new()
        {
            Action = nameof(MatchesController.GetAsync),
            Controller = nameof(MatchesController),
            RouteValues = new { id }
        };
}
