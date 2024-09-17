using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using Database.Enums;

namespace Database.Entities;

/// <summary>
/// Represents the highest ranks of a player
/// </summary>
[Table(name: "player_highest_ranks")]
[SuppressMessage("ReSharper", "CollectionNeverUpdated.Global")]
[SuppressMessage("ReSharper", "PropertyCanBeMadeInitOnly.Global")]
public class PlayerHighestRanks : UpdateableEntityBase
{
    /// <summary>
    /// <see cref="Enums.Ruleset"/> as set on the <see cref="Player"/>'s osu! profile
    /// </summary>
    [Column("default_ruleset")]
    public Ruleset Ruleset { get; set; }

    /// <summary>
    /// Global rank at the current time
    /// </summary>
    [Column("global_rank")]
    public int GlobalRank { get; set; }

    /// <summary>
    /// Date the global rank was achieved
    /// </summary>
    [Column("global_rank_date")]
    public DateTime GlobalRankDate { get; set; }

    /// <summary>
    /// Country rank at the current time
    /// </summary>
    [Column("country_rank")]
    public int CountryRank { get; set; }

    /// <summary>
    /// Date the current country rank was achieved
    /// </summary>
    [Column("country_rank_date")]
    public DateTime CountryRankDate { get; set; }

    // FK, required
    [Column("player_id")]
    public int PlayerId { get; set; }

    // Backwards navigation
    public Player Player { get; set; } = null!;
}
