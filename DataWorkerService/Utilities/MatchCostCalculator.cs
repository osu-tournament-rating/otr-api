using Database.Entities;
using Database.Enums.Verification;
using MathNet.Numerics.Distributions;
using MathNet.Numerics.Statistics;

namespace DataWorkerService.Utilities;

/// <summary>
/// Calculates Match Costs for a <see cref="Database.Entities.Match"/>
/// </summary>
public static class MatchCostCalculator
{
    /// <summary>
    /// Calculates the match cost for each player in a given list of games using the o!TR Match Cost formula
    /// </summary>
    /// <param name="games">Games to calculate match costs for</param>
    /// <returns>A dictionary of { <see cref="Player"/>.<see cref="Player.Id"/>, match cost }</returns>
    /// <remarks>
    /// Only <see cref="GameScore"/>s with a <see cref="VerificationStatus"/> of <see cref="VerificationStatus.Verified"/>
    /// and a <see cref="ScoreProcessingStatus"/> of <see cref="ScoreProcessingStatus.Done"/> are considered
    /// </remarks>
    public static IDictionary<int, double> CalculateOtrMatchCosts(IEnumerable<Game> games)
    {
        IEnumerable<Game> eGames = games.ToList();

        var zScores = eGames
            .SelectMany(g => g.Scores)
            .WhereValid()
            .Select(s => s.Player.Id)
            .Distinct()
            .Select(id => new KeyValuePair<int, List<double>>(id, []))
            .ToDictionary();

        foreach (Game game in eGames)
        {
            var scores = game.Scores.WhereValid().ToList();

            var gameScoresAvg = scores.Average(s => s.Score);
            var gameScoresStdev = scores.Select(s => (double)s.Score).StandardDeviation();

            // Calculate z-scores
            foreach (GameScore score in scores)
            {
                zScores[score.Player.Id].Add((score.Score - gameScoresAvg) / gameScoresStdev);
            }
        }

        // Create a normal distribution with z-score mean (mu) and z-score stdev (sigma)
        var zScoresNormal = new Normal(
            zScores.SelectMany(pair => pair.Value).Mean(),
            zScores.SelectMany(pair => pair.Value).StandardDeviation()
        );

        var gamesCount = eGames.Count();

        // Calculate match costs
        return zScores.Select(pair =>
            new KeyValuePair<int, double>(
                pair.Key,
                // Average z-score percentile
                (zScoresNormal.CumulativeDistribution(pair.Value.Average()) + 0.5)
                *
                // Performance bonus
                (1 + 0.3 * Math.Sqrt(eGames.Count(g => g.Scores.WhereValid().Any(s => s.Player.Id == pair.Key)) - 1 - (gamesCount - 1)))
            )
        ).ToDictionary();
    }

    private static IEnumerable<GameScore> WhereValid(this IEnumerable<GameScore> scores) =>
        scores.Where(s => s is { VerificationStatus: VerificationStatus.Verified, ProcessingStatus: ScoreProcessingStatus.Done });
}
