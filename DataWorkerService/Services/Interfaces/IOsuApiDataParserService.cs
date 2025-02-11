namespace DataWorkerService.Services.Interfaces;

/// <summary>
/// Interfaces the <see cref="MatchApiDataParserService"/>
/// </summary>
public interface IOsuApiDataParserService
{
    public Task ProcessBeatmapsAsync(IEnumerable<long> beatmapOsuIds);
}
