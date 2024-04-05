using API.DTOs;

namespace API.Services.Interfaces;

public interface IUrlHelperService
{
    /// <summary>
    /// Gets a resource route
    /// </summary>
    /// <param name="action">Name of the controller action</param>
    /// <param name="controller">Name of the controller</param>
    /// <param name="routeValues">Any required values to pass to the action</param>
    /// <returns>The resource route, or "unknown"</returns>
    /// <example>
    /// Action("Get", "Tournaments", { id = 12 }) => "/api/v1/tournaments/12"
    /// </example>
    string Action(string action, string controller, object routeValues);

    /// <summary>
    /// Get a resource route for the given <see cref="CreatedAtRouteValues"/>
    /// </summary>
    /// <returns>The resource route, or "unknown"</returns>
    string Action(CreatedAtRouteValues routeValues);
}
