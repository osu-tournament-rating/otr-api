using System.ComponentModel.DataAnnotations;
using Database.Enums;

namespace API.DTOs;

/// <summary>
/// Filtering parameters for tournaments requests
/// </summary>
public class TournamentRequestQueryDTO : PaginatedRequestQueryDTO
{
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
    public override int Page { get; init; }

    [Required]
    [Range(1, 100, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
    public override int PageSize { get; init; }

    /// <summary>
    /// Filters results for only tournaments that are verified
    /// </summary>
    public bool Verified { get; init; } = true;

    /// <summary>
    /// Filters results for only tournaments played in a specified ruleset
    /// </summary>
    public Ruleset? Ruleset { get; init; }

    /// <summary>
    /// The key used to sort results by
    /// </summary>
    public TournamentQuerySortType Sort { get; init; } = TournamentQuerySortType.StartTime;

    /// <summary>
    /// Whether the results are sorted in descending order by the <see cref="Sort"/>
    /// </summary>
    public bool Descending { get; init; } = false;
}
