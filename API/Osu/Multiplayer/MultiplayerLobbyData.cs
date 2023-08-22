using Newtonsoft.Json;

namespace API.Osu.Multiplayer;

public class MultiplayerLobbyData
{
	[JsonProperty("match")]
	public Match Match { get; set; }
	[JsonProperty("games")]
	public List<Game> Games { get; set; }
}

public class Match
{
    /// <summary>
    ///  Match ID
    /// </summary>
    [JsonProperty("match_id")]
	public string MatchId { get; set; }
    /// <summary>
    ///  Name of the match
    /// </summary>
    [JsonProperty("name")]
	public string Name { get; set; }
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
	public string GameId { get; set; }
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
	public string BeatmapId { get; set; }
    /// <summary>
    ///  Standard = 0, Taiko = 1, CTB = 2, o!m = 3
    /// </summary>
    [JsonProperty("play_mode")]
	public string PlayMode { get; set; }
    /// <summary>
    ///  Couldn't find
    /// </summary>
    [JsonProperty("match_type")]
	public string MatchType { get; set; }
    /// <summary>
    ///  Winning condition: score = 0, accuracy = 1, combo = 2, score v2 = 3
    /// </summary>
    [JsonProperty("scoring_type")]
	public string ScoringType { get; set; }
    /// <summary>
    ///  Head to head = 0, Tag Co-op = 1, Team vs = 2, Tag Team vs = 3
    /// </summary>
    [JsonProperty("team_type")]
	public string TeamType { get; set; }
    /// <summary>
    ///  Global mods, see reference below
    /// </summary>
    [JsonProperty("mods")]
	public string Mods { get; set; }
	[JsonProperty("scores")]
	public List<Score> Scores { get; set; }
}

public class Score
{
    /// <summary>
    ///  0 based index of player's slot
    /// </summary>
    [JsonProperty("slot")]
	public string Slot { get; set; }
    /// <summary>
    ///  If mode doesn't support teams it is 0, otherwise 1 = blue, 2 = red
    /// </summary>
    [JsonProperty("team")]
	public string Team { get; set; }
	[JsonProperty("user_id")]
	public string UserId { get; set; }
	[JsonProperty("score")]
	public string PlayerScore { get; set; }
	[JsonProperty("maxcombo")]
	public string MaxCombo { get; set; }
    /// <summary>
    ///  Not used
    /// </summary>
    [JsonProperty("rank")]
	public string Rank { get; set; }
	[JsonProperty("count50")]
	public string Count50 { get; set; }
	[JsonProperty("count100")]
	public string Count100 { get; set; }
	[JsonProperty("count300")]
	public string Count300 { get; set; }
	[JsonProperty("countmiss")]
	public string CountMiss { get; set; }
	[JsonProperty("countgeki")]
	public string CountGeki { get; set; }
	[JsonProperty("countkatu")]
	public string CountKatu { get; set; }
    /// <summary>
    ///  Full combo
    /// </summary>
    [JsonProperty("perfect")]
	public string Perfect { get; set; }
    /// <summary>
    ///  If the player failed at the end of the map it is 0, otherwise (pass or revive) it is 1
    /// </summary>
    [JsonProperty("pass")]
	public string Pass { get; set; }
	[JsonProperty("enabled_mods")]
	public string? EnabledMods { get; set; }
}