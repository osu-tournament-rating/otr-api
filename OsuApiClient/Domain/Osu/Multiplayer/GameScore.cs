using System.Diagnostics.CodeAnalysis;
using Common.Enums;

namespace OsuApiClient.Domain.Osu.Multiplayer;

/// <summary>
/// Represents a score set by a player in a <see cref="MultiplayerGame"/>
/// </summary>
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public class GameScore : IModel
{
    /// <summary>
    /// Score id
    /// </summary>
    /// <remarks>Unpopulated most of the time because sv2 scores are not submitted to Bancho</remarks>
    [SuppressMessage("ReSharper", "CommentTypo")]
    public long? Id { get; init; }

    /// <summary>
    /// Accuracy
    /// </summary>
    public double Accuracy { get; init; }

    /// <summary>
    /// Timestamp for the creation of the score
    /// </summary>
    public DateTimeOffset CreatedAt { get; init; }

    /// <summary>
    /// Max combo obtained
    /// </summary>
    public int MaxCombo { get; init; }

    /// <summary>
    /// The <see cref="Common.Enums.Ruleset"/> the score was set in
    /// </summary>
    public Ruleset Ruleset { get; init; }

    /// <summary>
    /// The enabled <see cref="Common.Enums.Mods"/>
    /// </summary>
    public Mods Mods { get; init; }

    /// <summary>
    /// Denotes if the player passed
    /// </summary>
    public bool Passed { get; init; }

    /// <summary>
    /// Denotes if the score was perfect
    /// </summary>
    public int Perfect { get; init; }

    /// <summary>
    /// The amount of pp gained by the player for the score
    /// </summary>
    /// <remarks>Unpopulated most of the time because sv2 scores are not submitted to Bancho</remarks>
    [SuppressMessage("ReSharper", "CommentTypo")]
    public double? Pp { get; init; }

    /// <summary>
    /// Letter grade
    /// </summary>
    public ScoreGrade Grade { get; init; }

    /// <summary>
    /// Denotes if the score has a replay available
    /// </summary>
    public bool Replay { get; init; }

    /// <summary>
    /// Score
    /// </summary>
    public int Score { get; init; }

    /// <summary>
    /// Statistics (accuracy values)
    /// </summary>
    public GameScoreStatistics Statistics { get; init; } = null!;

    /// <summary>
    /// No description
    /// </summary>
    /// <remarks>
    /// Seems to be an identifier internal to osu! as no descriptions could be found. Almost always "legacy_match_score"
    /// </remarks>
    public string Type { get; init; } = null!;

    /// <summary>
    /// Id of the player who set the score
    /// </summary>
    public long UserId { get; init; }

    // This field is always {"pin": null}
    // public object? CurrentUserAttributes { get; set; }

    /// <summary>
    /// Information about what lobby slot the player was in
    /// </summary>
    public GameSlotInfo SlotInfo { get; init; } = null!;
}
