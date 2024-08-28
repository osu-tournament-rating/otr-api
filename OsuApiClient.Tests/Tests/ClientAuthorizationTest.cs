using OsuApiClient.Net.Authorization;

namespace OsuApiClient.Tests.Tests;

/// <summary>
/// Tests the ability for the client to obtain access credentials
/// </summary>
public class ClientAuthorizationTest(IOsuClient client) : IOsuClientTest
{
    public string Name => "Client Authorization";

    public async Task<bool> RunAsync(CancellationToken cancellationToken)
    {
        client.ClearCredentials();
        OsuCredentials? credentials = await client.UpdateCredentialsAsync(cancellationToken);

        return credentials is not null;
    }
}
