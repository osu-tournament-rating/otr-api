using Newtonsoft.Json;

namespace API.Osu.Multiplayer;

public enum ScoringType
{
	Score = 0,
	Accuracy = 1,
	Combo = 2,
	ScoreV2 = 3
}

/// <summary>
/// The mode the match was played in.
/// </summary>
public enum PlayMode
{
	Standard = 0,
	Taiko = 1,
	Catch = 2,
	Mania = 3
}

/// <summary>
/// The team type of the match (e.g. TeamVs)
/// </summary>
public enum TeamType
{
	HeadToHead = 0,
	TagCoop = 1,
	TeamVs = 2,
	TagTeamVs = 3
}

public class MultiplayerLobbyData
{
	[JsonProperty("match")]
	public Match Match { get; set; }
	[JsonProperty("games")]
	public List<Game> Games { get; set; }
}

public enum Team
{
	NoTeam = 0,
	Blue = 1,
	Red = 2
}

/// <copyright>
/// ppy 2023 https://github.com/ppy/osu-api/wiki#mods
/// </copyright>
public enum Mods
{
	None           = 0,
	NoFail         = 1,
	Easy           = 2,
	TouchDevice    = 4,
	Hidden         = 8,
	HardRock       = 16,
	SuddenDeath    = 32,
	DoubleTime     = 64,
	Relax          = 128,
	HalfTime       = 256,
	Nightcore      = 512, // Only set along with DoubleTime. i.e: NC only gives 576
	Flashlight     = 1024,
	Autoplay       = 2048,
	SpunOut        = 4096,
	Relax2         = 8192,  // Autopilot
	Perfect        = 16384, // Only set along with SuddenDeath. i.e: PF only gives 16416  
	Key4           = 32768,
	Key5           = 65536,
	Key6           = 131072,
	Key7           = 262144,
	Key8           = 524288,
	FadeIn         = 1048576,
	Random         = 2097152,
	Cinema         = 4194304,
	Target         = 8388608,
	Key9           = 16777216,
	KeyCoop        = 33554432,
	Key1           = 67108864,
	Key3           = 134217728,
	Key2           = 268435456,
	ScoreV2        = 536870912,
	Mirror         = 1073741824,
	KeyMod = Key1 | Key2 | Key3 | Key4 | Key5 | Key6 | Key7 | Key8 | Key9 | KeyCoop,
	FreeModAllowed = NoFail | Easy | Hidden | HardRock | SuddenDeath | Flashlight | FadeIn | Relax | Relax2 | SpunOut | KeyMod,
	ScoreIncreaseMods = Hidden | HardRock | DoubleTime | Flashlight | FadeIn
}

public class Match
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

public class Game
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
	public DateTime EndTime { get; set; }
	[JsonProperty("beatmap_id")]
	public long BeatmapId { get; set; }
    /// <summary>
    ///  Standard = 0, Taiko = 1, CTB = 2, o!m = 3
    /// </summary>
    [JsonProperty("play_mode")]
	public PlayMode PlayMode { get; set; }
    /// <summary>
    ///  Couldn't find
    /// </summary>
    [JsonProperty("match_type")]
	public MatchType MatchType { get; set; }
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
	public List<Score> Scores { get; set; }
}

public class Score
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