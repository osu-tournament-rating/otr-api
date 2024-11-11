using Database.Enums;
using Microsoft.Extensions.Logging;
using OsuApiClient.Configurations.Interfaces;
using OsuApiClient.Domain.Osu;
using OsuApiClient.Domain.Osu.Beatmaps;
using OsuApiClient.Domain.Osu.Multiplayer;
using OsuApiClient.Domain.Osu.Users;
using OsuApiClient.Domain.OsuTrack;
using OsuApiClient.Enums;
using OsuApiClient.Extensions;
using OsuApiClient.Net.Authorization;
using OsuApiClient.Net.Constants;
using OsuApiClient.Net.JsonModels;
using OsuApiClient.Net.JsonModels.Osu.Beatmaps;
using OsuApiClient.Net.JsonModels.Osu.Multiplayer;
using OsuApiClient.Net.JsonModels.Osu.Users;
using OsuApiClient.Net.JsonModels.OsuTrack;
using OsuApiClient.Net.Requests;
using OsuApiClient.Net.Requests.RequestHandler;

namespace OsuApiClient;

/// <summary>
/// The default implementation of a client that communicates with the osu! API
/// </summary>
public sealed class OsuClient(
    IOsuClientConfiguration configuration,
    ILogger<OsuClient> logger,
    IRequestHandler handler
    ) : IOsuClient
{
    private readonly IRequestHandler _handler = handler ?? throw new ArgumentNullException(nameof(handler));
    private OsuCredentials? _credentials;

    private bool _disposed;

    /// <summary>
    /// Gets the configuration for the client
    /// </summary>
    public IOsuClientConfiguration Configuration { get; } =
        configuration ?? throw new ArgumentNullException(nameof(configuration));

    public void Dispose()
    {
        CheckDisposed();

        _disposed = true;
    }

    public async Task<OsuCredentials?> UpdateCredentialsAsync(CancellationToken cancellationToken = default)
    {
        CheckDisposed();

        if (_credentials is { HasExpired: false })
        {
            return _credentials;
        }

        var body = new Dictionary<string, string>
        {
            ["client_id"] = Configuration.ClientId.ToString(),
            ["client_secret"] = Configuration.ClientSecret
        };

        // Requesting credentials for the first time
        if (_credentials is null)
        {
            body.Add("grant_type", "client_credentials");
            body.Add("scope", "public");
        }
        // Refreshing access token
        else if (_credentials.RefreshToken is not null)
        {
            body.Add("grant_type", "refresh_token");
            body.Add("refresh_token", _credentials.RefreshToken);
        }

        Uri.TryCreate(Endpoints.Osu.Credentials, UriKind.Relative, out Uri? uri);
        AccessCredentialsModel? response = await _handler
            .FetchAsync<AccessCredentialsModel, AccessCredentialsJsonModel>(
                new ApiRequest
                {
                    Credentials = _credentials,
                    Method = HttpMethod.Post,
                    Route = uri!,
                    RequestBody = body
                },
                cancellationToken
            );

        if (response is not null)
        {
            OsuCredentials newCredentials = _credentials = new OsuCredentials
            {
                AccessToken = response.AccessToken,
                RefreshToken = response.RefreshToken,
                ExpiresInSeconds = response.ExpiresIn
            };

            logger.LogDebug(
                "Obtained new access credentials [Access Expires In: {Expiry}]",
                newCredentials.ExpiresIn.ToString("g")
            );

            return newCredentials;
        }

        logger.LogWarning("Failed to fetch access credentials");
        return null;
    }

    public async Task<OsuCredentials?> AuthorizeUserWithCodeAsync(
        string authorizationCode,
        CancellationToken cancellationToken = default
    )
    {
        CheckDisposed();

        var body = new Dictionary<string, string>
        {
            ["client_id"] = Configuration.ClientId.ToString(),
            ["client_secret"] = Configuration.ClientSecret,
            ["grant_type"] = "authorization_code",
            ["code"] = authorizationCode,
            ["redirect_uri"] = Configuration.RedirectUrl
        };

        Uri.TryCreate(Endpoints.Osu.Credentials, UriKind.Relative, out Uri? uri);
        AccessCredentialsModel? response = await _handler
            .FetchAsync<AccessCredentialsModel, AccessCredentialsJsonModel>(
                new ApiRequest
                {
                    Credentials = _credentials,
                    Method = HttpMethod.Post,
                    Route = uri!,
                    RequestBody = body
                }, cancellationToken);

        if (response is not null)
        {
            OsuCredentials newCredentials = _credentials = new OsuCredentials
            {
                AccessToken = response.AccessToken,
                RefreshToken = response.RefreshToken,
                ExpiresInSeconds = response.ExpiresIn
            };

            logger.LogDebug(
                "Obtained new access credentials [Access Expires In: {Expiry}]",
                newCredentials.ExpiresIn.ToString("g")
            );

            return newCredentials;
        }

        logger.LogWarning("Failed to fetch access credentials");
        return null;
    }

    public async Task<OsuCredentials?> AuthorizeUserWithTokenAsync(
        string refreshToken,
        CancellationToken cancellationToken = default
    )
    {
        CheckDisposed();

        _credentials = new OsuCredentials { AccessToken = "", RefreshToken = refreshToken, ExpiresInSeconds = 0 };

        return await UpdateCredentialsAsync(cancellationToken);
    }

    public async Task<UserExtended?> GetCurrentUserAsync(
        Ruleset? ruleset = null,
        CancellationToken cancellationToken = default
    )
    {
        CheckDisposed();
        await UpdateCredentialsAsync(cancellationToken);

        if (_credentials is { RefreshToken: null })
        {
            return null;
        }

        var endpoint = Endpoints.Osu.Me;
        if (ruleset.HasValue)
        {
            endpoint += $"/{ruleset}";
        }

        Uri.TryCreate(endpoint, UriKind.Relative, out Uri? uri);
        return await _handler.FetchAsync<UserExtended, UserExtendedJsonModel>(
            new ApiRequest
            {
                Credentials = _credentials,
                Method = HttpMethod.Get,
                Route = uri!
            },
            cancellationToken
        );
    }

    public async Task<UserExtended?> GetUserAsync(
        long id,
        Ruleset? ruleset = null,
        CancellationToken cancellationToken = default
    )
    {
        CheckDisposed();
        await UpdateCredentialsAsync(cancellationToken);

        return await GetUserAsync(id.ToString(), "id", ruleset, cancellationToken);
    }

    public async Task<UserExtended?> GetUserAsync(
        string username,
        Ruleset? ruleset = null,
        CancellationToken cancellationToken = default
    )
    {
        CheckDisposed();
        await UpdateCredentialsAsync(cancellationToken);

        return await GetUserAsync(username, "username", ruleset, cancellationToken);
    }

    public async Task<MultiplayerMatch?> GetPartialMatchAsync(
        long matchId,
        long? eventsBeforeId = null,
        long? eventsAfterId = null,
        int? eventsLimit = null,
        CancellationToken cancellationToken = default
    )
    {
        CheckDisposed();
        await UpdateCredentialsAsync(cancellationToken);

        var queryParams = new Dictionary<string, string>();
        if (eventsBeforeId.HasValue)
        {
            queryParams.Add("before", eventsBeforeId.Value.ToString());
        }

        if (eventsAfterId.HasValue)
        {
            queryParams.Add("after", eventsAfterId.Value.ToString());
        }

        if (eventsLimit.HasValue)
        {
            queryParams.Add("limit", eventsLimit.Value.ToString());
        }

        var endpoint = Endpoints.Osu.Matches + $"/{matchId}";
        Uri.TryCreate(endpoint, UriKind.Relative, out Uri? uri);
        return await _handler.FetchAsync<MultiplayerMatch, MultiplayerMatchJsonModel>(
            new ApiRequest
            {
                Credentials = _credentials,
                Method = HttpMethod.Get,
                Route = uri!,
                QueryParameters = queryParams
            },
            cancellationToken
        );
    }

    public async Task<MultiplayerMatch?> GetMatchAsync(
        long matchId,
        CancellationToken cancellationToken = default
    )
    {
        CheckDisposed();
        await UpdateCredentialsAsync(cancellationToken);

        // Get the first 100 events
        MultiplayerMatch? initialMatch = await GetPartialMatchAsync(
            matchId,
            eventsAfterId: 0,
            cancellationToken: cancellationToken
        );

        if (initialMatch is null)
        {
            return null;
        }

        // osu! API uses 0 as a fallback for null event ids
        // https://github.com/ppy/osu-web/blob/master/app/Http/Controllers/MatchesController.php#L121
        if (initialMatch.LatestEventId == 0)
        {
            logger.LogWarning(
                "Cannot identify the last event id for a match [Match Id: {MatchId}]",
                matchId
            );
            return initialMatch;
        }

        // Get batches of 100 events until we have them all
        while (initialMatch.Events.Max(ev => ev.Id) != initialMatch.LatestEventId)
        {
            var eventsAfterId = initialMatch.Events.Max(ev => ev.Id);
            logger.LogDebug(
                "Attempting to fetch a batch of match events [Match Id: {MatchId} | Most Recent Event: {RecentEvId}]",
                matchId,
                eventsAfterId
            );

            MultiplayerMatch? nextBatch = await GetPartialMatchAsync(
                matchId,
                eventsAfterId: eventsAfterId,
                cancellationToken: cancellationToken
            );

            if (nextBatch is null)
            {
                logger.LogWarning("Failed to fetch the next batch of match events [Match Id: {MatchId}]", matchId);
                break;
            }

            // Add new data
            initialMatch.Events = [.. initialMatch.Events, .. nextBatch.Events];
            initialMatch.Users = initialMatch.Users.Concat(nextBatch.Users).DistinctBy(u => u.Id).ToArray();
        }

        logger.LogDebug(
            "Successfully fetched a full match [Match Id: {MatchId} | Events: {CountEvents}]",
            matchId,
            initialMatch.Events.Length
        );
        return initialMatch;
    }

    public async Task<BeatmapExtended?> GetBeatmapAsync(
        long beatmapId,
        CancellationToken cancellationToken = default
    )
    {
        CheckDisposed();
        await UpdateCredentialsAsync(cancellationToken);

        var endpoint = Endpoints.Osu.Beatmaps + $"/{beatmapId}";
        Uri.TryCreate(endpoint, UriKind.Relative, out Uri? uri);
        return await _handler.FetchAsync<BeatmapExtended, BeatmapExtendedJsonModel>(
            new ApiRequest
            {
                Credentials = _credentials,
                Method = HttpMethod.Get,
                Route = uri!
            },
            cancellationToken
        );
    }

    public async Task<IEnumerable<UserStatUpdate>?> GetUserStatsHistoryAsync(
        long id,
        Ruleset ruleset,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        CancellationToken cancellationToken = default
    )
    {
        CheckDisposed();

        var queryParams = new Dictionary<string, string>
        {
            ["user"] = id.ToString(),
            ["mode"] = ((int)ruleset).ToString()
        };

        if (fromDate.HasValue)
        {
            queryParams.Add("from", fromDate.Value.ToString("yyyy-mm-dd"));
        }

        if (toDate.HasValue)
        {
            queryParams.Add("to", toDate.Value.ToString("yyyy-mm-dd"));
        }

        Uri.TryCreate(Endpoints.OsuTrack.StatsHistory, UriKind.Relative, out Uri? uri);
        return await _handler.FetchEnumerableAsync<UserStatUpdate, UserStatUpdateJsonModel>(
            new ApiRequest
            {
                Platform = FetchPlatform.OsuTrack,
                Method = HttpMethod.Get,
                Route = uri!,
                QueryParameters = queryParams
            },
            cancellationToken
        );
    }

    public async Task<IEnumerable<User>?> GetUserFriendsAsync(CancellationToken cancellationToken = default)
    {
        CheckDisposed();

        if (Uri.TryCreate(Endpoints.Osu.Friends, UriKind.Relative, out Uri? uri))
        {
            return await _handler.FetchEnumerableAsync<User, UserJsonModel>(
                new ApiRequest { Credentials = _credentials, Method = HttpMethod.Get, Route = uri },
                cancellationToken
            );
        }

        logger.LogError("Failed to create Friends URI");
        return null;
    }


    public void ClearCredentials()
    {
        CheckDisposed();

        _credentials = null;
        logger.LogDebug("Cleared current credentials");
    }

    /// <summary>
    /// Helper function for getting a user
    /// </summary>
    /// <param name="identifier">User id as a string or username</param>
    /// <param name="key">"id" or "username"</param>
    /// <param name="ruleset">Ruleset to query for</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A user, or null if the request was unsuccessful</returns>
    private async Task<UserExtended?> GetUserAsync(
        string identifier,
        string key,
        Ruleset? ruleset = null,
        CancellationToken cancellationToken = default
    )
    {
        var queryParams = new Dictionary<string, string> { ["key"] = key };

        var endpoint = Endpoints.Osu.Users + $"/{identifier}";
        if (ruleset.HasValue)
        {
            endpoint += $"/{ruleset.GetDescription()}";
        }

        Uri.TryCreate(endpoint, UriKind.Relative, out Uri? uri);
        return await _handler.FetchAsync<UserExtended, UserExtendedJsonModel>(
            new ApiRequest
            {
                Credentials = _credentials,
                Method = HttpMethod.Get,
                Route = uri!,
                QueryParameters = queryParams
            },
            cancellationToken
        );
    }

    /// <summary>
    /// Ensures requests are not attempted if the client is disposed
    /// </summary>
    /// <exception cref="ObjectDisposedException">Thrown if the client is disposed</exception>
    private void CheckDisposed()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(OsuClient), "The client is disposed");
        }
    }
}
