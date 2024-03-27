namespace API.DTOs;

/// <summary>
/// Represents a created match
/// </summary>
public class MatchCreatedResultDTO : CreatedResultDTO
{
    /// <summary>
    /// osu! match id
    /// </summary>
    public long MatchId { get; set; }
}
