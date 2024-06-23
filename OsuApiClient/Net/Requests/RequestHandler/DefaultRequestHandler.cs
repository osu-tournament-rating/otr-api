using System.Net.Http.Headers;
using System.Text;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OsuApiClient.Domain;
using OsuApiClient.Extensions;
using OsuApiClient.Net.Authorization;
using OsuApiClient.Net.Constants;
using OsuApiClient.Net.JsonModels;

namespace OsuApiClient.Net.Requests.RequestHandler;

/// <summary>
/// The default implementation of the handler that makes direct requests to the osu! API
/// </summary>
internal sealed class DefaultRequestHandler(ILogger<DefaultRequestHandler> logger) : IRequestHandler
{
    private readonly HttpClient _httpClient = new()
    {
        BaseAddress = new Uri(Endpoints.BaseUrl),
        DefaultRequestHeaders =
                {
                    UserAgent = { new ProductInfoHeaderValue("OsuTournamentRating", "1.0") },
                    Accept = { new MediaTypeWithQualityHeaderValue("application/json") }
                }
    };
    private readonly Mapper _mapper = new(
        new MapperConfiguration(cfg =>
        cfg.AddMaps(typeof(DefaultRequestHandler).Assembly)
        ));
    private readonly JsonSerializer _serializer = new();

