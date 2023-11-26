using API.Entities;

namespace APITests.SeedData;

public static class SeededBeatmap
{
	public static Beatmap Get() => new()
	{
		Id = 1,
		Artist = "Stage & Onkar",
		BeatmapId = 3170094,
		Bpm = 168,
		MapperId = 2841009,
		MapperName = "Mirash",
		Sr = 6.5,
		AimDiff = 2.4,
		SpeedDiff = 3.2,
		Cs = 4.2,
		Ar = 9.4,
		Hp = 4.2,
		Od = 9.1,
		DrainTime = 100,
		Length = 100,
		Title = "Beatmap Title",
		DiffName = "Insane",
		GameMode = 0,
		CircleCount = 100,
		SliderCount = 200,
		SpinnerCount = 3,
		MaxCombo = 500,
		Created = new DateTime(2023,11,12),
	};
}