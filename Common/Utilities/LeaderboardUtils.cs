namespace Common.Utilities;

public static class LeaderboardUtils
{
    public static Dictionary<string, string> DependentTerritoriesMapping { get; } =
        new()
        {
            { "CK", "NZ" },
            { "NU", "NZ" },
            { "TK", "NZ" },
            { "SM", "IT" }
            // Need to add more
        };
}
