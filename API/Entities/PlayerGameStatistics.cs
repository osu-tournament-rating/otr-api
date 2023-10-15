using System.ComponentModel.DataAnnotations.Schema;

namespace API.Entities;

/// <summary>
/// A set of statistics updated by the rating processor for each player.
/// This tracks data over time and can be filtered by date. Generally, one
/// entry is made per game.
/// </summary>
[Table("player_statistics")]
public class PlayerGameStatistics
{
	[Column("id")]
	public int Id { get; set; }
	[Column("player_id")]
	public int PlayerId { get; set; }
	[Column("game_id")]
	public int GameId { get; set; }
	[Column("mode")]
	public int Mode { get; set; }
	[Column("won")]
	public bool Won { get; set; }
	[Column("average_opponent_rating")]
	public double AverageOpponentRating { get; set; }
	[Column("average_teammate_rating")]
	public double AverageTeammateRating { get; set; }
	[Column("placing")]
	public int Placing { get; set; }
	[Column("played_hr")]
	public bool PlayedHR { get; set; }
	[Column("played_hd")]
	public bool PlayedHD { get; set; }
	[Column("played_dt")]
	public bool PlayedDT { get; set; }
	[Column("teammate_ids")]
	public int[] TeammateIds { get; set; } = Array.Empty<int>();
	[Column("opponent_ids")]
	public int[] OpponentIds { get; set; } = Array.Empty<int>();
	
	[InverseProperty("GameStatistics")]
	public virtual Player Player { get; set; } = null!;
	
	[InverseProperty("Statistics")]
	public virtual Game Game { get; set; } = null!;
}