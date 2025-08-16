using Common.Enums;
using Common.Enums.Verification;
using Database.Entities;
using DWS.Models;
using DWS.Utilities;

namespace DWS.Calculators;

/// <summary>
/// Functional calculator for tournament statistics that performs all calculations in-memory.
/// </summary>
/// <remarks>
/// This calculator processes tournament statistics in multiple phases:
/// 1. Validates tournament and match verification status
/// 2. Processes individual match and game statistics
/// 3. Aggregates player performance across the tournament
/// 4. Updates processing timestamps for tracking
/// </remarks>
public class TournamentStatsCalculator : IStatsCalculator
{
    /// <inheritdoc />
    public StatsCalculationResult CalculateAllStatistics(Tournament tournament)
    {
        if (tournament.VerificationStatus is not VerificationStatus.Verified)
        {
            return new StatsCalculationResult
            {
                Success = false,
                ErrorMessage = $"Tournament {tournament.Id} is not verified (status: {tournament.VerificationStatus})"
            };
        }

        // Filter to only verified matches
        var verifiedMatches = tournament.Matches
            .Where(m => m.VerificationStatus == VerificationStatus.Verified)
            .ToList();

        if (verifiedMatches.Count == 0)
        {
            return new StatsCalculationResult
            {
                Success = false,
                ErrorMessage = $"No verified matches found for tournament {tournament.Id}"
            };
        }

        // Process all matches and their games
        int totalPlayerMatchStats = 0;
        foreach (Match match in verifiedMatches)
        {
            if (!ProcessMatchStatistics(match))
            {
                return new StatsCalculationResult
                {
                    Success = false,
                    ErrorMessage = $"Failed to process match statistics for match {match.Id} in tournament {tournament.Id}"
                };
            }
            totalPlayerMatchStats += match.PlayerMatchStats.Count;
        }

        // Validate processor data exists
        if (!ValidateProcessorData(verifiedMatches))
        {
            return new StatsCalculationResult
            {
                Success = false,
                ErrorMessage = $"Missing processor data for tournament {tournament.Id}"
            };
        }

        // Aggregate player tournament statistics
        AggregatePlayerTournamentStatistics(tournament, verifiedMatches);


        return new StatsCalculationResult
        {
            Success = true,
            PlayerTournamentStatsCount = tournament.PlayerTournamentStats.Count,
            PlayerMatchStatsCount = totalPlayerMatchStats,
            VerifiedMatchesCount = verifiedMatches.Count
        };
    }

    /// <summary>
    /// Processes statistics for a single match and all its games.
    /// </summary>
    /// <remarks>
    /// This method handles:
    /// - Game score placement calculation
    /// - Roster generation for both games and matches
    /// - Player match statistics aggregation
    /// </remarks>
    /// <param name="match">The match to process.</param>
    /// <returns>True if processing succeeded, false otherwise.</returns>
    private static bool ProcessMatchStatistics(Match match)
    {
        if (match.VerificationStatus is not VerificationStatus.Verified)
        {
            return false;
        }

        var verifiedGames = match.Games
            .Where(g => g.VerificationStatus == VerificationStatus.Verified)
            .ToList();

        if (verifiedGames.Count == 0)
        {
            return false;
        }

        // Process each game's statistics
        foreach (Game game in verifiedGames)
        {
            ProcessGameStatistics(game);
        }

        // Validate all games have rosters
        if (verifiedGames.Any(g => g.Rosters.Count == 0))
        {
            return false;
        }

        // Generate match rosters from games
        match.Rosters.Clear();
        ICollection<MatchRoster> newMatchRosters = RostersHelper.GenerateRosters(verifiedGames);
        foreach (MatchRoster roster in newMatchRosters)
        {
            match.Rosters.Add(roster);
        }

        // Generate player match statistics
        match.PlayerMatchStats.Clear();
        IEnumerable<PlayerMatchStats> playerStats = GeneratePlayerMatchStatistics(verifiedGames);
        foreach (PlayerMatchStats stat in playerStats)
        {
            match.PlayerMatchStats.Add(stat);
        }

        return true;
    }

