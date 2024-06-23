using System.Diagnostics.CodeAnalysis;
using Database.Enums;

namespace API.DTOs;

/// <summary>
/// Represents a collection of statistics for a player in a ruleset
/// </summary>
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public class PlayerStatsDTO
{
    /// <summary>
    /// Player info
    /// </summary>
    public PlayerInfoDTO PlayerInfo { get; init; } = null!;

    /// <summary>
    /// Ruleset the statistics were calculated for
    /// </summary>
    public Ruleset Ruleset { get; init; }

    /// <summary>
    /// Base stats for the player
    /// </summary>
    public PlayerRatingDTO? Rating { get; init; }

    /// <summary>
    /// Match stats for the player
    /// </summary>
    public AggregatePlayerMatchStatsDTO? MatchStats { get; init; }

    /// <summary>
    /// Mod stats for the player
    /// </summary>
    public PlayerModStatsDTO? ModStats { get; init; }

    /// <summary>
    /// Tournament participation and performance stats for the player
    /// </summary>
    public PlayerTournamentStatsDTO? TournamentStats { get; init; }

    /// <summary>
    /// List of frequencies of the player's teammates
    /// </summary>
    public IEnumerable<PlayerFrequencyDTO>? FrequentTeammates { get; init; }

    /// <summary>
    /// List of frequencies of the player's opponents
    /// </summary>
    public IEnumerable<PlayerFrequencyDTO>? FrequentOpponents { get; init; }

    /// <summary>
    /// Rating chart for the player
    /// </summary>
    public PlayerRatingChartDTO? RatingChart { get; init; }
}
