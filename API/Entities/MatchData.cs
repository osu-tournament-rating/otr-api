using API.Entities.Bases;

namespace API.Entities;

public class MatchData : EntityBase
{
	public int PlayerId { get; set; }
	public long OsuMatchId { get; set; }
	public long GameId { get; set; }
	public string ScoringType { get; set; }
	public long Score { get; set; }
	public long OsuBeatmapId { get; set; }
	public int GameRawMods { get; set; }
	public int RawMods { get; set; }
	public string MatchName { get; set; }
	public string Mode { get; set; }
	public DateTime MatchStartDate { get; set; }
	public bool? Freemod { get; set; }
	public bool? Forcemod { get; set; }
	public string? TeamType { get; set; }
	public string? Team { get; set; }
	public string? OsuName { get; set; }
	public int? OsuRank { get; set; }
	public int OsuBadge { get; set; }
	public double? OsuDuelStarRating { get; set; }
	public double? Accuracy { get; set; }
	public double? CS { get; set; }
	public double? AR { get; set; }
	public double? OD { get; set; }
}