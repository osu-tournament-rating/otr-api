using Database;
using Database.Entities;
using Database.Repositories.Interfaces;
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
    IBeatmapsRepository beatmapsRepository,
    IBeatmapsetsRepository beatmapsetsRepository,
    IPlayersRepository playersRepository,
    IOsuClient osuClient)
    : IBeatmapsetFetchService
{
    public async Task<bool> FetchAndPersistBeatmapsetByBeatmapIdAsync(long beatmapId, CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Fetching beatmapset for beatmap {BeatmapId}", beatmapId);

        try
        {
            // Check if beatmap already exists and has no data
            Beatmap? existingBeatmap = await beatmapsRepository.GetAsync(beatmapId);

            if (existingBeatmap is not null && !existingBeatmap.HasData)
            {
                logger.LogDebug("Beatmap {BeatmapId} already marked as HasData = false, skipping API calls", beatmapId);
                return false;
            }

            // Fetch the individual beatmap to get the beatmapset ID
            BeatmapExtended? apiBeatmap = await osuClient.GetBeatmapAsync(beatmapId, cancellationToken);

            if (apiBeatmap is null)
            {
                logger.LogWarning("Beatmap {BeatmapId} not found in osu! API", beatmapId);

                // Create or update beatmap with no data flag
                await CreateOrUpdateBeatmapWithNoData(beatmapId);
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

            logger.LogDebug("Successfully processed beatmapset {BeatmapsetId} for beatmap {BeatmapId}", apiBeatmapset.Id, beatmapId);
            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing beatmapset for beatmap {BeatmapId}", beatmapId);
            throw;
        }
    }

    private async Task CreateOrUpdateBeatmapWithNoData(long beatmapId)
    {
        Beatmap? beatmap = await beatmapsRepository.GetAsync(beatmapId);

        if (beatmap is null)
        {
            beatmap = new Beatmap
            {
                OsuId = beatmapId,
                HasData = false
            };
            await beatmapsRepository.CreateAsync(beatmap);
        }
        else
        {
            // If we already have data for this beatmap, keep it and just mark HasData as false
            // This preserves existing data when the API returns null
            beatmap.HasData = false;
            logger.LogDebug("Beatmap {BeatmapId} returned null from API, marking HasData as false", beatmapId);
            await beatmapsRepository.UpdateAsync(beatmap);
        }
    }

    private async Task CreateOrUpdateBeatmapsetWithNoData(long beatmapsetId, CancellationToken cancellationToken)
    {
        Beatmapset? beatmapset = await beatmapsetsRepository.GetWithDetailsAsync(beatmapsetId);

        if (beatmapset is null)
        {
            beatmapset = new Beatmapset { OsuId = beatmapsetId };
            beatmapsetsRepository.Add(beatmapset);
        }

        // Mark all beatmaps in this set as HasData = false
        if (beatmapset.Id > 0)
        {
            await beatmapsetsRepository.MarkBeatmapsAsNoDataAsync(beatmapset.Id);
        }

        await context.SaveChangesAsync(cancellationToken);
    }

    private async Task ProcessBeatmapsetAsync(BeatmapsetExtended apiBeatmapset, CancellationToken cancellationToken)
    {
        // Check if beatmapset already exists
        Beatmapset? beatmapset = await beatmapsetsRepository.GetWithDetailsAsync(apiBeatmapset.Id);

        if (beatmapset is null)
        {
            beatmapset = new Beatmapset { OsuId = apiBeatmapset.Id };
            beatmapsetsRepository.Add(beatmapset);
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
            Player creator = await LoadOrCreatePlayerAsync(apiBeatmapset.User);
            beatmapset.Creator = creator;
        }

        // Process all beatmaps in the set
        foreach (BeatmapExtended apiBeatmap in apiBeatmapset.Beatmaps)
        {
            Beatmap? existingBeatmap = beatmapset.Beatmaps.FirstOrDefault(b => b.OsuId == apiBeatmap.Id);

            if (existingBeatmap is null)
            {
                // Check if beatmap exists in database
                existingBeatmap = await context.Beatmaps
                    .FirstOrDefaultAsync(b => b.OsuId == apiBeatmap.Id, cancellationToken)
                    ?? await beatmapsRepository.GetAsync(apiBeatmap.Id);

                if (existingBeatmap is null)
                {
                    // Create new beatmap with correct beatmapset relationship
                    existingBeatmap = new Beatmap
                    {
                        OsuId = apiBeatmap.Id,
                        BeatmapsetId = beatmapset.Id
                    };
                    beatmapset.Beatmaps.Add(existingBeatmap);
                }
                else if (existingBeatmap.BeatmapsetId != beatmapset.Id)
                {
                    // Beatmap exists but belongs to a different beatmapset
                    // This shouldn't happen in normal circumstances, log a warning
                    logger.LogWarning(
                        "Beatmap {BeatmapId} already exists with BeatmapsetId {ExistingBeatmapsetId}, " +
                        "but API indicates it should belong to BeatmapsetId {ExpectedBeatmapsetId}",
                        apiBeatmap.Id, existingBeatmap.BeatmapsetId, beatmapset.Id);

                    // Still add it to the collection to ensure navigation property is correct
                    beatmapset.Beatmaps.Add(existingBeatmap);
                }
                else
                {
                    // Beatmap exists with correct beatmapset ID, just add to collection
                    beatmapset.Beatmaps.Add(existingBeatmap);
                }
            }

            // Update beatmap from API data
            UpdateBeatmapFromApi(existingBeatmap, apiBeatmap);
        }

        logger.LogDebug("Updated beatmapset {BeatmapsetId} with {BeatmapCount} beatmaps",
            apiBeatmapset.Id, apiBeatmapset.Beatmaps.Length);
    }

    private static void UpdateBeatmapFromApi(Beatmap beatmap, BeatmapExtended apiBeatmap)
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

    private async Task<Player> LoadOrCreatePlayerAsync(ApiUser apiUser)
    {
        Player player = await playersRepository.GetOrCreateAsync(apiUser.Id);

        // Update player data
        player.Username = apiUser.Username;
        player.Country = apiUser.CountryCode;

        if (player.Id == 0)
        {
            // Player is newly created (not yet saved)
            logger.LogDebug("Created new player {PlayerId} ({Username})", apiUser.Id, apiUser.Username);
        }

        await playersRepository.UpdateAsync(player);

        return player;
    }
}