    /// <summary>
    /// Processes statistics for a single game.
    /// </summary>
    /// <param name="game">The game to process.</param>
    private static void ProcessGameStatistics(Game game)
    {
        if (game.VerificationStatus is not VerificationStatus.Verified)
        {
            return;
        }

        var verifiedScores = game.Scores
            .Where(s => s.VerificationStatus == VerificationStatus.Verified)
            .OrderByDescending(s => s.Score)
            .ToList();

        // Assign placements (1-indexed, ordered by score descending)
        const int initialPlacement = 1;
        int placement = initialPlacement;
        foreach (GameScore score in verifiedScores)
        {
            score.Placement = placement++;
        }

        // Generate game rosters
        game.Rosters.Clear();
        ICollection<GameRoster> newGameRosters = RostersHelper.GenerateRosters(verifiedScores);
        foreach (GameRoster roster in newGameRosters)
        {
            game.Rosters.Add(roster);
        }

    }

    /// <summary>
    /// Generates player match statistics from verified games.
    /// </summary>
    /// <remarks>
    /// Calculates comprehensive statistics including:
    /// - Match costs per player
    /// - Win/loss records at game and match levels
    /// - Average performance metrics (score, placement, accuracy)
    /// - Teammate and opponent tracking
    /// </remarks>
    /// <param name="games">The games to generate statistics from.</param>
    /// <returns>Collection of player match statistics.</returns>
    private static IEnumerable<PlayerMatchStats> GeneratePlayerMatchStatistics(List<Game> games)
    {
        if (games.Count == 0)
        {
            return [];
        }

        // Validate all games have rosters and belong to same match
        if (games.Any(g => g.Rosters.Count == 0))
        {
            return [];
        }

        int matchId = games[0].MatchId;
        if (games.Any(g => g.MatchId != matchId))
        {
            return [];
        }

        // Calculate match costs
        IDictionary<int, double> matchCosts = MatchCostCalculator.CalculateOtrMatchCosts(games);

        // Pre-calculate winning teams for performance
        Dictionary<int, Team?> gameWinningTeams = CalculateGameWinningTeams(games);

        // Get match rosters and max score
        ICollection<MatchRoster> matchRosters = games[0].Match.Rosters;
        int maxMatchScore = matchRosters.Count > 0 ? matchRosters.Max(r => r.Score) : 0;

        // Group scores by player
        var playerScoreGroups = games
            .SelectMany(g => g.Scores)
            .Where(s => s.VerificationStatus == VerificationStatus.Verified)
            .GroupBy(s => s.PlayerId)
            .Select(group => new
            {
                PlayerId = group.Key,
                Scores = group.ToList()
            })
            .ToList();

        return playerScoreGroups.Select(playerData =>
        {
            Game[] playerGames = playerData.Scores.Select(s => s.Game).Distinct().ToArray();

            // Calculate games won/lost
            int gamesWon = 0;
            int gamesLost = 0;
            foreach (Game game in playerGames)
            {
                Team playerTeam = playerData.Scores.First(s => s.GameId == game.Id).Team;
                Team? winningTeam = gameWinningTeams.GetValueOrDefault(game.Id);

                if (winningTeam == playerTeam)
                {
                    gamesWon++;
                }
                else if (winningTeam != null)
                {
                    gamesLost++;
                }
            }

            // Determine match outcome
            MatchRoster? playerMatchRoster = matchRosters.FirstOrDefault(r => r.Roster.Contains(playerData.PlayerId));
            bool matchWon = playerMatchRoster != null && playerMatchRoster.Score == maxMatchScore;

            // Get teammates and opponents
            List<int> teammateIds = playerMatchRoster?.Roster.Where(id => id != playerData.PlayerId).ToList() ?? [];
            var opponentIds = matchRosters
                .Where(r => r.Team != playerMatchRoster?.Team)
                .SelectMany(r => r.Roster)
                .Where(id => id != playerData.PlayerId)
                .Distinct()
                .ToList();

            return new PlayerMatchStats
            {
                PlayerId = playerData.PlayerId,
                MatchId = matchId,
                MatchCost = matchCosts.TryGetValue(playerData.PlayerId, out double mc) ? mc : 0,
                AverageScore = playerData.Scores.Average(s => s.Score),
                AveragePlacement = playerData.Scores.Average(s => s.Placement),
                AverageMisses = playerData.Scores.Average(s => s.CountMiss),
                AverageAccuracy = playerData.Scores.Average(s => s.Accuracy),
                GamesPlayed = playerData.Scores.Count,
                GamesWon = gamesWon,
                GamesLost = gamesLost,
                Won = matchWon,
                TeammateIds = [.. teammateIds],
                OpponentIds = [.. opponentIds]
            };
        });
    }

