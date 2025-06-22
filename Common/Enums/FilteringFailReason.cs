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
    /// The player does not have a rating for the ruleset
    /// specified by the filter
    /// </summary>
    NoData = 1 << 0,

    /// <summary>
    /// The player's rating is below the minimum threshold
    /// </summary>
    MinRating = 1 << 1,

    /// <summary>
    /// The player's rating is above the maximum threshold
    /// </summary>
    MaxRating = 1 << 2,

    /// <summary>
    /// The player's rating is provisional and the filter
    /// disallows provisional ratings
    /// </summary>
    IsProvisional = 1 << 3,

    /// <summary>
    /// The player has not played in the minimum specified
    /// number of tournaments
    /// </summary>
    NotEnoughTournaments = 1 << 4,

    /// <summary>
    /// The player's all-time peak rating is above the maximum threshold
    /// </summary>
    PeakRatingTooHigh = 1 << 5,

    /// <summary>
    /// The player has not played in the minimum specified number of matches
    /// </summary>
    NotEnoughMatches = 1 << 6,

    /// <summary>
    /// The player's osu! global rank is below the minimum threshold
    /// </summary>
    MinRank = 1 << 7,

    /// <summary>
    /// The player's osu! global rank is above the maximum threshold
    /// </summary>
    MaxRank = 1 << 8
}
