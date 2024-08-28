using Database.Enums;
using Database.Enums.Queries;
using Database.Enums.Verification;

namespace Database.Queries.Filters;

/// <summary>
/// Filtering options for querying a page of <see cref="Database.Entities.Tournament"/>s
/// </summary>
public class TournamentsQueryFilter
{
    /// <summary>
    /// Filters results for <see cref="Database.Entities.Tournament"/>s with a
    /// matching <see cref="Database.Enums.Ruleset"/>
    /// </summary>
    public Ruleset? Ruleset { get; set; }

    /// <summary>
    /// Filters results for <see cref="Entities.Tournament"/>s with a partially
    /// matching <see cref="Entities.Tournament.Name"/>
    /// or <see cref="Entities.Tournament.Abbreviation"/>
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Filters results for <see cref="Entities.Tournament"/>s with a
    /// matching <see cref="Entities.Tournament.LobbySize"/>
    /// </summary>
    public int? LobbySize { get; set; }

    /// <summary>
    /// Filters results for <see cref="Entities.Tournament"/>s with a
    /// matching <see cref="Entities.Tournament.RankRangeLowerBound"/>
    /// </summary>
    public int? RankRangeLowerBound { get; set; }

    /// <summary>
    /// Filters results for <see cref="Entities.Tournament"/>s with a
    /// matching <see cref="Enums.Verification.VerificationStatus"/>
    /// </summary>
    public VerificationStatus? VerificationStatus { get; set; }

    /// <summary>
    /// Filters results for <see cref="Entities.Tournament"/>s with a
    /// matching <see cref="TournamentProcessingStatus"/>
    /// </summary>
    public TournamentRejectionReason? RejectionReason { get; set; }

    /// <summary>
    /// Filters results for <see cref="Entities.Tournament"/>s with a
    /// matching <see cref="TournamentProcessingStatus"/>
    /// </summary>
    public TournamentProcessingStatus? ProcessingStatus { get; set; }

    /// <summary>
    /// Filters results for <see cref="Entities.Tournament"/>s where the id of the
    /// <see cref="Entities.User"/> that submitted it matches this value
    /// </summary>
    public int? SubmittedBy { get; set; }

    /// <summary>
    /// Filters results for <see cref="Entities.Tournament"/>s where the id of the
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

    /// <summary>
    /// Controls the filtering options for the included <see cref="Entities.Match"/>es
    /// </summary>
    public MatchesQueryFilter? Matches { get; set; }
}
