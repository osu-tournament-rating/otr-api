using API.Controllers;

namespace API.DTOs;

/// <summary>
/// Used by the <see cref="ScreeningController"/> to
/// determine what types of players are eligible/ineligible
/// to play in a tournament that screens by criteria here.
/// </summary>
public class ScreeningDTO
{
    /// <summary>
    /// The ruleset by which data will be referenced, required
    /// </summary>
    public int Ruleset { get; set; }
    /// <summary>
    /// Players with a current rating below this value will be screened
    /// </summary>
    public int? MinRating { get; set; }
    /// <summary>
    /// Players with a current rating above this value will be screened
    /// </summary>
    public int? MaxRating { get; set; }
    /// <summary>
    /// Whether to consider players that currently have a provisional rating
    /// </summary>
    public bool AllowProvisional { get; set; } = true;
    /// <summary>
    /// If set, requires players to have participated in at least
    /// this many distinct tournaments
    /// </summary>
    public int? TournamentsPlayed { get; set; }
    /// <summary>
    /// If set, requires players to have an all-time peak rating less than
    /// this value
    /// </summary>
    public int? PeakRating { get; set; }
    /// <summary>
    /// If set, requires players to have played in at least
    /// this many matches
    /// </summary>
    public int? MatchesPlayed { get; set; }
    /// <summary>
    /// A list of osu! player ids that will be screened
    /// </summary>
    public IEnumerable<long> OsuPlayerIds { get; set; } = new List<long>();
}
