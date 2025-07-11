using System.Text.Json.Serialization;
using API.Utilities;
using JetBrains.Annotations;

namespace API.DTOs;

/// <summary>
/// Represents a created tournament
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.Members)]
public class TournamentCreatedResultDTO : CreatedResultBaseDTO
{
    [JsonIgnore]
    public override CreatedAtRouteValues CreatedAtRouteValues => CreatedAtRouteValuesHelper.GetTournament(Id);

    /// <summary>
    /// The name of the tournament
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Acronym / shortened name of the tournament
    /// <example>For osu! World Cup 2023, this value would be "OWC23"</example>
    /// </summary>
    public required string Abbreviation { get; set; }

    /// <summary>
    /// List of created matches
    /// </summary>
    public MatchCreatedResultDTO[] Matches { get; set; } = [];
}
