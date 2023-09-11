using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Entities;

[Table("beatmap_mod_sr")]
public class BeatmapModSr
{
	[Key]
	[Column("beatmap_id")]
	public int BeatmapId { get; set; }
	[Key]
	[Column("mods")]
	public int Mods { get; set; }
	[Column("post_mod_sr")]
	public double PostModSr { get; set; }
	[InverseProperty("BeatmapModSr")]
	public virtual Beatmap Beatmap { get; set; } = null!;
}