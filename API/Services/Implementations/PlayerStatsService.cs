using API.DTOs;
using API.Enums;
using API.Repositories.Interfaces;
using API.Services.Interfaces;
using API.Utilities;
using AutoMapper;
using Database.Entities;
using Database.Enums;

namespace API.Services.Implementations;

public class PlayerStatsService(
    IBaseStatsService baseStatsService,
    IGameWinRecordsService gameWinRecordsService,
    IMatchWinRecordRepository matchWinRecordRepository,
    IPlayerMatchStatsRepository matchStatsRepository,
    IPlayerRepository playerRepository,
    IRatingAdjustmentsService ratingAdjustmentsService,
    IMatchRatingStatsRepository ratingStatsRepository,
    ITournamentsRepository tournamentsRepository,
    IMapper mapper
) : IPlayerStatsService
{
    public async Task<PlayerTeammateComparisonDTO> GetTeammateComparisonAsync(
        int playerId,
        int teammateId,
        int mode,
        DateTime dateMin,
        DateTime dateMax
    )
    {
        var teammateRatingStats = (
            await ratingStatsRepository.TeammateRatingStatsAsync(
                playerId,
                teammateId,
                mode,
                dateMin,
                dateMax
            )
        ).ToList();
        var teammateMatchStats = (
            await matchStatsRepository.TeammateStatsAsync(playerId, teammateId, mode, dateMin, dateMax)
        ).ToList();

        var matchesPlayed = teammateMatchStats.Count;
        var matchesWon = teammateMatchStats.Sum(x => x.Won ? 1 : 0);
        var matchesLost = teammateMatchStats.Sum(x => x.Won ? 0 : 1);
        var winRate = matchesWon / (double)matchesPlayed;

        return new PlayerTeammateComparisonDTO
        {
            PlayerId = playerId,
            TeammateId = teammateId,
            GamesPlayed = teammateMatchStats.Sum(x => x.GamesPlayed),
            MatchesPlayed = matchesPlayed,
            MatchesWon = matchesWon,
            MatchesLost = matchesLost,
            RatingDelta = teammateRatingStats.Sum(x => x.RatingChange),
            WinRate = winRate
        };
    }

    public async Task<PlayerOpponentComparisonDTO> GetOpponentComparisonAsync(
        int playerId,
        int opponentId,
        int mode,
        DateTime dateMin,
        DateTime dateMax
    )
    {
        var opponentRatingStats = (
            await ratingStatsRepository.OpponentRatingStatsAsync(
                playerId,
                opponentId,
                mode,
                dateMin,
                dateMax
            )
        ).ToList();
        var opponentMatchStats = (
            await matchStatsRepository.OpponentStatsAsync(playerId, opponentId, mode, dateMin, dateMax)
        ).ToList();

        var matchesWon = opponentMatchStats.Sum(x => x.Won ? 1 : 0);
        var matchesPlayed = opponentMatchStats.Count;
        var winRate = matchesWon / (double)matchesPlayed;

        return new PlayerOpponentComparisonDTO
        {
            PlayerId = playerId,
            OpponentId = opponentId,
            GamesPlayed = opponentMatchStats.Sum(x => x.GamesPlayed),
            MatchesPlayed = matchesPlayed,
            MatchesWon = matchesWon,
            MatchesLost = opponentMatchStats.Sum(x => x.Won ? 0 : 1),
            RatingDelta = opponentRatingStats.Sum(x => x.RatingChange),
            WinRate = winRate
        };
    }

    public async Task<PlayerRankChartDTO> GetRankChartAsync(
        int playerId,
        int mode,
        LeaderboardChartType chartType,
        DateTime? dateMin = null,
        DateTime? dateMax = null
    ) => await ratingStatsRepository.GetRankChartAsync(playerId, mode, chartType, dateMin, dateMax);

    public async Task<PlayerStatsDTO?> GetAsync(
        string key,
        Ruleset? ruleset = null,
        DateTime? dateMin = null,
        DateTime? dateMax = null
    )
    {
        dateMin ??= DateTime.MinValue;
        dateMax ??= DateTime.MaxValue;

        Player? player = await playerRepository.GetVersatileAsync(key, true);

        if (player is null)
        {
            return null;
        }

        ruleset ??= player.User?.Settings is not null
            // Use the User's selected ruleset (o!tr)
            ? player.User.Settings.DefaultRuleset
            // Use the Player's selected ruleset (osu!)
            : player.Ruleset
              // Use standard as a fallback
              ?? Ruleset.Standard;

        PlayerInfoDTO playerInfo = mapper.Map<PlayerInfoDTO>(player);

        BaseStatsDTO? baseStats = await GetBaseStatsAsync(player.Id, (int)ruleset!.Value);

        if (baseStats is null)
        {
            return new PlayerStatsDTO { PlayerInfo = playerInfo, Ruleset = ruleset.Value };
        }

        AggregatePlayerMatchStatsDTO? matchStats =
            await GetMatchStatsAsync(player.Id, (int)ruleset.Value, dateMin.Value, dateMax.Value);

        PlayerModStatsDTO modStats =
            await GetModStatsAsync(player.Id, (int)ruleset.Value, dateMin.Value, dateMax.Value);

        PlayerTournamentStatsDTO tournamentStats =
            await GetTournamentStatsAsync(player.Id, (int)ruleset.Value, dateMin.Value, dateMax.Value);

        PlayerRatingChartDTO ratingChart = await ratingStatsRepository.GetRatingChartAsync(
            player.Id,
            (int)ruleset.Value,
            dateMin.Value,
            dateMax.Value
        );

        IEnumerable<PlayerFrequencyDTO> frequentTeammates = await matchWinRecordRepository.GetFrequentTeammatesAsync(
            player.Id,
            (int)ruleset.Value,
            dateMin.Value,
            dateMax.Value
        );

        IEnumerable<PlayerFrequencyDTO> frequentOpponents = await matchWinRecordRepository.GetFrequentOpponentsAsync(
            player.Id,
            (int)ruleset.Value,
            dateMin.Value,
            dateMax.Value
        );

        return new PlayerStatsDTO
        {
            PlayerInfo = playerInfo,
            Ruleset = ruleset.Value,
            BaseStats = baseStats,
            MatchStats = matchStats,
            ModStats = modStats,
            TournamentStats = tournamentStats,
            RatingChart = ratingChart,
            FrequentTeammates = frequentTeammates,
            FrequentOpponents = frequentOpponents
        };
    }

    public async Task BatchInsertAsync(IEnumerable<PlayerMatchStatsDTO> postBody)
    {
        var items = new List<PlayerMatchStats>();

        foreach (PlayerMatchStatsDTO item in postBody)
        {
            var stats = new PlayerMatchStats
            {
                PlayerId = item.PlayerId,
                MatchId = item.MatchId,
                Won = item.Won,
                AverageScore = item.AverageScore,
                AverageMisses = item.AverageMisses,
                AverageAccuracy = item.AverageAccuracy,
                GamesPlayed = item.GamesPlayed,
                AveragePlacement = item.AveragePlacement,
                GamesWon = item.GamesWon,
                GamesLost = item.GamesLost,
                TeammateIds = item.TeammateIds,
                OpponentIds = item.OpponentIds
            };

            items.Add(stats);
        }

        await matchStatsRepository.InsertAsync(items);
    }

    public async Task BatchInsertAsync(IEnumerable<MatchRatingStatsDTO> postBody)
    {
        var items = new List<MatchRatingStats>();
        foreach (MatchRatingStatsDTO item in postBody)
        {
            var stats = new MatchRatingStats
            {
                PlayerId = item.PlayerId,
                MatchId = item.MatchId,
                MatchCost = item.MatchCost,
                RatingBefore = item.RatingBefore,
                RatingAfter = item.RatingAfter,
                RatingChange = item.RatingChange,
                VolatilityBefore = item.VolatilityBefore,
                VolatilityAfter = item.VolatilityAfter,
                VolatilityChange = item.VolatilityChange,
                GlobalRankBefore = item.GlobalRankBefore,
                GlobalRankAfter = item.GlobalRankAfter,
                GlobalRankChange = item.GlobalRankChange,
                CountryRankBefore = item.CountryRankBefore,
                CountryRankAfter = item.CountryRankAfter,
                CountryRankChange = item.CountryRankChange,
                PercentileBefore = item.PercentileBefore,
                PercentileAfter = item.PercentileAfter,
                PercentileChange = item.PercentileChange,
                AverageTeammateRating = item.AverageTeammateRating,
                AverageOpponentRating = item.AverageOpponentRating
            };

            items.Add(stats);
        }

        await ratingStatsRepository.InsertAsync(items);
    }

    public async Task BatchInsertAsync(IEnumerable<BaseStatsPostDTO> postBody) =>
        await baseStatsService.BatchInsertAsync(postBody);

    public async Task BatchInsertAsync(IEnumerable<RatingAdjustmentDTO> postBody) =>
        await ratingAdjustmentsService.BatchInsertAsync(postBody);

    public async Task BatchInsertAsync(IEnumerable<GameWinRecordDTO> postBody) =>
        await gameWinRecordsService.BatchInsertAsync(postBody);

    public async Task BatchInsertAsync(IEnumerable<MatchWinRecordDTO> postBody) =>
        await matchWinRecordRepository.BatchInsertAsync(postBody);

    public async Task TruncateAsync()
    {
        await baseStatsService.TruncateAsync();
        await gameWinRecordsService.TruncateAsync();
        await matchStatsRepository.TruncateAsync();
        await ratingStatsRepository.TruncateAsync();
        await matchWinRecordRepository.TruncateAsync();
        await ratingAdjustmentsService.TruncateAsync();
    }

    public async Task TruncateRatingAdjustmentsAsync() => await ratingAdjustmentsService.TruncateAsync();

    public async Task<double> GetPeakRatingAsync(int playerId, int mode, DateTime? dateMin = null,
        DateTime? dateMax = null)
    {
        return (await ratingStatsRepository.GetForPlayerAsync(playerId, mode, dateMin, dateMax))
            .SelectMany(x => x)
            .Max(x => x.RatingAfter);
    }

    // Returns overall stats for the player, no need to filter by date.
    private async Task<BaseStatsDTO?> GetBaseStatsAsync(int playerId, int mode)
    {
        BaseStatsDTO? dto = await baseStatsService.GetAsync(null, playerId, mode);

        if (dto == null)
        {
            return null;
        }

        var matchesPlayed = await matchStatsRepository.CountMatchesPlayedAsync(playerId, mode);
        var winRate = await matchStatsRepository.GlobalWinrateAsync(playerId, mode);
        var highestRank = await ratingStatsRepository.HighestGlobalRankAsync(playerId, mode);

        dto.MatchesPlayed = matchesPlayed;
        dto.WinRate = winRate;
        dto.HighestGlobalRank = highestRank;

        dto.RankProgress = new RankProgressDTO
        {
            CurrentTier = RatingUtils.GetTier(dto.Rating),
            CurrentSubTier = RatingUtils.GetSubTier(dto.Rating),
            RatingForNextTier = RatingUtils.GetNextTierRatingDelta(dto.Rating),
            RatingForNextMajorTier = RatingUtils.GetNextMajorTierRatingDelta(dto.Rating),
            NextMajorTier = RatingUtils.GetNextMajorTier(dto.Rating),
            SubTierFillPercentage = RatingUtils.GetNextTierFillPercentage(dto.Rating),
            MajorTierFillPercentage = RatingUtils.GetNextMajorTierFillPercentage(dto.Rating)
        };

        return dto;
    }

    public async Task<PlayerModStatsDTO> GetModStatsAsync(
        int playerId,
        int mode,
        DateTime dateMin,
        DateTime dateMax
    ) => await matchStatsRepository.GetModStatsAsync(playerId, mode, dateMin, dateMax);

    private async Task<PlayerTournamentStatsDTO> GetTournamentStatsAsync(
        int playerId,
        int mode,
        DateTime dateMin,
        DateTime dateMax
    )
    {
        const int maxTournaments = 5;

        IEnumerable<PlayerTournamentMatchCostDTO> bestPerformances = await tournamentsRepository.GetPerformancesAsync(
            playerId,
            mode,
            dateMin,
            dateMax,
            maxTournaments, true);

        IEnumerable<PlayerTournamentMatchCostDTO> worstPerformances = await tournamentsRepository.GetPerformancesAsync(
            playerId,
            mode,
            dateMin,
            dateMax,
            maxTournaments, false);

        // Remove any best performances from worst performances
        // ReSharper disable PossibleMultipleEnumeration
        foreach (PlayerTournamentMatchCostDTO performance in bestPerformances)
        {
            worstPerformances = worstPerformances.Where(x => x.TournamentId != performance.TournamentId);
        }

        PlayerTournamentTeamSizeCountDTO counts = await tournamentsRepository.GetTeamSizeStatsAsync(
            playerId,
            mode,
            dateMin,
            dateMax
        );
        return new PlayerTournamentStatsDTO
        {
            TeamSizeCounts = counts,
            BestPerformances = bestPerformances,
            WorstPerformances = worstPerformances
        };
    }

    private async Task<AggregatePlayerMatchStatsDTO?> GetMatchStatsAsync(
        int id,
        int mode,
        DateTime dateMin,
        DateTime dateMax
    )
    {
        var matchStats = (await matchStatsRepository.GetForPlayerAsync(id, mode, dateMin, dateMax)).ToList();
        IEnumerable<MatchRatingStats> ratingStats =
            (await ratingStatsRepository.GetForPlayerAsync(id, mode, dateMin, dateMax))
            .ToList()
            .SelectMany(x => x);

        if (matchStats.Count == 0)
        {
            return new AggregatePlayerMatchStatsDTO();
        }

        return new AggregatePlayerMatchStatsDTO
        {
            AverageMatchCostAggregate = ratingStats.Average(x => x.MatchCost),
            HighestRating = ratingStats.Max(x => x.RatingAfter),
            HighestGlobalRank = ratingStats.Min(x => x.GlobalRankAfter),
            HighestCountryRank = ratingStats.Min(x => x.CountryRankAfter),
            HighestPercentile = ratingStats.Max(x => x.PercentileAfter),
            RatingGained = ratingStats.Last().RatingAfter - ratingStats.First().RatingAfter,
            GamesWon = matchStats.Sum(x => x.GamesWon),
            GamesLost = matchStats.Sum(x => x.GamesLost),
            GamesPlayed = matchStats.Sum(x => x.GamesPlayed),
            MatchesWon = matchStats.Count(x => x.Won),
            MatchesLost = matchStats.Count(x => !x.Won),
            AverageTeammateRating = ratingStats.Average(x => x.AverageTeammateRating),
            AverageOpponentRating = ratingStats.Average(x => x.AverageOpponentRating),
            BestWinStreak = GetHighestWinStreak(matchStats),
            MatchAverageScoreAggregate = matchStats.Average(x => x.AverageScore),
            MatchAverageAccuracyAggregate = matchStats.Average(x => x.AverageAccuracy),
            MatchAverageMissesAggregate = matchStats.Average(x => x.AverageMisses),
            AverageGamesPlayedAggregate = matchStats.Average(x => x.GamesPlayed),
            AveragePlacingAggregate = matchStats.Average(x => x.AveragePlacement),
            PeriodStart = dateMin,
            PeriodEnd = dateMax
        };
    }

    private static int GetHighestWinStreak(IEnumerable<PlayerMatchStats> stats)
    {
        var highest = 0;
        var current = 0;

        foreach (PlayerMatchStats item in stats)
        {
            if (item.Won)
            {
                current++;
            }
            else
            {
                if (current > highest)
                {
                    highest = current;
                }

                current = 0;
            }
        }

        return highest;
    }
}
