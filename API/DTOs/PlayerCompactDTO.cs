using Common.Enums.Enums;

namespace API.DTOs;

/// <summary>
/// Represents player information
/// </summary>
public class PlayerCompactDTO
{
    /// <summary>
    /// Id
    /// </summary>
    public int Id { get; init; }

    /// <summary>
    /// osu! id
    /// </summary>
    public long OsuId { get; init; }

    /// <summary>
    /// osu! username
    /// </summary>
    public string Username { get; init; } = null!;

    /// <summary>
    /// osu! country code
    /// </summary>
    public string Country { get; init; } = null!;

    /// <summary>
    /// The player's primary osu! ruleset
    /// </summary>
    public Ruleset DefaultRuleset { get; init; }

    /// <summary>
    /// Id of the associated user, if available
    /// </summary>
    public int? UserId { get; init; }
}
