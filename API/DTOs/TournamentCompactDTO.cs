using System.Diagnostics.CodeAnalysis;
using Database.Enums;
using Database.Enums.Verification;

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
    /// The timestamp of submission
    /// </summary>
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public DateTime Created { get; init; }

    /// <summary>
    /// Full name
    /// </summary>
    public string Name { get; init; } = null!;

    /// <summary>
    /// Acronym / shortened name
    /// <example>
    /// For osu! World Cup 2023, this value would be "OWC2023"
    /// </example>
    /// <remarks>
    /// This value represents a short prefix which should, for the most part,
    /// be included at the start of all match titles in the tournament
    /// </summary>
    public string Abbreviation { get; init; } = null!;

    /// <summary>
    /// The osu! forum post or wiki page this tournament is featured by
    /// </summary>
    /// <remarks>
    /// If both are present, the osu! forum post should be used
    /// </remarks>
    public string ForumUrl { get; init; } = null!;

    /// <summary>
    /// Lowest rank a player can be to participate
    /// </summary>
    /// <example>For a #10,000-50,000 tournament, this value would be 10,000</example>
    public int RankRangeLowerBound { get; init; }

    /// <summary>
    /// Ruleset in which all matches are played
    /// </summary>
    public Ruleset Ruleset { get; init; }

    /// <summary>
    /// Expected in-match team size
    /// </summary>
    /// <example>For a 2v2 team size 4 tournament, this value should be 2</example>
    public int LobbySize { get; init; }

    /// <summary>
    /// The start date of the first match
    /// </summary>
    public DateTime StartTime { get; init; }

    /// <summary>
    /// The end date of the last match
    /// </summary>
    public DateTime EndTime { get; init; }

    /// <summary>
    /// The state of verification
    /// </summary>
    public VerificationStatus VerificationStatus { get; init; }

    /// <summary>
    /// The state of processing
    /// </summary>
    public TournamentProcessingStatus ProcessingStatus { get; init; }

    /// <summary>
    /// The rejection reason
    /// </summary>
    public TournamentRejectionReason RejectionReason { get; init; }

    /// <summary>
    /// The user that submitted the tournament
    /// </summary>
    public UserCompactDTO? SubmittedByUser { get; init; }

    /// <summary>
    /// The user that verified the tournament
    /// </summary>
    public UserCompactDTO? VerifiedByUser { get; init; }
}
