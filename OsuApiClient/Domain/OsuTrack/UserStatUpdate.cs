using System.Diagnostics.CodeAnalysis;
using Common.Enums.Enums;

namespace OsuApiClient.Domain.OsuTrack;

/// <summary>
/// Represents a snapshot of <see cref="Domain.Osu.Users.Attributes.UserStatistics"/>
/// </summary>
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public class UserStatUpdate : IModel
{
    /// <summary>
    /// 300 count
    /// </summary>
    public int Count300 { get; init; }

    /// <summary>
    /// 100 count
    /// </summary>
    public int Count100 { get; init; }

    /// <summary>
    /// 50 count
    /// </summary>
    public int Count50 { get; init; }

    /// <summary>
    /// Play count
    /// </summary>
    public int PlayCount { get; init; }

    /// <summary>
    /// Total ranked score
    /// </summary>
    public long RankedScore { get; init; }

    /// <summary>
    /// Total score
    /// </summary>
    public long TotalScore { get; init; }

    /// <summary>
    /// Global rank
    /// </summary>
    public int Rank { get; init; }

    /// <summary>
    /// Player level
    /// </summary>
    public double Level { get; init; }

    /// <summary>
    /// Performance points
    /// </summary>
    public double Pp { get; init; }

    /// <summary>
    /// Total accuracy
    /// </summary>
    public double Accuracy { get; init; }

    /// <summary>
    /// Number of scores set with a grade of <see cref="ScoreGrade.SS"/>
    /// </summary>
    public int CountSs { get; init; }

    /// <summary>
    /// Number of scores set with a grade of <see cref="ScoreGrade.S"/>
    /// </summary>
    public int CountS { get; init; }

    /// <summary>
    /// Number of scores set with a grade of <see cref="ScoreGrade.A"/>
    /// </summary>
    public int CountA { get; init; }

    /// <summary>
    /// Timestamp for when the data was recorded
    /// </summary>
    public DateTime Timestamp { get; init; }
}
