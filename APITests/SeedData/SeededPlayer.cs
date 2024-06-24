using Database.Entities;

namespace APITests.SeedData;

public static class SeededPlayer
{
    public static Player Get(int? id = null, int? osuId = null) =>
        new()
        {
            Id = id ?? 334,
            OsuId = osuId ?? 4787150,
            Created = new DateTime(2023, 08, 12),
            Updated = new DateTime(2023, 09, 17),
            Username = "Vaxei",
            Country = "US",
        };
}
