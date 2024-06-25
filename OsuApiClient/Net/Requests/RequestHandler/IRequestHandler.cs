using OsuApiClient.Domain;
using OsuApiClient.Net.JsonModels;

namespace OsuApiClient.Net.Requests.RequestHandler;

/// <summary>
/// Interfaces the handler that makes direct requests an API
/// </summary>
public interface IRequestHandler : IDisposable
{
    /// <summary>
    /// Fetches an API
    /// </summary>
    /// <remarks>Does not return a response</remarks>
    /// <param name="request">API request details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task FetchAsync(
        IApiRequest request,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Fetches an API and deserializes the response into a JSON model
    /// </summary>
    /// <param name="request">API request details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <typeparam name="TJsonModel">Type of JSON model to output</typeparam>
    /// <returns>The response deserialized into the given JSON model, or null if fetching was not successful</returns>
    Task<TJsonModel?> FetchAsync<TJsonModel>(
        IApiRequest request,
        CancellationToken cancellationToken = default
    ) where TJsonModel : class, IJsonModel;

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
    ) where TModel : class, IModel where TJsonModel : class, IJsonModel;

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
    ) where TModel : class, IModel where TJsonModel : class, IJsonModel;
}
