using System.Text.Json.Serialization;
using API.Utilities;
using JetBrains.Annotations;

namespace API.DTOs;

/// <summary>
/// Represents a created match
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.Members)]
public class MatchCreatedResultDTO : CreatedResultBaseDTO
{
    [JsonIgnore]
    public override CreatedAtRouteValues CreatedAtRouteValues => CreatedAtRouteValuesHelper.GetMatch(Id);

    /// <summary>
    /// osu! match id
    /// </summary>
    public long OsuId { get; set; }
}
