namespace Database.Enums;

/// <summary>
/// Represents the team type used for a game
/// </summary>
/// <remarks>See <a href="https://osu.ppy.sh/wiki/en/Client/Interface/Multiplayer"> osu! wiki - Multiplayer</a></remarks>
public enum TeamType
{
    /// <summary>
    /// Free for all
    /// </summary>
    HeadToHead = 0,

    /// <summary>
    /// Free for all (Tag format)
    /// </summary>
    /// <remarks>All players play tag on the same beatmap</remarks>
    TagCoop = 1,

    /// <summary>
    /// Team red vs team blue
    /// </summary>
    TeamVs = 2,

    /// <summary>
    /// Team red vs team blue (Tag format)
    /// </summary>
    TagTeamVs = 3
}
