namespace Database.Enums;

/// <summary>
/// Explains why the player failed screening
/// </summary>
[Flags]
public enum ScreeningFailReason
{
    /// <summary>
    /// The player passed screening, thus there is no failure reason
    /// </summary>
    None = 0,
    /// <summary>
    /// The player does not have a rating / profile in the o!TR database
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
    /// The player is provisional and the screening criteria specifies
    /// exclusion of provisional players
    /// </summary>
    IsProvisional = 1 << 3,
    /// <summary>
    /// The player has not played in the minimum specified
    /// amount of tournaments
    /// </summary>
    NotEnoughTournaments = 1 << 4,
    /// <summary>
    /// The player's all-time peak rating is above the maximum allowed
    /// </summary>
    PeakRatingTooHigh = 1 << 5,
    /// <summary>
    /// The player has not played in the minimum specified amount of matches
    /// </summary>
    NotEnoughMatches = 1 << 6
}
