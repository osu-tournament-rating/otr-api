using API.Entities.Bases;
using API.Osu.Multiplayer;
using Dapper;

namespace API.Entities;


public class OsuScore : EntityBase
{
	[Column("match_id")]
	public Team Team { get; set; }
	public int PlayerId { get; set; }
	public long Score { get; set; }
	public int MaxCombo { get; set; }
	public int Count50 { get; set; }
	public int Count100 { get; set; }
	public int Count300 { get; set; }
	public int CountMiss { get; set; }
	public bool Perfect { get; set; }
	public bool Pass { get; set; }
	public Mods? EnabledMods { get; set; }
}