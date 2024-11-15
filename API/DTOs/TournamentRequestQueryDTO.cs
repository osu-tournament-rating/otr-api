using System.ComponentModel.DataAnnotations;
using Database.Enums;
using static Database.Enums.Ruleset;

namespace API.DTOs;

/// <summary>
/// Enables pagination and filtering of tournament requests
/// </summary>
public class TournamentRequestQueryDTO
{
    /// <summary>
    /// The page number
    /// </summary>
    [Required]
    [Range(0, int.MaxValue, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
    public int Page { get; init; } = 1;

    /// <summary>
    /// The size of the page
    /// </summary>
    [Required]
    [Range(5, 100, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
    public int PageSize { get; init; } = 20;

    /// <summary>
    /// Whether the tournaments must be verified
    /// </summary>
    public bool Verified { get; init; } = true;

    /// <summary>
    /// An optional ruleset to filter by
    /// </summary>
    public Ruleset? Ruleset { get; init; }

    /// <summary>
    /// The key used to sort results by
    /// </summary>
    public TournamentQuerySortType QuerySortType { get; init; } = TournamentQuerySortType.StartTime;

    /// <summary>
    /// Whether the tournaments are sorted in descending order by the <see cref="QuerySortType"/>
    /// </summary>
    public bool Descending { get; init; } = false;
}
