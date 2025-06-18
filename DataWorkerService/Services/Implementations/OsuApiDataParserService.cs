using Common.Enums;
using Database;
using Database.Entities;
using DataWorkerService.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using OsuApiClient;
using OsuApiClient.Domain.Osu.Beatmaps;
using OsuApiClient.Domain.Osu.Multiplayer;
using OsuApiClient.Enums;
using ApiScore = OsuApiClient.Domain.Osu.Multiplayer.GameScore;
using ApiUser = OsuApiClient.Domain.Osu.Users.User;
using Beatmap = Database.Entities.Beatmap;
using Beatmapset = Database.Entities.Beatmapset;
using GameScore = Database.Entities.GameScore;

namespace DataWorkerService.Services.Implementations;

public class OsuApiDataParserService(
    ILogger<OsuApiDataParserService> logger,
    OtrContext context,
    IOsuClient osuClient
) : IOsuApiDataParserService
{
    private const double EzScoreMultiplier = 1.75;

    private readonly Dictionary<long, Beatmapset> _beatmapSetCache = [];
    private readonly Dictionary<long, Beatmap> _beatmapCache = [];
    private readonly Dictionary<long, Player> _playerCache = [];

    public async Task ParseMatchAsync(Match match, MultiplayerMatch apiMatch)
    {
        match.Name = apiMatch.Match.Name;

        await LoadPlayersAsync(apiMatch.Users);
        await ProcessBeatmapsAsync(apiMatch.Events.Select(ev => ev.Game).OfType<MultiplayerGame>().Select(g => g.BeatmapId));

        // Create games
        foreach (MultiplayerGame mpGame in apiMatch.Events.Select(ev => ev.Game).OfType<MultiplayerGame>())
        {
            // Skip games with no scores
            if (mpGame.Scores.Length == 0)
            {
                logger.LogDebug(
                    "Game contains no scores and was likely aborted, skipping [Match osu! Id: {OsuId} | Event Id: {EvId}]",
                    apiMatch.Match.Id,
                    mpGame.Id
                );

                continue;
            }

            _beatmapCache.TryGetValue(mpGame.BeatmapId, out Beatmap? beatmap);

            var game = new Game
            {
                OsuId = mpGame.Id,
                Ruleset = mpGame.Ruleset,
                ScoringType = mpGame.ScoringType,
                TeamType = mpGame.TeamType,
                Mods = mpGame.Mods,
                StartTime = mpGame.StartTime,
                Beatmap = beatmap
            };

            // Create scores
            foreach (ApiScore mpScore in mpGame.Scores)
            {
                if (!_playerCache.TryGetValue(mpScore.UserId, out Player? player))
                {
                    logger.LogWarning(
                        "Expected player to be loaded, skipping score " +
                        "[Match osu! id: {mId} | Game osu! id: {gId} | Player osu! id: {pId}]",
                        apiMatch.Match.Id,
                        mpGame.Id,
                        mpScore.UserId
                    );

                    continue;
                }

                game.Scores.Add(new GameScore
                {
                    // Scale up EZ scores
                    Score = mpScore.Mods.HasFlag(Mods.Easy)
                        ? (int)(mpScore.Score * EzScoreMultiplier)
                        : mpScore.Score,
                    MaxCombo = mpScore.MaxCombo,
                    Count50 = mpScore.Statistics.Count50,
                    Count100 = mpScore.Statistics.Count100,
                    Count300 = mpScore.Statistics.Count300,
                    CountMiss = mpScore.Statistics.CountMiss,
                    CountGeki = mpScore.Statistics.CountGeki,
                    CountKatu = mpScore.Statistics.CountKatu,
                    Pass = mpScore.Passed,
                    Perfect = mpScore.Perfect,
                    Grade = mpScore.Grade,
                    Mods = mpScore.Mods,
                    Ruleset = mpScore.Ruleset,
                    Team = mpScore.SlotInfo.Team,
                    Player = player
                });
            }

            // Determine game end time
            DateTime? gameEndTime = mpGame.EndTime;

            if (game.Beatmap is not null)
            {
                gameEndTime ??= mpGame.StartTime.AddSeconds(game.Beatmap.TotalLength);
            }

            if (gameEndTime.HasValue)
            {
                game.EndTime = gameEndTime.Value;
            }

            match.Games.Add(game);
        }

        // Determine match time borders
        DateTime? matchStartTime = apiMatch.Match.StartTime == DateTime.MinValue ? null : apiMatch.Match.StartTime;
        DateTime? matchEndTime = apiMatch.Match.EndTime;

        // Try to use the timestamp of an event if available
        matchStartTime ??= apiMatch.Events
            .FirstOrDefault(ev => ev.Detail.Type == MultiplayerEventType.MatchCreated)?.Timestamp;
        matchEndTime ??= apiMatch.Events
            .FirstOrDefault(ev => ev.Detail.Type == MultiplayerEventType.MatchDisbanded)?.Timestamp;

        // Try to use times of the games if available
        matchStartTime ??= match.Games.Min(g => g.StartTime);
        matchEndTime ??= match.Games.Max(g => g.EndTime);

        match.StartTime ??= matchStartTime;
        match.EndTime ??= matchEndTime;
    }

    public async Task ProcessBeatmapsAsync(IEnumerable<long> beatmapOsuIds)
    {
        foreach (long beatmapOsuId in beatmapOsuIds)
        {
            // Test cache
            if (_beatmapCache.ContainsKey(beatmapOsuId))
            {
                continue;
            }

            // Test database
            Beatmap? beatmap = await context.Beatmaps
                .Include(b => b.Beatmapset)
                .ThenInclude(bs => bs!.Creator)
                .Include(b => b.Creators)
                .FirstOrDefaultAsync(b => b.OsuId == beatmapOsuId);

            // If one exists already, cache
            if (beatmap is not null)
            {
                _beatmapCache.TryAdd(beatmap.OsuId, beatmap);
                beatmap.Creators.ToList().ForEach(p => _playerCache.TryAdd(p.OsuId, p));
                if (beatmap.Beatmapset is not null)
                {
                    _beatmapSetCache.TryAdd(beatmap.Beatmapset.OsuId, beatmap.Beatmapset);
                    if (beatmap.Beatmapset.Creator is not null)
                    {
                        _playerCache.TryAdd(beatmap.Beatmapset.Creator.OsuId, beatmap.Beatmapset.Creator);
                    }
                }
            }
            else
            {
                beatmap = new Beatmap { OsuId = beatmapOsuId };

                context.Beatmaps.Add(beatmap);
                _beatmapCache.TryAdd(beatmap.OsuId, beatmap);
            }

            if (beatmap.Beatmapset is not null || beatmap.HasData)
            {
                continue;
            }

            // Get the beatmap from the osu! API
            BeatmapExtended? apiBeatmap = await osuClient.GetBeatmapAsync(beatmapOsuId);

            if (apiBeatmap is null)
            {
                continue;
            }

            await ProcessBeatmapsetAsync(apiBeatmap.BeatmapsetId);
        }
    }

    private async Task ProcessBeatmapsetAsync(long beatmapSetOsuId)
    {
        // Test cache
        if (_beatmapSetCache.ContainsKey(beatmapSetOsuId))
        {
            return;
        }

        // Test database
        Beatmapset? beatmapSet = await context.Beatmapsets
            .AsSplitQuery()
            .Include(bs => bs.Creator)
            .Include(bs => bs.Beatmaps)
            .ThenInclude(b => b.Creators)
            .FirstOrDefaultAsync(bs => bs.OsuId == beatmapSetOsuId);

        // If one exists already, cache and continue
        if (beatmapSet is not null)
        {
            _beatmapSetCache.TryAdd(beatmapSet.OsuId, beatmapSet);
            if (beatmapSet.Creator is not null)
            {
                _playerCache.TryAdd(beatmapSet.Creator.OsuId, beatmapSet.Creator);
            }

            beatmapSet.Beatmaps.ToList().ForEach(b =>
            {
                _beatmapCache.TryAdd(b.OsuId, b);
                b.Creators.ToList().ForEach(p => _playerCache.TryAdd(p.OsuId, p));
            });

            return;
        }

        BeatmapsetExtended? apiBeatmapset = await osuClient.GetBeatmapsetAsync(beatmapSetOsuId);

        // Could not fetch the set, create an empty entity
        if (apiBeatmapset is null)
        {
            beatmapSet = new Beatmapset { OsuId = beatmapSetOsuId };

            context.Beatmapsets.Add(beatmapSet);
            _beatmapSetCache.TryAdd(beatmapSet.OsuId, beatmapSet);

            return;
        }

        // Now we parse
        // Load players
        // Filtering because this collection also contains beatmap nominators, etc. and we only want to target mappers
        await LoadPlayersAsync(apiBeatmapset.RelatedUsers.Where(u =>
            u.Id == apiBeatmapset.CreatorId || apiBeatmapset.Beatmaps
                .SelectMany(b => b.Owners)
                .Any(o => o.Id == u.Id))
        );

        // Create new set
        beatmapSet = new Beatmapset
        {
            OsuId = beatmapSetOsuId,
            Artist = apiBeatmapset.Artist,
            Title = apiBeatmapset.Title,
            RankedStatus = apiBeatmapset.RankedStatus,
            RankedDate = apiBeatmapset.RankedDate,
            SubmittedDate = apiBeatmapset.SubmittedDate
        };

        // Assign set creator if possible
        if (apiBeatmapset.CreatorId is not null && _playerCache.TryGetValue(apiBeatmapset.CreatorId.Value, out Player? setCreator))
        {
            beatmapSet.Creator = setCreator;
        }

        // Parse maps
        foreach (BeatmapExtended apiBeatmap in apiBeatmapset.Beatmaps)
        {
            // Test cache
            _beatmapCache.TryGetValue(apiBeatmap.Id, out Beatmap? beatmap);
            // Test database
            beatmap ??= await context.Beatmaps.FirstOrDefaultAsync(b => b.OsuId == apiBeatmap.Id);

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
        context.Beatmapsets.Add(beatmapSet);
    }

    private async Task LoadPlayersAsync(IEnumerable<ApiUser> apiUsers)
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
                    Country = apiUser.CountryCode
                };
                context.Players.Add(player);
            }

            // Cache
            _playerCache.TryAdd(apiUser.Id, player);
        }
    }
}
