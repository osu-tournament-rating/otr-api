namespace API.DTOs;

public class PlayerRanksDTO
{
    public int Id { get; set; }
    public long OsuId { get; set; }
    public int? RankStandard { get; set; }
    public int? RankTaiko { get; set; }
    public int? RankCatch { get; set; }
    public int? RankMania { get; set; }

    public int? EarliestOsuGlobalRank { get; set; }
    public DateTime? EarliestOsuGlobalRankDate { get; set; }
    public int? EarliestTaikoGlobalRank { get; set; }
    public DateTime? EarliestTaikoGlobalRankDate { get; set; }
    public int? EarliestCatchGlobalRank { get; set; }
    public DateTime? EarliestCatchGlobalRankDate { get; set; }
    public int? EarliestManiaGlobalRank { get; set; }
    public DateTime? EarliestManiaGlobalRankDate { get; set; }
}
