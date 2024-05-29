using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace API.DTOs;

/// <summary>
/// Represents data used to create a tournament
/// </summary>
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public class TournamentWebSubmissionDTO : MatchesWebSubmissionDTO
{
    /// <summary>
    /// The name of the tournament
    /// </summary>
    public required string TournamentName { get; set; }

    /// <summary>
    /// Acronym / shortened name of the tournament
    /// <example>For osu! World Cup 2023, this value would be "OWC23"</example>
    /// </summary>
    public required string Abbreviation { get; set; }

    /// <summary>
    /// The osu! forum post advertising this tournament
    /// </summary>
    public required string ForumPost { get; set; }

    /// <summary>
    /// Lowest rank a player can be to participate in the tournament
    /// </summary>
    /// <example>For a 10,000-50,000 tournament, this value would be 10,000</example>
    [Range(1, int.MaxValue)]
    public int RankRangeLowerBound { get; set; }

    /// <summary>
    /// Expected in-match team size
    /// </summary>
    /// <example>For a 2v2 team size 4 tournament, this value should be 2</example>
    [Range(1, 8)]
    public int TeamSize { get; set; }

    /// <summary>
    /// osu! ruleset
    /// </summary>
    [Range(0, 3)]
    public int Ruleset { get; set; }
}
