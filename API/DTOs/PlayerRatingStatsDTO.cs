using API.Utilities;
using JetBrains.Annotations;

namespace API.DTOs;

/// <summary>
/// Describes tournament rating based information for a player in a ruleset with additional statistics
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public class PlayerRatingStatsDTO : PlayerRatingDTO
{
    /// <summary>
    /// Total number of tournaments played
    /// </summary>
    public int TournamentsPlayed { get; set; }

    /// <summary>
    /// Total number of matches played
    /// </summary>
    public int MatchesPlayed { get; set; }

    /// <summary>
    /// Match win rate
    /// </summary>
    public double WinRate { get; set; }

    /// <summary>
    /// Rating tier progress information
    /// </summary>
    public required RankProgressDTO RankProgress { get; set; }

    /// <summary>
    /// Denotes the current rating as being provisional
    /// </summary>
    public bool IsProvisional => RatingUtils.IsProvisional(Volatility, MatchesPlayed, TournamentsPlayed);
}
