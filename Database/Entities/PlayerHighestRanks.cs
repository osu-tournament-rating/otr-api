using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using Database.Enums;

namespace Database.Entities;

/// <summary>
/// Represents the highest ranks of a player for a given ruleset
/// </summary>
[Table("player_highest_ranks")]
[SuppressMessage("ReSharper", "CollectionNeverUpdated.Global")]
[SuppressMessage("ReSharper", "PropertyCanBeMadeInitOnly.Global")]
public class PlayerHighestRanks : UpdateableEntityBase
{
    /// <summary>
    /// The ruleset in which this highest rank record occurred in
    /// </summary>
    [Column("ruleset")]
    public Ruleset Ruleset { get; set; }

    /// <summary>
    /// The highest global rank for this user in the given ruleset
    /// </summary>
    [Column("global_rank")]
    public int GlobalRank { get; set; }

    /// <summary>
    /// Date this player's peak global rank was achieved in the given ruleset
    /// </summary>
    [Column("global_rank_date")]
    public DateTime GlobalRankDate { get; set; }

    /// <summary>
    /// The highest country rank for this user in the given ruleset
    /// </summary>
    [Column("country_rank")]
    public int CountryRank { get; set; }

    /// <summary>
    /// Date this player's peak country rank was achieved in the given ruleset
    /// </summary>
    [Column("country_rank_date")]
    public DateTime CountryRankDate { get; set; }

    // FK, required
    [Column("player_id")]
    public int PlayerId { get; set; }

    // Backwards navigation
    public Player Player { get; set; } = null!;
}
