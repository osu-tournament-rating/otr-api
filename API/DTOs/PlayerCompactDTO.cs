namespace API.DTOs;

/// <summary>
/// Represents player information
/// </summary>
public class PlayerCompactDTO
{
    /// <summary>
    /// Id
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// osu! id
    /// </summary>
    public long OsuId { get; set; }

    /// <summary>
    /// osu! username
    /// </summary>
    public string Username { get; set; } = null!;

    /// <summary>
    /// osu! country code
    /// </summary>
    public string Country { get; set; } = null!;

    /// <summary>
    /// A collection of <see cref="PlayerOsuRulesetDataDTO"/>, one for each <see cref="Enums.Ruleset"/>
    /// </summary>
    public PlayerOsuRulesetDataDTO[] RulesetData { get; set; } = [];
}
