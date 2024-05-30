using System.Diagnostics.CodeAnalysis;
using OsuApiClient.Domain.Users;

namespace OsuApiClient.Tests.Tests;

/// <summary>
/// Tests the ability for the client to get user data
/// </summary>
[SuppressMessage("ReSharper", "IdentifierTypo")]
public class GetUserTest(IOsuClient client) : IOsuClientTest
{
    private const string MysstoUsername = "rimjob";
    private const long StageUserId = 8191845;

    public string Name => "Get User";

    public async Task<bool> RunAsync(CancellationToken cancellationToken)
    {
        client.ClearCredentials();

        UserExtended? stage = await client.GetUserAsync(StageUserId, null, cancellationToken);
        UserExtended? myssto = await client.GetUserAsync(MysstoUsername, null, cancellationToken);

        return stage is not null && myssto is not null;
    }
}
