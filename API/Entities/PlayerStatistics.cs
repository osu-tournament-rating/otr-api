using API.Utilities;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Entities;

/// <summary>
/// A set of statistics updated by the rating processor for each player.
/// This tracks data over time and can be filtered by date.
/// </summary>
[Table("player_statistics")]
public class PlayerStatistics
{
	[Column("id")]
	public int Id { get; set; }
	[Column("player_id")]
	public int PlayerId { get; set; }
	[Column("mode")]
	public int Mode { get; set; }
	
	[NotMapped]
	public string Tier => RatingUtils.GetTier(CurrentRating);
}