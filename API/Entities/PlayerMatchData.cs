using API.Entities.Bases;
using Dapper;

#pragma warning disable CS8618

namespace API.Entities
{
	[Table("playermatchdata")]
	public class PlayerMatchData : EntityBase
	{
		[Column("id")]
		public int ID { get; set; }
		[Column("player_id")]
		public int PlayerId { get; set; }
		[Column("matches_id")]
		public int MatchesId { get; set; }
		[Column("beatmap_id")]
		public int BeatmapId { get; set; }
	}
}