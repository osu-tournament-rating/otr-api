// ReSharper disable CommentTypo

using System.Diagnostics.CodeAnalysis;

namespace API.DTOs;

public class TournamentDTO
{
    /// <summary>
    /// The name of the tournament
    /// </summary>
    public string Name { get; set; } = null!;
    /// <summary>
    /// The tournament abbreviation (e.g. OWC2023)
    /// </summary>
    public string Abbreviation { get; set; } = null!;
    /// <summary>
    /// The osu! forum post advertising this tournament
    /// </summary>
    public string ForumUrl { get; set; } = null!;
    /// <summary>
    /// The 'higher skill' portion of the rank range. 1 for open rank.
    /// e.g. a 10,000-50,000 tournament would have 10,000 for this value.
    /// </summary>
    public int RankRangeLowerBound { get; set; }
    /// <summary>
    /// The tournament ruleset
    /// </summary>
    public int Mode { get; set; }
    /// <summary>
    /// The expected in-match team size.
    /// e.g. a 2v2 team size 4 tournament should have '2'
    /// for this value.
    /// </summary>
    public int TeamSize { get; set; }
    // Requested by Cytusine, normally we don't return this info.
    /// <summary>
    /// The time this tournament was submitted
    /// </summary>
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public DateTime Created { get; set; }
    /// <summary>
    /// All associated match data. Null in bulk GET requests
    /// (such as get all)
    /// </summary>
    public IEnumerable<MatchDTO>? Matches { get; set; }
}
