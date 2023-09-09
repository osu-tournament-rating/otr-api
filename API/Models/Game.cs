using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace API.Models;

[Table("games")]
[Index("GameId", Name = "osugames_gameid", IsUnique = true)]
public partial class Game
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

    [Column("game_id")]
    public long GameId { get; set; }

    [Column("created", TypeName = "timestamp without time zone")]
    public DateTime Created { get; set; }

    [Column("start_time", TypeName = "timestamp without time zone")]
    public DateTime StartTime { get; set; }

    [Column("end_time", TypeName = "timestamp without time zone")]
    public DateTime? EndTime { get; set; }

    [Column("updated", TypeName = "timestamp without time zone")]
    public DateTime? Updated { get; set; }

    [ForeignKey("MatchId")]
    [InverseProperty("Games")]
    public virtual Match Match { get; set; } = null!;

    [InverseProperty("Game")]
    public virtual ICollection<MatchScore> MatchScores { get; set; } = new List<MatchScore>();
}
