using System.Diagnostics.CodeAnalysis;
using Database.Enums;

namespace API.DTOs;

/// <summary>
/// Represents a tournament with minimal data
/// </summary>
public class TournamentCompactDTO
{
    /// <summary>
    /// Id
    /// </summary>
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public int Id { get; init; }

    /// <summary>
    /// Full name
    /// </summary>
    public string Name { get; init; } = null!;

    /// <summary>
    /// Acronym / shortened name
    /// <example>For osu! World Cup 2023, this value would be "OWC23"</example>
    /// </summary>
    public string Abbreviation { get; init; } = null!;

    /// <summary>
    /// The osu! forum post advertising this tournament
    /// </summary>
    public string ForumUrl { get; init; } = null!;

    /// <summary>
    /// Lowest rank a player can be to participate
    /// </summary>
    /// <example>For a 10,000-50,000 tournament, this value would be 10,000</example>
    public int RankRangeLowerBound { get; init; }

    /// <summary>
    /// osu! ruleset
    /// </summary>
    public Ruleset Ruleset { get; init; }

    /// <summary>
    /// Expected in-match team size
    /// </summary>
    /// <example>For a 2v2 team size 4 tournament, this value should be 2</example>
    public int LobbySize { get; init; }

    /// <summary>
    /// The user that submitted the tournament
    /// </summary>
    public UserCompactDTO? SubmittedByUser { get; init; }

    /// <summary>
    /// The user that verified the tournament
    /// </summary>
    public UserCompactDTO? VerifiedByUser { get; init; }
}
