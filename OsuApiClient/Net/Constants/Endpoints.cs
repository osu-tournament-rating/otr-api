using System.Diagnostics.CodeAnalysis;
using OsuApiClient.Enums;

namespace OsuApiClient.Net.Constants;

[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
public static class Endpoints
{
    public static class Osu
    {
        public const string BaseUrl = "https://osu.ppy.sh";
        public const string Api = "/api/v2";

        public const string OAuth = "/oauth";
        public const string Authorize = "/authorize";
        public const string Token = "/token";

        public const string Credentials = OAuth + Token;
        public const string AuthorizationCode = OAuth + Authorize;

        public const string Matches = Api + "/matches";

        public const string Users = Api + "/users";
        public const string Me = Api + "/me";

        public const string Beatmaps = Api + "/beatmaps";
        public const string BeatmapAttributes = Beatmaps + "/{0:long}/attributes";
        public const string Beatmapsets = Api + "/beatmapsets/{0:long}";
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
