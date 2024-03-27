namespace API.DTOs;

/// <summary>
/// Represents a created tournament
/// </summary>
public class TournamentCreatedResultDTO : CreatedResultDTO
{
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
