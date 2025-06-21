namespace TestingUtils.SeededData;

/// <summary>
/// Utility for generating <see cref="DateTime"/>s with seeded data
/// </summary>
public static class SeededDate
{
    /// <summary>
    /// Placeholder date used by the database
    /// </summary>
    public static readonly DateTime Placeholder = new(2007, 9, 17);

    private static readonly Random s_rand = new();

    /// <summary>
    /// Generates a <see cref="DateTime"/> with a random date between the <see cref="Placeholder"/> and <see cref="DateTime.Now"/>
    /// </summary>
    public static DateTime Generate() =>
        Placeholder.AddSeconds(s_rand.NextInclusive((int)(DateTime.Now - Placeholder).TotalSeconds));

    /// <summary>
    /// Generates a <see cref="DateTime"/> with a random between a given date and <see cref="DateTime.Now"/>
    /// </summary>
    public static DateTime GenerateAfter(DateTime afterDate) =>
        afterDate.AddSeconds(s_rand.NextInclusive((int)(DateTime.Now - afterDate).TotalSeconds));
}
