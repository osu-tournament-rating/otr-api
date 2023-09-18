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
	public ICollection<RatingHistoryDTO> RatingHistories { get; set; } = new List<RatingHistoryDTO>();
	public ICollection<RatingDTO> Ratings { get; set; } = new List<RatingDTO>();
	public UserDTO? User { get; set; }
	public Unmapped_PlayerStatisticsDTO? Statistics { get; set; } = new();
}