namespace TestingUtils;

/// <summary>
/// Extension methods for <see cref="Random"/>
/// </summary>
public static class RandomExtensions
{
    /// <summary>
    /// Returns a random integer that is less than or equal to a max value
    /// </summary>
    public static int NextInclusive(this Random rand, int maxValue) =>
        rand.Next(maxValue + 1);

    /// <summary>
    /// Returns a random integer that is within a specified range (inclusive of the max)
    /// </summary>
    public static int NextInclusive(this Random rand, int minValue, int maxValue) =>
        rand.Next(minValue, maxValue + 1);

    /// <summary>
    /// Chooses a <typeparamref name="T"/> at random
    /// </summary>
    public static T NextEnum<T>(this Random rand) where T : struct, Enum =>
        rand.GetItems(Enum.GetValues<T>(), 1).First();

    /// <summary>
    /// Returns true or false at random
    /// </summary>
    public static bool NextBool(this Random rand) =>
        rand.Next(2) == 0;

    /// <summary>
    /// Returns a random double that is less than or equal to a max value
    /// </summary>
    public static double NextDouble(this Random rand, double maxValue) =>
        rand.NextDouble() * maxValue;

    /// <summary>
    /// Returns a random double that is within a specified range
    /// </summary>
    public static double NextDouble(this Random rand, double minValue, double maxValue) =>
        rand.NextDouble() * (maxValue - minValue) + minValue;
}
