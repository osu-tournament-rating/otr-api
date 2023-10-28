using System.ComponentModel.DataAnnotations.Schema;

namespace API.Entities;

[Table("match_rating_statistics")]
public class MatchRatingStatistics
{
	[Column("id")]
	public int Id { get; set; }
	[Column("player_id")]
	public int PlayerId { get; set; }
	[Column("match_id")]
	public int MatchId { get; set; }
	[Column("match_cost")]
	public double MatchCost { get; set; }
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
	[Column("volatility_change")]
	public double VolatilityChange { get; set; }
	[Column("global_rank_before")]
	public int GlobalRankBefore { get; set; }
	[Column("global_rank_after")]
	public int GlobalRankAfter { get; set; }
	[Column("global_rank_change")]
	public int GlobalRankChange { get; set; }
	[Column("country_rank_before")]
	public int CountryRankBefore { get; set; }
	[Column("country_rank_after")]
	public int CountryRankAfter { get; set; }
	[Column("country_rank_change")]
	public int CountryRankChange { get; set; }
	[Column("percentile_before")]
	public double PercentileBefore { get; set; }
	[Column("percentile_after")]
	public double PercentileAfter { get; set; }
	[Column("percentile_change")]
	public double PercentileChange { get; set; }
	[Column("average_teammate_rating")]
	public double? AverageTeammateRating { get; set; }
	[Column("average_opponent_rating")]
	public double? AverageOpponentRating { get; set; }
	
	[InverseProperty("MatchRatingStatistics")]
	public virtual Player Player { get; set; } = null!;
	[InverseProperty("RatingStatistics")]
	public virtual Match Match { get; set; } = null!;
}