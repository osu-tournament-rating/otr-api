using System.Diagnostics.CodeAnalysis;
using Common.Enums;
using Common.Enums.Verification;
using Database.Entities;
using DataWorkerService.Processors;
using DataWorkerService.Tests.Mocks;
using TestingUtils.SeededData;

namespace DataWorkerService.Tests.AutomationChecks;

public class IntegratedAutomationChecksTests
{
    [Fact]
    [SuppressMessage("ReSharper", "ParameterOnlyUsedForPreconditionCheck.Local")]
    public async Task Processor_Integrated_ProcessesGameSuccessfully()
    {
        // Arrange
        IProcessor<Match> processor = MockResolvers.MatchProcessorResolver.GetAutomationChecksProcessor();

        Tournament tournament = SeededTournament.Generate(
            name: "osu! World Cup 2023",
            abbreviation: "OWC2023",
            ruleset: Ruleset.Osu,
            teamSize: 4
        );

        Match match = SeededMatch.ExampleMatch();

        match.Tournament = tournament;
        match.TournamentId = tournament.Id;

        match.VerificationStatus = VerificationStatus.None;
        match.RejectionReason = MatchRejectionReason.None;
        match.ProcessingStatus = MatchProcessingStatus.NeedsAutomationChecks;

        foreach (Game game in match.Games)
        {
            game.VerificationStatus = VerificationStatus.None;
            game.RejectionReason = GameRejectionReason.None;
            game.ProcessingStatus = GameProcessingStatus.NeedsAutomationChecks;

            foreach (GameScore score in game.Scores)
            {
                score.VerificationStatus = VerificationStatus.None;
                score.RejectionReason = ScoreRejectionReason.None;
                score.ProcessingStatus = ScoreProcessingStatus.NeedsAutomationChecks;
            }
        }

        // Act
        await processor.ProcessAsync(match, CancellationToken.None);

        // Assert
        Assert.Equal(VerificationStatus.PreVerified, match.VerificationStatus);
        Assert.Equal(MatchRejectionReason.None, match.RejectionReason);
        Assert.Equal(MatchProcessingStatus.NeedsVerification, match.ProcessingStatus);
        Assert.All(match.Games, game =>
        {
            Assert.Equal(VerificationStatus.PreVerified, game.VerificationStatus);
            Assert.Equal(GameRejectionReason.None, game.RejectionReason);
            Assert.Equal(GameProcessingStatus.NeedsVerification, game.ProcessingStatus);
            Assert.All(game.Scores, score =>
            {
                Assert.Equal(VerificationStatus.PreVerified, score.VerificationStatus);
                Assert.Equal(ScoreRejectionReason.None, score.RejectionReason);
                Assert.Equal(ScoreProcessingStatus.NeedsVerification, score.ProcessingStatus);
            });
        });
    }
}
