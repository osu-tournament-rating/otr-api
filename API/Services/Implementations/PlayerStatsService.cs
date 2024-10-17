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
    IPlayerRatingService playerRatingService,
    IApiMatchWinRecordRepository matchWinRecordRepository,
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

        ruleset ??= player.User?.Settings.DefaultRuleset ?? player.Ruleset;

        PlayerCompactDTO playerInfo = mapper.Map<PlayerCompactDTO>(player);

        PlayerRatingStatsDTO? baseStats = await GetBaseStatsAsync(player.Id, ruleset.Value);

        if (baseStats is null)
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

        IEnumerable<PlayerFrequencyDTO> frequentTeammates = await matchWinRecordRepository.GetFrequentTeammatesAsync(
            player.Id,
            ruleset.Value,
            dateMin.Value,
            dateMax.Value
        );

        IEnumerable<PlayerFrequencyDTO> frequentOpponents = await matchWinRecordRepository.GetFrequentOpponentsAsync(
            player.Id,
            ruleset.Value,
            dateMin.Value,
            dateMax.Value
        );

        return new PlayerStatsDTO
        {
            PlayerInfo = playerInfo,
            Ruleset = ruleset.Value,
            Rating = baseStats,
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

    public async Task<double> GetPeakRatingAsync(int playerId, Ruleset ruleset, DateTime? dateMin = null,
        DateTime? dateMax = null)
    {
        return (await ratingStatsRepository.GetForPlayerAsync(playerId, ruleset, dateMin, dateMax))
            .SelectMany(x => x)
            .Max(x => x.RatingAfter);
    }

    // Returns overall stats for the player, no need to filter by date.
    private async Task<PlayerRatingStatsDTO?> GetBaseStatsAsync(int playerId, Ruleset ruleset)
    {
        PlayerRatingStatsDTO? dto = await playerRatingService.GetAsync(playerId, ruleset);

        if (dto == null)
        {
            return null;
        }

        var matchesPlayed = await matchStatsRepository.CountMatchesPlayedAsync(playerId, ruleset);
        var winRate = await matchStatsRepository.GlobalWinrateAsync(playerId, ruleset);

        dto.MatchesPlayed = matchesPlayed;
        dto.WinRate = winRate;

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
            .ToList()
            .SelectMany(x => x)
            .ToList();

        if (matchStats.Count == 0)
        {
            return new AggregatePlayerMatchStatsDTO();
        }

        return new AggregatePlayerMatchStatsDTO
        {
            // TODO: Different way of calcing this
            // AverageMatchCostAggregate = ratingStats.Average(x => x.MatchCost),
            HighestRating = adjustments.Max(x => x.RatingAfter),
            RatingGained = adjustments.Last().RatingAfter - adjustments.First().RatingAfter,
            GamesWon = matchStats.Sum(x => x.GamesWon),
            GamesLost = matchStats.Sum(x => x.GamesLost),
            GamesPlayed = matchStats.Sum(x => x.GamesPlayed),
            MatchesWon = matchStats.Count(x => x.Won),
            MatchesLost = matchStats.Count(x => !x.Won),
            // TODO: Different way of calcing this
            // AverageTeammateRating = ratingStats.Average(x => x.AverageTeammateRating),
            // AverageOpponentRating = ratingStats.Average(x => x.AverageOpponentRating),
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
