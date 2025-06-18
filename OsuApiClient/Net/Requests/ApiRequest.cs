using OsuApiClient.Enums;
using OsuApiClient.Net.Authorization;

namespace OsuApiClient.Net.Requests;

/// <summary>
/// Represents the details of an outgoing request
/// </summary>
internal sealed class ApiRequest : IApiRequest
{
    public FetchPlatform Platform { get; init; } = FetchPlatform.Osu;

    public OsuCredentials? Credentials { get; init; }

    public HttpMethod Method { get; init; } = null!;

    public Uri Route { get; init; } = null!;

    public IDictionary<string, string>? QueryParameters { get; init; } = new Dictionary<string, string>();

    public IDictionary<string, string>? RequestBody { get; init; } = new Dictionary<string, string>();
}
