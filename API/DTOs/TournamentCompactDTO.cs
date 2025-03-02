using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using API.Utilities.DataAnnotations;
using Common.Enums.Enums;
using Common.Enums.Enums.Verification;

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
    [Required]
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
    [Required]
    public string Abbreviation { get; init; } = null!;

    /// <summary>
    /// The osu! forum post or wiki page this tournament is featured by
    /// </summary>
    /// <remarks>
    /// If both are present, the osu! forum post should be used
    /// </remarks>
    [Required]
    public string ForumUrl { get; init; } = null!;

    /// <summary>
    /// Lowest rank a player can be to participate
    /// </summary>
    /// <example>For a #10,000-50,000 tournament, this value would be 10,000</example>
    [Positive]
    public int RankRangeLowerBound { get; init; }

    /// <summary>
    /// Ruleset in which all matches are played
    /// </summary>
    [EnumDataType(typeof(Ruleset))]
    public Ruleset Ruleset { get; init; }

    /// <summary>
    /// Expected in-match team size
    /// </summary>
    /// <example>For a 2v2 team size 4 tournament, this value should be 2</example>
    [Range(1, 8)]
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
    [EnumDataType(typeof(VerificationStatus))]
    public VerificationStatus VerificationStatus { get; init; }

    /// <summary>
    /// The state of processing
    /// </summary>
    [EnumDataType(typeof(TournamentProcessingStatus))]
    public TournamentProcessingStatus ProcessingStatus { get; init; }

    /// <summary>
    /// The rejection reason
    /// </summary>
    [EnumDataType(typeof(TournamentRejectionReason))]
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
