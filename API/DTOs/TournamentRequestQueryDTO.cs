using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using API.DTOs.Interfaces;
using API.Utilities.DataAnnotations;
using Common.Enums;
using Common.Enums.Verification;
using JetBrains.Annotations;

namespace API.DTOs;

/// <summary>
/// Filtering parameters for tournaments requests
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public class TournamentRequestQueryDTO : IPaginated
{
    [Required]
    [Positive]
    [DefaultValue(1)]
    public int Page { get; init; } = 1;

    [Required]
    [Range(1, 100)]
    [DefaultValue(25)]
    public int PageSize { get; init; } = 25;

    /// <summary>
    /// Filters results for only tournaments that are verified
    /// </summary>
    [DefaultValue(true)]
    public bool Verified { get; init; } = true;

    /// <summary>
    /// Filters results for only tournaments played in a specified ruleset
    /// </summary>
    [EnumDataType(typeof(Ruleset))]
    public Ruleset? Ruleset { get; init; }

    /// <summary>
    /// Filters results for only tournaments with a partially matching name or abbreviation (case insensitive)
    /// </summary>
    public string? SearchQuery { get; init; }

    /// <summary>
    /// Filters results for only tournaments that occurred on or after a specified date
    /// </summary>
    public DateTime? DateMin { get; init; }

    /// <summary>
    /// Filters results for only tournaments that occurred on or before a specified date
    /// </summary>
    public DateTime? DateMax { get; init; }

    /// <summary>
    /// Filters results for only tournaments with a specified verification status
    /// </summary>
    [EnumDataType(typeof(VerificationStatus))]
    public VerificationStatus? VerificationStatus { get; init; }

    /// <summary>
    /// Filters results for only tournaments with a specified rejection reason
    /// </summary>
    [EnumDataType(typeof(TournamentRejectionReason))]
    public TournamentRejectionReason? RejectionReason { get; init; }

    /// <summary>
    /// Filters results for only tournaments with a specified processing status
    /// </summary>
    [EnumDataType(typeof(TournamentProcessingStatus))]
    [Obsolete("This property will be removed in a future version. Processing is now handled through event-driven message queue system.")]
    public TournamentProcessingStatus? ProcessingStatus { get; init; }

    /// <summary>
    /// Filters results for only tournaments submitted by a user with a specified id
    /// </summary>
    public int? SubmittedBy { get; init; }

    /// <summary>
    /// Filters results for only tournaments verified by a user with a specified id
    /// </summary>
    public int? VerifiedBy { get; init; }

    /// <summary>
    /// Filters results for only tournaments played with a specified lobby size
    /// </summary>
    public int? LobbySize { get; init; }

    /// <summary>
    /// The key used to sort results by
    /// </summary>
    [DefaultValue(TournamentQuerySortType.EndTime)]
    [EnumDataType(typeof(TournamentQuerySortType))]
    public TournamentQuerySortType Sort { get; init; } = TournamentQuerySortType.EndTime;

    /// <summary>
    /// Whether the results are sorted in descending order by the <see cref="Sort"/>
    /// </summary>
    [DefaultValue(true)]
    public bool Descending { get; init; } = true;
}
