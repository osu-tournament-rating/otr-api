namespace API.DTOs;

/// <summary>
/// Represents a search result for a match
/// </summary>
public class MatchSearchResultDTO
{
    /// <summary>
    /// Id of the match
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// osu! match id of the match
    /// </summary>
    public long MatchId { get; set; }

    /// <summary>
    /// Name of the match
    /// </summary>
    public string? Name { get; set; }
}
