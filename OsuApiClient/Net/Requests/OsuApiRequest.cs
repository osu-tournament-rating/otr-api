using OsuApiClient.Net.Authorization;

namespace OsuApiClient.Net.Requests;

/// <summary>
/// Represents the details of an outgoing request to the osu! API
/// </summary>
internal sealed class OsuApiRequest : IOsuApiRequest
{
    public OsuCredentials? Credentials { get; set; } = null!;

    public HttpMethod Method { get; set; } = null!;

    public Uri Route { get; set; } = null!;

    public IDictionary<string, string>? QueryParameters { get; set; } = new Dictionary<string, string>();

    public IDictionary<string, string>? RequestBody { get; set; } = new Dictionary<string, string>();
}
