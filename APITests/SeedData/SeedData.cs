using API.Entities;

namespace APITests.SeedData;

public static class SeedData
{
	public static BaseStats GetBaseStats() => SeededBaseStats.Get();
	public static Beatmap GetBeatmap() => SeededBeatmap.Get();
	public static Config GetConfig() => SeededConfig.Get();
	public static Game GetGame() => SeededGame.Get();
}