namespace Common.Constants;

public static class QueueConstants
{
    public static class AutomatedChecks
    {
        public const string Tournaments = "processing.checks.tournaments";

        [Obsolete("Use tournament-level automation checks instead. Child entity checks are now orchestrated within tournament processing.")]
        public const string Matches = "processing.checks.matches";

        [Obsolete("Use tournament-level automation checks instead. Child entity checks are now orchestrated within tournament processing.")]
        public const string Games = "processing.checks.games";

        [Obsolete("Use tournament-level automation checks instead. Child entity checks are now orchestrated within tournament processing.")]
        public const string Scores = "processing.checks.scores";
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
        public const string Matches = "processing.stats.matches";
    }

    public static class Processing
    {
        public const string TournamentsProcessed = "tournaments.processed";
    }
}
