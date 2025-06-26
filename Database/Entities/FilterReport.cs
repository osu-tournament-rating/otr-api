using System.ComponentModel.DataAnnotations;
using Common.Enums;
using JetBrains.Annotations;

namespace Database.Entities;

/// <summary>
/// Stores a filtering request and its result for transparency
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.Members)]
public class FilterReport : EntityBase
{
    /// <summary>
    /// The ID of the user who made the filtering request
    /// </summary>
    [Required]
    public int UserId { get; set; }

    /// <summary>
    /// The ruleset for which the filtering was performed
    /// </summary>
    [Required]
    public Ruleset Ruleset { get; set; }

    /// <summary>
    /// Players with a current rating below this value will be filtered
    /// </summary>
    public int? MinRating { get; set; }

    /// <summary>
    /// Players with a current rating above this value will be filtered
    /// </summary>
    public int? MaxRating { get; set; }

    /// <summary>
    /// If set, requires players to have participated in at least this many distinct tournaments
    /// </summary>
    public int? TournamentsPlayed { get; set; }

    /// <summary>
    /// If set, requires players to have an all-time peak rating less than this value
    /// </summary>
    public int? PeakRating { get; set; }

    /// <summary>
    /// If set, requires players to have played in at least this many matches
    /// </summary>
    public int? MatchesPlayed { get; set; }

    /// <summary>
    /// The number of players who passed filtering
    /// </summary>
    [Required]
    public int PlayersPassed { get; set; }

    /// <summary>
    /// The number of players who failed filtering
    /// </summary>
    [Required]
    public int PlayersFailed { get; set; }


    /// <summary>
    /// Navigation property to the user who made the request
    /// </summary>
    public User User { get; set; } = null!;

    /// <summary>
    /// Navigation property to the filtering results for each player
    /// </summary>
    public ICollection<FilterReportPlayer> FilterReportPlayers { get; set; } = new List<FilterReportPlayer>();
}
