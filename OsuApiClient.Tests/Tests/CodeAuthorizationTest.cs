using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OsuApiClient.Domain.Users;
using OsuApiClient.Net.Authorization;

namespace OsuApiClient.Tests.Tests;

/// <summary>
/// Tests the ability for the client to obtain user access credentials from an authorization code
/// </summary>
public class CodeAuthorizationTest(
    ILogger<CodeAuthorizationTest> logger,
    IOsuClient client,
    IConfiguration configuration
    ) : IOsuClientTest
{
    public string Name => "Code Authorization";

    public async Task<bool> RunAsync(CancellationToken cancellationToken)
    {
        client.ClearCredentials();
        var authCode = configuration.GetSection("OsuClient").GetValue<string>("AuthCode");

        if (string.IsNullOrEmpty(authCode))
        {
            logger.LogWarning("No authorization code provided, skipping {Name} test", Name);
            return true;
        }

        OsuCredentials? credentials = await client.AuthorizeUserWithCodeAsync(authCode, cancellationToken);

        if (credentials is null)
        {
            return false;
        }

        UserExtended? user = await client.GetCurrentUserAsync(null, cancellationToken);

        return user is not null;
    }
}
