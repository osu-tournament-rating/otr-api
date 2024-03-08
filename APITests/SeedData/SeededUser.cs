using API.Entities;

namespace APITests.SeedData;

public static class SeededUser
{
    public static User Get() =>
        new()
        {
            Id = 95,
            PlayerId = 7535,
            LastLogin = new DateTime(2023, 11, 22),
            Created = new DateTime(2023, 9, 25),
            Updated = new DateTime(2023, 11, 22),
            Scopes = new[] { "MatchVerifier" },
        };
}
