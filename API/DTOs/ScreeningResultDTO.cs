namespace API.DTOs;

/// <summary>
/// Represents a screenings result for a collection of players
/// </summary>
public class ScreeningResultDTO
{
    /// <summary>
    /// The number of players who passed screening
    /// </summary>
    public int PlayersPassed { get; set; }
    /// <summary>
    /// The number of players who failed screening
    /// </summary>
    public int PlayersFailed { get; set; }
    /// <summary>
    /// A collection of screening results, one per submitted player,
    /// in the same order as submitted in the <see cref="ScreeningRequestDTO"/>
    /// </summary>
    public IList<PlayerScreeningResultDTO> ScreeningResults { get; set; } = new List<PlayerScreeningResultDTO>();
}
