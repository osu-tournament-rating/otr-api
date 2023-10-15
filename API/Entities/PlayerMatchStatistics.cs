using System.ComponentModel.DataAnnotations.Schema;

namespace API.Entities;

/// <summary>
/// A set of statistics updated by the rating processor for each player.
/// This set of stats is for a single match. This is here to avoid duplicate data
/// in <see cref="PlayerGameStatistics"/>, as that data gets set on a per-game basis.
/// </summary>
[Table("player_match_statistics")]
public class PlayerMatchStatistics
{
	[Column("id")]
	public int Id { get; set; }
	[Column("player_id")]
	public int PlayerId { get; set; }
	[Column("match_id")]
	public int MatchId { get; set; }
	[Column("won")]
	public bool Won { get; set; }
	[Column("match_cost")]
	public double MatchCost { get; set; }
	[Column("points_earned")]
	public int PointsEarned { get; set; }
	[Column("rating_before")]
	public double RatingBefore { get; set; }
	[Column("rating_after")]
	public double RatingAfter { get; set; }
	[Column("rating_change")]
	public double RatingChange { get; set; }
	[Column("volatility_before")]
	public double VolatilityBefore { get; set; }
	[Column("volatility_after")]
	public double VolatilityAfter { get; set; }
	[Column("average_score")]
	public int AverageScore { get; set; }
	[Column("average_misses")]
	public int AverageMisses { get; set; }
	[Column("average_accuracy")]
	public double AverageAccuracy { get; set; }
	[Column("games_played")]
	public int GamesPlayed { get; set; }
	[Column("global_rank_before")]
	public int GlobalRankBefore { get; set; }
	[Column("global_rank_after")]
	public int GlobalRankAfter { get; set; }
	[Column("country_rank_before")]
	public int CountryRankBefore { get; set; }
	[Column("country_rank_after")]
	public int CountryRankAfter { get; set; }
	[Column("percentile_before")]
	public double PercentileBefore { get; set; }
	[Column("percentile_after")]
	public double PercentileAfter { get; set; }
	
	[InverseProperty("MatchStatistics")]
	public virtual Player Player { get; set; } = null!;
	
	[InverseProperty("Statistics")]
	public virtual Match Match { get; set; } = null!;
}