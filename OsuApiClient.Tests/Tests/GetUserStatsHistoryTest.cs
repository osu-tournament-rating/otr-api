using Database.Enums;
using OsuApiClient.Domain.OsuTrack;

namespace OsuApiClient.Tests.Tests;

/// <summary>
/// Tests the ability for the client to get user statistics history
/// </summary>
public class GetUserStatsHistoryTest(IOsuClient client) : IOsuClientTest
{
    private const long StageUserId = 8191845;

    public string Name => "Get User Stats History";

    public async Task<bool> RunAsync(CancellationToken cancellationToken)
    {
        IEnumerable<UserStatUpdate>? statsHistory = await client.GetUserStatsHistoryAsync(
            StageUserId,
            Ruleset.Osu,
            DateTime.Today.AddMonths(-1),
            DateTime.Today,
            cancellationToken
        );

        return statsHistory is not null;
    }
}
