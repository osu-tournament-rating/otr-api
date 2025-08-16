namespace TestingUtils.SeededData;

/// <summary>
/// Utility for generating <see cref="DateTime"/>s with seeded data
/// </summary>
public static class SeededDate
{
    /// <summary>
    /// Placeholder date used by the database
    /// </summary>
    public static readonly DateTime Placeholder = new(2007, 9, 17, 0, 0, 0, DateTimeKind.Utc);

    private static readonly Random _rand = new();

    /// <summary>
    /// Generates a <see cref="DateTime"/> with a random date between the <see cref="Placeholder"/> and <see cref="DateTime.Now"/>
    /// </summary>
    public static DateTime Generate() =>
        DateTime.SpecifyKind(Placeholder.AddSeconds(_rand.NextInclusive((int)(DateTime.UtcNow - Placeholder).TotalSeconds)), DateTimeKind.Utc);

    /// <summary>
    /// Generates a <see cref="DateTime"/> with a random between a given date and <see cref="DateTime.Now"/>
    /// </summary>
    public static DateTime GenerateAfter(DateTime afterDate) =>
        DateTime.SpecifyKind(afterDate.AddSeconds(_rand.NextInclusive((int)(DateTime.UtcNow - afterDate).TotalSeconds)), DateTimeKind.Utc);
}
