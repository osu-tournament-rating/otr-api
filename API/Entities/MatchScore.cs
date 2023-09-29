using API.Osu;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Entities;

[Table("match_scores")]
[Index("GameId", "PlayerId", Name = "match_scores_gameid_playerid", IsUnique = true)]
public class MatchScore
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
	[Column("is_valid")]
	public bool? IsValid { get; set; }
	[InverseProperty("MatchScores")]
	public virtual Game Game { get; set; } = null!;
	[ForeignKey("PlayerId")]
	[InverseProperty("MatchScores")]
	public virtual Player Player { get; set; } = null!;
	[NotMapped]
	public OsuEnums.Mods? EnabledModsEnum
	{
		get
		{
			if (EnabledMods != null)
			{
				return (OsuEnums.Mods)EnabledMods;
			}

			return null;
		}
	}
	/// <summary>
	///  Accuracy represented as a full percentage, e.g. 98.5 (instead of 0.985)
	/// </summary>
	[NotMapped]
	public double AccuracyStandard => (100 * ((300d * Count300) + (100d * Count100) + (50d * Count50))) / (300d * (Count300 + Count100 + Count50 + CountMiss));
	/// <summary>
	///  Accuracy represented as a full percentage, e.g. 98.5 (instead of 0.985)
	/// </summary>
	[NotMapped]
	public double AccuracyTaiko => (100 * (Count300 + (0.5 * Count100))) / (Count300 + Count100 + CountMiss);
	/// <summary>
	///  Accuracy represented as a full percentage, e.g. 98.5 (instead of 0.985).
	/// </summary>
	[NotMapped]
	public double AccuracyCatch
	{
		get
		{
			int nFruitsCaught = Count300;
			int nDropsCaught = Count100;
			int nDropletsCaught = Count50;

			double divisor = nFruitsCaught + nDropsCaught + nDropletsCaught + CountMiss + CountKatu;

			return (100 * (Count300 + Count100 + Count50)) / divisor;
		}
	}
	/// <summary>
	///  Accuracy represented as a full percentage, e.g. 98.5 (instead of 0.985). ScoreV2 accuracy as shown here
	///  https://osu.ppy.sh/wiki/en/Gameplay/Accuracy
	/// </summary>
	[NotMapped]
	public double AccuracyMania => (100 * ((305d * CountGeki) + (300 * Count300) + (200 * CountKatu) + (100 * Count100) + (50 * Count50))) /
	                               (305d * (CountGeki + Count300 + CountKatu + Count100 + Count50 + CountMiss));
}