using System.Diagnostics.CodeAnalysis;
using AutoMapper;
using Common.Enums.Enums;
using OsuApiClient.Net.JsonModels.Osu.Users.Attributes;

namespace OsuApiClient.Domain.Osu.Users.Attributes;

/// <summary>
/// Represents an aggregate of statistics for a <see cref="User"/> in a <see cref="Ruleset"/>
/// </summary>
[AutoMap(typeof(UserStatisticsJsonModel))]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public class UserStatistics : IModel
{
    /// <summary>
    /// Total 50 count
    /// </summary>
    public int Count50 { get; init; }

    /// <summary>
    /// Total 100 count
    /// </summary>
    public int Count100 { get; init; }

    /// <summary>
    /// Total 300 count
    /// </summary>
    public int Count300 { get; init; }

    /// <summary>
    /// Total miss count
    /// </summary>
    public int CountMiss { get; init; }

    /// <summary>
    /// Hit accuracy percentage
    /// </summary>
    public double HitAccuracy { get; init; }

    /// <summary>
    /// Total number of hits
    /// </summary>
    public int TotalHits { get; init; }

    /// <summary>
    ///	Highest maximum combo
    /// </summary>
    public int MaxCombo { get; init; }

    /// <summary>
    /// Current ranked score
    /// </summary>
    public long RankedScore { get; init; }

    /// <summary>
    /// Total score
    /// </summary>
    public long TotalScore { get; init; }

    /// <summary>
    /// Number of maps played
    /// </summary>
    public int PlayCount { get; init; }

    /// <summary>
    /// Cumulative time played
    /// </summary>
    public int PlayTime { get; init; }

    /// <summary>
    /// Performance points
    /// </summary>
    public double Pp { get; init; }

    /// <summary>
    /// Experimental (lazer) performance points
    /// </summary>
    public double? PpExp { get; init; }

    /// <summary>
    /// Current rank according to <see cref="Pp"/>
    /// </summary>
    public int? GlobalRank { get; init; }

    /// <summary>
    /// Current country rank according to <see cref="Pp"/>
    /// </summary>
    public int? CountryRank { get; init; }

    /// <summary>
    /// Current rank according to <see cref="PpExp"/>
    /// </summary>
    public int? GlobalRankExp { get; init; }

    /// <summary>
    /// Number of replays watched by other users
    /// </summary>
    public int ReplaysWatched { get; init; }

    /// <summary>
    /// Is actively ranked
    /// </summary>
    public bool IsRanked { get; init; }

    /// <summary>
    /// Number of ranked scores set with a <see cref="ScoreGrade"/> of <see cref="ScoreGrade.A"/> or above
    /// </summary>
    public IDictionary<ScoreGrade, int> GradeCounts { get; init; } = new Dictionary<ScoreGrade, int>();

    /// <summary>
    /// All available variants of the <see cref="UserStatistics"/>
    /// </summary>
    public UserStatisticsVariant[] Variants { get; init; } = [];
}
