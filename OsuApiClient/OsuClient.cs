using Database.Enums;
using Microsoft.Extensions.Logging;
using OsuApiClient.Configurations.Interfaces;
using OsuApiClient.Domain;
using OsuApiClient.Domain.Users;
using OsuApiClient.Net.Authorization;
using OsuApiClient.Net.Constants;
using OsuApiClient.Net.JsonModels;
using OsuApiClient.Net.JsonModels.Users;
using OsuApiClient.Net.Requests;
using OsuApiClient.Net.Requests.RequestHandler;

namespace OsuApiClient;

/// <summary>
/// The default implementation of a client that communicates with the osu! API
/// </summary>
public sealed class OsuClient(IOsuClientConfiguration configuration, ILogger<OsuClient> logger, IRequestHandler handler) : IOsuClient
{
    private readonly IRequestHandler _handler = handler ?? throw new ArgumentNullException(nameof(handler));
    private OsuCredentials? _credentials;

    private bool _disposed;

    /// <summary>
    /// Gets the configuration for the client
    /// </summary>
    public IOsuClientConfiguration Configuration { get; } = configuration ?? throw new ArgumentNullException(nameof(configuration));

    public void Dispose()
    {
        CheckDisposed();

        _disposed = true;
        _handler.Dispose();
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

        Uri.TryCreate(Endpoints.Credentials, UriKind.Relative, out Uri? uri);
        AccessCredentialsModel? response = await _handler
            .FetchAsync<AccessCredentialsModel, AccessCredentialsJsonModel>(new OsuApiRequest
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
            ["redirect_url"] = Configuration.RedirectUrl
        };

        Uri.TryCreate(Endpoints.Credentials, UriKind.Relative, out Uri? uri);
        AccessCredentialsModel? response = await _handler
            .FetchAsync<AccessCredentialsModel, AccessCredentialsJsonModel>(new OsuApiRequest
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

        var endpoint = Endpoints.Me;
        if (ruleset.HasValue)
        {
            endpoint += $"/{ruleset}";
        }

        Uri.TryCreate(endpoint, UriKind.Relative, out Uri? uri);
        return await _handler
            .FetchAsync<UserExtended, UserExtendedJsonModel>(new OsuApiRequest
            {
                Credentials = _credentials,
                Method = HttpMethod.Get,
                Route = uri!
            }, cancellationToken);
    }

    public void ClearCredentials()
    {
        CheckDisposed();

        _credentials = null;
        logger.LogDebug("Cleared current credentials");
    }

    private void CheckDisposed()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(OsuClient), "The client is disposed");
        }
    }
}
