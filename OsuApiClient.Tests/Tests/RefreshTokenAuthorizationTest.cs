using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OsuApiClient.Domain.Osu.Users;
using OsuApiClient.Net.Authorization;

namespace OsuApiClient.Tests.Tests;

/// <summary>
/// Tests the ability for the client to obtain user access credentials from a refresh token
/// </summary>
public class RefreshTokenAuthorizationTest(
    ILogger<RefreshTokenAuthorizationTest> logger,
    IOsuClient client,
    IConfiguration configuration
    ) : IOsuClientTest
{
    public string Name => "Refresh Token Authorization";

    public async Task<bool> RunAsync(CancellationToken cancellationToken)
    {
        client.ClearCredentials();
        string? refreshToken = configuration.GetSection("OsuClient").GetValue<string>("RefreshToken");

        if (string.IsNullOrEmpty(refreshToken))
        {
            logger.LogWarning("No refresh token provided, skipping {Name} test", Name);
            return true;
        }

        OsuCredentials? credentials = await client.AuthorizeUserWithTokenAsync(
            refreshToken,
            cancellationToken
        );

        if (credentials is null)
        {
            return false;
        }

        UserExtended? user = await client.GetCurrentUserAsync(null, cancellationToken);

        return user is not null;
    }
}
