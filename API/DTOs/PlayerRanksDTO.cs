namespace API.DTOs;

/// <summary>
/// Represents a player with included ranking info
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
    /// Earliest known standard rank available for the player after they started playing tournaments
    /// </summary>
    public int? EarliestStandardGlobalRank { get; set; }

    /// <summary>
    /// Date for the earliest known standard rank available for the player after they started playing tournaments
    /// </summary>
    public DateTime? EarliestStandardGlobalRankDate { get; set; }

    /// <summary>
    /// Earliest known taiko rank available for the player after they started playing tournaments
    /// </summary>
    public int? EarliestTaikoGlobalRank { get; set; }

    /// <summary>
    /// Date for the earliest known taiko rank available for the player after they started playing tournaments
    /// </summary>
    public DateTime? EarliestTaikoGlobalRankDate { get; set; }

    /// <summary>
    /// Earliest known catch rank available for the player after they started playing tournaments
    /// </summary>
    public int? EarliestCatchGlobalRank { get; set; }

    /// <summary>
    /// Date for the earliest known catch rank available for the player after they started playing tournaments
    /// </summary>
    public DateTime? EarliestCatchGlobalRankDate { get; set; }

    /// <summary>
    /// Earliest known mania rank available for the player after they started playing tournaments
    /// </summary>
    public int? EarliestManiaGlobalRank { get; set; }

    /// <summary>
    /// Date for the earliest known mania rank available for the player after they started playing tournaments
    /// </summary>
    public DateTime? EarliestManiaGlobalRankDate { get; set; }
}
