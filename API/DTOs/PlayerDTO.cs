namespace API.DTOs;

public class PlayerDTO
{
	public int Id { get; set; }
	public long OsuId { get; set; }
	public int? RankStandard { get; set; }
	public int? RankTaiko { get; set; }
	public int? RankCatch { get; set; }
	public int? RankMania { get; set; }
	
	public ICollection<MatchScoreDTO> MatchScores { get; set; } = new List<MatchScoreDTO>();
	public ICollection<RatingHistoryDTO> RatingHistories { get; set; } = new List<RatingHistoryDTO>();
	public ICollection<RatingDTO> Ratings { get; set; } = new List<RatingDTO>();
	public UserDTO? User { get; set; }
}