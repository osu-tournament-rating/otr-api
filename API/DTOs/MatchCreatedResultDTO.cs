using System.Text.Json.Serialization;
using API.Utilities;

namespace API.DTOs;

/// <summary>
/// Represents a created match
/// </summary>
public class MatchCreatedResultDTO : CreatedResultBaseDTO
{
    [JsonIgnore]
    public override CreatedAtRouteValues CreatedAtRouteValues => CreatedAtRouteValuesHelper.GetMatch(Id);

    /// <summary>
    /// osu! match id
    /// </summary>
    public long OsuId { get; set; }
}
