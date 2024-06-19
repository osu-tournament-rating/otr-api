namespace Database.Entities.Interfaces;

/// <summary>
/// Interfaces judgement statistics for a score
/// </summary>
public interface IScoreStatistics
{
    /// <summary>
    /// Count of notes hit with "MEH" timing
    /// </summary>
    /// <remarks>See <a href="https://osu.ppy.sh/wiki/en/Gameplay/Judgement/osu%21">osu! Judgement</a></remarks>
    public int Count50 { get; }

    /// <summary>
    /// Count of notes hit with "OK" timing
    /// </summary>
    /// <remarks>See <a href="https://osu.ppy.sh/wiki/en/Gameplay/Judgement/osu%21">osu! Judgement</a></remarks>
    public int Count100 { get; }

    /// <summary>
    /// Count of notes hit with "GREAT" timing
    /// </summary>
    /// <remarks>See <a href="https://osu.ppy.sh/wiki/en/Gameplay/Judgement/osu%21">osu! Judgement</a></remarks>
    public int Count300 { get; }

    /// <summary>
    /// Count of misses
    /// </summary>
    public int CountMiss { get; }
}
