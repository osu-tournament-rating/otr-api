using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using Database.Enums;

namespace Database.Entities;

/// <summary>
/// Represents the highest o!TR ranks of a <see cref="Entities.Player"/> in a <see cref="Enums.Ruleset"/>
/// </summary>
[Table("player_highest_ranks")]
[SuppressMessage("ReSharper", "CollectionNeverUpdated.Global")]
[SuppressMessage("ReSharper", "PropertyCanBeMadeInitOnly.Global")]
public class PlayerHighestRanks : UpdateableEntityBase
{
    /// <summary>
    /// The <see cref="Enums.Ruleset"/> in which the ranks were recorded in
    /// </summary>
    [Column("ruleset")]
    public Ruleset Ruleset { get; set; }

    /// <summary>
    /// Highest global rank achieved
    /// </summary>
    [Column("global_rank")]
    public int GlobalRank { get; set; }

    /// <summary>
    /// Timestamp of when the <see cref="GlobalRank"/> was achieved
    /// </summary>
    [Column("global_rank_date")]
    public DateTime GlobalRankDate { get; set; }

    /// <summary>
    /// Highest country rank achieved
    /// </summary>
    [Column("country_rank")]
    public int CountryRank { get; set; }

    /// <summary>
    /// Timestamp of when the <see cref="CountryRank"/> was achieved
    /// </summary>
    [Column("country_rank_date")]
    public DateTime CountryRankDate { get; set; }

    /// <summary>
    /// The id of the <see cref="Player"/> this record belongs to
    /// </summary>
    [Column("player_id")]
    public int PlayerId { get; set; }

    /// <summary>
    /// The <see cref="Player"/> this record belongs to
    /// </summary>
    public Player Player { get; set; } = null!;
}
