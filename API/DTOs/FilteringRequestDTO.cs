using System.ComponentModel.DataAnnotations;
using API.Controllers;
using Common.Enums;

namespace API.DTOs;

/// <summary>
/// Represents a set of criteria used by the <see cref="FilteringController"/>
/// to determine player eligibility for a tournament
/// </summary>
public class FilteringRequestDTO
{
    /// <summary>
    /// The ruleset by which data will be referenced, required
    /// </summary>
    [Range(0, 5, ErrorMessage = "Invalid ruleset (0 = osu!, " +
                                "1 = osu!Taiko, 2 = osu!Catch, 3 = osu!Mania (Other), " +
                                "4 = osu!mania 4K, 5 = osu!mania 7K)")]
    public required Ruleset Ruleset { get; set; }

    /// <summary>
    /// Players with a current rating below this value will be filtered
    /// </summary>
    [Range(100, int.MaxValue, ErrorMessage = "Minimum rating value is 100")]
    public int? MinRating { get; set; }

    /// <summary>
    /// Players with a current rating above this value will be filtered
    /// </summary>
    [Range(100, int.MaxValue, ErrorMessage = "Minimum rating value is 100")]
    public int? MaxRating { get; set; }

    /// <summary>
    /// Whether to filter players that currently have a provisional rating
    /// </summary>
    public bool AllowProvisional { get; set; } = true;

    /// <summary>
    /// If set, requires players to have participated in at least
    /// this many distinct tournaments
    /// </summary>
    [Range(1, int.MaxValue, ErrorMessage = "Tournaments played must be at least 1.")]
    public int? TournamentsPlayed { get; set; }

    /// <summary>
    /// If set, requires players to have an all-time peak rating less than
    /// this value
    /// </summary>
    [Range(100, int.MaxValue, ErrorMessage = "Minimum rating value is 100")]
    public int? PeakRating { get; set; }

    /// <summary>
    /// If set, requires players to have played in at least
    /// this many matches
    /// </summary>
    [Range(1, int.MaxValue, ErrorMessage = "Matches played must be at least 1.")]
    public int? MatchesPlayed { get; set; }

    /// <summary>
    /// A list of osu! player ids that will be filtered
    /// </summary>
    public required IEnumerable<long> OsuPlayerIds { get; set; } = [];
}
