namespace Common.Enums;

/// <summary>
/// Indicates why the player failed filtering
/// </summary>
[Flags]
public enum FilteringFailReason
{
    /// <summary>
    /// No failure reason
    /// </summary>
    None = 0,

    /// <summary>
    /// The player's rating is below the minimum threshold
    /// </summary>
    MinRating = 1 << 0,

    /// <summary>
    /// The player's rating is above the maximum threshold
    /// </summary>
    MaxRating = 1 << 1,

    /// <summary>
    /// The player has not played in the minimum specified
    /// number of tournaments
    /// </summary>
    NotEnoughTournaments = 1 << 2,

    /// <summary>
    /// The player's all-time peak rating is above the maximum threshold
    /// </summary>
    PeakRatingTooHigh = 1 << 3,

    /// <summary>
    /// The player has not played in the minimum specified number of matches
    /// </summary>
    NotEnoughMatches = 1 << 4,

    /// <summary>
    /// The player has played in more than the maximum specified number of matches
    /// </summary>
    TooManyMatches = 1 << 5,

    /// <summary>
    /// The player has participated in more than the maximum specified number of tournaments
    /// </summary>
    TooManyTournaments = 1 << 6
}
