using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using API.DTOs.Interfaces;
using Database.Enums;
using Database.Enums.Queries;
using Database.Enums.Verification;

namespace API.DTOs;

/// <summary>
/// Filtering parameters for matches requests
/// </summary>
public class MatchRequestQueryDTO : IPaginated
{
    [Required]
    [Range(1, int.MaxValue)]
    public int Page { get; init; }

    [Required]
    [Range(1, 100)]
    public int PageSize { get; init; }

    /// <summary>
    /// Filters results for only matches played in a specified ruleset
    /// </summary>
    [EnumDataType(typeof(Ruleset))]
    public Ruleset? Ruleset { get; init; }

    /// <summary>
    /// Filters results for only matches with a partially matching name (case insensitive)
    /// </summary>
    public string? Name { get; init; }

    /// <summary>
    /// Filters results for only matches that occurred on or after a specified date
    /// </summary>
    public DateTime? DateMin { get; init; }

    /// <summary>
    /// Filters results for only matches that occurred on or before a specified date
    /// </summary>
    public DateTime? DateMax { get; init; }

    /// <summary>
    /// Filters results for only matches with a specified verification status
    /// </summary>
    [EnumDataType(typeof(VerificationStatus))]
    public VerificationStatus? VerificationStatus { get; init; }

    /// <summary>
    /// Filters results for only matches with a specified rejection reason
    /// </summary>
    [EnumDataType(typeof(MatchRejectionReason))]
    public MatchRejectionReason? RejectionReason { get; init; }

    /// <summary>
    /// Filters results for only matches with a specified processing status
    /// </summary>
    [EnumDataType(typeof(MatchProcessingStatus))]
    public MatchProcessingStatus? ProcessingStatus { get; init; }

    /// <summary>
    /// Filters results for only matches submitted by a user with a specified id
    /// </summary>
    public int? SubmittedBy { get; init; }

    /// <summary>
    /// Filters results for only matches verified by a user with a specified id
    /// </summary>
    public int? VerifiedBy { get; init; }

    /// <summary>
    /// The key used to sort results by
    /// </summary>
    [DefaultValue(MatchQuerySortType.StartTime)]
    [EnumDataType(typeof(MatchQuerySortType))]
    public MatchQuerySortType? Sort { get; init; } = MatchQuerySortType.StartTime;

    /// <summary>
    /// Whether the results are sorted in descending order by the <see cref="Sort"/>
    /// </summary>
    public bool? Descending { get; init; } = true;
}
