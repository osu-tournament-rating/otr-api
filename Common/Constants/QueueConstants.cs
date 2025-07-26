namespace Common.Constants;

public static class QueueConstants
{
    public static class AutomatedChecks
    {
        public const string Tournaments = "processing.checks.tournaments";
    }

    public static class Osu
    {
        public const string Beatmaps = "data.osu.beatmaps";
        public const string Matches = "data.osu.matches";
        public const string Players = "data.osu.players";
    }

    public static class OsuTrack
    {
        public const string Players = "data.osutrack.players";
    }

    public static class Stats
    {
        public const string Tournaments = "processing.stats.tournaments";
    }

    /// <summary>
    /// Published by otr-processor
    /// </summary>
    public static class Processing
    {
        public const string TournamentsProcessed = "processing.ratings.tournaments";
    }
}
