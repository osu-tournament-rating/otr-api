using OsuApiClient.Enums;

namespace OsuApiClient.Net.Constants;

public static class Endpoints
{
    public static class Osu
    {
        public const string BaseUrl = "https://osu.ppy.sh";
        private const string Api = "/api/v2";

        private const string OAuth = "/oauth";
        private const string Token = "/token";

        public const string Credentials = OAuth + Token;

        public const string Matches = Api + "/matches";

        public const string Users = Api + "/users";
        public const string Me = Api + "/me";

        public const string Beatmaps = Api + "/beatmaps";
        public const string BeatmapAttributes = Beatmaps + "/{0}/attributes";
        public const string Beatmapsets = Api + "/beatmapsets/{0}";
    }

    public static class OsuTrack
    {
        public const string BaseUrl = "https://osutrack-api.ameo.dev";

        public const string StatsHistory = "/stats_history";
    }

    /// <summary>
    /// Gets the base url for the given <see cref="FetchPlatform"/>
    /// </summary>
    public static string GetBaseUrl(FetchPlatform platform) =>
        platform switch
        {
            FetchPlatform.Osu => Osu.BaseUrl,
            FetchPlatform.OsuTrack => OsuTrack.BaseUrl,
            _ => string.Empty
        };
}
