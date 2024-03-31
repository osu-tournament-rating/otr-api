namespace API.DTOs;

/// <summary>
/// Represents a player with included osu! ranking info
/// </summary>
public class PlayerRanksDTO : PlayerDTO
{
    /// <summary>
    /// Current standard rank for the player
    /// </summary>
    public int? RankStandard { get; set; }

    /// <summary>
    /// Current taiko rank for the player
    /// </summary>
    public int? RankTaiko { get; set; }

    /// <summary>
    /// Current catch rank for the player
    /// </summary>
    public int? RankCatch { get; set; }

    /// <summary>
    /// Current mania rank for the player
    /// </summary>
    public int? RankMania { get; set; }

    /// <summary>
    /// Earliest highest standard rank available for the player
    /// </summary>
    public int? EarliestStandardGlobalRank { get; set; }

    /// <summary>
    /// Date of the earliest highest standard rank available for the player
    /// </summary>
    public DateTime? EarliestStandardGlobalRankDate { get; set; }

    /// <summary>
    /// Earliest highest taiko rank available for the player
    /// </summary>
    public int? EarliestTaikoGlobalRank { get; set; }

    /// <summary>
    /// Date of the earliest highest taiko rank available for the player
    /// </summary>
    public DateTime? EarliestTaikoGlobalRankDate { get; set; }

    /// <summary>
    /// Earliest highest catch rank available for the player
    /// </summary>
    public int? EarliestCatchGlobalRank { get; set; }

    /// <summary>
    /// Date of the earliest highest catch rank available for the player
    /// </summary>
    public DateTime? EarliestCatchGlobalRankDate { get; set; }

    /// <summary>
    /// Earliest highest mania rank available for the player
    /// </summary>
    public int? EarliestManiaGlobalRank { get; set; }

    /// <summary>
    /// Date of the earliest highest mania rank available for the player
    /// </summary>
    public DateTime? EarliestManiaGlobalRankDate { get; set; }
}
