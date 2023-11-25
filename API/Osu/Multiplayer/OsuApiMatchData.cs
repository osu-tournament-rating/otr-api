using Newtonsoft.Json;
using static API.Osu.OsuEnums;
#pragma warning disable CS0472

namespace API.Osu.Multiplayer;

public class OsuApiMatchData
{
	[JsonProperty("match")]
	public OsuApiMatch? OsuApiMatch { get; set; }
	[JsonProperty("games")]
	public List<OsuApiGame>? Games { get; set; }
}

public class OsuApiMatch
{
    /// <summary>
    ///  Match ID
    /// </summary>
    [JsonProperty("match_id")]
	public long MatchId { get; set; }
    /// <summary>
    ///  Name of the match
    /// </summary>
    [JsonProperty("name")]
    public string Name { get; set; } = null!;
    /// <summary>
    ///  Start time in UTC
    /// </summary>
    [JsonProperty("start_time")]
	public DateTime StartTime { get; set; }
    /// <summary>
    ///  Null if not ended, date in UTC when match is disbanded
    /// </summary>
    [JsonProperty("end_time")]
	public DateTime? EndTime { get; set; }
}

public class OsuApiGame
{
	[JsonProperty("game_id")]
	public long GameId { get; set; }
    /// <summary>
    ///  Start time in UTC
    /// </summary>
    [JsonProperty("start_time")]
	public DateTime StartTime { get; set; }
    /// <summary>
    ///  End time in UTC
    /// </summary>
    [JsonProperty("end_time")]
	public DateTime? EndTime { get; set; }
	[JsonProperty("beatmap_id")]
	public long BeatmapId { get; set; }
    /// <summary>
    ///  Standard = 0, Taiko = 1, CTB = 2, o!m = 3
    /// </summary>
    [JsonProperty("play_mode")]
	public Mode PlayMode { get; set; }
    /// <summary>
    ///  Couldn't find
    /// </summary>
    [JsonProperty("match_type")]
	public object? MatchType { get; set; }
    /// <summary>
    ///  Winning condition: score = 0, accuracy = 1, combo = 2, score v2 = 3
    /// </summary>
    [JsonProperty("scoring_type")]
	public ScoringType ScoringType { get; set; }
    /// <summary>
    ///  Head to head = 0, Tag Co-op = 1, Team vs = 2, Tag Team vs = 3
    /// </summary>
    [JsonProperty("team_type")]
	public TeamType TeamType { get; set; }
    /// <summary>
    ///  Global mods, see reference below
    /// </summary>
    [JsonProperty("mods")]
	public Mods Mods { get; set; }
	[JsonProperty("scores")]
	public List<OsuApiScore>? Scores { get; set; }
}

public class OsuApiScore
{
    /// <summary>
    ///  0 based index of player's slot
    /// </summary>
    [JsonProperty("slot")]
	public int Slot { get; set; }
    /// <summary>
    ///  If mode doesn't support teams it is 0, otherwise 1 = blue, 2 = red
    /// </summary>
    [JsonProperty("team")]
	public Team Team { get; set; }
	[JsonProperty("user_id")]
	public long UserId { get; set; }
	[JsonProperty("score")]
	public long PlayerScore { get; set; }
	[JsonProperty("maxcombo")]
	public int MaxCombo { get; set; }
    /// <summary>
    ///  Not used
    /// </summary>
    [JsonProperty("rank")]
	public long Rank { get; set; }
	[JsonProperty("count50")]
	public int Count50 { get; set; }
	[JsonProperty("count100")]
	public int Count100 { get; set; }
	[JsonProperty("count300")]
	public int Count300 { get; set; }
	[JsonProperty("countmiss")]
	public int CountMiss { get; set; }
	[JsonProperty("countgeki")]
	public int CountGeki { get; set; }
	[JsonProperty("countkatu")]
	public int CountKatu { get; set; }
    /// <summary>
    ///  Full combo: 1 = yes, 0 = no
    /// </summary>
    [JsonProperty("perfect")]
	public int Perfect { get; set; }
    /// <summary>
    ///  If the player failed at the end of the map it is 0, otherwise (pass or revive) it is 1
    /// </summary>
    [JsonProperty("pass")]
	public int Pass { get; set; }
    [JsonProperty("enabled_mods")]
    public Mods? EnabledMods { get; set; }
}