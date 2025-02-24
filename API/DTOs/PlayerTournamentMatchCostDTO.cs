using System.Diagnostics.CodeAnalysis;
using Database.Enums;

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
    /// The associated tournament
    /// </summary>
    public TournamentCompactDTO Tournament { get; set; } = null!;

    /// <summary>
    /// Average match cost across the tournament for the player
    /// </summary>
    public double MatchCost { get; set; }
}
