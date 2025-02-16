using System.ComponentModel.DataAnnotations.Schema;
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
    public double MatchCost { get; init; }

    /// <summary>
    /// Average score
    /// </summary>
    public double AverageScore { get; init; }

    /// <summary>
    /// Average placement based on score
    /// </summary>
    public double AveragePlacement { get; init; }

    /// <summary>
    /// Average miss count
    /// </summary>
    public double AverageMisses { get; init; }

    /// <summary>
    /// Average accuracy
    /// </summary>
    public double AverageAccuracy { get; init; }

    /// <summary>
    /// Total number of games played
    /// </summary>
    public int GamesPlayed { get; init; }

    /// <summary>
    /// Total number of games won
    /// </summary>
    public int GamesWon { get; init; }

    /// <summary>
    /// Total number of games lost
    /// </summary>
    public int GamesLost { get; init; }

    /// <summary>
    /// Denotes if the <see cref="Player"/> won
    /// </summary>
    public bool Won { get; init; }

    /// <summary>
    /// List of ids of the <see cref="Player"/>'s teammates
    /// </summary>
    public int[] TeammateIds { get; init; } = [];

    /// <summary>
    /// List of ids of the <see cref="Player"/>'s opponents
    /// </summary>
    public int[] OpponentIds { get; init; } = [];

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
