using Common.Enums;
using Common.Enums.Verification;
using Database.Entities;
using DataWorkerService.Processors;
using DataWorkerService.Processors.Games;
using DataWorkerService.Processors.Matches;
using DataWorkerService.Tests.Mocks;
using DataWorkerService.Utilities;
using TestingUtils.SeededData;

namespace DataWorkerService.Tests.Stats;

public class MatchStatsProcessorTests
{
    [Fact]
    public void Processor_ProperlyCreates_MatchRosterRecord()
    {
        // Arrange
        Match match = SeededMatch.ExampleMatch();

        match.VerificationStatus = VerificationStatus.Verified;
        foreach (Game game in match.Games)
        {
            game.VerificationStatus = VerificationStatus.Verified;
            foreach (GameScore score in game.Scores)
            {
                score.VerificationStatus = VerificationStatus.Verified;
                score.ProcessingStatus = ScoreProcessingStatus.Done;
            }

            GameStatsProcessor.AssignScorePlacements(game.Scores);
            game.Rosters = RostersHelper.GenerateRosters(game.Scores);
        }

        // Act
        ICollection<MatchRoster> rosters = RostersHelper.GenerateRosters(match.Games);

        // Assert
        MatchRoster? redRoster = rosters.FirstOrDefault(r => r.Team == Team.Red);
        MatchRoster? blueRoster = rosters.FirstOrDefault(r => r.Team == Team.Blue);

        Assert.NotNull(blueRoster);
        Assert.NotNull(redRoster);

        Assert.NotEmpty(blueRoster.Roster);
        Assert.NotEmpty(redRoster.Roster);

        Assert.Equal(blueRoster.Roster.Distinct().Count(), blueRoster.Roster.Length);
        Assert.Equal(redRoster.Roster.Distinct().Count(), redRoster.Roster.Length);

        Assert.Equal(2, rosters.Count);
    }

    [Fact]
    public async Task Processor_MovesGamesToNextProcessingStatus()
    {
        Match match = SeededMatch.ExampleMatch();

        match.ProcessingStatus = MatchProcessingStatus.NeedsStatCalculation;
        match.VerificationStatus = VerificationStatus.Verified;
        foreach (Game game in match.Games)
        {
            game.VerificationStatus = VerificationStatus.Verified;
            game.ProcessingStatus = GameProcessingStatus.NeedsStatCalculation;
            foreach (GameScore score in game.Scores)
            {
                score.VerificationStatus = VerificationStatus.Verified;
                score.ProcessingStatus = ScoreProcessingStatus.Done;
            }
        }

        // Act
        IProcessor<Match> processor = MockResolvers.MatchProcessorResolver.GetStatsProcessor();
        await processor.ProcessAsync(match, default);

        // Assert
        Assert.Equal(MatchProcessingStatus.NeedsRatingProcessorData, match.ProcessingStatus);
        Assert.All(match.Games, g => { Assert.Equal(GameProcessingStatus.Done, g.ProcessingStatus); });
    }
}
