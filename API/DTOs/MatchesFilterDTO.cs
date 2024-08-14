using Database.Enums;
using Database.Enums.Queries;
using Database.Enums.Verification;

namespace API.DTOs;

public class MatchesFilterDTO
{
    public Ruleset? Ruleset { get; set; }

    public string? Name { get; set; }

    public DateTime? DateMin { get; set; }

    public DateTime? DateMax { get; set; }

    public VerificationStatus? VerificationStatus { get; set; }

    public MatchRejectionReason? RejectionReason { get; set; }

    public MatchProcessingStatus? ProcessingStatus { get; set; }

    public int? SubmittedBy { get; set; }

    public int? VerifiedBy { get; set; }

    public MatchesQuerySortType? Sort { get; set; } = MatchesQuerySortType.Id;

    public bool? SortDescending { get; set; }
}
