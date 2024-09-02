using Database.Enums;
using Database.Enums.Verification;
using Database.Queries.Enums;

namespace Database.Queries.Filters;

/// <summary>
/// Filtering options for querying a page of <see cref="Entities.Match"/>es
/// </summary>
public class MatchesQueryFilter : PagedFilterBase
{
    /// <summary>
    /// Filters results for <see cref="Entities.Match"/>es with a
    /// matching <see cref="Enums.Ruleset"/>
    /// </summary>
    public Ruleset? Ruleset { get; set; }

    /// <summary>
    /// Filters results for <see cref="Entities.Match"/>es with a partially
    /// matching <see cref="Entities.Match.Name"/>
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Filters results for <see cref="Entities.Match"/>es with a
    /// <see cref="Entities.Match.StartTime"/> greater than this value
    /// </summary>
    public DateTime? DateMin { get; set; }

    /// <summary>
    /// Filters results for <see cref="Entities.Match"/>es with an
    /// <see cref="Entities.Match.EndTime"/> less than this value
    /// </summary>
    public DateTime? DateMax { get; set; }

    /// <summary>
    /// Filters results for <see cref="Entities.Match"/>es with a
    /// matching <see cref="Enums.Verification.VerificationStatus"/>
    /// </summary>
    public VerificationStatus? VerificationStatus { get; set; }

    /// <summary>
    /// Filters results for <see cref="Entities.Match"/>es with a matching <see cref="MatchRejectionReason"/>
    /// </summary>
    public MatchRejectionReason? RejectionReason { get; set; }

    /// <summary>
    /// Filters results for <see cref="Entities.Match"/>es with a matching <see cref="MatchProcessingStatus"/>
    /// </summary>
    public MatchProcessingStatus? ProcessingStatus { get; set; }

    /// <summary>
    /// Filters results for <see cref="Entities.Match"/>es where the id of the
    /// <see cref="Entities.User"/> that submitted it matches this value
    /// </summary>
    public int? SubmittedBy { get; set; }

    /// <summary>
    /// Filters results for <see cref="Entities.Match"/>es where the id of the
    /// <see cref="Entities.User"/> that verified it matches this value
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
