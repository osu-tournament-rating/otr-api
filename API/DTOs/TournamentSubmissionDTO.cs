using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Common.Enums;
using Common.Enums.Verification;

namespace API.DTOs;

/// <summary>
/// An incoming tournament submission
/// </summary>
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public class TournamentSubmissionDTO
{
    /// <summary>
    /// The name of the tournament
    /// </summary>
    [Required]
    public string Name { get; set; } = null!;

    /// <summary>
    /// Acronym / shortened name of the tournament
    /// </summary>
    /// <example>For osu! World Cup 2023, this value would be "OWC23"</example>
    [Required]
    public string Abbreviation { get; set; } = null!;

    /// <summary>
    /// The osu! forum post advertising this tournament
    /// </summary>
    [Required]
    public string ForumUrl { get; set; } = null!;

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
    public int TeamLobbySize { get; set; }

    /// <summary>
    /// osu! ruleset
    /// </summary>
    [EnumDataType(typeof(Ruleset))]
    public Ruleset Ruleset { get; set; }

    /// <summary>
    /// Optional rejection reason. If set, the created tournament and all matches will be rejected
    /// for this reason and go through no additional processing
    /// </summary>
    /// <remarks>Submissions with a rejection reason will only be accepted from admin users</remarks>
    [EnumDataType(typeof(TournamentRejectionReason))]
    public TournamentRejectionReason? RejectionReason { get; set; }

    /// <summary>
    /// List of osu! match ids
    /// </summary>
    /// <example>For a match link https://osu.ppy.sh/mp/98119977, add 98119977 to this list</example>
    public IEnumerable<long> Ids { get; set; } = [];

    /// <summary>
    /// A collection of pooled osu! beatmap ids
    /// </summary>
    public IEnumerable<long> BeatmapIds { get; set; } = [];
}
