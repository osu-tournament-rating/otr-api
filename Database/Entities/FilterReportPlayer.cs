using System.ComponentModel.DataAnnotations;
using Common.Enums;
using JetBrains.Annotations;

namespace Database.Entities;

/// <summary>
/// Represents a player's result in a filter report
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.Members)]
public class FilterReportPlayer : EntityBase
{
    /// <summary>
    /// The ID of the filter report this result belongs to
    /// </summary>
    [Required]
    public int FilterReportId { get; set; }

    /// <summary>
    /// The ID of the player
    /// </summary>
    [Required]
    public int PlayerId { get; set; }

    /// <summary>
    /// Whether the player successfully passes all conditions of the filter
    /// </summary>
    [Required]
    public bool IsSuccess { get; set; }

    /// <summary>
    /// If the player failed filtering, the fail reason
    /// </summary>
    public FilteringFailReason? FailureReason { get; set; }

    /// <summary>
    /// The player's current rating for the requested ruleset at the time of filtering
    /// </summary>
    public double? CurrentRating { get; set; }

    /// <summary>
    /// The number of tournaments the player has participated in at the time of filtering
    /// </summary>
    public int? TournamentsPlayed { get; set; }

    /// <summary>
    /// The number of matches the player has played at the time of filtering
    /// </summary>
    public int? MatchesPlayed { get; set; }

    /// <summary>
    /// The player's all-time peak rating for the requested ruleset at the time of filtering
    /// </summary>
    public double? PeakRating { get; set; }

    /// <summary>
    /// Navigation property to the filter report
    /// </summary>
    public FilterReport FilterReport { get; set; } = null!;

    /// <summary>
    /// Navigation property to the player
    /// </summary>
    public Player Player { get; set; } = null!;
}
