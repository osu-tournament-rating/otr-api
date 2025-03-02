using System.Diagnostics.CodeAnalysis;
using Database.Enums;

namespace Database.Entities;

/// <summary>
/// Represents the highest o!TR ranks of a <see cref="Entities.Player"/> in a <see cref="Enums.Ruleset"/>
/// </summary>
[SuppressMessage("ReSharper", "CollectionNeverUpdated.Global")]
[SuppressMessage("ReSharper", "PropertyCanBeMadeInitOnly.Global")]
public class PlayerHighestRanks : UpdateableEntityBase
{
    /// <summary>
    /// The <see cref="Enums.Ruleset"/> in which the ranks were recorded in
    /// </summary>
    public Ruleset Ruleset { get; set; }

    /// <summary>
    /// Highest global rank achieved
    /// </summary>
    public int GlobalRank { get; set; }

    /// <summary>
    /// Timestamp of when the <see cref="GlobalRank"/> was achieved
    /// </summary>
    public DateTime GlobalRankDate { get; set; }

    /// <summary>
    /// Highest country rank achieved
    /// </summary>
    public int CountryRank { get; set; }

    /// <summary>
    /// Timestamp of when the <see cref="CountryRank"/> was achieved
    /// </summary>
    public DateTime CountryRankDate { get; set; }

    /// <summary>
    /// The id of the <see cref="Player"/> this record belongs to
    /// </summary>
    public int PlayerId { get; set; }

    /// <summary>
    /// The <see cref="Player"/> this record belongs to
    /// </summary>
    public Player Player { get; set; } = null!;
}
