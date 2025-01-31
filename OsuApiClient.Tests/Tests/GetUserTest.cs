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
    private const string PeppyUsername = "peppy";

    public string Name => "Get User";

    public async Task<bool> RunAsync(CancellationToken cancellationToken)
    {
        client.ClearCredentials();

        UserExtended? stage = await client.GetUserAsync(StageUserId, null, cancellationToken);
        UserExtended? cytusine = await client.GetUserAsync(CytusineUserId, Ruleset.ManiaOther, cancellationToken);
        UserExtended? peppy = await client.GetUserAsync(PeppyUsername, null, cancellationToken);

        return stage is not null
               && peppy is not null
               && cytusine?.Statistics is not null
               && cytusine.Statistics.Variants.All(v => v.Ruleset is Ruleset.Mania4k or Ruleset.Mania7k);
    }
}
