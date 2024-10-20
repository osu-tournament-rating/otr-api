using System.ComponentModel.DataAnnotations;
using API.Enums;
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
    [Range(1, int.MaxValue, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
    public int Page { get; set; } = 1;

    /// <summary>
    /// The size of the page
    /// </summary>
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
    public int PageSize { get; set; } = 20;

    /// <summary>
    /// Whether the tournaments must be verified
    /// </summary>
    public bool Verified { get; set; } = true;

    /// <summary>
    /// An optional ruleset to filter by
    /// </summary>
    public Ruleset? Ruleset { get; set; } = Osu;

    /// <summary>
    /// The key used to sort results by
    /// </summary>
    public TournamentSortKey SortKey { get; init; } = TournamentSortKey.None;
}
