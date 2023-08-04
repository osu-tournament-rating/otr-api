using API.Entities.Bases;
using Dapper;
#pragma warning disable CS8618

namespace API.Entities
{
	[Table("matchdata")]
	public class MatchData : EntityBase
	{
		[Column("player_id")]
		public int PlayerId { get; set; }

		[Column("osu_match_id")]
		public long OsuMatchId { get; set; }

		[Column("game_id")]
		public long GameId { get; set; }

		[Column("scoring_type")]
		public string? ScoringType { get; set; }

		[Column("score")]
		public double Score { get; set; }

		[Column("osu_beatmap_id")]
		public long OsuBeatmapId { get; set; }

		[Column("game_raw_mods")]
		public int GameRawMods { get; set; }

		[Column("raw_mods")]
		public int RawMods { get; set; }

		[Column("match_name")]
		public string? MatchName { get; set; }

		[Column("mode")]
		public string Mode { get; set; }

		[Column("match_start_date")]
		public DateTime MatchStartDate { get; set; }

		[Column("free_mod")]
		public bool? Freemod { get; set; }

		[Column("force_mod")]
		public bool? Forcemod { get; set; }

		[Column("team_type")]
		public string? TeamType { get; set; }

		[Column("team")]
		public string? Team { get; set; }

		[Column("osu_name")]
		public string? OsuName { get; set; }

		[Column("osu_rank")]
		public int? OsuRank { get; set; }

		[Column("osu_badges")]
		public int OsuBadges { get; set; }

		[Column("osu_duel_starrating")]
		public double? OsuDuelStarRating { get; set; }

		[Column("accuracy")]
		public double? Accuracy { get; set; }

		[Column("cs")]
		public double? CS { get; set; }

		[Column("ar")]
		public double? AR { get; set; }

		[Column("od")]
		public double? OD { get; set; }
	}
}