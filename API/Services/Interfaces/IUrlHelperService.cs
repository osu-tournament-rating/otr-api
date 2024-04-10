using API.DTOs;

namespace API.Services.Interfaces;

/// <summary>
/// Service for dynamically generating resource routes
/// </summary>
public interface IUrlHelperService
{
    /// <summary>
    /// Generate a resource route for the given <see cref="CreatedAtRouteValues"/>
    /// </summary>
    /// <returns>A resource route, or "unknown"</returns>
    /// <example>
    /// Action("Get", "Tournaments", { id = 12 }) => "http://localhost:5075/api/v1/tournaments/12"
    /// </example>
    string Action(CreatedAtRouteValues routeValues);
}
