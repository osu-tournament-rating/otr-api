using Database;
using Database.Entities;
using DataWorkerService.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using OsuApiClient;
using OsuApiClient.Domain.Osu.Beatmaps;
using ApiUser = OsuApiClient.Domain.Osu.Users.User;
using Beatmap = Database.Entities.Beatmap;

namespace DataWorkerService.Services.Implementations;

public class OsuApiDataParserService(
    ILogger<OsuApiDataParserService> logger,
    OtrContext context,
    IOsuClient osuClient
) : IOsuApiDataParserService
{
    private readonly IDictionary<long, BeatmapSet> _beatmapSetCache = new Dictionary<long, BeatmapSet>();
    private readonly IDictionary<long, Beatmap> _beatmapCache = new Dictionary<long, Beatmap>();
    private readonly IDictionary<long, Player> _playerCache = new Dictionary<long, Player>();

    public async Task ProcessBeatmapsAsync(IEnumerable<long> beatmapOsuIds)
    {
        foreach (var beatmapOsuId in beatmapOsuIds)
        {
            // Test cache
            if (_beatmapCache.ContainsKey(beatmapOsuId))
            {
                logger.LogDebug("Beatmap {OsuId}: Cache hit", beatmapOsuId);
                continue;
            }

            // Try to pull an existing beatmap from the database
            Beatmap? beatmap = await context.Beatmaps
                .AsSplitQuery()
                .Include(b => b.BeatmapSet)
                .ThenInclude(bs => bs!.Creator)
                .Include(b => b.Creators)
                .FirstOrDefaultAsync(b => b.OsuId == beatmapOsuId);

            // If one exists already, cache
            if (beatmap is not null)
            {
                logger.LogDebug("Beatmap {OsuId}: Database hit", beatmapOsuId);

                _beatmapCache.TryAdd(beatmap.OsuId, beatmap);
                beatmap.Creators.ToList().ForEach(p => _playerCache.TryAdd(p.OsuId, p));
                if (beatmap.BeatmapSet is not null)
                {
                    _beatmapSetCache.TryAdd(beatmap.BeatmapSet.OsuId, beatmap.BeatmapSet);
                    if (beatmap.BeatmapSet.Creator is not null)
                    {
                        _playerCache.TryAdd(beatmap.BeatmapSet.Creator.OsuId, beatmap.BeatmapSet.Creator);
                    }
                }
            }
            else
            {
                beatmap = new Beatmap { OsuId = beatmapOsuId };

                context.Beatmaps.Add(beatmap);
                _beatmapCache.TryAdd(beatmap.OsuId, beatmap);
            }

            if (beatmap.BeatmapSet is not null || beatmap.HasData)
            {
                continue;
            }

            // Get the beatmap from the osu! API
            BeatmapExtended? apiBeatmap = await osuClient.GetBeatmapAsync(beatmapOsuId);

            if (apiBeatmap is null)
            {
                continue;
            }

            logger.LogDebug("Beatmap {OsuId}: Moving to process set", beatmapOsuId);
            await ProcessBeatmapSetAsync(apiBeatmap.BeatmapsetId);
        }
    }

    public async Task ProcessBeatmapSetAsync(long beatmapSetOsuId)
    {
        // Test cache
        if (_beatmapSetCache.ContainsKey(beatmapSetOsuId))
        {
            logger.LogDebug("Beatmap Set {OsuId}: Cache hit", beatmapSetOsuId);
            return;
        }

        // Try to pull an existing set from the database
        BeatmapSet? beatmapSet = await context.BeatmapSets
            .AsSplitQuery()
            .Include(bs => bs.Creator)
            .Include(bs => bs.Beatmaps)
            .ThenInclude(b => b.Creators)
            .FirstOrDefaultAsync(bs => bs.OsuId == beatmapSetOsuId);

        // If one exists already, cache and continue
        if (beatmapSet is not null)
        {
            logger.LogDebug("Beatmap Set {OsuId}: Database hit", beatmapSetOsuId);

            _beatmapSetCache.TryAdd(beatmapSet.OsuId, beatmapSet);
            if (beatmapSet.Creator is not null)
            {
                _playerCache.Add(beatmapSet.Creator.OsuId, beatmapSet.Creator);
            }

            beatmapSet.Beatmaps.ToList().ForEach(b =>
            {
                _beatmapCache.TryAdd(b.OsuId, b);
                b.Creators.ToList().ForEach(p => _playerCache.TryAdd(p.OsuId, p));
            });

            return;
        }

        BeatmapsetExtended? apiBeatmapSet = await osuClient.GetBeatmapsetAsync(beatmapSetOsuId);

        // Could not fetch the set, create an empty entity
        if (apiBeatmapSet is null)
        {
            logger.LogDebug("Beatmap Set {OsuId}: Fetch failed", beatmapSetOsuId);

            beatmapSet = new BeatmapSet { OsuId = beatmapSetOsuId };

            context.BeatmapSets.Add(beatmapSet);
            _beatmapSetCache.TryAdd(beatmapSet.OsuId, beatmapSet);

            return;
        }

        // Now we parse
        logger.LogDebug("Beatmap Set {OsuId}: Parsing...", beatmapSetOsuId);

        // Load players
        // Filtering because this collection also contains beatmap nominators, etc. and we only want to target mappers
        await LoadPlayersAsync(apiBeatmapSet.RelatedUsers.Where(u =>
            u.Id == apiBeatmapSet.CreatorId || apiBeatmapSet.Beatmaps
                .SelectMany(b => b.Owners)
                .Any(o => o.Id == u.Id))
        );

        // Create new set
        beatmapSet = new BeatmapSet
        {
            OsuId = beatmapSetOsuId,
            Artist = apiBeatmapSet.Artist,
            Title = apiBeatmapSet.Title,
            RankedStatus = apiBeatmapSet.RankedStatus,
            RankedDate = apiBeatmapSet.RankedDate,
            SubmittedDate = apiBeatmapSet.SubmittedDate,
        };

        // Assign set creator if possible
        if (apiBeatmapSet.CreatorId is not null && _playerCache.TryGetValue(apiBeatmapSet.CreatorId.Value, out Player? setCreator))
        {
            beatmapSet.Creator = setCreator;
        }

        // Parse maps
        foreach (BeatmapExtended apiBeatmap in apiBeatmapSet.Beatmaps)
        {
            // Test cache
            _beatmapCache.TryGetValue(apiBeatmap.Id, out Beatmap? beatmap);

            if (beatmap is null)
            {
                beatmap = new Beatmap { OsuId = apiBeatmap.Id };
                _beatmapCache.TryAdd(apiBeatmap.Id, beatmap);
            }

            beatmap.HasData = true;
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

            // Set any owners
            foreach (BeatmapOwner beatmapOwner in apiBeatmap.Owners)
            {
                if (_playerCache.TryGetValue(beatmapOwner.Id, out Player? player))
                {
                    beatmap.Creators.Add(player);
                }
            }

            beatmapSet.Beatmaps.Add(beatmap);
        }

        _beatmapSetCache.TryAdd(beatmapSet.OsuId, beatmapSet);
        context.BeatmapSets.Add(beatmapSet);
    }

    public async Task LoadPlayersAsync(IEnumerable<ApiUser> apiUsers)
    {
        foreach (ApiUser apiUser in apiUsers)
        {
            // Test cache
            if (_playerCache.ContainsKey(apiUser.Id))
            {
                continue;
            }

            // Try to pull an existing player from the database
            Player? player = await context.Players.FirstOrDefaultAsync(p => p.OsuId == apiUser.Id);

            // Create if one doesnt exist
            if (player is null)
            {
                player = new Player
                {
                    OsuId = apiUser.Id,
                    Username = apiUser.Username,
                    Country = apiUser.CountryCode,
                };
                context.Players.Add(player);
            }

            // Cache
            _playerCache.TryAdd(apiUser.Id, player);
        }
    }
}