    private FixedWindowRateLimit _rateLimit = new();
    private bool _disposed;

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        _httpClient.Dispose();
    }

    public async Task FetchAsync(
        IOsuApiRequest request,
        CancellationToken cancellationToken = default
    ) =>
        await SendRequestAsync(request, cancellationToken);

    public async Task<TJsonModel?> FetchAsync<TJsonModel>(
        IOsuApiRequest request,
        CancellationToken cancellationToken = default
    ) where TJsonModel : class, IJsonModel
    {
        var responseContent = await SendRequestAsync(request, cancellationToken);
        return responseContent is not null
            ? DeserializeResponseContent<TJsonModel>(responseContent)
            : null;
    }

    public async Task<TModel?> FetchAsync<TModel, TJsonModel>(
        IOsuApiRequest request,
        CancellationToken cancellationToken = default
    ) where TModel : class, IModel where TJsonModel : class, IJsonModel
        => _mapper.Map<TModel?>(await FetchAsync<TJsonModel>(request, cancellationToken));

    /// <summary>
    /// Sends a request to the osu! API
    /// </summary>
    /// <param name="request">osu! API request details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The resulting <see cref="HttpResponseMessage"/> content as a string, or null if not successful</returns>
    private async Task<string?> SendRequestAsync(
        IOsuApiRequest request,
        CancellationToken cancellationToken = default
    )
    {
        _httpClient.DefaultRequestHeaders.Authorization = request.Credentials is not null
            ? new AuthenticationHeaderValue("Bearer", request.Credentials.AccessToken)
            : null;

        HttpRequestMessage requestMessage = await PrepareRequestAsync(request, cancellationToken);
        HttpResponseMessage response = await _httpClient.SendAsync(requestMessage, cancellationToken);
        return await OnResponseReceivedAsync(response, cancellationToken);
    }

    /// <summary>
    /// Prepares the given request for fetching the osu! API
    /// </summary>
    /// <param name="request">osu! API request details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A formatted <see cref="HttpRequestMessage"/></returns>
    private async Task<HttpRequestMessage> PrepareRequestAsync(
        IOsuApiRequest request,
        CancellationToken cancellationToken = default
    )
    {
        Uri uri = request.QueryParameters is { Count: > 0 }
            ? new Uri(request.Route + request.QueryParameters.ToQueryString(), UriKind.Relative)
            : request.Route;

        FormUrlEncodedContent? content = request.RequestBody is { Count: > 0 }
            ? new FormUrlEncodedContent(request.RequestBody)
            : null;

        var requestMessage = new HttpRequestMessage
        {
            Method = request.Method,
            RequestUri = uri,
            Content = content
        };

        logger.LogDebug(
            "Preparing to fetch the osu! API [Endpoint: {Endpoint} | Method: {Method}]",
            uri.ToString(),
            request.Method.ToString()
        );

        await RespectRateLimitAsync(cancellationToken);

        return requestMessage;
    }

    /// <summary>
    /// Ensures the client respects the defined rate limits of the osu! API
    /// </summary>
    /// <remarks>See <a href="https://osu.ppy.sh/docs/index.html#terms-of-use">osu! API Terms of Use</a></remarks>
    /// <param name="cancellationToken">Cancellation token</param>
    private async Task RespectRateLimitAsync(CancellationToken cancellationToken = default)
    {
        // Window timer has elapsed, reset the limit
        if (_rateLimit.HasExpired)
        {
            _rateLimit = new FixedWindowRateLimit();
        }

        if (_rateLimit.RemainingTokens <= 0)
        {
            logger.LogWarning(
                "Throttling client for rate limit violation: [Tokens: {Remaining}/{Limit} | Waiting: {Expiry:mm\\:ss}]",
                _rateLimit.RemainingTokens,
                _rateLimit.TokenLimit,
                _rateLimit.ExpiresIn
            );

            // Throttle until the window expires
            await Task.Delay(_rateLimit.ExpiresIn, cancellationToken);
            // Reset the limit
            _rateLimit = new FixedWindowRateLimit();
        }
    }

    /// <summary>
    /// Attempts to update the current rate limit values with header data contained in
    /// a response from the osu! API
    /// </summary>
    /// <param name="response">osu! API response</param>
    /// <param name="cancellationToken">Cancellation token</param>
    // private void UpdateRateLimit(HttpResponseMessage response)
    private void UpdateRateLimit()
    {
        _rateLimit.RemainingTokens -= 1;

        // The osu! API rate limit headers return values based on the burst limit
        // This method is reliable, but we should respect the courtesy limit of 60 r/m
        // See https://osu.ppy.sh/docs/index.html#terms-of-use
        // _rateLimit.TokenLimit =
        //     response.Headers.TryGetValues("X-RateLimit-Limit", out IEnumerable<string>? limitHeaders)
        //         ? int.Parse(limitHeaders.First())
        //         : _rateLimit.TokenLimit;
        // _rateLimit.RemainingTokens =
        //     response.Headers.TryGetValues("X-RateLimit-Remaining", out IEnumerable<string>? remainingHeaders)
        //         ? int.Parse(remainingHeaders.First())
        //         : _rateLimit.RemainingTokens - 1;
        // if (_rateLimit.HasExpired)
        // {
        //     _rateLimit.Created = DateTimeOffset.Now;
        // }

        logger.LogDebug(
            "Rate limit updated [Tokens: {Remaining}/{Limit} | Resets In: {Expiry:mm\\:ss}]",
            _rateLimit.RemainingTokens,
            _rateLimit.TokenLimit,
            _rateLimit.ExpiresIn
        );
    }

    /// <summary>
    /// Performs post-request operations.
    /// Validates the response, updates the rate limit, and reads the response content
    /// </summary>
    /// <param name="response">osu! API response</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Response content as a string, or null if any post-request operations were unsuccessful</returns>
    private async Task<string?> OnResponseReceivedAsync(
        HttpResponseMessage response,
        CancellationToken cancellationToken = default
    )
    {
        UpdateRateLimit();

        // Parse response body
        var responseBytes = await response.Content.ReadAsByteArrayAsync(cancellationToken);
        var responseContent = Encoding.UTF8.GetString(responseBytes);

        logger.LogTrace("Received response from osu! API: {Content}", responseContent);

        // Check validity
        return ResponseIsSuccessful(response, responseContent)
            ? responseContent
            : null;
    }

    /// <summary>
    /// Validates an osu! API response and logs potential errors
    /// </summary>
    /// <param name="response">osu! API response</param>
    /// <returns>True if <see cref="HttpResponseMessage.IsSuccessStatusCode"/> denotes a successful response</returns>
    private bool ResponseIsSuccessful(
        HttpResponseMessage response,
        string responseContent
    )
    {
        if (response.IsSuccessStatusCode)
        {
            return true;
        }

        logger.LogWarning(
            "Error fetching the osu! API [Code: {Code} | Reason: {Reason} | Response: {Response}]",
            response.StatusCode,
            response.ReasonPhrase,
            responseContent.Length > 500
                // Truncate responses > 500 chars
                ? string.Join("", responseContent.Take(500)) + "..."
                : responseContent
        );

        return false;
    }

    /// <summary>
    /// Deserializes a string into the given JSON model
    /// </summary>
    /// <param name="responseContent"><see cref="HttpResponseMessage"/>.Content as a string</param>
    /// <typeparam name="TModel">The desired JSON model</typeparam>
    /// <returns>Response content deserialized into the desired JSON model, or null if not successful</returns>
    private TModel? DeserializeResponseContent<TModel>(string responseContent) where TModel : IJsonModel =>
        _serializer.Deserialize<TModel?>(new JsonTextReader(new StringReader(responseContent)));
}
