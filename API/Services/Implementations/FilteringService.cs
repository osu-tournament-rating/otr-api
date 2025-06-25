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

        // Ensure all players exist in database
        Dictionary<long, Player> playersByOsuId = await EnsurePlayersExistAsync(osuIdHashSet);

        // Get player data for filtering
        var playerDtos = (await playerService.GetAsync(osuIdHashSet)).ToList();
        var playerDtosByOsuId = playerDtos.ToDictionary(p => p.OsuId);

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
            MinOsuRank = request.MinOsuRank,
            MaxOsuRank = request.MaxOsuRank,
            PlayersPassed = 0, // Will be updated after processing
            PlayersFailed = 0  // Will be updated after processing
        };

        await filterReportsRepository.CreateAsync(filterReport);

        // Process each player and create FilterReportPlayer records
        var results = new List<PlayerFilteringResultDTO>();
        var filterReportPlayers = new List<FilterReportPlayer>();

        foreach (long osuId in osuIdHashSet)
        {
            Player player = playersByOsuId[osuId];
            PlayerFilteringResultDTO filterResult;

            if (!playerDtosByOsuId.TryGetValue(osuId, out PlayerCompactDTO? playerDto))
            {
                // Player has no data
                filterResult = new PlayerFilteringResultDTO
                {
                    PlayerId = player.Id,
                    Username = player.Username,
                    OsuId = osuId,
                    IsSuccess = false,
                    FailureReason = FilteringFailReason.NoData
                };

                filterReportPlayers.Add(new FilterReportPlayer
                {
                    FilterReportId = filterReport.Id,
                    PlayerId = player.Id,
                    IsSuccess = false,
                    FailureReason = FilteringFailReason.NoData
                });
            }
            else
            {
                // Filter the player
                filterResult = await FilterPlayerAsync(request, playerDto);

                // Get player stats for storing in FilterReportPlayer
                PlayerStatsData playerStats = await GetPlayerStatsAsync(playerDto.Id, request.Ruleset);

                filterReportPlayers.Add(new FilterReportPlayer
                {
                    FilterReportId = filterReport.Id,
                    PlayerId = player.Id,
                    IsSuccess = filterResult.IsSuccess,
                    FailureReason = filterResult.FailureReason,
                    CurrentRating = playerStats.RatingStats?.Rating,
                    TournamentsPlayed = playerStats.RatingStats?.TournamentsPlayed,
                    MatchesPlayed = playerStats.RatingStats?.MatchesPlayed,
                    PeakRating = playerStats.PeakRating,
                    OsuGlobalRank = playerStats.GlobalRank
                });
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

    private async Task<PlayerFilteringResultDTO> FilterPlayerAsync(
        FilteringRequestDTO request,
        PlayerCompactDTO playerInfo)
    {
        var result = new PlayerFilteringResultDTO
        {
            PlayerId = playerInfo.Id,
            Username = playerInfo.Username,
            OsuId = playerInfo.OsuId
        };

        PlayerRatingStatsDTO? ratingStats = await playerRatingsService.GetAsync(
            playerInfo.Id,
            request.Ruleset,
            includeAdjustments: false);

        if (ratingStats is null)
        {
            result.IsSuccess = false;
            result.FailureReason = FilteringFailReason.NoData;
            return result;
        }

        // Populate detailed player data
        result.CurrentRating = ratingStats.Rating;
        result.TournamentsPlayed = ratingStats.TournamentsPlayed;
        result.MatchesPlayed = ratingStats.MatchesPlayed;

        // Get additional player stats
        PlayerStatsData playerStats = await GetPlayerStatsAsync(playerInfo.Id, request.Ruleset);
        result.PeakRating = playerStats.PeakRating;
        result.OsuGlobalRank = playerStats.GlobalRank;

        FilteringFailReason failReason = EnforceFilteringConditions(request, ratingStats, playerStats.PeakRating, playerStats.GlobalRank);
        result.IsSuccess = failReason == FilteringFailReason.None;
        result.FailureReason = failReason == FilteringFailReason.None ? null : failReason;

        return result;
    }

    /// <summary>
    /// Checks all fields of the filter against a player
    /// and applies the appropriate fail reason if needed
    /// </summary>
    /// <param name="request">Filter request</param>
    /// <param name="ratingStats">Rating stats of the player we are checking</param>
    /// <param name="peakRating">The player's all-time peak rating</param>
    /// <param name="globalRank">The player's osu! global rank for the ruleset</param>
    /// <returns></returns>
    private static FilteringFailReason EnforceFilteringConditions(
        FilteringRequestDTO request,
        PlayerRatingStatsDTO ratingStats,
        double peakRating,
        int? globalRank)
    {
        FilteringFailReason failReason = FilteringFailReason.None;

        if (ratingStats.Rating < request.MinRating)
        {
            failReason |= FilteringFailReason.MinRating;
        }

        if (ratingStats.Rating > request.MaxRating)
        {
            failReason |= FilteringFailReason.MaxRating;
        }


        if (ratingStats.TournamentsPlayed < request.TournamentsPlayed)
        {
            failReason |= FilteringFailReason.NotEnoughTournaments;
        }

        if (peakRating > request.PeakRating)
        {
            failReason |= FilteringFailReason.PeakRatingTooHigh;
        }

        if (ratingStats.MatchesPlayed < request.MatchesPlayed)
        {
            failReason |= FilteringFailReason.NotEnoughMatches;
        }

        // Check osu! global rank constraints
        if (request.MinOsuRank.HasValue && (globalRank == null || globalRank < request.MinOsuRank))
        {
            failReason |= FilteringFailReason.MinRank;
        }

        if (request.MaxOsuRank.HasValue && globalRank.HasValue && globalRank > request.MaxOsuRank)
        {
            failReason |= FilteringFailReason.MaxRank;
        }

        return failReason;
    }

    private async Task<Dictionary<long, Player>> EnsurePlayersExistAsync(ImmutableHashSet<long> osuIds)
    {
        var players = new Dictionary<long, Player>(osuIds.Count);

        foreach (long osuId in osuIds)
        {
            Player player = await playersRepository.GetOrCreateAsync(osuId);
            players[osuId] = player;
        }

        return players;
    }

    private async Task<PlayerStatsData> GetPlayerStatsAsync(int playerId, Ruleset ruleset)
    {
        PlayerRatingStatsDTO? ratingStats = await playerRatingsService.GetAsync(
            playerId,
            ruleset,
            includeAdjustments: false);

        double peakRating = await playerStatsService.GetPeakRatingAsync(playerId, ruleset);

        Player? playerWithRulesetData = await playersRepository
            .GetWithIncludesAsync(playerId, p => p.RulesetData);

        int? globalRank = playerWithRulesetData?.RulesetData
            .FirstOrDefault(rd => rd.Ruleset == ruleset)?.GlobalRank;

        return new PlayerStatsData
        {
            RatingStats = ratingStats,
            PeakRating = peakRating,
            GlobalRank = globalRank
        };
    }

    private sealed class PlayerStatsData
    {
        public PlayerRatingStatsDTO? RatingStats { get; init; }
        public double PeakRating { get; init; }
        public int? GlobalRank { get; init; }
    }
}
