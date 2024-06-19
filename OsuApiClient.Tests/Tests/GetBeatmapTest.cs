using System.Diagnostics.CodeAnalysis;
using OsuApiClient.Domain.Beatmaps;

namespace OsuApiClient.Tests.Tests;

/// <summary>
/// Tests the ability for the client to get beatmap data
/// </summary>
[SuppressMessage("ReSharper", "CommentTypo")]
public class GetBeatmapTest(IOsuClient client) : IOsuClientTest
{
    /// <summary>
    /// Will Stetson - Harumachi Clover (Swing Arrangement) [Fiery's Extreme]
    /// </summary>
    private const long BeatmapId = 1893461;

    public string Name => "Get Beatmap";

    public async Task<bool> RunAsync(CancellationToken cancellationToken)
    {
        client.ClearCredentials();

        BeatmapExtended? beatmap = await client.GetBeatmapAsync(BeatmapId, cancellationToken);

        return beatmap is not null;
    }
}
