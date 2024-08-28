using OsuApiClient.Enums;
using OsuApiClient.Net.Authorization;

namespace OsuApiClient.Net.Requests;

/// <summary>
/// Represents the details of an outgoing request
/// </summary>
internal sealed class ApiRequest : IApiRequest
{
    public FetchPlatform Platform { get; set; } = FetchPlatform.Osu;

    public OsuCredentials? Credentials { get; set; }

    public HttpMethod Method { get; set; } = null!;

    public Uri Route { get; set; } = null!;

    public IDictionary<string, string>? QueryParameters { get; set; } = new Dictionary<string, string>();

    public IDictionary<string, string>? RequestBody { get; set; } = new Dictionary<string, string>();
}
