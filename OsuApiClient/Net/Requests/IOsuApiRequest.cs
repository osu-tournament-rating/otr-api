using OsuApiClient.Net.Authorization;

namespace OsuApiClient.Net.Requests;

/// <summary>
/// Interfaces an outgoing request to the osu! API
/// </summary>
public interface IOsuApiRequest
{
    /// <summary>
    /// Gets or sets the credentials of the requesting client
    /// </summary>
    OsuCredentials? Credentials { get; set; }

    /// <summary>
    /// Gets or sets the http method to be used in the request
    /// </summary>
    HttpMethod Method { get; set; }

    /// <summary>
    /// Gets or sets the route of the request
    /// </summary>
    Uri Route { get; set; }

    /// <summary>
    /// Gets or sets a dictionary of query parameters
    /// </summary>
    IDictionary<string, string>? QueryParameters { get; set; }

    /// <summary>
    /// Gets or sets a dictionary of values to be used as the request body
    /// </summary>
    IDictionary<string, string>? RequestBody { get; set; }
}
