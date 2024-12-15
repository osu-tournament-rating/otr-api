using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using API.DTOs.Interfaces;
using Database.Enums;
using Database.Enums.Verification;

namespace API.DTOs;

/// <summary>
/// Filtering parameters for tournaments requests
/// </summary>
public class TournamentRequestQueryDTO : IPaginated
{
    [Required]
    [Range(1, int.MaxValue)]
    public int Page { get; init; }

    [Required]
    [Range(1, 100)]
    public int PageSize { get; init; }

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
    public string? Name { get; init; }

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
