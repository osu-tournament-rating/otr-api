using System.Net;
using System.Net.Http.Headers;
using System.Text;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OsuApiClient.Configurations.Interfaces;
using OsuApiClient.Domain;
using OsuApiClient.Enums;
using OsuApiClient.Extensions;
using OsuApiClient.Net.Authorization;
using OsuApiClient.Net.Constants;
using OsuApiClient.Net.JsonModels;
using RedLockNet;
using RedLockNet.SERedis;

namespace OsuApiClient.Net.Requests.RequestHandler;

/// <summary>
/// The default implementation of the handler that makes direct requests to the osu! API
/// </summary>
/// <remarks>
/// Any program which uses the OsuClient MUST have a Redis and RedLock configuration.
/// This class contains resources which are managed by a distributed locker (RedLock).
/// </remarks>
internal sealed class DefaultRequestHandler(
    ILogger<DefaultRequestHandler> logger,
    RedLockFactory redLockFactory,
    IOsuClientConfiguration configuration
) : IRequestHandler
{
    private readonly HttpClient _httpClient = new()
    {
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

    private readonly Dictionary<FetchPlatform, FixedWindowRateLimit> _rateLimits =
        new()
        {
            [FetchPlatform.Osu] = new FixedWindowRateLimit(configuration.OsuRateLimit),
            [FetchPlatform.OsuTrack] = new FixedWindowRateLimit(configuration.OsuTrackRateLimit)
        };

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
        IApiRequest request,
        CancellationToken cancellationToken = default
    ) =>
        await SendRequestAsync(request, cancellationToken);

    public async Task<TJsonModel?> FetchAsync<TJsonModel>(
        IApiRequest request,
        CancellationToken cancellationToken = default
    ) where TJsonModel : class, IJsonModel
    {
        var responseContent = await SendRequestAsync(request, cancellationToken);
        return responseContent is not null
            ? DeserializeResponseContent<TJsonModel>(responseContent)
            : null;
    }

    public async Task<TModel?> FetchAsync<TModel, TJsonModel>(
        IApiRequest request,
        CancellationToken cancellationToken = default
    ) where TModel : class, IModel where TJsonModel : class, IJsonModel
        => _mapper.Map<TModel?>(await FetchAsync<TJsonModel>(request, cancellationToken));

    public async Task<IEnumerable<TModel>?> FetchEnumerableAsync<TModel, TJsonModel>(
        IApiRequest request,
        CancellationToken cancellationToken = default
    ) where TModel : class, IModel where TJsonModel : class, IJsonModel
    {
        var responseContent = await SendRequestAsync(request, cancellationToken);
        return responseContent is not null
            ? _mapper.Map<IEnumerable<TModel>>(DeserializeResponseContent<IEnumerable<TJsonModel>>(responseContent))
            : null;
    }

    /// <summary>
    /// Sends a request
    /// </summary>
    /// <remarks>
    /// Requests targeting the osu! platform are protected by a distributed lock.
    /// </remarks>
    /// <param name="request">Request details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The resulting <see cref="HttpResponseMessage"/> content as a string, or null if not successful</returns>
    private async Task<string?> SendRequestAsync(
        IApiRequest request,
        CancellationToken cancellationToken = default
    )
    {
        const string resource = "osu-api-default-request-handler-send-async";
        var expiry = TimeSpan.FromSeconds(10);
        var wait = TimeSpan.FromSeconds(10);
        var retry = TimeSpan.FromSeconds(1);

        await using IRedLock? redLock = await redLockFactory.CreateLockAsync(resource, expiry, wait, retry);
        try
        {
            // Short-circuit the lock if we're making an osu!track request.
            // This should help with the fact that the DataWorkerService is competing
            // with the API /oauth/authorize endpoint
            if (request.Platform == FetchPlatform.OsuTrack || redLock.IsAcquired)
            {
                HttpRequestMessage requestMessage = await PrepareRequestAsync(request, cancellationToken);
                HttpResponseMessage response = await _httpClient.SendAsync(requestMessage, cancellationToken);

                return await OnResponseReceivedAsync(request.Platform, response, cancellationToken);
            }
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(
                "Http request exception while fetching platform [{platform}]: {ex}",
                request.Platform.ToString(),
                ex
            );
        }

        return null;
    }

    /// <summary>
    /// Formats a <see cref="HttpRequestMessage"/> from the given <see cref="IApiRequest"/>
    /// </summary>
    /// <param name="request">API request details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A formatted <see cref="HttpRequestMessage"/></returns>
    private async Task<HttpRequestMessage> PrepareRequestAsync(
        IApiRequest request,
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
            RequestUri = new Uri(Endpoints.GetBaseUrl(request.Platform) + uri, UriKind.Absolute),
            Content = content,
            Headers =
            {
                Authorization = request.Credentials is not null
                    ? new AuthenticationHeaderValue("Bearer", request.Credentials.AccessToken)
                    : null
            }
        };

        logger.LogTrace(
            "Preparing to fetch [Platform: {Platform} | Endpoint: {Endpoint} | Method: {Method}]",
            request.Platform,
            uri.ToString(),
            request.Method.ToString()
        );

        await RespectRateLimitAsync(request.Platform, cancellationToken);

        return requestMessage;
    }

    /// <summary>
    /// Ensures the client respects the defined rate limits of the API for the given <see cref="FetchPlatform"/>
    /// </summary>
    /// <param name="platform">The platform being fetched</param>
    /// <param name="cancellationToken">Cancellation token</param>
    private async Task RespectRateLimitAsync(
        FetchPlatform platform,
        CancellationToken cancellationToken = default
    )
    {
        FixedWindowRateLimit rateLimit = _rateLimits[platform];

        // Window timer has elapsed, reset the limit
        if (rateLimit.HasExpired)
        {
            rateLimit.Reset();
            logger.LogTrace(
                "Rate limit window elapsed, resetting [Platform: {Platform} | Tokens: {Remaining}/{Limit} " +
                "| Expires In: {Expiry:mm\\:ss}]",
                platform,
                rateLimit.RemainingTokens,
                rateLimit.TokenLimit,
                rateLimit.ExpiresIn
            );
        }

        // Throttle when no remaining tokens
        if (rateLimit.RemainingTokens <= 0)
        {
            logger.LogDebug(
                "Throttling client for rate limit violation [Platform: {Platform} | Tokens: {Remaining}/{Limit} " +
                "| Expires In: {Expiry:mm\\:ss}]",
                platform,
                rateLimit.RemainingTokens,
                rateLimit.TokenLimit,
                rateLimit.ExpiresIn
            );

            // Throttle until the window expires
            await Task.Delay(rateLimit.ExpiresIn, cancellationToken);
            // Reset the limit
            rateLimit.Reset();
        }
    }

    /// <summary>
    /// Updates the rate limit for the given <see cref="FetchPlatform"/>
    /// </summary>
    /// <param name="platform">The platform being fetched</param>
    private void UpdateRateLimit(FetchPlatform platform)
    {
        FixedWindowRateLimit rateLimit = _rateLimits[platform];

        rateLimit.DecrementRemainingTokens();

        logger.LogTrace(
            "Rate limit updated [Platform: {Platform} | Tokens: {Remaining}/{Limit} | Expires In: {Expiry:mm\\:ss}]",
            platform,
            rateLimit.RemainingTokens,
            rateLimit.TokenLimit,
            rateLimit.ExpiresIn
        );
    }

    /// <summary>
    /// Forcefully throttle the client for the specified duration
    /// </summary>
    /// <remarks>Fail-safe in the event we are being rate limited</remarks>
    /// <param name="platform">The platform being fetched</param>
    private async Task ForceRateLimitCooldown(FetchPlatform platform, int durationSeconds = 300, CancellationToken cancellationToken = default)
    {
        FixedWindowRateLimit rateLimit = _rateLimits[platform];
        rateLimit.ClearRemainingTokens();

        logger.LogInformation("Rate limit forcefully cooling down [Platform: {Platform} | Duration: {Duration} seconds]", platform, durationSeconds);

        await RespectRateLimitAsync(platform, cancellationToken);
    }

    /// <summary>
    /// Performs post-request operations for the given <see cref="FetchPlatform"/>.
    /// Validates the response, updates the rate limit, and reads the response content
    /// </summary>
    /// <param name="platform">The platform being fetched</param>
    /// <param name="response">API response</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Response content as a string, or null if any post-request operations were unsuccessful</returns>
    private async Task<string?> OnResponseReceivedAsync(
        FetchPlatform platform,
        HttpResponseMessage response,
        CancellationToken cancellationToken = default
    )
    {
        UpdateRateLimit(platform);

        // Parse response body
        var responseBytes = await response.Content.ReadAsByteArrayAsync(cancellationToken);
        var responseContent = Encoding.UTF8.GetString(responseBytes);

        logger.LogTrace(
            "Received response from {Platform} API: {Content}",
            platform,
            responseContent
        );

        // Check validity
        return await ResponseIsSuccessful(platform, response, responseContent, cancellationToken)
            ? responseContent
            : null;
    }

    /// <summary>
    /// Validates an API response and logs potential errors for the given <see cref="FetchPlatform"/>
    /// </summary>
    /// <param name="platform">The platform being fetched</param>
    /// <param name="response">API response</param>
    /// <returns>True if <see cref="HttpResponseMessage.IsSuccessStatusCode"/> denotes a successful response</returns>
    private async Task<bool> ResponseIsSuccessful(
        FetchPlatform platform,
        HttpResponseMessage response,
        string responseContent,
        CancellationToken cancellationToken = default
    )
    {
        if (response.IsSuccessStatusCode)
        {
            return true;
        }

        logger.LogInformation(
            "Fetch was unsuccessful [Platform: {Platform} | Route: {Route} | Code: {Code} | Reason: {Reason} | Response: {Response}]",
            platform,
            response.RequestMessage?.RequestUri,
            (int)response.StatusCode,
            response.ReasonPhrase,
            responseContent.Length > 500
                // Truncate responses > 500 chars
                ? string.Join("", responseContent.Take(500)) + "..."
                : responseContent
        );

        if (response.StatusCode != HttpStatusCode.TooManyRequests)
        {
            return false;
        }

        await ForceRateLimitCooldown(platform, cancellationToken: cancellationToken);
        return false;
    }

    /// <summary>
    /// Deserializes a string into the given JSON model
    /// </summary>
    /// <param name="responseContent"><see cref="HttpResponseMessage"/>.Content as a string</param>
    /// <typeparam name="TModel">The desired JSON model</typeparam>
    /// <returns>Response content deserialized into the desired JSON model, or null if not successful</returns>
    private TModel? DeserializeResponseContent<TModel>(string responseContent) where TModel : class
    {
        try
        {
            return _serializer.Deserialize<TModel?>(new JsonTextReader(new StringReader(responseContent)));
        }

        catch (Exception ex)
        {
            logger.LogError(
                "{ExceptionType} while processing response of target type [{Type}]: {Exception}",
                ex.GetType().Name,
                nameof(TModel),
                ex
            );
        }

        return null;
    }
}
