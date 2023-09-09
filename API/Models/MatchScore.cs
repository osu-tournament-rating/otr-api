using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace API.Models;

[Table("match_scores")]
[Index("GameId", "PlayerId", Name = "match_scores_gameid_playerid", IsUnique = true)]
public partial class MatchScore
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("game_id")]
    public int GameId { get; set; }

    [Column("team")]
    public int Team { get; set; }

    [Column("score")]
    public long Score { get; set; }

    [Column("max_combo")]
    public int MaxCombo { get; set; }

    [Column("count_50")]
    public int Count50 { get; set; }

    [Column("count_100")]
    public int Count100 { get; set; }

    [Column("count_300")]
    public int Count300 { get; set; }

    [Column("count_miss")]
    public int CountMiss { get; set; }

    [Column("perfect")]
    public bool Perfect { get; set; }

    [Column("pass")]
    public bool Pass { get; set; }

    [Column("enabled_mods")]
    public int? EnabledMods { get; set; }

    [Column("count_katu")]
    public int CountKatu { get; set; }

    [Column("count_geki")]
    public int CountGeki { get; set; }

    [Column("player_id")]
    public int PlayerId { get; set; }

    [ForeignKey("GameId")]
    [InverseProperty("MatchScores")]
    public virtual Game Game { get; set; } = null!;

    [ForeignKey("PlayerId")]
    [InverseProperty("MatchScores")]
    public virtual Player Player { get; set; } = null!;
}
