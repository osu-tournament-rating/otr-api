using API.DTOs;
using API.Entities;
using API.Enums;
using API.Osu.Enums;
using API.Repositories.Interfaces;
using API.Services.Interfaces;
using API.Utilities;
using AutoMapper;

namespace API.Services.Implementations;

public class PlayerStatsService(
    IBaseStatsService baseStatsService,
    IGameWinRecordsRepository gameWinRecordsRepository,
    IMatchWinRecordRepository matchWinRecordRepository,
    IPlayerMatchStatsRepository matchStatsRepository,
    IPlayerRepository playerRepository,
    IRatingAdjustmentsRepository ratingAdjustmentsRepository,
    IMatchRatingStatsRepository ratingStatsRepository,
    ITournamentsRepository tournamentsRepository,
    IMapper mapper
) : IPlayerStatsService
{
    private readonly IBaseStatsService _baseStatsService = baseStatsService;
    private readonly IGameWinRecordsRepository _gameWinRecordsRepository = gameWinRecordsRepository;
    private readonly IMatchWinRecordRepository _matchWinRecordRepository = matchWinRecordRepository;
    private readonly IPlayerMatchStatsRepository _matchStatsRepository = matchStatsRepository;
    private readonly IPlayerRepository _playerRepository = playerRepository;
    private readonly IRatingAdjustmentsRepository _ratingAdjustmentsRepository = ratingAdjustmentsRepository;
    private readonly IMatchRatingStatsRepository _ratingStatsRepository = ratingStatsRepository;
    private readonly ITournamentsRepository _tournamentsRepository = tournamentsRepository;
    private readonly IMapper _mapper = mapper;

    public async Task<PlayerTeammateComparisonDTO> GetTeammateComparisonAsync(
        int playerId,
        int teammateId,
        int mode,
        DateTime dateMin,
        DateTime dateMax
    )
    {
        var teammateRatingStats = (
            await _ratingStatsRepository.TeammateRatingStatsAsync(
                playerId,
                teammateId,
                mode,
                dateMin,
                dateMax
            )
        ).ToList();
        var teammateMatchStats = (
            await _matchStatsRepository.TeammateStatsAsync(playerId, teammateId, mode, dateMin, dateMax)
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
            await _ratingStatsRepository.OpponentRatingStatsAsync(
                playerId,
                opponentId,
                mode,
                dateMin,
                dateMax
            )
        ).ToList();
        var opponentMatchStats = (
            await _matchStatsRepository.OpponentStatsAsync(playerId, opponentId, mode, dateMin, dateMax)
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
    ) => await _ratingStatsRepository.GetRankChartAsync(playerId, mode, chartType, dateMin, dateMax);

    public async Task<PlayerStatsDTO?> GetAsync(
        string key,
        Ruleset? ruleset = null,
        DateTime? dateMin = null,
        DateTime? dateMax = null
    )
    {
        dateMin ??= DateTime.MinValue;
        dateMax ??= DateTime.MaxValue;

        Player? player = await _playerRepository.GetVersatileAsync(key, true);

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

        PlayerInfoDTO playerInfo = _mapper.Map<PlayerInfoDTO>(player);

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
            await GetTournamentStatsAsync(player.Id, ruleset.Value, dateMin.Value, dateMax.Value);

        PlayerRatingChartDTO ratingChart = await _ratingStatsRepository.GetRatingChartAsync(
            player.Id,
            (int)ruleset.Value,
            dateMin.Value,
            dateMax.Value
        );

        IEnumerable<PlayerFrequencyDTO> frequentTeammates = await _matchWinRecordRepository.GetFrequentTeammatesAsync(
            player.Id,
            (int)ruleset.Value,
            dateMin.Value,
            dateMax.Value
        );

        IEnumerable<PlayerFrequencyDTO> frequentOpponents = await _matchWinRecordRepository.GetFrequentOpponentsAsync(
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

        await _matchStatsRepository.InsertAsync(items);
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

        await _ratingStatsRepository.InsertAsync(items);
    }

    public async Task BatchInsertAsync(IEnumerable<BaseStatsPostDTO> postBody) =>
        await _baseStatsService.BatchInsertAsync(postBody);

    public async Task BatchInsertAsync(IEnumerable<RatingAdjustmentDTO> postBody) =>
        await _ratingAdjustmentsRepository.BatchInsertAsync(postBody);

    public async Task BatchInsertAsync(IEnumerable<GameWinRecordDTO> postBody) =>
        await _gameWinRecordsRepository.BatchInsertAsync(postBody);

    public async Task BatchInsertAsync(IEnumerable<MatchWinRecordDTO> postBody) =>
        await _matchWinRecordRepository.BatchInsertAsync(postBody);

    public async Task TruncateAsync()
    {
        await _baseStatsService.TruncateAsync();
        await _gameWinRecordsRepository.TruncateAsync();
        await _matchStatsRepository.TruncateAsync();
        await _ratingStatsRepository.TruncateAsync();
        await _matchWinRecordRepository.TruncateAsync();
        await _ratingAdjustmentsRepository.TruncateAsync();
    }

    public async Task TruncateRatingAdjustmentsAsync() => await _ratingAdjustmentsRepository.TruncateAsync();

    public async Task<double> GetPeakRatingAsync(int playerId, int mode, DateTime? dateMin = null,
        DateTime? dateMax = null)
    {
        return (await _ratingStatsRepository.GetForPlayerAsync(playerId, mode, dateMin, dateMax))
            .SelectMany(x => x)
            .Max(x => x.RatingAfter);
    }

    // Returns overall stats for the player, no need to filter by date.
    private async Task<BaseStatsDTO?> GetBaseStatsAsync(int playerId, int mode)
    {
        BaseStatsDTO? dto = await _baseStatsService.GetAsync(null, playerId, mode);

        if (dto == null)
        {
            return null;
        }

        var matchesPlayed = await _matchStatsRepository.CountMatchesPlayedAsync(playerId, mode);
        var winRate = await _matchStatsRepository.GlobalWinrateAsync(playerId, mode);
        var highestRank = await _ratingStatsRepository.HighestGlobalRankAsync(playerId, mode);

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
    ) => await _matchStatsRepository.GetModStatsAsync(playerId, mode, dateMin, dateMax);

    /// <summary>
    /// Generate tournament stats for a player
    /// </summary>
    /// <param name="playerId">Id of the player</param>
    /// <param name="ruleset">Ruleset to generate stats for</param>
    /// <param name="dateMin">Date range lower bound</param>
    /// <param name="dateMax">Date range upper bound</param>
    private async Task<PlayerTournamentStatsDTO> GetTournamentStatsAsync(
        int playerId,
        Ruleset ruleset,
        DateTime dateMin,
        DateTime dateMax
    )
    {
        IEnumerable<PlayerTournamentMatchCostDTO> bestPerformances = await _tournamentsRepository.GetPerformancesAsync(
            playerId,
            ruleset,
            dateMin,
            dateMax,
            TournamentPerformanceResultType.Best
        );

        IEnumerable<PlayerTournamentMatchCostDTO> recentPerformances = await _tournamentsRepository.GetPerformancesAsync(
            playerId,
            ruleset,
            dateMin,
            dateMax,
            TournamentPerformanceResultType.Recent
        );

        PlayerTournamentTeamSizeCountDTO counts = await _tournamentsRepository.GetTeamSizeStatsAsync(
            playerId,
            (int)ruleset,
            dateMin,
            dateMax
        );

        return new PlayerTournamentStatsDTO
        {
            TeamSizeCounts = counts,
            BestPerformances = bestPerformances,
            RecentPerformances = recentPerformances
        };
    }

    private async Task<AggregatePlayerMatchStatsDTO?> GetMatchStatsAsync(
        int id,
        int mode,
        DateTime dateMin,
        DateTime dateMax
    )
    {
        var matchStats = (await _matchStatsRepository.GetForPlayerAsync(id, mode, dateMin, dateMax)).ToList();
        IEnumerable<MatchRatingStats> ratingStats =
            (await _ratingStatsRepository.GetForPlayerAsync(id, mode, dateMin, dateMax))
            .SelectMany(x => x)
            .ToList();

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
