namespace API.DTOs;

public class PlayerTournamentStatsDTO
{
    /// <summary>
    /// Average change in rating
    /// </summary>
    public double AverageRatingDelta { get; init; }

    /// <summary>
    /// Average match cost
    /// </summary>
    public double AverageMatchCost { get; init; }

    /// <summary>
    /// Average score
    /// </summary>
    public int AverageScore { get; init; }

    /// <summary>
    /// Average placement
    /// </summary>
    public double AveragePlacement { get; init; }

    /// <summary>
    /// Average accuracy
    /// </summary>
    public double AverageAccuracy { get; init; }

    /// <summary>
    /// Total number of <see cref="Match"/>es played
    /// </summary>
    public int MatchesPlayed { get; init; }

    /// <summary>
    /// Total number of <see cref="Match"/>es won
    /// </summary>
    public int MatchesWon { get; init; }

    /// <summary>
    /// Total number of <see cref="Match"/>es lost
    /// </summary>
    public int MatchesLost { get; init; }

    /// <summary>
    /// Total number of <see cref="Game"/>s played
    /// </summary>
    public int GamesPlayed { get; init; }

    /// <summary>
    /// Total number of <see cref="Game"/>s won
    /// </summary>
    public int GamesWon { get; init; }

    /// <summary>
    /// Total number of <see cref="Game"/>s lost
    /// </summary>
    public int GamesLost { get; init; }

    /// <summary>
    /// The player who owns these stats
    /// </summary>
    public PlayerCompactDTO Player { get; init; } = null!;

    /// <summary>
    /// Tournament
    /// </summary>
    public TournamentCompactDTO Tournament { get; init; } = null!;
}
