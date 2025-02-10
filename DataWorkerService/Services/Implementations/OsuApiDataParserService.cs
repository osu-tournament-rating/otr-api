using Database;
using Database.Entities;
using Database.Enums;
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
        // NOTE: Only used for tournament pooled beatmap processing
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
                continue;
            }

            BeatmapsetExtended? apiBeatmapset = await osuClient.GetBeatmapsetAsync(osuBeatmapsetId);
            if (apiBeatmapset is null)
            {
                var emptySet = new BeatmapSet
                {
                    OsuId = osuBeatmapsetId,
                    RankedStatus = BeatmapRankedStatus.Graveyard
                };
                context.BeatmapSets.Add(emptySet);

                continue;
            }

            // Create new beatmapset and store
            var beatmapset = new BeatmapSet();
            LoadBeatmapset(beatmapset, apiBeatmapset);

            IEnumerable<long> osuUserIds = apiBeatmapset.RelatedUsers.Select(ru => ru.Id);
            IEnumerable<long> osuBeatmapIds = apiBeatmapset.Beatmaps.Select(b => b.Id);

            Dictionary<long, Beatmap> existingBeatmaps = await context.Beatmaps
                .Where(b => osuBeatmapIds.Contains(b.OsuId))
                .ToDictionaryAsync(k => k.OsuId, v => v);

            Dictionary<long, Player> existingPlayers = await context.Players
                .Where(p => osuUserIds.Contains(p.OsuId))
                .ToDictionaryAsync(k => k.OsuId, v => v);

            var newPlayers = new Dictionary<long, Player>();

            foreach (ApiUser relatedUser in apiBeatmapset.RelatedUsers)
            {
                if (!existingPlayers.TryGetValue(relatedUser.Id, out Player? entity))
                {
                    entity = new Player();
                    LoadPlayer(entity, relatedUser);

                    newPlayers.Add(entity.OsuId, entity);

                    continue;
                }

                LoadPlayer(entity, relatedUser);
            }

            foreach (BeatmapExtended apiBeatmap in apiBeatmapset.Beatmaps)
            {
                if (!existingBeatmaps.TryGetValue(apiBeatmap.Id, out Beatmap? entity))
                {
                    // TODO: Fetch player from existing or new players dicts and link to beatmap.
                    if (!existingPlayers.TryGetValue(apiBeatmapset.CreatorId ?? 0, out Player? player))
                    {
                        newPlayers.TryGetValue(apiBeatmapset.CreatorId ?? 0, out player);
                    }

                    entity = new Beatmap
                    {
                        BeatmapSet = beatmapset,
                        Creators = player is null ? [] : [player]
                    };
                    LoadBeatmap(entity, apiBeatmap);

                    context.Beatmaps.Add(entity);
                    continue;
                }

                if (entity.HasData)
                {
                    continue;
                }

                LoadBeatmap(entity, apiBeatmap);
            }

            context.Players.AddRange(newPlayers.Values);
        }
    }

    private static void LoadBeatmap(Beatmap beatmap, BeatmapExtended apiBeatmap)
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

    private static void LoadPlayer(Player player, ApiUser apiUser)
    {
        player.OsuId = apiUser.Id;
        player.Username = apiUser.Username;
        player.Country = apiUser.CountryCode;
        player.Ruleset = Ruleset.Osu;
    }

    private static void LoadBeatmapset(BeatmapSet beatmapset, BeatmapsetExtended apiBeatmapset)
    {
        beatmapset.OsuId = apiBeatmapset.Id;
        beatmapset.Artist = apiBeatmapset.Artist;
        beatmapset.Title = apiBeatmapset.Title;
        beatmapset.RankedStatus = apiBeatmapset.RankedStatus;
        beatmapset.RankedDate = apiBeatmapset.RankedDate;
        beatmapset.SubmittedDate = apiBeatmapset.SubmittedDate;
    }
}
