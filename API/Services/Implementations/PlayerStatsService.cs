using API.DTOs;
using API.Enums;
using API.Repositories.Interfaces;
using API.Services.Interfaces;
using API.Utilities;
using AutoMapper;
using Database.Entities;
using Database.Enums;
using Database.Repositories.Interfaces;

namespace API.Services.Implementations;

public class PlayerStatsService(
    IPlayerRatingsService playerRatingsService,
    IApiMatchRosterRepository matchRosterRepository,
    IApiPlayerMatchStatsRepository matchStatsRepository,
    IPlayersRepository playerRepository,
    IApiMatchRatingStatsRepository ratingStatsRepository,
    IApiTournamentsRepository tournamentsRepository,
    IMapper mapper
) : IPlayerStatsService
{
    public async Task<PlayerTeammateComparisonDTO> GetTeammateComparisonAsync(
        int playerId,
        int teammateId,
        Ruleset ruleset,
        DateTime dateMin,
        DateTime dateMax
    )
    {
        var teammateRatingStats = (
            await ratingStatsRepository.TeammateRatingStatsAsync(
                playerId,
                teammateId,
                ruleset,
                dateMin,
                dateMax
            )
        ).ToList();
        var teammateMatchStats = (
            await matchStatsRepository.TeammateStatsAsync(playerId, teammateId, ruleset, dateMin, dateMax)
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
            RatingDelta = teammateRatingStats.Sum(x => x.RatingDelta),
            WinRate = winRate
        };
    }

    public async Task<PlayerOpponentComparisonDTO> GetOpponentComparisonAsync(
        int playerId,
        int opponentId,
        Ruleset ruleset,
        DateTime dateMin,
        DateTime dateMax
    )
    {
        var opponentRatingStats = (
            await ratingStatsRepository.OpponentRatingStatsAsync(
                playerId,
                opponentId,
                ruleset,
                dateMin,
                dateMax
            )
        ).ToList();
        var opponentMatchStats = (
            await matchStatsRepository.OpponentStatsAsync(playerId, opponentId, ruleset, dateMin, dateMax)
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
            RatingDelta = opponentRatingStats.Sum(x => x.RatingDelta),
            WinRate = winRate
        };
    }

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

        ruleset ??= player.User?.Settings.DefaultRuleset ?? player.DefaultRuleset;

        PlayerCompactDTO playerInfo = mapper.Map<PlayerCompactDTO>(player);

        PlayerRatingStatsDTO? ratingStats = await GetCurrentAsync(player.Id, ruleset.Value);

        if (ratingStats is null)
        {
            return new PlayerStatsDTO { PlayerInfo = playerInfo, Ruleset = ruleset.Value };
        }

        AggregatePlayerMatchStatsDTO? matchStats =
            await GetMatchStatsAsync(player.Id, ruleset.Value, dateMin.Value, dateMax.Value);

        PlayerModStatsDTO modStats =
            await GetModStatsAsync(player.Id, ruleset.Value, dateMin.Value, dateMax.Value);

        PlayerTournamentStatsDTO tournamentStats =
            await GetTournamentStatsAsync(player.Id, ruleset.Value, dateMin.Value, dateMax.Value);

        PlayerRatingChartDTO ratingChart = await ratingStatsRepository.GetRatingChartAsync(
            player.Id,
            ruleset.Value,
            dateMin.Value,
            dateMax.Value
        );

        IEnumerable<PlayerFrequencyDTO> frequentTeammates = await matchRosterRepository.GetFrequentTeammatesAsync(
            player.Id,
            ruleset.Value,
            dateMin.Value,
            dateMax.Value
        );

        IEnumerable<PlayerFrequencyDTO> frequentOpponents = await matchRosterRepository.GetFrequentOpponentsAsync(
            player.Id,
            ruleset.Value,
            dateMin.Value,
            dateMax.Value
        );

        return new PlayerStatsDTO
        {
            PlayerInfo = playerInfo,
            Ruleset = ruleset.Value,
            Rating = ratingStats,
            MatchStats = matchStats,
            ModStats = modStats,
            TournamentStats = tournamentStats,
            RatingChart = ratingChart,
            FrequentTeammates = frequentTeammates,
            FrequentOpponents = frequentOpponents
        };
    }

    public async Task<double> GetPeakRatingAsync(int playerId, Ruleset ruleset, DateTime? dateMin = null,
        DateTime? dateMax = null)
    {
        return (await ratingStatsRepository.GetForPlayerAsync(playerId, ruleset, dateMin, dateMax))
            .Max(ra => ra.RatingAfter);
    }

    private async Task<PlayerRatingStatsDTO?> GetCurrentAsync(int playerId, Ruleset ruleset)
    {
        PlayerRatingStatsDTO? ratingStats = await playerRatingsService.GetAsync(null, playerId, ruleset);

        if (ratingStats == null)
        {
            return null;
        }

        var matchesPlayed = await matchStatsRepository.CountMatchesPlayedAsync(playerId, ruleset);
        var winRate = await matchStatsRepository.GlobalWinrateAsync(playerId, ruleset);

        ratingStats.MatchesPlayed = matchesPlayed;
        ratingStats.WinRate = winRate;

        ratingStats.RankProgress = new RankProgressDTO
        {
            CurrentTier = RatingUtils.GetTier(ratingStats.Rating),
            CurrentSubTier = RatingUtils.GetSubTier(ratingStats.Rating),
            RatingForNextTier = RatingUtils.GetNextTierRatingDelta(ratingStats.Rating),
            RatingForNextMajorTier = RatingUtils.GetNextMajorTierRatingDelta(ratingStats.Rating),
            NextMajorTier = RatingUtils.GetNextMajorTier(ratingStats.Rating),
            SubTierFillPercentage = RatingUtils.GetNextTierFillPercentage(ratingStats.Rating),
            MajorTierFillPercentage = RatingUtils.GetNextMajorTierFillPercentage(ratingStats.Rating)
        };

        return ratingStats;
    }

    private async Task<PlayerModStatsDTO> GetModStatsAsync(
        int playerId,
        Ruleset ruleset,
        DateTime dateMin,
        DateTime dateMax
    ) => await matchStatsRepository.GetModStatsAsync(playerId, ruleset, dateMin, dateMax);

    private async Task<PlayerTournamentStatsDTO> GetTournamentStatsAsync(
        int playerId,
        Ruleset ruleset,
        DateTime dateMin,
        DateTime dateMax
    )
    {
        const int maxTournaments = 5;

        IEnumerable<PlayerTournamentMatchCostDTO> bestPerformances = await tournamentsRepository.GetPerformancesAsync(
            playerId,
            ruleset,
            dateMin,
            dateMax,
            maxTournaments, TournamentPerformanceResultType.Best);

        IEnumerable<PlayerTournamentMatchCostDTO> recentPerformances = await tournamentsRepository.GetPerformancesAsync(
            playerId,
            ruleset,
            dateMin,
            dateMax,
            maxTournaments, TournamentPerformanceResultType.Recent);

        PlayerTournamentLobbySizeCountDTO counts = await tournamentsRepository.GetLobbySizeStatsAsync(
            playerId,
            ruleset,
            dateMin,
            dateMax
        );
        return new PlayerTournamentStatsDTO
        {
            LobbySizeCounts = counts,
            BestPerformances = bestPerformances,
            RecentPerformances = recentPerformances
        };
    }

    private async Task<AggregatePlayerMatchStatsDTO?> GetMatchStatsAsync(
        int id,
        Ruleset ruleset,
        DateTime dateMin,
        DateTime dateMax
    )
    {
        var matchStats = (await matchStatsRepository.GetForPlayerAsync(id, ruleset, dateMin, dateMax)).ToList();
        var adjustments =
            (await ratingStatsRepository.GetForPlayerAsync(id, ruleset, dateMin, dateMax))
            .GroupBy(ra => ra.Timestamp)
            .SelectMany(ra => ra)
            .ToList();

        if (matchStats.Count == 0 || adjustments.Count == 0)
        {
            return new AggregatePlayerMatchStatsDTO();
        }

        /**
         * If the adjustments list contains an initial rating, we need to subtract the
         * rating gained value by the initial rating. Essentially we are displaying
         * the net gain in rating without considering the initial rating.
         */
        var initialRatingValue = adjustments
            .FirstOrDefault(ra => ra.AdjustmentType == RatingAdjustmentType.Initial)?.RatingAfter
            ?? 0;

        return new AggregatePlayerMatchStatsDTO
        {
            // TODO: Different way of calcing this
            // AverageMatchCostAggregate = ratingStats.Average(x => x.MatchCost),
            HighestRating = adjustments.Max(ra => ra.RatingAfter),
            RatingGained = adjustments.Sum(ra => ra.RatingDelta) - initialRatingValue,
            GamesWon = matchStats.Sum(ra => ra.GamesWon),
            GamesLost = matchStats.Sum(ra => ra.GamesLost),
            GamesPlayed = matchStats.Sum(ra => ra.GamesPlayed),
            MatchesWon = matchStats.Count(ra => ra.Won),
            MatchesLost = matchStats.Count(ra => !ra.Won),
            // TODO: Different way of calcing this
            // AverageTeammateRating = ratingStats.Average(x => x.AverageTeammateRating),
            // AverageOpponentRating = ratingStats.Average(x => x.AverageOpponentRating),
            BestWinStreak = GetHighestWinStreak(matchStats),
            MatchAverageScoreAggregate = matchStats.Average(pms => pms.AverageScore),
            MatchAverageAccuracyAggregate = matchStats.Average(pms => pms.AverageAccuracy),
            MatchAverageMissesAggregate = matchStats.Average(pms => pms.AverageMisses),
            AverageGamesPlayedAggregate = matchStats.Average(pms => pms.GamesPlayed),
            AveragePlacingAggregate = matchStats.Average(pms => pms.AveragePlacement),
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
