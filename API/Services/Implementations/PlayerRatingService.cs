using API.DTOs;
using API.Enums;
using API.Repositories.Interfaces;
using API.Services.Interfaces;
using API.Utilities;
using Database.Entities.Processor;
using Database.Enums;
using Database.Repositories.Interfaces;

namespace API.Services.Implementations;

public class PlayerRatingService(
    IApiPlayerRatingRepository playerRatingRepository,
    IPlayerMatchStatsRepository matchStatsRepository,
    IPlayersRepository playerRepository,
    ITournamentsService tournamentsService
    ) : IPlayerRatingService
{
    public async Task<IEnumerable<PlayerRatingStatsDTO?>> GetAsync(long osuPlayerId)
    {
        var id = await playerRepository.GetIdAsync(osuPlayerId);

        if (!id.HasValue)
        {
            return new List<PlayerRatingStatsDTO?>();
        }

        IEnumerable<PlayerRating> baseStats = await playerRatingRepository.GetForPlayerAsync(osuPlayerId);
        var ret = new List<PlayerRatingStatsDTO?>();

        foreach (PlayerRating stat in baseStats)
        {
            // One per ruleset
            ret.Add(await GetAsync(stat, id.Value, stat.Ruleset));
        }

        return ret;
    }

    public async Task<PlayerRatingStatsDTO?> GetAsync(PlayerRating? currentStats, int playerId, Ruleset ruleset)
    {
        currentStats ??= await playerRatingRepository.GetForPlayerAsync(playerId, ruleset);

        if (currentStats == null)
        {
            return null;
        }

        var matchesPlayed = await matchStatsRepository.CountMatchesPlayedAsync(playerId, ruleset);
        var winRate = await matchStatsRepository.GlobalWinrateAsync(playerId, ruleset);
        var tournamentsPlayed = await tournamentsService.CountPlayedAsync(playerId, ruleset);
        var rankProgress = new RankProgressDTO
        {
            CurrentTier = RatingUtils.GetTier(currentStats.Rating),
            CurrentSubTier = RatingUtils.GetSubTier(currentStats.Rating),
            RatingForNextTier = RatingUtils.GetNextTierRatingDelta(currentStats.Rating),
            RatingForNextMajorTier = RatingUtils.GetNextMajorTierRatingDelta(currentStats.Rating),
            NextMajorTier = RatingUtils.GetNextMajorTier(currentStats.Rating),
            SubTierFillPercentage = RatingUtils.GetNextTierFillPercentage(currentStats.Rating),
            MajorTierFillPercentage = RatingUtils.GetNextMajorTierFillPercentage(currentStats.Rating)
        };

        return new PlayerRatingStatsDTO
        {
            PlayerId = playerId,
            CountryRank = currentStats.CountryRank,
            GlobalRank = currentStats.GlobalRank,
            MatchesPlayed = matchesPlayed,
            Ruleset = ruleset,
            Percentile = currentStats.Percentile,
            Rating = currentStats.Rating,
            Volatility = currentStats.Volatility,
            WinRate = winRate,
            TournamentsPlayed = tournamentsPlayed,
            RankProgress = rankProgress
        };
    }

    public async Task<int> BatchInsertAsync(IEnumerable<PlayerRatingDTO> stats)
    {
        var toInsert = new List<PlayerRating>();
        foreach (PlayerRatingDTO item in stats)
        {
            toInsert.Add(
                new PlayerRating
                {
                    PlayerId = item.PlayerId,
                    Rating = item.Rating,
                    Volatility = item.Volatility,
                    Ruleset = item.Ruleset,
                    Percentile = item.Percentile,
                    GlobalRank = item.GlobalRank,
                    CountryRank = item.CountryRank
                }
            );
        }

        return await playerRatingRepository.BatchInsertAsync(toInsert);
    }

    public async Task<IEnumerable<PlayerRatingStatsDTO?>> GetLeaderboardAsync(
        Ruleset ruleset,
        int page,
        int pageSize,
        LeaderboardChartType chartType,
        LeaderboardFilterDTO filter,
        int? playerId
    )
    {
        IEnumerable<PlayerRating> baseStats = await playerRatingRepository.GetLeaderboardAsync(
            page,
            pageSize,
            ruleset,
            chartType,
            filter,
            playerId
        );

        var leaderboard = new List<PlayerRatingStatsDTO?>();

        foreach (PlayerRating baseStat in baseStats)
        {
            leaderboard.Add(await GetAsync(baseStat, baseStat.PlayerId, ruleset));
        }

        return leaderboard;
    }

    public async Task TruncateAsync() => await playerRatingRepository.TruncateAsync();

    public async Task<int> LeaderboardCountAsync(
        Ruleset requestQueryRuleset,
        LeaderboardChartType requestQueryChartType,
        LeaderboardFilterDTO requestQueryFilter,
        int? playerId
    ) =>
        await playerRatingRepository.LeaderboardCountAsync(
            requestQueryRuleset,
            requestQueryChartType,
            requestQueryFilter,
            playerId
        );

    public async Task<LeaderboardFilterDefaultsDTO> LeaderboardFilterDefaultsAsync(
        Ruleset requestQueryRuleset,
        LeaderboardChartType requestQueryChartType
    ) =>
        new()
        {
            MaxRating = await playerRatingRepository.HighestRatingAsync(requestQueryRuleset),
            MaxMatches = await playerRatingRepository.HighestMatchesAsync(requestQueryRuleset),
            MaxRank = 100_000
        };

    public async Task<IDictionary<int, int>> GetHistogramAsync(Ruleset ruleset) =>
        await playerRatingRepository.GetHistogramAsync(ruleset);
}
