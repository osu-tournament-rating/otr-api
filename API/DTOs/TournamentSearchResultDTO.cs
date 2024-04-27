using API.Osu.Enums;

namespace API.DTOs;

/// <summary>
/// Represents a search result for a tournament
/// </summary>
public class TournamentSearchResultDTO
{
    /// <summary>
    /// Id of the tournament
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Ruleset of the tournament
    /// </summary>
    public Ruleset Ruleset { get; set; }

    /// <summary>
    /// Expected team size of the tournament
    /// </summary>
    public int TeamSize { get; set; }

    /// <summary>
    /// Name of the tournament
    /// </summary>
    public required string Name { get; set; }
}
