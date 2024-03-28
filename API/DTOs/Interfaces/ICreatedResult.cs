using Newtonsoft.Json;

namespace API.DTOs.Interfaces;

/// <summary>
/// An object yielded from a 201 Created response
/// </summary>
public interface ICreatedResult
{
    /// <summary>
    /// Data used to construct the <see cref="Location"/>
    /// </summary>
    /// <remarks>This field should always be decorated with a <see cref="JsonIgnoreAttribute"/></remarks>
    public CreatedAtRouteValues CreatedAtRouteValues { get; }

    /// <summary>
    /// Location of the resource
    /// </summary>
    public string Location { get; set; }
}
