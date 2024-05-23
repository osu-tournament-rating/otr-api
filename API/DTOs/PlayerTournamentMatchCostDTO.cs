using System.Diagnostics.CodeAnalysis;
using API.Osu.Enums;

namespace API.DTOs;

/// <summary>
/// Represents match cost data across an entire tournament for a player
/// </summary>
[SuppressMessage("ReSharper", "PropertyCanBeMadeInitOnly.Global")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public class PlayerTournamentMatchCostDTO
{
    /// <summary>
    /// Id of the player
    /// </summary>
    public int PlayerId { get; set; }

    /// <summary>
    /// Id of the tournament
    /// </summary>
    public int TournamentId { get; set; }

    /// <summary>
    /// Name of the tournament
    /// </summary>
    public string TournamentName { get; set; } = null!;

    /// <summary>
    /// Abbreviated name of the tournament
    /// </summary>
    public string TournamentAcronym { get; set; } = null!;

    /// <summary>
    /// Ruleset of the tournament
    /// </summary>
    public Ruleset Ruleset { get; set; }

    /// <summary>
    /// Average match cost across the tournament for the player
    /// </summary>
    public double MatchCost { get; set; }
}
