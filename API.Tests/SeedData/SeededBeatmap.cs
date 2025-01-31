using Database.Entities;

namespace APITests.SeedData;

public static class SeededBeatmap
{
    public static Beatmap Get() =>
        new()
        {
            Id = 1369,
            OsuId = 2699135,
            Bpm = 165,
            Sr = 6.5007,
            Cs = 4,
            Ar = 9.5,
            Hp = 6,
            Od = 9,
            TotalLength = 188,
            DiffName = "Extreme",
            Ruleset = 0,
            CountCircle = 674,
            CountSlider = 430,
            CountSpinner = 0,
            MaxCombo = 1551,
            Created = new DateTime(2023, 09, 03)
        };
}
