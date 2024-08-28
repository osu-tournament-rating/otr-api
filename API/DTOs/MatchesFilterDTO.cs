using Database.Enums;
using Database.Enums.Queries;
using Database.Enums.Verification;

namespace API.DTOs;

/// <summary>
/// Filtering options for querying <see cref="Database.Entities.Match"/>es
/// used by <see cref="Controllers.MatchesController.ListAsync"/>
/// </summary>
public class MatchesFilterDTO
{
    /// <summary>
    /// Filters results for <see cref="Database.Entities.Match"/>es with a
    /// matching <see cref="Database.Enums.Ruleset"/>
    /// </summary>
    public Ruleset? Ruleset { get; set; }

    /// <summary>
    /// Filters results for <see cref="Database.Entities.Match"/>es with a partially
    /// matching <see cref="Database.Entities.Match.Name"/>
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Filters results for <see cref="Database.Entities.Match"/>es with a
    /// <see cref="Database.Entities.Match.StartTime"/> greater than this value
    /// </summary>
    public DateTime? DateMin { get; set; }

    /// <summary>
    /// Filters results for <see cref="Database.Entities.Match"/>es with an
    /// <see cref="Database.Entities.Match.EndTime"/> less than this value
    /// </summary>
    public DateTime? DateMax { get; set; }

    /// <summary>
    /// Filters results for <see cref="Database.Entities.Match"/>es with a
    /// matching <see cref="Database.Enums.Verification.VerificationStatus"/>
    /// </summary>
    public VerificationStatus? VerificationStatus { get; set; }

    /// <summary>
    /// Filters results for <see cref="Database.Entities.Match"/>es with a matching <see cref="MatchRejectionReason"/>
    /// </summary>
    public MatchRejectionReason? RejectionReason { get; set; }

    /// <summary>
    /// Filters results for <see cref="Database.Entities.Match"/>es with a matching <see cref="MatchProcessingStatus"/>
    /// </summary>
    public MatchProcessingStatus? ProcessingStatus { get; set; }

    /// <summary>
    /// Filters results for <see cref="Database.Entities.Match"/>es where the id of the
    /// <see cref="Database.Entities.User"/> that submitted it matches this value
    /// </summary>
    public int? SubmittedBy { get; set; }

    /// <summary>
    /// Filters results for <see cref="Database.Entities.Match"/>es where the id of the
    /// <see cref="Database.Entities.User"/> that verified it matches this value
    /// </summary>
    public int? VerifiedBy { get; set; }

    /// <summary>
    /// Controls the manner in which results are sorted
    /// </summary>
    public MatchesQuerySortType? Sort { get; set; } = MatchesQuerySortType.Id;

    /// <summary>
    /// Denotes whether to sort results in ascending or descending order
    /// </summary>
    public bool? SortDescending { get; set; }
}
