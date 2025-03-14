namespace API.DTOs;

/// <summary>
/// Represents counts of participation in tournaments of differing formats
/// </summary>
public class PlayerTournamentFormatCountDTO
{
    /// <summary>
    /// Number of 1v1 tournaments played
    /// </summary>
    public int? Count1v1 { get; set; }

    /// <summary>
    /// Number of 2v2 tournaments played
    /// </summary>
    public int? Count2v2 { get; set; }

    /// <summary>
    /// Number of 3v3 tournaments played
    /// </summary>
    public int? Count3v3 { get; set; }

    /// <summary>
    /// Number of 4v4 tournaments played
    /// </summary>
    public int? Count4v4 { get; set; }

    /// <summary>
    /// Number of tournaments played outside of standard team sizes
    /// </summary>
    public int? CountOther { get; set; }
}
