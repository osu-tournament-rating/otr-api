namespace API.DTOs;

/// <summary>
/// Represents a created tournament
/// </summary>
public class TournamentCreatedResultDTO : CreatedResultDTO
{
    /// <inheritdoc cref="TournamentDTO.Name"/>
    public string Name { get; set; } = null!;

    /// <inheritdoc cref="TournamentDTO.Abbreviation"/>
    public string Abbreviation { get; set; } = null!;

    /// <summary>
    /// List of created matches
    /// </summary>
    public MatchCreatedResultDTO[] Matches { get; set; } = [];
}
