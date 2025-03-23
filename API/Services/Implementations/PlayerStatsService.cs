using API.DTOs;
using API.Services.Interfaces;
using API.Utilities;
using AutoMapper;
using Common.Enums;
using Database.Entities;
using Database.Repositories.Interfaces;

namespace API.Services.Implementations;

public class PlayerStatsService(
    IMapper mapper,
    ITournamentsRepository tournamentsRepository,
    IGameScoresRepository gameScoresRepository,
    IPlayerMatchStatsRepository playerMatchStatsRepository,
    IPlayerRatingsService playerRatingsService,
    IPlayersRepository playersRepository,
    IRatingAdjustmentsRepository ratingAdjustmentsRepository,
    IPlayerTournamentStatsRepository playerTournamentStatsRepository
) : IPlayerStatsService
{
    private const int MaxFrequencyTeammatesOpponents = 20;

    public async Task<PlayerDashboardStatsDTO?> GetAsync(
        string key,
        Ruleset? ruleset = null,
        DateTime? dateMin = null,
        DateTime? dateMax = null
    )
    {
        dateMin ??= DateTime.MinValue;
        dateMax ??= DateTime.MaxValue;

        Player? player = await playersRepository.GetVersatileAsync(key, true);

        if (player is null)
        {
            return null;
        }

        ruleset ??= player.User?.Settings.DefaultRuleset ?? player.DefaultRuleset;

        PlayerCompactDTO playerInfo = mapper.Map<PlayerCompactDTO>(player);

        PlayerRatingStatsDTO? ratingStats = await GetCurrentAsync(player.Id, ruleset.Value);

        if (ratingStats is null)
        {
            return new PlayerDashboardStatsDTO { PlayerInfo = playerInfo, Ruleset = ruleset.Value };
        }

        AggregatePlayerMatchStatsDTO? matchStats =
            await GetMatchStatsAsync(player.Id, ruleset.Value, dateMin.Value, dateMax.Value);

        IEnumerable<PlayerModStatsDTO> modStats = await GetModStatsAsync(player.Id, ruleset.Value, dateMin.Value, dateMax.Value);

        PlayerTournamentPerformanceDTO tournamentPerformance =
            await GetTournamentStatsAsync(player.Id, ruleset.Value, dateMin.Value, dateMax.Value);

        Dictionary<bool, List<PlayerFrequencyDTO>> frequentTeammatesOpponents = await GetFrequentMatchupsAsync(player.Id, ruleset.Value, dateMin, dateMax);

        return new PlayerDashboardStatsDTO
        {
            PlayerInfo = playerInfo,
            Ruleset = ruleset.Value,
            Rating = ratingStats,
            MatchStats = matchStats,
            ModStats = modStats,
            TournamentPerformanceStats = tournamentPerformance,
            FrequentTeammates = frequentTeammatesOpponents[true].Take(MaxFrequencyTeammatesOpponents),
            FrequentOpponents = frequentTeammatesOpponents[false].Take(MaxFrequencyTeammatesOpponents)
        };
    }

    public async Task<double> GetPeakRatingAsync(int playerId, Ruleset ruleset, DateTime? dateMin = null,
        DateTime? dateMax = null)
    {
        return (await ratingAdjustmentsRepository.GetForPlayerAsync(playerId, ruleset, dateMin, dateMax))
            .Max(ra => ra.RatingAfter);
    }

    public async Task<Dictionary<bool, List<PlayerFrequencyDTO>>> GetFrequentMatchupsAsync(
        int playerId,
        Ruleset ruleset,
        DateTime? dateMin = null,
        DateTime? dateMax = null
    )
    {
        dateMin ??= DateTime.MinValue;
        dateMax ??= DateTime.MaxValue;

        // Fetch match stats for the player
        IList<PlayerMatchStats> stats = [.. await playerMatchStatsRepository.GetForPlayerAsync(
            playerId, ruleset, dateMin.Value, dateMax.Value
        )];

        // Calculate frequencies for teammates and opponents
        Dictionary<int, int> frequencyTeammates = CalculateFrequencies(stats, stat => stat.TeammateIds);
        Dictionary<int, int> frequencyOpponents = CalculateFrequencies(stats, stat => stat.OpponentIds);

        // Fetch all unique player IDs (teammates and opponents)
        var uniquePlayerIds = frequencyTeammates.Keys.Union(frequencyOpponents.Keys).Distinct().ToList();

        // Fetch player data in a single query
        var players = (await playersRepository.GetAsync(uniquePlayerIds))
                .ToDictionary(k => k.Id, v => v);

        // Create the result dictionary
        var result = new Dictionary<bool, List<PlayerFrequencyDTO>>
        {
            [true] = frequencyTeammates.Count > 0 ? CreatePlayerFrequencyList(frequencyTeammates, players) : [],
            [false] = frequencyOpponents.Count > 0 ? CreatePlayerFrequencyList(frequencyOpponents, players) : []
        };

        if (result[true].Count + result[false].Count != uniquePlayerIds.Count)
        {
            throw new InvalidOperationException("Mismatch between frequency counts and player data. Some players could not be fetched from the database.");
        }

        return result;
    }

    /// <summary>
    /// Helper method to calculate frequencies of players (teammates or opponents).
    /// </summary>
    private static Dictionary<int, int> CalculateFrequencies(
        IEnumerable<PlayerMatchStats> stats,
        Func<PlayerMatchStats, IEnumerable<int>> playerIdsSelector
    )
    {
        var frequencyDict = new Dictionary<int, int>();

        foreach (PlayerMatchStats stat in stats)
        {
            foreach (var id in playerIdsSelector(stat))
            {
                frequencyDict.TryAdd(id, 0);
                frequencyDict[id]++;
            }
        }

        return frequencyDict;
    }

    /// <summary>
    /// Helper method to create a list of PlayerFrequencyDTO objects.
    /// </summary>
    private List<PlayerFrequencyDTO> CreatePlayerFrequencyList(
        Dictionary<int, int> frequencyDict,
        Dictionary<int, Player> players
    )
    {
        return
        [
            .. frequencyDict
                .Where(kvp => players.ContainsKey(kvp.Key)) // Ensure the player exists
                .Select(kvp => new PlayerFrequencyDTO
                {
                    Player = mapper.Map<PlayerCompactDTO>(players[kvp.Key]), Frequency = kvp.Value
                })
                .OrderByDescending(pf => pf.Frequency)
        ];
    }


    private async Task<PlayerRatingStatsDTO?> GetCurrentAsync(int playerId, Ruleset ruleset)
    {
        PlayerRatingStatsDTO? ratingStats = await playerRatingsService.GetAsync(playerId, ruleset, true);

        if (ratingStats == null)
        {
            return null;
        }

        var matchesPlayed = await playerMatchStatsRepository.CountMatchesPlayedAsync(playerId, ruleset);
        var winRate = await playerMatchStatsRepository.GlobalWinrateAsync(playerId, ruleset);

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

    private async Task<IEnumerable<PlayerModStatsDTO>> GetModStatsAsync(
        int playerId,
        Ruleset ruleset,
        DateTime dateMin,
        DateTime dateMax
    )
    {
        Dictionary<Mods, int> modScores = await gameScoresRepository.GetAverageModScoresAsync(playerId, ruleset, dateMin, dateMax);
        return (await gameScoresRepository.GetModFrequenciesAsync(playerId, ruleset, dateMin, dateMax))
            .Select(kvp => new PlayerModStatsDTO { Mods = kvp.Key, Count = kvp.Value, AverageScore = modScores[kvp.Key] });
    }

    private async Task<PlayerTournamentPerformanceDTO> GetTournamentStatsAsync(
        int playerId,
        Ruleset ruleset,
        DateTime dateMin,
        DateTime dateMax
    )
    {
        const int count = 5;

        ICollection<PlayerTournamentStats> bestPerformances = await playerTournamentStatsRepository.GetBestPerformancesAsync(
            playerId,
            count,
            ruleset,
            dateMin,
            dateMax);

        ICollection<PlayerTournamentStats> recentPerformances =
            await playerTournamentStatsRepository.GetRecentPerformancesAsync(
                playerId,
                count,
                ruleset,
                dateMin,
                dateMax);

        Dictionary<int, int> counts = await tournamentsRepository.GetLobbySizeStatsAsync(
            playerId,
            ruleset,
            dateMin,
            dateMax
        );

        var lobbyStats = new PlayerTournamentLobbySizeCountDTO
        {
            Count1v1 = counts[1],
            Count2v2 = counts[2],
            Count3v3 = counts[3],
            Count4v4 = counts[4],
            CountOther = counts[-1]
        };

        return new PlayerTournamentPerformanceDTO
        {
            LobbySizeCounts = lobbyStats,
            BestPerformances = mapper.Map<IEnumerable<PlayerTournamentStatsDTO>>(bestPerformances),
            RecentPerformances = mapper.Map<IEnumerable<PlayerTournamentStatsDTO>>(recentPerformances)
        };
    }

    private async Task<AggregatePlayerMatchStatsDTO?> GetMatchStatsAsync(
        int id,
        Ruleset ruleset,
        DateTime dateMin,
        DateTime dateMax
    )
    {
        var matchStats = (await playerMatchStatsRepository.GetForPlayerAsync(id, ruleset, dateMin, dateMax)).ToList();
        var adjustments =
            (await ratingAdjustmentsRepository.GetForPlayerAsync(id, ruleset, dateMin, dateMax))
            .GroupBy(ra => ra.Timestamp)
            .SelectMany(ra => ra)
            .ToList();

        if (matchStats.Count == 0 || adjustments.Count == 0)
        {
            return new AggregatePlayerMatchStatsDTO();
        }

        /**
         * If the adjustments list contains an initial rating, we need to subtract the
         * rating gained value by the initial rating. Essentially we are displaying`
         * the net gain in rating without considering the initial rating.
         */
        var initialRatingValue = adjustments
            .FirstOrDefault(ra => ra.AdjustmentType == RatingAdjustmentType.Initial)?.RatingAfter
            ?? 0;

        return new AggregatePlayerMatchStatsDTO
        {
            AverageMatchCostAggregate = matchStats.Average(x => x.MatchCost),
            HighestRating = adjustments.Max(ra => ra.RatingAfter),
            RatingGained = adjustments.Sum(ra => ra.RatingDelta) - initialRatingValue,
            GamesWon = matchStats.Sum(ra => ra.GamesWon),
            GamesLost = matchStats.Sum(ra => ra.GamesLost),
            GamesPlayed = matchStats.Sum(ra => ra.GamesPlayed),
            MatchesWon = matchStats.Count(ra => ra.Won),
            MatchesLost = matchStats.Count(ra => !ra.Won),
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
