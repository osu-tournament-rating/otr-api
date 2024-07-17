using Database.Entities;
using Database.Enums;
using Database.Enums.Verification;

namespace DataWorkerService.Processors.Games;

/// <summary>
/// Processor tasked with generating aggregate stats for a <see cref="Game"/>
/// </summary>
public class GameStatsProcessor(
    ILogger<GameStatsProcessor> logger
) : ProcessorBase<Game>(logger)
{
    protected override Task OnProcessingAsync(Game entity, CancellationToken cancellationToken)
    {
        if (entity.ProcessingStatus is not GameProcessingStatus.NeedsStatCalculation)
        {
            logger.LogDebug(
                "Game does not require processing [Id: {Id} | Processing Status: {Status}]",
                entity.Id,
                entity.ProcessingStatus
            );

            return Task.CompletedTask;
        }

        // Assign placements
        IEnumerable<GameScore> verifiedScores = entity.Scores
            .Where(s => s is { VerificationStatus: VerificationStatus.Verified, ProcessingStatus: ScoreProcessingStatus.Done })
            .OrderByDescending(s => s.Score)
            .ToList();

        foreach (var placement in verifiedScores.Select((s, idx) => new { Score = s, Index = idx + 1 }))
        {
            placement.Score.Placement = placement.Index;
        }

        IEnumerable<GameScore> blueTeamScores = verifiedScores.Where(s => s.Team is Team.Blue).ToList();
        IEnumerable<GameScore> redTeamScores = verifiedScores.Where(s => s.Team is Team.Red).ToList();

        entity.WinRecord = blueTeamScores.Sum(gs => gs.Score) > redTeamScores.Sum(gs => gs.Score)
            ? GenerateWinRecord(blueTeamScores, redTeamScores)
            : GenerateWinRecord(redTeamScores, blueTeamScores);

        entity.ProcessingStatus += 1;

        return Task.CompletedTask;
    }

    public static GameWinRecord GenerateWinRecord(
        IEnumerable<GameScore> winnerTeamScores,
        IEnumerable<GameScore> loserTeamScores
    )
    {
        IEnumerable<GameScore> winnerScores = winnerTeamScores as GameScore[] ?? winnerTeamScores.ToArray();
        IEnumerable<GameScore> loserScores = loserTeamScores as GameScore[] ?? loserTeamScores.ToArray();

        return new GameWinRecord
        {
            WinnerRoster = winnerScores.Select(gs => gs.Player.Id).ToArray(),
            LoserRoster = loserScores.Select(gs => gs.Player.Id).ToArray(),
            WinnerTeam = winnerScores.First().Team,
            LoserTeam = loserScores.First().Team,
            WinnerScore = winnerScores.Sum(gs => gs.Score),
            LoserScore = loserScores.Sum(gs => gs.Score)
        };
    }
}
