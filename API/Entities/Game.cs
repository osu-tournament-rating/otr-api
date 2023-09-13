using API.Osu;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Entities;

[Table("games")]
[Index("GameId", Name = "osugames_gameid", IsUnique = true)]
public class Game
{
	[Key]
	[Column("id")]
	public int Id { get; set; }
	[Column("match_id")]
	public int MatchId { get; set; }
	[Column("beatmap_id")]
	public int? BeatmapId { get; set; }
	[Column("play_mode")]
	public int PlayMode { get; set; }
	[Column("match_type")]
	public int MatchType { get; set; }
	[Column("scoring_type")]
	public int ScoringType { get; set; }
	[Column("team_type")]
	public int TeamType { get; set; }
	[Column("mods")]
	public int Mods { get; set; }
	[Column("post_mod_sr")]
	public double PostModSr { get; set; }
	[Column("game_id")]
	public long GameId { get; set; }
	[Column("created", TypeName = "timestamp with time zone")]
	public DateTime Created { get; set; }
	[Column("start_time", TypeName = "timestamp with time zone")]
	public DateTime StartTime { get; set; }
	[Column("end_time", TypeName = "timestamp with time zone")]
	public DateTime? EndTime { get; set; }
	[Column("updated", TypeName = "timestamp with time zone")]
	public DateTime? Updated { get; set; }
	[ForeignKey("MatchId")]
	[InverseProperty("Games")]
	public virtual Match Match { get; set; } = null!;
	[ForeignKey("BeatmapId")]
	[InverseProperty("Games")]
	public virtual Beatmap? Beatmap { get; set; }
	[InverseProperty("Game")]
	public virtual ICollection<MatchScore> MatchScores { get; set; } = new List<MatchScore>();
	[NotMapped]
	public OsuEnums.Mods ModsEnum => (OsuEnums.Mods)Mods;
	[NotMapped]
	public OsuEnums.Mode PlayModeEnum => (OsuEnums.Mode)PlayMode;
	[NotMapped]
	public OsuEnums.ScoringType ScoringTypeEnum => (OsuEnums.ScoringType)ScoringType;
	[NotMapped]
	public OsuEnums.TeamType TeamTypeEnum => (OsuEnums.TeamType)TeamType;
}