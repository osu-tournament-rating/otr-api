using API.Entities;

namespace APITests.SeedData;

public static class SeededConfig
{
	public static Config Get() => new()
	{
		Key = "WEB_CLIENT_SECRET",
		Value = "abcdefg()@1616*#@@@1315imgs",
		Id = 1,
		Created = new DateTime(2023,11,12),
	};
}