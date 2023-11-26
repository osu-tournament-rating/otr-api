using API.Entities;

namespace APITests.SeedData;

public static class SeedData
{
	public static BaseStats GetBaseStats() => SeededBaseStats.Get();
}