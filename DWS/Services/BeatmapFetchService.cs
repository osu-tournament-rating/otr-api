using Database;
using Database.Entities;
using Microsoft.EntityFrameworkCore;
using OsuApiClient;
using OsuApiClient.Domain.Osu.Beatmaps;
using ApiUser = OsuApiClient.Domain.Osu.Users.User;
using Beatmap = Database.Entities.Beatmap;
using Beatmapset = Database.Entities.Beatmapset;

namespace DWS.Services;

public class BeatmapFetchService(
    ILogger<BeatmapFetchService> logger,
    OtrContext context,
    IOsuClient osuClient)
    : IBeatmapFetchService
{
    public async Task<bool> FetchAndPersistBeatmapAsync(long beatmapId, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Fetching beatmap {BeatmapId} from osu! API", beatmapId);

        try
        {
            // Always fetch fresh data from osu! API
            BeatmapExtended? apiBeatmap = await osuClient.GetBeatmapAsync(beatmapId, cancellationToken);

            if (apiBeatmap is null)
            {
                logger.LogWarning("Beatmap {BeatmapId} not found in osu! API", beatmapId);

                // Create or update beatmap with no data flag
                await CreateOrUpdateBeatmapWithNoData(beatmapId, cancellationToken);
                return false;
            }

            // Process the beatmap and its beatmapset
            await ProcessBeatmapAsync(apiBeatmap, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Successfully processed beatmap {BeatmapId}", beatmapId);
            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing beatmap {BeatmapId}", beatmapId);
            throw;
        }
    }

    private async Task CreateOrUpdateBeatmapWithNoData(long beatmapId, CancellationToken cancellationToken)
    {
        Beatmap? beatmap = await context.Beatmaps
            .FirstOrDefaultAsync(b => b.OsuId == beatmapId, cancellationToken);

        if (beatmap is null)
        {
            beatmap = new Beatmap
            {
                OsuId = beatmapId,
                HasData = false
            };
            context.Beatmaps.Add(beatmap);
        }
        else
        {
            beatmap.HasData = false;
        }
    }

    private async Task ProcessBeatmapAsync(BeatmapExtended apiBeatmap, CancellationToken cancellationToken)
    {
        // First process the beatmapset
        Beatmapset? beatmapset = await ProcessBeatmapsetAsync(apiBeatmap.BeatmapsetId, cancellationToken);

        // Find or create the beatmap
        Beatmap? beatmap = await context.Beatmaps
            .Include(b => b.Creators)
            .FirstOrDefaultAsync(b => b.OsuId == apiBeatmap.Id, cancellationToken);

        if (beatmap is null)
        {
            beatmap = new Beatmap { OsuId = apiBeatmap.Id };
            context.Beatmaps.Add(beatmap);
        }

        // Update beatmap data
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
        beatmap.Beatmapset = beatmapset;

        logger.LogDebug("Updated beatmap {BeatmapId} data", apiBeatmap.Id);
    }

    private async Task<Beatmapset?> ProcessBeatmapsetAsync(long beatmapsetId, CancellationToken cancellationToken)
    {
        // Check if beatmapset already exists
        Beatmapset? beatmapset = await context.Beatmapsets
            .Include(bs => bs.Creator)
            .Include(bs => bs.Beatmaps)
            .FirstOrDefaultAsync(bs => bs.OsuId == beatmapsetId, cancellationToken);

        if (beatmapset is not null)
        {
            logger.LogDebug("Beatmapset {BeatmapsetId} already exists, fetching latest data", beatmapsetId);
        }

        // Always fetch fresh beatmapset data
        BeatmapsetExtended? apiBeatmapset = await osuClient.GetBeatmapsetAsync(beatmapsetId, cancellationToken);

        if (apiBeatmapset is null)
        {
            logger.LogWarning("Could not fetch beatmapset {BeatmapsetId} from osu! API", beatmapsetId);

            if (beatmapset is null)
            {
                beatmapset = new Beatmapset { OsuId = beatmapsetId };
                context.Beatmapsets.Add(beatmapset);
            }

            return beatmapset;
        }

        // Create beatmapset if it doesn't exist
        if (beatmapset is null)
        {
            beatmapset = new Beatmapset { OsuId = beatmapsetId };
            context.Beatmapsets.Add(beatmapset);
        }

        // Update beatmapset data
        beatmapset.Artist = apiBeatmapset.Artist;
        beatmapset.Title = apiBeatmapset.Title;
        beatmapset.RankedStatus = apiBeatmapset.RankedStatus;
        beatmapset.RankedDate = apiBeatmapset.RankedDate;
        beatmapset.SubmittedDate = apiBeatmapset.SubmittedDate;

        // Handle creator
        if (apiBeatmapset.User is not null)
        {
            Player creator = await LoadOrCreatePlayerAsync(apiBeatmapset.User, cancellationToken);
            beatmapset.Creator = creator;
        }

        // Process all beatmaps in the set
        foreach (BeatmapExtended apiBeatmap in apiBeatmapset.Beatmaps)
        {
            Beatmap? existingBeatmap = beatmapset.Beatmaps.FirstOrDefault(b => b.OsuId == apiBeatmap.Id);
            existingBeatmap ??= await context.Beatmaps
                    .FirstOrDefaultAsync(b => b.OsuId == apiBeatmap.Id, cancellationToken);

            if (existingBeatmap is null)
            {
                existingBeatmap = new Beatmap { OsuId = apiBeatmap.Id };
                context.Beatmaps.Add(existingBeatmap);
                beatmapset.Beatmaps.Add(existingBeatmap);
            }

            // Update beatmap from beatmapset data
            existingBeatmap.HasData = true;
            existingBeatmap.Ruleset = apiBeatmap.Ruleset;
            existingBeatmap.RankedStatus = apiBeatmap.RankedStatus;
            existingBeatmap.DiffName = apiBeatmap.DifficultyName;
            existingBeatmap.TotalLength = apiBeatmap.TotalLength;
            existingBeatmap.DrainLength = apiBeatmap.HitLength;
            existingBeatmap.Bpm = apiBeatmap.Bpm;
            existingBeatmap.CountCircle = apiBeatmap.CountCircles;
            existingBeatmap.CountSlider = apiBeatmap.CountSliders;
            existingBeatmap.CountSpinner = apiBeatmap.CountSpinners;
            existingBeatmap.Cs = apiBeatmap.CircleSize;
            existingBeatmap.Hp = apiBeatmap.HpDrain;
            existingBeatmap.Od = apiBeatmap.OverallDifficulty;
            existingBeatmap.Ar = apiBeatmap.ApproachRate;
            existingBeatmap.Sr = apiBeatmap.StarRating;
            existingBeatmap.MaxCombo = apiBeatmap.MaxCombo;
        }

        logger.LogDebug("Updated beatmapset {BeatmapsetId} with {BeatmapCount} beatmaps",
            beatmapsetId, apiBeatmapset.Beatmaps.Length);

        return beatmapset;
    }

    private async Task<Player> LoadOrCreatePlayerAsync(ApiUser apiUser, CancellationToken cancellationToken)
    {
        Player? player = await context.Players
            .FirstOrDefaultAsync(p => p.OsuId == apiUser.Id, cancellationToken);

        if (player is null)
        {
            player = new Player
            {
                OsuId = apiUser.Id,
                Username = apiUser.Username,
                Country = apiUser.CountryCode
            };
            context.Players.Add(player);

            logger.LogDebug("Created new player {PlayerId} ({Username})", apiUser.Id, apiUser.Username);
        }
        else
        {
            // Update player data
            player.Username = apiUser.Username;
            player.Country = apiUser.CountryCode;
        }

        return player;
    }
}
