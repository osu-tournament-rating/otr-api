using Database;
using Database.Entities;
using Database.Repositories.Interfaces;
using DataWorkerService.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using OsuApiClient;
using OsuApiClient.Domain.Osu.Beatmaps;
using ApiUser = OsuApiClient.Domain.Osu.Users.User;
using Beatmap = Database.Entities.Beatmap;

namespace DataWorkerService.Services.Implementations;

public class OsuApiDataParserService(IOsuClient osuClient, IBeatmapsRepository beatmapsRepository, OtrContext context) : IOsuApiDataParserService
{
    public async Task<IEnumerable<long>> FetchBeatmapsetIdsAsync(IEnumerable<long> osuBeatmapIds)
    {
        var setIds = new List<long>();
        IEnumerable<long[]> chunks = osuBeatmapIds.Chunk(50);

        foreach (var chunk in chunks)
        {
            IEnumerable<BeatmapExtended>? beatmaps = await osuClient.GetBeatmapsAsync(chunk);

            if (beatmaps is null)
            {
                continue;
            }

            setIds.AddRange(beatmaps.Select(b => b.BeatmapsetId));
        }

        return setIds;
    }

    public async Task ProcessBeatmapsetsAsync(IEnumerable<long> osuBeatmapsetIds)
    {
        Dictionary<long, BeatmapSet> existingSets = await context.BeatmapSets
            .Where(bs => osuBeatmapsetIds.Contains(bs.OsuId))
            .ToDictionaryAsync(k => k.OsuId, v => v);

        foreach (var osuBeatmapsetId in osuBeatmapsetIds)
        {
            if (existingSets.ContainsKey(osuBeatmapsetId))
            {
                return;
            }

            BeatmapsetExtended? beatmapset = await osuClient.GetBeatmapsetAsync(osuBeatmapsetId);
            if (beatmapset is null)
            {
                return;
            }

            IEnumerable<long> osuBeatmapIds = beatmapset.Beatmaps.Select(b => b.Id);
            var beatmapEntities = (await beatmapsRepository.GetAsync(osuBeatmapIds))
                .ToDictionary(k => k.OsuId, v => v);

            foreach (BeatmapExtended apiBeatmap in beatmapset.Beatmaps)
            {
                if (!beatmapEntities.TryGetValue(apiBeatmap.Id, out Beatmap? entity))
                {
                    entity = new Beatmap();
                    LoadBeatmapAsync(entity, apiBeatmap);

                    context.Beatmaps.Add(entity);

                    continue;
                }

                if (entity.HasData)
                {
                    continue;
                }

                LoadBeatmapAsync(entity, apiBeatmap);
            }

            foreach (ApiUser relatedUser in beatmapset.RelatedUsers)
            {

            }
        }
    }

    private static void LoadBeatmapAsync(Beatmap beatmap, BeatmapExtended apiBeatmap)
    {
        beatmap.OsuId = apiBeatmap.Id;
        beatmap.Ruleset = apiBeatmap.Ruleset;
        beatmap.RankedStatus = apiBeatmap.RankedStatus;
        beatmap.DiffName = apiBeatmap.DifficultyName;
        beatmap.TotalLength = apiBeatmap.TotalLength;
        beatmap.DrainLength = apiBeatmap.HitLength;
        beatmap.Bpm = apiBeatmap.Bpm;
        beatmap.CountCircle = apiBeatmap.CountCircles;
        beatmap.CountSlider = apiBeatmap.CountSliders;
        beatmap.CountSpinner = apiBeatmap.CountSpinners;
        beatmap.Cs = apiBeatmap.CircleSize;
        beatmap.Hp = apiBeatmap.HpDrain;
        beatmap.Od = apiBeatmap.OverallDifficulty;
        beatmap.Ar = apiBeatmap.ApproachRate;
        beatmap.Sr = apiBeatmap.StarRating;
        beatmap.MaxCombo = apiBeatmap.MaxCombo;
        beatmap.HasData = true;
    }
}
