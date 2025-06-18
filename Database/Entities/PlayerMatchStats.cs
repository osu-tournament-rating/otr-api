using System.Diagnostics.CodeAnalysis;

namespace Database.Entities;

/// <summary>
/// Describes the performance of a <see cref="Entities.Player"/> over all <see cref="Game"/>s in a <see cref="Entities.Match"/>
/// </summary>
[SuppressMessage("ReSharper", "EntityFramework.ModelValidation.CircularDependency")]
public class PlayerMatchStats : EntityBase
{
    /// <summary>
    /// Match cost
    /// </summary>
    public double MatchCost { get; set; }

    /// <summary>
    /// Average score
    /// </summary>
    public double AverageScore { get; set; }

    /// <summary>
    /// Average placement based on score
    /// </summary>
    public double AveragePlacement { get; set; }

    /// <summary>
    /// Average miss count
    /// </summary>
    public double AverageMisses { get; set; }

    /// <summary>
    /// Average accuracy
    /// </summary>
    public double AverageAccuracy { get; set; }

    /// <summary>
    /// Total number of games played
    /// </summary>
    public int GamesPlayed { get; set; }

    /// <summary>
    /// Total number of games won
    /// </summary>
    public int GamesWon { get; set; }

    /// <summary>
    /// Total number of games lost
    /// </summary>
    public int GamesLost { get; set; }

    /// <summary>
    /// Denotes if the <see cref="Player"/> won
    /// </summary>
    public bool Won { get; set; }

    /// <summary>
    /// List of ids of the <see cref="Player"/>'s teammates
    /// </summary>
    public int[] TeammateIds { get; set; } = [];

    /// <summary>
    /// List of ids of the <see cref="Player"/>'s opponents
    /// </summary>
    public int[] OpponentIds { get; set; } = [];

    /// <summary>
    /// Id of the <see cref="Entities.Player"/> the <see cref="PlayerMatchStats"/> was generated for
    /// </summary>
    public int PlayerId { get; init; }

    /// <summary>
    /// The <see cref="Entities.Player"/> the <see cref="PlayerMatchStats"/> was generated for
    /// </summary>
    public Player Player { get; init; } = null!;

    /// <summary>
    /// Id of the <see cref="Entities.Match"/> the <see cref="PlayerMatchStats"/> was generated for
    /// </summary>
    public int MatchId { get; init; }

    /// <summary>
    /// The <see cref="Entities.Match"/> the <see cref="PlayerMatchStats"/> was generated for
    /// </summary>
    public Match Match { get; init; } = null!;
}
