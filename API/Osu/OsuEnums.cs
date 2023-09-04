namespace API.Osu;

public class OsuEnums
{
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
	public enum Mode
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

	public enum Team
	{
		NoTeam = 0,
		Blue = 1,
		Red = 2
	}

	/// <copyright>
	/// ppy 2023 https://github.com/ppy/osu-api/wiki#mods
	/// </copyright>
	[Flags]
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
}