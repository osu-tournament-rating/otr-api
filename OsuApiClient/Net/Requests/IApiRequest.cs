using OsuApiClient.Enums;
using OsuApiClient.Net.Authorization;

namespace OsuApiClient.Net.Requests;

/// <summary>
/// Interfaces an outgoing request
/// </summary>
public interface IApiRequest
{
    /// <summary>
    /// Gets or sets the platform being fetched
    /// </summary>
    FetchPlatform Platform { get; }

    /// <summary>
    /// Gets or sets the credentials of the requesting client
    /// </summary>
    OsuCredentials? Credentials { get; }

    /// <summary>
    /// Gets or sets the http method to be used in the request
    /// </summary>
    HttpMethod Method { get; }

    /// <summary>
    /// Gets or sets the route of the request
    /// </summary>
    Uri Route { get; }

    /// <summary>
    /// Gets or sets a dictionary of query parameters
    /// </summary>
    IDictionary<string, string>? QueryParameters { get; }

    /// <summary>
    /// Gets or sets a dictionary of values to be used as the request body
    /// </summary>
    IDictionary<string, string>? RequestBody { get; }
}
