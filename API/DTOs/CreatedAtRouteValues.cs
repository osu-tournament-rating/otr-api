using Microsoft.AspNetCore.Mvc;

namespace API.DTOs;

/// <summary>
/// Represents data for constructing <see cref="CreatedResult"/>
/// </summary>
public class CreatedAtRouteValues
{
    private string? _controller;
    private string? _action;

    /// <summary>
    /// Any route or query parameters that must be included in the URI
    /// </summary>
    public object? RouteValues { get; set; }

    /// <summary>
    /// The controller method that produces the resource
    /// </summary>
    public string? Action
    {
        get => _action?.Replace("Async", string.Empty);
        set => _action = value;
    }

    /// <summary>
    /// The controller that produces the resource
    /// </summary>
    public string? Controller
    {
        get => _controller?.Replace("Controller", string.Empty);
        set => _controller = value;
    }
}
