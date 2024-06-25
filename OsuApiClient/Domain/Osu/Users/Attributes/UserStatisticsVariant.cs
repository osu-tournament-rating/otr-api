using Database.Enums;

namespace OsuApiClient.Domain.Osu.Users.Attributes;

/// <summary>
/// Represents a compact version of <see cref="UserStatistics"/> for a variant of a <see cref="Database.Enums.Ruleset"/>
/// </summary>
public class UserStatisticsVariant : IModel
{
    /// <summary>
    /// Variant of a <see cref="Database.Enums.Ruleset"/>
    /// </summary>
    public Ruleset Ruleset { get; init; }

    /// <summary>
    /// Global rank
    /// </summary>
    public int? GlobalRank { get; init; }

    /// <summary>
    /// Country rank
    /// </summary>
    public int? CountryRank { get; init; }

    /// <summary>
    /// Performance points
    /// </summary>
    public double Pp { get; init; }

    /// <summary>
    /// Denotes if the player is ranked in the variant
    /// </summary>
    public bool IsRanked { get; init; }
}
