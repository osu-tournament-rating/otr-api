using System.Text.Json.Serialization;
using JetBrains.Annotations;

namespace API.DTOs;

/// <summary>
/// Represents a newly created resource
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.Members)]
public abstract class CreatedResultBaseDTO
{
    /// <summary>
    /// Id of the resource
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Location of the resource
    /// </summary>
    public string Location { get; set; } = null!;

    /// <summary>
    /// Data used to generate the <see cref="Location"/>
    /// </summary>
    /// <remarks>This field should always be decorated with a <see cref="JsonIgnoreAttribute"/></remarks>
    [JsonIgnore]
    public abstract CreatedAtRouteValues CreatedAtRouteValues { get; }
}
