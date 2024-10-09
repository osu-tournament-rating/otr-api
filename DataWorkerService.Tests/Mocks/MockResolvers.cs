using Database.Entities;
using DataWorkerService.AutomationChecks;
using DataWorkerService.AutomationChecks.Games;
using DataWorkerService.AutomationChecks.Matches;
using DataWorkerService.AutomationChecks.Scores;
using DataWorkerService.AutomationChecks.Tournaments;
using DataWorkerService.Processors.Games;
using DataWorkerService.Processors.Matches;
using DataWorkerService.Processors.Resolvers.Implementations;
using DataWorkerService.Processors.Scores;
using DataWorkerService.Processors.Tournaments;
using Microsoft.Extensions.Logging;
using Serilog.Extensions.Logging;

namespace DataWorkerService.Tests.Mocks;

public static class MockResolvers
{
    private static readonly SerilogLoggerFactory s_loggerFactory = new();

    private static readonly MockContext s_mockContext = new();

    private static Logger<T> Logger<T>() where T : class => new(s_loggerFactory);

    public static ScoreProcessorResolver ScoreProcessorResolver => new([
        new ScoreVerificationProcessor(Logger<ScoreVerificationProcessor>()),
        new ScoreAutomationChecksProcessor(
            Logger<ScoreAutomationChecksProcessor>(),
            new List<IAutomationCheck<GameScore>>
            {
                new ScoreMinimumCheck(Logger<ScoreMinimumCheck>()),
                new ScoreModCheck(Logger<ScoreModCheck>()),
                new ScoreRulesetCheck(Logger<ScoreRulesetCheck>())
            }
        )
    ]);

    public static GameProcessorResolver GameProcessorResolver => new([
        new GameStatsProcessor(Logger<GameStatsProcessor>()),
        new GameVerificationProcessor(Logger<GameVerificationProcessor>(), ScoreProcessorResolver),
        new GameAutomationChecksProcessor(
            Logger<GameAutomationChecksProcessor>(),
            new List<IAutomationCheck<Game>>
            {
                new GameEndTimeCheck(Logger<GameEndTimeCheck>()),
                new GameModCheck(Logger<GameModCheck>()),
                new GameRulesetCheck(Logger<GameRulesetCheck>()),
                new GameScoreCountCheck(Logger<GameScoreCountCheck>()),
                new GameScoringTypeCheck(Logger<GameScoringTypeCheck>()),
                new GameTeamTypeCheck(Logger<GameTeamTypeCheck>())
            },
            ScoreProcessorResolver
        )
    ]);

    public static MatchProcessorResolver MatchProcessorResolver => new([
        new MatchStatsProcessor(Logger<MatchStatsProcessor>(), GameProcessorResolver),
        new MatchVerificationProcessor(Logger<MatchVerificationProcessor>(), GameProcessorResolver),
        new MatchAutomationChecksProcessor(
            Logger<MatchAutomationChecksProcessor>(),
            new List<IAutomationCheck<Match>>
            {
                new MatchEndTimeCheck(Logger<MatchEndTimeCheck>()),
                new MatchGameCountCheck(Logger<MatchGameCountCheck>()),
                new MatchHeadToHeadCheck(Logger<MatchHeadToHeadCheck>()),
                new MatchNamePrefixCheck(Logger<MatchNamePrefixCheck>())
            },
            GameProcessorResolver
        )
    ]);

    public static TournamentProcessorResolver TournamentProcessorResolver => new([
        new TournamentStatsProcessor(Logger<TournamentStatsProcessor>(), MatchProcessorResolver, s_mockContext.Object),
        new TournamentVerificationProcessor(Logger<TournamentVerificationProcessor>(), MatchProcessorResolver, s_mockContext.Object),
        new TournamentAutomationChecksProcessor(
            Logger<TournamentAutomationChecksProcessor>(),
            new List<IAutomationCheck<Tournament>>
            {
                new TournamentMatchCountCheck(Logger<TournamentMatchCountCheck>())
            },
            MatchProcessorResolver,
            s_mockContext.Object
        )
    ]);
}
