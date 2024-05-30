namespace OsuApiClient.Net.Constants;

public static class Endpoints
{
    public const string BaseUrl = "https://osu.ppy.sh";
    public const string Api = "/api/v2";

    #region OAuth

    public const string OAuth = "/oauth";

    public const string Authorize = "/authorize";
    public const string Token = "/token";

    public const string Credentials = OAuth + Token;
    public const string AuthorizationCode = OAuth + Authorize;

    # endregion

    public const string Matches = "/matches";

    public const string Users = Api + "/users";
    public const string Me = Api + "/me";
}
