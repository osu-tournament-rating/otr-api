namespace API.DTOs;

public class PlayerDTO
{
	public int Id { get; set; }
	public long OsuId { get; set; }
	public int? RankStandard { get; set; }
	public int? RankTaiko { get; set; }
	public int? RankCatch { get; set; }
	public int? RankMania { get; set; }
	
	public int? EarliestOsuGlobalRank { get; set; }
	public int? EarliestManiaGlobalRank { get; set; }
	public int? EarliestTaikoGlobalRank { get; set; }
	public int? EarliestCatchGlobalRank { get; set; }
	
	public DateTime? EarliestOsuGlobalRankDate { get; set; }
	public DateTime? EarliestManiaGlobalRankDate { get; set; }
	public DateTime? EarliestTaikoGlobalRankDate { get; set; }
	public DateTime? EarliestCatchGlobalRankDate { get; set; }
	
	public ICollection<MatchScoreDTO> MatchScores { get; set; } = new List<MatchScoreDTO>();
	public ICollection<BaseStatsDTO> BaseStats { get; set; } = new List<BaseStatsDTO>();
	public UserDTO? User { get; set; }
}