    /// <summary>
    /// Calculates the winning team for each game.
    /// </summary>
    /// <remarks>
    /// Determines winners by summing team scores for each game.
    /// Returns null for the team if no verified scores exist.
    /// </remarks>
    /// <param name="games">The games to analyze.</param>
    /// <returns>Dictionary mapping game ID to winning team.</returns>
    private static Dictionary<int, Team?> CalculateGameWinningTeams(List<Game> games)
    {
        var result = new Dictionary<int, Team?>();

        foreach (Game game in games)
        {
            var teamScores = game.Scores
                .Where(s => s.VerificationStatus == VerificationStatus.Verified)
                .GroupBy(s => s.Team)
                .Select(g => new
                {
                    Team = g.Key,
                    TotalScore = g.Sum(s => s.Score)
                })
                .OrderByDescending(ts => ts.TotalScore)
                .ToList();

            result[game.Id] = teamScores.FirstOrDefault()?.Team;
        }

        return result;
    }

    /// <summary>
    /// Validates that all matches have required processor data.
    /// </summary>
    /// <param name="matches">The matches to validate.</param>
    /// <returns>True if all matches have processor data, false otherwise.</returns>
    private static bool ValidateProcessorData(List<Match> matches)
    {
        return matches.All(match => match.PlayerRatingAdjustments.Count > 0);
    }

    /// <summary>
    /// Aggregates player statistics across all tournament matches.
    /// </summary>
    /// <remarks>
    /// Creates comprehensive tournament-level statistics for each player including:
    /// - Average rating changes and match costs
    /// - Cumulative win/loss records
    /// - Overall performance metrics
    /// - Complete teammate identification
    /// Only includes players with rating adjustments (excludes restricted players).
    /// </remarks>
    /// <param name="tournament">The tournament to update.</param>
    /// <param name="verifiedMatches">The verified matches to aggregate from.</param>
    private static void AggregatePlayerTournamentStatistics(Tournament tournament, List<Match> verifiedMatches)
    {
        // Clear existing statistics
        tournament.PlayerTournamentStats.Clear();

        // Pre-aggregate all player data for efficiency
        var playerDataGroups = verifiedMatches
            .SelectMany(m => m.PlayerMatchStats.Select(pms => new
            {
                MatchStats = pms,
                RatingAdjustments = m.PlayerRatingAdjustments.Where(ra => ra.PlayerId == pms.PlayerId).ToList()
            }))
            .Where(x => x.RatingAdjustments.Count > 0) // Skip restricted players
            .GroupBy(x => x.MatchStats.PlayerId)
            .Select(g => new
            {
                PlayerId = g.Key,
                MatchStatsList = g.Select(x => x.MatchStats).ToList(),
                AllRatingAdjustments = g.SelectMany(x => x.RatingAdjustments).ToArray()
            })
            .ToList();

        foreach (var playerData in playerDataGroups)
        {
            if (playerData.AllRatingAdjustments.Length == 0)
            {
                continue;
            }

            var stats = new PlayerTournamentStats
            {
                PlayerId = playerData.PlayerId,
                TournamentId = tournament.Id,
                AverageRatingDelta = playerData.AllRatingAdjustments.Average(ra => ra.RatingDelta),
                AverageMatchCost = playerData.MatchStatsList.Average(pms => pms.MatchCost),
                AverageScore = (int)playerData.MatchStatsList.Average(pms => pms.AverageScore),
                AveragePlacement = playerData.MatchStatsList.Average(pms => pms.AveragePlacement),
                AverageAccuracy = playerData.MatchStatsList.Average(pms => pms.AverageAccuracy),
                MatchesPlayed = playerData.MatchStatsList.Count,
                MatchesWon = playerData.MatchStatsList.Count(pms => pms.Won),
                MatchesLost = playerData.MatchStatsList.Count(pms => !pms.Won),
                GamesPlayed = playerData.MatchStatsList.Sum(pms => pms.GamesPlayed),
                GamesWon = playerData.MatchStatsList.Sum(pms => pms.GamesWon),
                GamesLost = playerData.MatchStatsList.Sum(pms => pms.GamesLost),
                MatchWinRate = playerData.MatchStatsList.Count(pms => pms.Won) / (double)playerData.MatchStatsList.Count,
                TeammateIds = [.. playerData.MatchStatsList.SelectMany(pms => pms.TeammateIds).Distinct()]
            };

            tournament.PlayerTournamentStats.Add(stats);
        }
    }
}
