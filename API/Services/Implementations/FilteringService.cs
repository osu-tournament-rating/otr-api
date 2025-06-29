using System.Collections.Immutable;
using API.DTOs;
using API.Services.Interfaces;
using Common.Enums;
using Database;
using Database.Entities;
using Database.Repositories.Interfaces;

namespace API.Services.Implementations;

/// <summary>
/// Service for filtering players based on specified criteria
/// </summary>
public class FilteringService(
    IPlayerService playerService,
    IPlayerRatingsService playerRatingsService,
    IPlayerStatsService playerStatsService,
    IPlayersRepository playersRepository,
    IFilterReportsRepository filterReportsRepository,
    OtrContext context) : IFilteringService
{
    public async Task<FilteringResultDTO> FilterAsync(FilteringRequestDTO request, int userId)
    {
        var osuIdHashSet = request.OsuPlayerIds.ToImmutableHashSet();
        var playersByOsuId = (await playersRepository.GetAsync(osuIdHashSet)).ToDictionary(k => k.OsuId, v => v);

        // Create the filter report entity
        var filterReport = new FilterReport
        {
            UserId = userId,
            Ruleset = request.Ruleset,
            MinRating = request.MinRating,
            MaxRating = request.MaxRating,
            TournamentsPlayed = request.TournamentsPlayed,
            PeakRating = request.PeakRating,
            MatchesPlayed = request.MatchesPlayed,
            MaxMatchesPlayed = request.MaxMatchesPlayed,
            MaxTournamentsPlayed = request.MaxTournamentsPlayed,
            PlayersPassed = 0, // Will be updated after processing
            PlayersFailed = 0  // Will be updated after processing
        };

        await filterReportsRepository.CreateAsync(filterReport);

        var existingPlayerIds = playersByOsuId.Values.Where(p => p.Id > 0).Select(p => p.Id).ToList();

        // Fetch all data in bulk for existing players
        var playerDtos = (await playerService.GetAsync(osuIdHashSet)).ToList();
        var playerDtosByOsuId = playerDtos.ToDictionary(p => p.OsuId);

        Dictionary<int, PlayerRatingStatsDTO?> ratingStatsByPlayerId = await playerRatingsService.GetAsync(
            existingPlayerIds,
            request.Ruleset,
            includeAdjustments: false
        );

        Dictionary<int, double?> peakRatingsByPlayerId = await playerStatsService.GetPeakRatingsAsync(
            existingPlayerIds,
            request.Ruleset
        );

        // Process each player and create FilterReportPlayer records
        var results = new List<PlayerFilteringResultDTO>();
        var filterReportPlayers = new List<FilterReportPlayer>();

        foreach (long osuId in osuIdHashSet)
        {
            playersByOsuId.TryGetValue(osuId, out Player? player);
            PlayerFilteringResultDTO filterResult;

            // Check if player exists in our database (has a valid ID)
            bool playerExists = player is { Id: > 0 };

            if (playerExists && playerDtosByOsuId.TryGetValue(osuId, out PlayerCompactDTO? playerDto))
            {
                // Player exists - use bulk-fetched data
                PlayerRatingStatsDTO? ratingStats = ratingStatsByPlayerId.GetValueOrDefault(player!.Id);
                double? peakRating = peakRatingsByPlayerId.GetValueOrDefault(player.Id);

                if (ratingStats == null)
                {
                    // Player exists but has no rating data
                    filterResult = ApplyFiltersForPlayerWithNoRating(request, playerDto);

                    filterReportPlayers.Add(new FilterReportPlayer
                    {
                        FilterReportId = filterReport.Id,
                        PlayerId = player.Id,
                        IsSuccess = filterResult.IsSuccess,
                        FailureReason = filterResult.FailureReason,
                        CurrentRating = null,
                        TournamentsPlayed = null,
                        MatchesPlayed = null,
                        PeakRating = null
                    });
                }
                else
                {
                    // Player has rating data - apply normal filters
                    filterResult = new PlayerFilteringResultDTO
                    {
                        PlayerId = playerDto.Id,
                        Username = playerDto.Username,
                        OsuId = playerDto.OsuId,
                        CurrentRating = ratingStats.Rating,
                        TournamentsPlayed = ratingStats.TournamentsPlayed,
                        MatchesPlayed = ratingStats.MatchesPlayed,
                        PeakRating = peakRating
                    };

                    FilteringFailReason failReason = EnforceFilteringConditions(
                        request,
                        ratingStats.Rating,
                        ratingStats.TournamentsPlayed,
                        ratingStats.MatchesPlayed,
                        peakRating
                    );

                    filterResult.FailureReason = failReason == FilteringFailReason.None ? null : failReason;

                    filterReportPlayers.Add(new FilterReportPlayer
                    {
                        FilterReportId = filterReport.Id,
                        PlayerId = player.Id,
                        IsSuccess = filterResult.IsSuccess,
                        FailureReason = filterResult.FailureReason,
                        CurrentRating = ratingStats.Rating,
                        TournamentsPlayed = ratingStats.TournamentsPlayed,
                        MatchesPlayed = ratingStats.MatchesPlayed,
                        PeakRating = peakRating
                    });
                }
            }
            else
            {
                // Player doesn't exist in our system - treat as rating null
                filterResult = ApplyFiltersForNonExistentPlayer(request, player, osuId);

                // Only create FilterReportPlayer records for players that exist in our database
                if (playerExists)
                {
                    filterReportPlayers.Add(new FilterReportPlayer
                    {
                        FilterReportId = filterReport.Id,
                        PlayerId = player!.Id,
                        IsSuccess = filterResult.IsSuccess,
                        FailureReason = filterResult.FailureReason,
                        CurrentRating = null,
                        TournamentsPlayed = null,
                        MatchesPlayed = null,
                        PeakRating = null
                    });
                }
            }

            results.Add(filterResult);
        }

        // Bulk insert all FilterReportPlayer records
        await context.FilterReportPlayers.AddRangeAsync(filterReportPlayers);
        await context.SaveChangesAsync();

        // Update the filter report with counts
        filterReport.PlayersPassed = results.Count(r => r.IsSuccess);
        filterReport.PlayersFailed = results.Count(r => !r.IsSuccess);
        await filterReportsRepository.UpdateAsync(filterReport);

        FilteringResultDTO filteringResult = new()
        {
            PlayersPassed = filterReport.PlayersPassed,
            PlayersFailed = filterReport.PlayersFailed,
            FilteringResults = results,
            FilterReportId = filterReport.Id
        };

        return filteringResult;
    }

    /// <summary>
    /// Apply filters for a player that doesn't exist in our system
    /// Treats them as having 0 rating, 0 tournaments played, 0 matches played
    /// </summary>
    private static PlayerFilteringResultDTO ApplyFiltersForNonExistentPlayer(
        FilteringRequestDTO request,
        Player? player,
        long osuId)
    {
        var result = new PlayerFilteringResultDTO
        {
            PlayerId = player?.Id > 0 ? player.Id : 0,
            Username = player?.Username ?? $"osu! user {osuId}",
            OsuId = osuId,
            CurrentRating = null,
            TournamentsPlayed = null,
            MatchesPlayed = null,
            PeakRating = null
        };

        FilteringFailReason failReason = EnforceFilteringConditions(request, 0, 0, 0, 0);
        result.FailureReason = failReason == FilteringFailReason.None ? null : failReason;

        return result;
    }

    /// <summary>
    /// Apply filters for a player that exists but has no rating data
    /// Treats them as having 0 rating, 0 tournaments played, 0 matches played
    /// </summary>
    private static PlayerFilteringResultDTO ApplyFiltersForPlayerWithNoRating(
        FilteringRequestDTO request,
        PlayerCompactDTO playerDto)
    {
        var result = new PlayerFilteringResultDTO
        {
            PlayerId = playerDto.Id,
            Username = playerDto.Username,
            OsuId = playerDto.OsuId,
            CurrentRating = null,
            TournamentsPlayed = null,
            MatchesPlayed = null,
            PeakRating = null
        };

        FilteringFailReason failReason = EnforceFilteringConditions(request, 0, 0, 0, 0);
        result.FailureReason = failReason == FilteringFailReason.None ? null : failReason;

        return result;
    }

    /// <summary>
    /// Checks all fields of the filter against player stats
    /// and applies the appropriate fail reason if needed
    /// </summary>
    private static FilteringFailReason EnforceFilteringConditions(
        FilteringRequestDTO request,
        double rating,
        int tournamentsPlayed,
        int matchesPlayed,
        double? peakRating)
    {
        FilteringFailReason failReason = FilteringFailReason.None;

        if (request.MinRating.HasValue && rating < request.MinRating.Value)
        {
            failReason |= FilteringFailReason.MinRating;
        }

        if (request.MaxRating.HasValue && rating > request.MaxRating.Value)
        {
            failReason |= FilteringFailReason.MaxRating;
        }

        if (request.TournamentsPlayed.HasValue && tournamentsPlayed < request.TournamentsPlayed.Value)
        {
            failReason |= FilteringFailReason.NotEnoughTournaments;
        }

        if (request.PeakRating.HasValue && peakRating.HasValue && peakRating > request.PeakRating)
        {
            failReason |= FilteringFailReason.PeakRatingTooHigh;
        }

        if (request.MatchesPlayed.HasValue && matchesPlayed < request.MatchesPlayed.Value)
        {
            failReason |= FilteringFailReason.NotEnoughMatches;
        }

        if (request.MaxMatchesPlayed.HasValue && matchesPlayed > request.MaxMatchesPlayed.Value)
        {
            failReason |= FilteringFailReason.TooManyMatches;
        }

        if (request.MaxTournamentsPlayed.HasValue && tournamentsPlayed > request.MaxTournamentsPlayed.Value)
        {
            failReason |= FilteringFailReason.TooManyTournaments;
        }

        return failReason;
    }
}
