using OsuApiClient.Domain.Osu.Multiplayer;

namespace OsuApiClient.Tests.Tests;

/// <summary>
/// Tests the ability for the client to get multiplayer match data
/// </summary>
public class GetMultiplayerMatchTest(IOsuClient client) : IOsuClientTest
{
    /// <summary>
    /// osu! World Cup 2023 United States vs South Korea (Bracket Reset)
    /// </summary>
    private const long MatchId = 111555364;

    public string Name => "Get Multiplayer Match";

    public async Task<bool> RunAsync(CancellationToken cancellationToken)
    {
        client.ClearCredentials();

        MultiplayerMatch? match = await client.GetMatchAsync(MatchId, cancellationToken);

        return match is not null;
    }
}
