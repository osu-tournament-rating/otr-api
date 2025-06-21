using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;

namespace API.DTOs;

/// <summary>
/// Represents data for constructing <see cref="CreatedResult"/>
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.Members)]
public class CreatedAtRouteValues
{
    private readonly string? _controller;
    private readonly string? _action;

    /// <summary>
    /// Any route or query parameters that must be included in the URI
    /// </summary>
    public object? RouteValues { get; init; }

    /// <summary>
    /// The controller method that produces the resource
    /// </summary>
    public string? Action
    {
        get => _action?.Replace("Async", string.Empty);
        init => _action = value;
    }

    /// <summary>
    /// The controller that produces the resource
    /// </summary>
    public string? Controller
    {
        get => _controller?.Replace("Controller", string.Empty);
        init => _controller = value;
    }
}
