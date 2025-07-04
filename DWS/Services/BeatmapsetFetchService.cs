using Database;
using Database.Entities;
using Microsoft.EntityFrameworkCore;
using OsuApiClient;
using OsuApiClient.Domain.Osu.Beatmaps;
using ApiUser = OsuApiClient.Domain.Osu.Users.User;
using Beatmap = Database.Entities.Beatmap;
using Beatmapset = Database.Entities.Beatmapset;

namespace DWS.Services;

public class BeatmapsetFetchService(
    ILogger<BeatmapsetFetchService> logger,
    OtrContext context,
    IOsuClient osuClient)
    : IBeatmapsetFetchService
{
    public async Task<bool> FetchAndPersistBeatmapsetByBeatmapIdAsync(long beatmapId, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Fetching beatmapset for beatmap {BeatmapId}", beatmapId);

        try
        {
            // Check if beatmap already exists and has no data
            Beatmap? existingBeatmap = await context.Beatmaps
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.OsuId == beatmapId, cancellationToken);

            if (existingBeatmap is not null && !existingBeatmap.HasData)
            {
                logger.LogInformation("Beatmap {BeatmapId} already marked as HasData = false, skipping API calls", beatmapId);
                return false;
            }

            // Fetch the individual beatmap to get the beatmapset ID
            BeatmapExtended? apiBeatmap = await osuClient.GetBeatmapAsync(beatmapId, cancellationToken);

            if (apiBeatmap is null)
            {
                logger.LogWarning("Beatmap {BeatmapId} not found in osu! API", beatmapId);

                // Create or update beatmap with no data flag
                await CreateOrUpdateBeatmapWithNoData(beatmapId, cancellationToken);
                return false;
            }

            // Fetch the entire beatmapset using the beatmapset ID
            BeatmapsetExtended? apiBeatmapset = await osuClient.GetBeatmapsetAsync(apiBeatmap.BeatmapsetId, cancellationToken);

            if (apiBeatmapset is null)
            {
                logger.LogWarning("Beatmapset {BeatmapsetId} not found in osu! API for beatmap {BeatmapId}",
                    apiBeatmap.BeatmapsetId, beatmapId);

                // Create a minimal beatmapset entry
                await CreateOrUpdateBeatmapsetWithNoData(apiBeatmap.BeatmapsetId, cancellationToken);
                return false;
            }

            // Process the entire beatmapset (includes all beatmaps)
            await ProcessBeatmapsetAsync(apiBeatmapset, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Successfully processed beatmapset for beatmap {BeatmapId}", beatmapId);
            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing beatmapset for beatmap {BeatmapId}", beatmapId);
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
            // If we already have data for this beatmap, keep it and just mark HasData as false
            // This preserves existing data when the API returns null
            beatmap.HasData = false;
            logger.LogWarning("Beatmap {BeatmapId} returned null from API but we have existing data. Keeping existing data and marking HasData as false", beatmapId);
        }

        await context.SaveChangesAsync(cancellationToken);
    }

    private async Task CreateOrUpdateBeatmapsetWithNoData(long beatmapsetId, CancellationToken cancellationToken)
    {
        Beatmapset? beatmapset = await context.Beatmapsets
            .Include(bs => bs.Beatmaps)
            .FirstOrDefaultAsync(bs => bs.OsuId == beatmapsetId, cancellationToken);

        if (beatmapset is null)
        {
            beatmapset = new Beatmapset { OsuId = beatmapsetId };
            context.Beatmapsets.Add(beatmapset);
        }

        // Mark all beatmaps in this set as HasData = false
        foreach (var beatmap in beatmapset.Beatmaps)
        {
            beatmap.HasData = false;
        }

        await context.SaveChangesAsync(cancellationToken);
    }

    private async Task ProcessBeatmapsetAsync(BeatmapsetExtended apiBeatmapset, CancellationToken cancellationToken)
    {
        // Check if beatmapset already exists
        Beatmapset? beatmapset = await context.Beatmapsets
            .Include(bs => bs.Creator)
            .Include(bs => bs.Beatmaps)
            .FirstOrDefaultAsync(bs => bs.OsuId == apiBeatmapset.Id, cancellationToken);

        if (beatmapset is null)
        {
            beatmapset = new Beatmapset { OsuId = apiBeatmapset.Id };
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

            // Also check if it was added to the context but not yet saved
            existingBeatmap ??= context.ChangeTracker.Entries<Beatmap>()
                .Where(e => e.Entity.OsuId == apiBeatmap.Id)
                .Select(e => e.Entity)
                .FirstOrDefault();

            if (existingBeatmap is null)
            {
                existingBeatmap = new Beatmap { OsuId = apiBeatmap.Id };
                beatmapset.Beatmaps.Add(existingBeatmap);
            }

            // Update beatmap from API data
            UpdateBeatmapFromApi(existingBeatmap, apiBeatmap);
        }

        logger.LogDebug("Updated beatmapset {BeatmapsetId} with {BeatmapCount} beatmaps",
            apiBeatmapset.Id, apiBeatmapset.Beatmaps.Length);
    }

    private void UpdateBeatmapFromApi(Beatmap beatmap, BeatmapExtended apiBeatmap)
    {
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
