using Common.Enums;
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
    private bool _disposed;

    public OsuCredentials? Credentials { private get; set; }

    public void Dispose()
    {
        CheckDisposed();

        _disposed = true;
    }

    public async Task<OsuCredentials?> UpdateCredentialsAsync(CancellationToken cancellationToken = default)
    {
        CheckDisposed();

        if (Credentials is { HasExpired: false })
        {
            return Credentials;
        }

        var body = new Dictionary<string, string>
        {
            ["client_id"] = configuration.ClientId.ToString(),
            ["client_secret"] = configuration.ClientSecret
        };

        // Requesting credentials for the first time
        if (Credentials is null)
        {
            body.Add("grant_type", "client_credentials");
            body.Add("scope", "public");
        }
        // Refreshing access token
        else if (Credentials.RefreshToken is not null)
        {
            body.Add("grant_type", "refresh_token");
            body.Add("refresh_token", Credentials.RefreshToken);
        }

        Uri.TryCreate(Endpoints.Osu.Credentials, UriKind.Relative, out Uri? uri);
        AccessCredentialsModel? response = await _handler
            .FetchAsync<AccessCredentialsModel, AccessCredentialsJsonModel>(
                new ApiRequest
                {
                    Credentials = Credentials,
                    Method = HttpMethod.Post,
                    Route = uri!,
                    RequestBody = body
                },
                cancellationToken
            );

        if (response is not null)
        {
            Credentials = new OsuCredentials
            {
                AccessToken = response.AccessToken,
                RefreshToken = response.RefreshToken,
                ExpiresInSeconds = response.ExpiresIn
            };

            logger.LogDebug(
                "Obtained new access credentials [Access Expires In: {Expiry}]",
                Credentials.ExpiresIn.ToString("g")
            );

            return Credentials;
        }

        logger.LogWarning("Failed to fetch access credentials");
        return null;
    }

    public async Task<OsuCredentials?> AuthorizeUserWithCodeAsync(
        string authorizationCode,
        string? authorizationCodeVerifier = null,
        CancellationToken cancellationToken = default
    )
    {
        CheckDisposed();

        var body = new Dictionary<string, string>
        {
            ["client_id"] = configuration.ClientId.ToString(),
            ["client_secret"] = configuration.ClientSecret,
            ["grant_type"] = "authorization_code",
            ["code"] = authorizationCode,
            ["redirect_uri"] = configuration.RedirectUrl
        };

        if (!string.IsNullOrEmpty(authorizationCodeVerifier))
        {
            body.Add("code_verifier", authorizationCodeVerifier);
        }

        AccessCredentialsModel? response = await _handler
            .FetchAsync<AccessCredentialsModel, AccessCredentialsJsonModel>(
                new ApiRequest
                {
                    Credentials = Credentials,
                    Method = HttpMethod.Post,
                    Route = new Uri(Endpoints.Osu.Credentials, UriKind.Relative),
                    RequestBody = body
                }, cancellationToken);

        if (response is not null)
        {
            Credentials = new OsuCredentials
            {
                AccessToken = response.AccessToken,
                RefreshToken = response.RefreshToken,
                ExpiresInSeconds = response.ExpiresIn
            };

            logger.LogDebug(
                "Obtained new access credentials [Access Expires In: {Expiry}]",
                Credentials.ExpiresIn.ToString("g")
            );

            return Credentials;
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

        Credentials = new OsuCredentials { AccessToken = string.Empty, RefreshToken = refreshToken, ExpiresInSeconds = 0 };
        return await UpdateCredentialsAsync(cancellationToken);
    }

    public async Task<UserExtended?> GetCurrentUserAsync(
        Ruleset? ruleset = null,
        CancellationToken cancellationToken = default
    )
    {
        CheckDisposed();
        await UpdateCredentialsAsync(cancellationToken);

        if (Credentials is { RefreshToken: null })
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
                Credentials = Credentials,
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
                Credentials = Credentials,
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
            initialMatch.Users = [.. initialMatch.Users.Concat(nextBatch.Users).DistinctBy(u => u.Id)];
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
                Credentials = Credentials,
                Method = HttpMethod.Get,
                Route = uri!
            },
            cancellationToken
        );
    }

    public async Task<IEnumerable<BeatmapExtended>?> GetBeatmapsAsync(ICollection<long> beatmapIds,
        CancellationToken cancellationToken = default)
    {
        CheckDisposed();
        await UpdateCredentialsAsync(cancellationToken);

        return (await _handler.FetchAsync<BeatmapCollection, BeatmapCollectionJsonModel>(
            new ApiRequest
            {
                Credentials = Credentials,
                Method = HttpMethod.Get,
                Route = new Uri(Endpoints.Osu.Beatmaps, UriKind.Relative),
                QueryParameters = beatmapIds
                    .Distinct()
                    .Select((id, index) => new KeyValuePair<string, string>($"ids%5B{index}%5D", id.ToString()))
                    .ToDictionary()
            },
            cancellationToken
        ))?.Beatmaps;
    }

    public async Task<BeatmapAttributes?> GetBeatmapAttributesAsync(
        long beatmapId,
        Mods mods,
        CancellationToken cancellationToken = default
    )
    {
        CheckDisposed();
        await UpdateCredentialsAsync(cancellationToken);

        return await _handler.FetchAsync<BeatmapAttributes, BeatmapAttributesJsonModel>(
            new ApiRequest
            {
                Credentials = Credentials,
                Method = HttpMethod.Get,
                Route = new Uri(string.Format(Endpoints.Osu.BeatmapAttributes, beatmapId), UriKind.Relative),
                QueryParameters = new Dictionary<string, string> { ["mods"] = mods.ToString() }
            },
            cancellationToken
        );
    }

    public async Task<BeatmapsetExtended?> GetBeatmapsetAsync(
        long beatmapsetId,
        CancellationToken cancellationToken = default
    )
    {
        CheckDisposed();
        await UpdateCredentialsAsync(cancellationToken);

        return await _handler.FetchAsync<BeatmapsetExtended, BeatmapsetExtendedJsonModel>(
            new ApiRequest
            {
                Credentials = Credentials,
                Method = HttpMethod.Get,
                Route = new Uri(string.Format(Endpoints.Osu.Beatmapsets, beatmapsetId), UriKind.Relative)
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
        await UpdateCredentialsAsync(cancellationToken);

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

    public void ClearCredentials()
    {
        CheckDisposed();

        Credentials = null;
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
                Credentials = Credentials,
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
