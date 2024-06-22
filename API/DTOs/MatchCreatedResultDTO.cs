using API.DTOs.Interfaces;
using API.Utilities;
using Newtonsoft.Json;

namespace API.DTOs;

/// <summary>
/// Represents a created match
/// </summary>
public class MatchCreatedResultDTO : CreatedResultBaseDTO, ICreatedResult
{
    [JsonIgnore]
    public CreatedAtRouteValues CreatedAtRouteValues => CreatedAtRouteValuesHelper.GetMatch(Id);

    /// <summary>
    /// osu! match id
    /// </summary>
    public long OsuId { get; set; }
}
