// ReSharper disable CommentTypo

using System.Diagnostics.CodeAnalysis;

namespace API.DTOs;

/// <summary>
/// Represents a tournament
/// </summary>
public class TournamentDTO
{
    public int Id { get; set; }
    /// <summary>
    /// The name of the tournament
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// Acronym / shortened name of the tournament
    /// <example>For osu! World Cup 2023, this value would be "OWC23"</example>
    /// </summary>
    public string Abbreviation { get; set; } = null!;

    /// <summary>
    /// The osu! forum post advertising this tournament
    /// </summary>
    public string ForumUrl { get; set; } = null!;

    /// <summary>
    /// Lowest rank a player can be to participate in the tournament
    /// </summary>
    /// <example>For a 10,000-50,000 tournament, this value would be 10,000</example>
    public int RankRangeLowerBound { get; set; }

    /// <summary>
    /// osu! ruleset
    /// </summary>
    public int Ruleset { get; set; }

    /// <summary>
    /// Expected in-match team size
    /// </summary>
    /// <example>For a 2v2 team size 4 tournament, this value should be 2</example>
    public int TeamSize { get; set; }

    // Requested by Cytusine, normally we don't return this info.
    /// <summary>
    /// The timestamp of submission for the tournament
    /// </summary>
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public DateTime Created { get; set; }

    /// <summary>
    /// All associated match data
    /// </summary>
    /// <remarks>Will be empty for bulk requests such as List</remarks>
    public ICollection<MatchDTO> Matches { get; set; } = new List<MatchDTO>();
}
