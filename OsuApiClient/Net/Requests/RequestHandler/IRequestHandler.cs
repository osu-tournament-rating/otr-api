using OsuApiClient.Domain;

namespace OsuApiClient.Net.Requests.RequestHandler;

/// <summary>
/// Interfaces the handler that makes direct requests an API
/// </summary>
public interface IRequestHandler : IDisposable
{
    /// <summary>
    /// Fetches an API and deserializes the response into a model
    /// </summary>
    /// <param name="request">API request details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <typeparam name="TModel">Type of the final model</typeparam>
    /// <typeparam name="TJsonModel">Type of the JSON model</typeparam>
    /// <returns>The response deserialized into the given model, or null if fetching was not successful</returns>
    Task<TModel?> FetchAsync<TModel, TJsonModel>(
        IApiRequest request,
        CancellationToken cancellationToken = default
    ) where TModel : class, IModel where TJsonModel : class;

    /// <summary>
    /// Fetches an API and deserializes the response into a list of a model
    /// </summary>
    /// <param name="request">API request details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <typeparam name="TModel">Type of the final model</typeparam>
    /// <typeparam name="TJsonModel">Type of the JSON model</typeparam>
    /// <returns>The response deserialized into a list of the given model, or null if fetching was not successful</returns>
    Task<IEnumerable<TModel>?> FetchEnumerableAsync<TModel, TJsonModel>(
        IApiRequest request,
        CancellationToken cancellationToken = default
    ) where TModel : class, IModel where TJsonModel : class;
}
