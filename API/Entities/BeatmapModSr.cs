using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Entities;

[Table("beatmap_mod_sr")]
[PrimaryKey(nameof(BeatmapId), nameof(Mods))]
public class BeatmapModSr
{
	[Column("beatmap_id")]
	public int BeatmapId { get; set; }
	[Column("mods")]
	public int Mods { get; set; }
	[Column("post_mod_sr")]
	public double PostModSr { get; set; }
}