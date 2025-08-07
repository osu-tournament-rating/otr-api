using AutoMapper;
using Common.Enums;
using Database;
using Database.Entities;
using Database.Repositories.Interfaces;
using DWS.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using OsuApiClient;
using OsuApiClient.Domain.Osu.Beatmaps;
using ApiUser = OsuApiClient.Domain.Osu.Users.User;
using Beatmap = Database.Entities.Beatmap;
using Beatmapset = Database.Entities.Beatmapset;

namespace DWS.Services.Implementations;

public class BeatmapsetFetchService(
    ILogger<BeatmapsetFetchService> logger,
    OtrContext context,
    IBeatmapsRepository beatmapsRepository,
    IBeatmapsetsRepository beatmapsetsRepository,
    IPlayersRepository playersRepository,
    IOsuClient osuClient,
    ITournamentDataCompletionService dataCompletionService,
    IMapper mapper)
    : IBeatmapsetFetchService
{
    public async Task<bool> FetchAndPersistBeatmapsetAsync(long osuBeatmapId, CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Fetching beatmapset for beatmap {BeatmapId}", osuBeatmapId);

        Beatmap? existingBeatmap = null;
        try
        {
            // Check if beatmap already exists and has no data
            existingBeatmap = await beatmapsRepository.GetAsync(osuBeatmapId);

            if (existingBeatmap is not null && !existingBeatmap.HasData)
            {
                logger.LogDebug("Beatmap {BeatmapId} already marked as HasData = false, skipping API calls", osuBeatmapId);
                await dataCompletionService.UpdateBeatmapFetchStatusAsync(existingBeatmap.Id, DataFetchStatus.NotFound, cancellationToken);
                return false;
            }

            // Set status to Fetching if beatmap exists
            if (existingBeatmap is not null)
            {
                existingBeatmap.DataFetchStatus = DataFetchStatus.Fetching;
                await beatmapsRepository.UpdateAsync(existingBeatmap);
                await context.SaveChangesAsync(cancellationToken);
            }

            // Fetch the individual beatmap to get the beatmapset ID
            BeatmapExtended? apiBeatmap = await osuClient.GetBeatmapAsync(osuBeatmapId, cancellationToken);

            if (apiBeatmap is null)
            {
                logger.LogWarning("Beatmap {BeatmapId} not found in osu! API", osuBeatmapId);

                // Create or update beatmap with no data flag
                int beatmapId = await CreateOrUpdateBeatmapWithNoData(osuBeatmapId);
                await dataCompletionService.UpdateBeatmapFetchStatusAsync(beatmapId, DataFetchStatus.NotFound, cancellationToken);
                return false;
            }

            // Fetch the entire beatmapset using the beatmapset ID
            BeatmapsetExtended? apiBeatmapset = await osuClient.GetBeatmapsetAsync(apiBeatmap.BeatmapsetId, cancellationToken);

            if (apiBeatmapset is null)
            {
                logger.LogWarning("Beatmapset {BeatmapsetId} not found in osu! API for beatmap {BeatmapId}",
                    apiBeatmap.BeatmapsetId, osuBeatmapId);

                // Create a minimal beatmapset entry
                await CreateOrUpdateBeatmapsetWithNoData(apiBeatmap.BeatmapsetId, cancellationToken);

                // Mark beatmap as NotFound
                if (existingBeatmap is not null)
                {
                    await dataCompletionService.UpdateBeatmapFetchStatusAsync(existingBeatmap.Id, DataFetchStatus.NotFound, cancellationToken);
                }

                return false;
            }

            // Process the entire beatmapset (includes all beatmaps)
            await ProcessBeatmapsetAsync(apiBeatmapset, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);

            // Mark all beatmaps in this set as Fetched
            List<Beatmap> beatmapsToUpdate = await context.Beatmapsets
                .Where(bs => bs.OsuId == apiBeatmapset.Id)
                .SelectMany(bs => bs.Beatmaps)
                .ToListAsync(cancellationToken);

            foreach (Beatmap beatmap in beatmapsToUpdate)
            {
                await dataCompletionService.UpdateBeatmapFetchStatusAsync(beatmap.Id, DataFetchStatus.Fetched, cancellationToken);
            }

            logger.LogDebug("Successfully processed beatmapset {BeatmapsetId} for beatmap {BeatmapId}", apiBeatmapset.Id, osuBeatmapId);
            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing beatmapset for beatmap {BeatmapId}", osuBeatmapId);

            // Mark as Error if beatmap exists
            if (existingBeatmap is not null)
            {
                await dataCompletionService.UpdateBeatmapFetchStatusAsync(existingBeatmap.Id, DataFetchStatus.Error, cancellationToken);
            }

            throw;
        }
    }

    /// <summary>
    /// Creates a new beatmap marked as having no data, or updates an existing one.
    /// </summary>
    /// <param name="beatmapId">The osu! beatmap ID.</param>
    /// <returns>The database ID of the beatmap.</returns>
    private async Task<int> CreateOrUpdateBeatmapWithNoData(long beatmapId)
    {
        Beatmap? beatmap = await beatmapsRepository.GetAsync(beatmapId);

        if (beatmap is null)
        {
            beatmap = new Beatmap
            {
                OsuId = beatmapId,
                HasData = false,
                DataFetchStatus = DataFetchStatus.NotFound
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

        return beatmap.Id;
    }

    /// <summary>
    /// Creates or updates a beatmapset with no data available.
    /// </summary>
    /// <param name="beatmapsetId">The osu! beatmapset ID.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
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

    /// <summary>
    /// Processes a beatmapset from the API and persists it to the database.
    /// </summary>
    /// <param name="apiBeatmapset">The beatmapset data from the API.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    private async Task ProcessBeatmapsetAsync(BeatmapsetExtended apiBeatmapset, CancellationToken cancellationToken)
    {
        // Get or create beatmapset
        Beatmapset beatmapset = await GetOrCreateBeatmapsetAsync(apiBeatmapset.Id);

        // Update beatmapset data
        mapper.Map(apiBeatmapset, beatmapset);

        // Handle creator
        if (apiBeatmapset.User is not null)
        {
            Player creator = await LoadOrCreatePlayerAsync(apiBeatmapset.User);
            beatmapset.Creator = creator;
        }

        // Process all beatmaps in the set
        await ProcessBeatmapsAsync(beatmapset, apiBeatmapset.Beatmaps, cancellationToken);

        logger.LogDebug("Updated beatmapset {BeatmapsetId} with {BeatmapCount} beatmaps",
            apiBeatmapset.Id, apiBeatmapset.Beatmaps.Length);
    }

    /// <summary>
    /// Gets an existing beatmapset or creates a new one.
    /// </summary>
    /// <param name="beatmapsetId">The osu! beatmapset ID.</param>
    /// <returns>The beatmapset entity.</returns>
    private async Task<Beatmapset> GetOrCreateBeatmapsetAsync(long beatmapsetId)
    {
        Beatmapset? beatmapset = await beatmapsetsRepository.GetWithDetailsAsync(beatmapsetId);

        if (beatmapset is not null)
        {
            return beatmapset;
        }

        beatmapset = new Beatmapset { OsuId = beatmapsetId };
        beatmapsetsRepository.Add(beatmapset);

        return beatmapset;
    }

    /// <summary>
    /// Processes individual beatmaps within a beatmapset.
    /// </summary>
    /// <param name="beatmapset">The parent beatmapset entity.</param>
    /// <param name="apiBeatmaps">The beatmap data from the API.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    private async Task ProcessBeatmapsAsync(Beatmapset beatmapset, BeatmapExtended[] apiBeatmaps, CancellationToken cancellationToken)
    {
        foreach (BeatmapExtended apiBeatmap in apiBeatmaps)
        {
            Beatmap existingBeatmap = await GetOrCreateBeatmapAsync(beatmapset, apiBeatmap, cancellationToken);

            // Update beatmap from API data
            mapper.Map(apiBeatmap, existingBeatmap);
        }
    }

    /// <summary>
    /// Gets an existing beatmap or creates a new one for the beatmapset.
    /// </summary>
    /// <param name="beatmapset">The parent beatmapset entity.</param>
    /// <param name="apiBeatmap">The beatmap data from the API.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>The beatmap entity.</returns>
    private async Task<Beatmap> GetOrCreateBeatmapAsync(Beatmapset beatmapset, BeatmapExtended apiBeatmap, CancellationToken cancellationToken)
    {
        Beatmap? existingBeatmap = beatmapset.Beatmaps.FirstOrDefault(b => b.OsuId == apiBeatmap.Id);

        if (existingBeatmap is not null)
        {
            return existingBeatmap;
        }

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
                    BeatmapsetId = beatmapset.Id,
                    DataFetchStatus = DataFetchStatus.Fetching
                };
            }
            else if (existingBeatmap.BeatmapsetId != beatmapset.Id)
            {
                // Beatmap exists but belongs to a different beatmapset
                // This shouldn't happen in normal circumstances, log a warning
                logger.LogWarning(
                    "Beatmap {BeatmapId} already exists with BeatmapsetId {ExistingBeatmapsetId}, " +
                    "but API indicates it should belong to BeatmapsetId {ExpectedBeatmapsetId}",
                    apiBeatmap.Id, existingBeatmap.BeatmapsetId, beatmapset.Id);
            }

            // Beatmap exists with correct beatmapset ID, just add to collection
            beatmapset.Beatmaps.Add(existingBeatmap);
        }

        return existingBeatmap;
    }


    /// <summary>
    /// Loads an existing player or creates a new one from API data.
    /// </summary>
    /// <param name="apiUser">The user data from the API.</param>
    /// <returns>The player entity.</returns>
    private async Task<Player> LoadOrCreatePlayerAsync(ApiUser apiUser)
    {
        Player player = await playersRepository.GetOrCreateAsync(apiUser.Id);

        // Update player data
        mapper.Map(apiUser, player);

        if (player.Id == 0)
        {
            // Player is newly created (not yet saved)
            logger.LogDebug("Created new player {PlayerId} ({Username})", apiUser.Id, apiUser.Username);
        }

        await playersRepository.UpdateAsync(player);

        return player;
    }
}
