using Database.Enums;

namespace API.DTOs;

// Can be expanded to support filters like leaderboards if needed
public class TournamentRequestQueryDTO
{
    /// <summary>
    /// The page
    /// </summary>
    public int Page { get; set; }
    /// <summary>
    /// The size of the page
    /// </summary>
    public int PageSize { get; set; } = 20;
    /// <summary>
    /// Whether the tournaments must be verified
    /// </summary>
    public bool Verified { get; set; } = true;
    /// <summary>
    /// An optional ruleset to filter by
    /// </summary>
    public Ruleset? Ruleset { get; set; }
}
