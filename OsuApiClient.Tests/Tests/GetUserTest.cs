using System.Diagnostics.CodeAnalysis;
using Database.Enums;
using OsuApiClient.Domain.Osu.Users;

namespace OsuApiClient.Tests.Tests;

/// <summary>
/// Tests the ability for the client to get user data
/// </summary>
[SuppressMessage("ReSharper", "IdentifierTypo")]
public class GetUserTest(IOsuClient client) : IOsuClientTest
{
    private const long StageUserId = 8191845;
    private const long CytusineUserId = 11557554;
    private const string MysstoUsername = "rimjob";

    public string Name => "Get User";

    public async Task<bool> RunAsync(CancellationToken cancellationToken)
    {
        client.ClearCredentials();

        UserExtended? stage = await client.GetUserAsync(StageUserId, null, cancellationToken);
        UserExtended? cytusine = await client.GetUserAsync(CytusineUserId, Ruleset.ManiaOther, cancellationToken);
        UserExtended? myssto = await client.GetUserAsync(MysstoUsername, null, cancellationToken);

        return stage is not null
               && myssto is not null
               && cytusine?.Statistics is not null
               && cytusine.Statistics.Variants.All(v => v.Ruleset is Ruleset.Mania4k or Ruleset.Mania7k);
    }
}
