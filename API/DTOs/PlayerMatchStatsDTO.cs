namespace API.DTOs;

/// <summary>
/// Used for POSTing match statistics to the API
/// </summary>
public class PlayerMatchStatsDTO
{
    public int Id { get; set; }
    public int MatchId { get; set; }
    public bool Won { get; set; }
    public int AverageScore { get; set; }
    public double AverageMisses { get; set; }
    public double AverageAccuracy { get; set; }
    public double AveragePlacement { get; set; }
    public int GamesWon { get; set; }
    public int GamesLost { get; set; }
    public int GamesPlayed { get; set; }
    public int[] TeammateIds { get; set; } = [];
    public int[] OpponentIds { get; set; } = [];
}
