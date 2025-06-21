using DataWorkerService.AutomationChecks.Games;
using DataWorkerService.AutomationChecks.Matches;
using DataWorkerService.AutomationChecks.Scores;
using DataWorkerService.Processors.Games;
using DataWorkerService.Processors.Matches;
using DataWorkerService.Processors.Resolvers.Implementations;
using DataWorkerService.Processors.Scores;
using Microsoft.Extensions.Logging;
using Serilog.Extensions.Logging;

namespace DataWorkerService.Tests.Mocks;

public static class MockResolvers
{
    private static readonly SerilogLoggerFactory s_loggerFactory = new();

    private static Logger<T> Logger<T>() where T : class => new(s_loggerFactory);

    private static ScoreProcessorResolver ScoreProcessorResolver => new([
        new ScoreVerificationProcessor(Logger<ScoreVerificationProcessor>()),
        new ScoreAutomationChecksProcessor(
            Logger<ScoreAutomationChecksProcessor>(),
            [
                new ScoreMinimumCheck(Logger<ScoreMinimumCheck>()),
                new ScoreModCheck(Logger<ScoreModCheck>()),
                new ScoreRulesetCheck(Logger<ScoreRulesetCheck>())
            ]
        )
    ]);

    private static GameProcessorResolver GameProcessorResolver => new([
        new GameStatsProcessor(Logger<GameStatsProcessor>()),
        new GameVerificationProcessor(Logger<GameVerificationProcessor>(), ScoreProcessorResolver),
        new GameAutomationChecksProcessor(
            Logger<GameAutomationChecksProcessor>(),
            [
                new GameBeatmapUsageCheck(Logger<GameBeatmapUsageCheck>()),
                new GameEndTimeCheck(Logger<GameEndTimeCheck>()),
                new GameModCheck(Logger<GameModCheck>()),
                new GameRulesetCheck(Logger<GameRulesetCheck>()),
                new GameScoreCountCheck(Logger<GameScoreCountCheck>()),
                new GameScoringTypeCheck(Logger<GameScoringTypeCheck>()),
                new GameTeamTypeCheck(Logger<GameTeamTypeCheck>())
            ],
            ScoreProcessorResolver
        )
    ]);

    public static MatchProcessorResolver MatchProcessorResolver => new([
        new MatchStatsProcessor(Logger<MatchStatsProcessor>(), GameProcessorResolver),
        new MatchVerificationProcessor(Logger<MatchVerificationProcessor>(), GameProcessorResolver),
        new MatchAutomationChecksProcessor(
            Logger<MatchAutomationChecksProcessor>(),
            [
                new MatchEndTimeCheck(Logger<MatchEndTimeCheck>()),
                new MatchGameCountCheck(Logger<MatchGameCountCheck>()),
                new MatchHeadToHeadCheck(Logger<MatchHeadToHeadCheck>()),
                new MatchNamePrefixCheck(Logger<MatchNamePrefixCheck>()),
                new MatchNameFormatCheck(Logger<MatchNameFormatCheck>()),
                new MatchTeamsIntegrityCheck(Logger<MatchTeamsIntegrityCheck>())
            ],
            GameProcessorResolver
        )
    ]);
}
