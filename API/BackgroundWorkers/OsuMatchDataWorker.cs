using API.Entities;
using API.Enums;
using API.Osu.AutomationChecks;
using API.Osu.Multiplayer;
using API.Repositories.Interfaces;

namespace API.BackgroundWorkers;

public class OsuMatchDataWorker(
    ILogger<OsuMatchDataWorker> logger,
    IServiceProvider serviceProvider,
    IOsuApiService apiService,
    IConfiguration configuration
    ) : BackgroundService
{
    private const int IntervalSeconds = 5;
    private readonly bool _allowDataFetching = configuration.GetValue<bool>("Osu:AllowDataFetching");

    /// <summary>
    ///  This background service constantly checks the database for pending matches and processes them.
    ///  The osu! API rate limit is taken into account.
    /// </summary>
    /// <param name="cancellationToken"></param>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken) =>
        await BackgroundTask(stoppingToken);

    private async Task BackgroundTask(CancellationToken cancellationToken = default)
    {
        if (!_allowDataFetching)
        {
            logger.LogInformation("Skipping osu! match data worker due to configuration");
            return;
        }

        logger.LogInformation("Initialized osu! match data worker");

        while (!cancellationToken.IsCancellationRequested)
        {
            using IServiceScope scope = serviceProvider.CreateScope();

            IMatchesRepository matchesRepository = scope.ServiceProvider.GetRequiredService<IMatchesRepository>();
            IApiMatchRepository apiMatchService = scope.ServiceProvider.GetRequiredService<IApiMatchRepository>();
            IGamesRepository gamesRepository = scope.ServiceProvider.GetRequiredService<IGamesRepository>();
            IMatchScoresRepository matchScoresRepository = scope.ServiceProvider.GetRequiredService<IMatchScoresRepository>();
            OtrContext context = scope.ServiceProvider.GetRequiredService<OtrContext>();

            Match? apiMatch = await matchesRepository.GetFirstMatchNeedingApiProcessingAsync();
            IList<Match> autoCheckMatches = await matchesRepository.GetMatchesNeedingAutoCheckAsync(3500);

            if (apiMatch is null && autoCheckMatches.Count == 0)
            {
                logger.LogTrace(
                    "No matches need processing, sleeping for {Interval} seconds",
                    IntervalSeconds
                );
                await Task.Delay(TimeSpan.FromSeconds(IntervalSeconds), cancellationToken);
                continue;
            }

            if (apiMatch is not null)
            {
                await ProcessMatchesOsuApiAsync(apiMatch, matchesRepository, apiMatchService);
            }

            if (autoCheckMatches.Count == 0)
            {
                continue;
            }

            foreach (Match match in autoCheckMatches)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }

                ProcessMatchNeedingAutomatedChecksAsync(
                    match,
                    gamesRepository
                );
            }

            await context.SaveChangesAsync(cancellationToken);
        }
    }

    /// <summary>
    /// Updates the match & navigation properties with the results of automated checks.
    /// Does not call the database for updates
    /// </summary>
    /// <param name="match">The match to update</param>
    /// <param name="gamesRepository">Games repository</param>
    private void ProcessMatchNeedingAutomatedChecksAsync(
        Match match,
        IGamesRepository gamesRepository
    )
    {
        logger.LogInformation("Performing automated checks on match {Match}", match.MatchId);
        // Match verification checks
        if (!MatchAutomationChecks.PassesAllChecks(match))
        {
            match.VerificationStatus = (int)MatchVerificationStatus.Rejected;
            match.VerificationSource = (int)MatchVerificationSource.System;
            match.VerificationInfo = "Failed automation checks";

            match.NeedsAutoCheck = false;
            logger.LogInformation("Match {Match} failed automation checks", match.MatchId);
        }
        else
        {
            if (match.VerifiedBy is not null)
            {
                match.VerificationStatus = (int)MatchVerificationStatus.Verified;
            }
            else
            {
                match.VerificationStatus = (int)MatchVerificationStatus.PreVerified;
            }

            match.VerificationSource = (int)MatchVerificationSource.System;
            match.VerificationInfo = null;
        }

        // Game verification checks
        foreach (Game game in match.Games)
        {
            // NOTE: MUST be done in this order: Scores -> Game
            // The game checker has a sanity check to ensure the game scores are valid.
            // If a game has any invalid scores, it will fail.

            // Score verification checks
            foreach (MatchScore score in game.MatchScores)
            {
                if (!ScoreAutomationChecks.PassesAutomationChecks(score))
                {
                    if (score.IsValid != false)
                    {
                        // Avoid unnecessary db calls
                        score.IsValid = false;
                        // await matchScoresRepository.UpdateAsync(score);
                    }

                    logger.LogDebug(
                        "Score [Player: {Player} | Game: {Game}] failed automation checks",
                        score.PlayerId,
                        game.GameId
                    );
                }
                else
                {
                    if (score.IsValid != true)
                    {
                        score.IsValid = true;
                        // await matchScoresRepository.UpdateAsync(score);
                    }

                    logger.LogTrace(
                        "Score [Player: {Player} | Game: {Game}] passed automation checks",
                        score.PlayerId,
                        game.GameId
                    );
                }
            }

            GameRejectionReason? rejectionReason = GameAutomationChecks.IdentifyRejectionReason(game);

            if (rejectionReason is not null)
            {
                game.VerificationStatus = (int)GameVerificationStatus.Rejected;
                game.RejectionReason = (int)rejectionReason;
                logger.LogInformation("Game {Game} failed automation checks with reason {Reason}",
                    game.GameId, rejectionReason.ToString());
            }
            else
            {
                // Game has passed automation checks
                game.RejectionReason = null;
                if (match.VerificationStatus == (int)MatchVerificationStatus.Verified)
                {
                    game.VerificationStatus = (int)GameVerificationStatus.Verified;
                }
                else
                {
                    game.VerificationStatus = (int)GameVerificationStatus.PreVerified;
                }


                logger.LogDebug(
                    "Game {Game} passed automation checks and is marked as {Status}",
                    game.GameId,
                    (GameVerificationStatus)game.VerificationStatus
                );
            }

            // await gamesRepository.UpdateAsync(game);
            gamesRepository.MarkUpdated(game);
        }

        match.NeedsAutoCheck = false;
        logger.LogInformation("Match {Match} has completed automated checks", match.MatchId);
    }

    private async Task ProcessMatchesOsuApiAsync(
        Match match,
        IMatchesRepository matchesRepository,
        IApiMatchRepository apiMatchRepository
    )
    {
        try
        {
            // Matches at this point should only contain data posted from the web interface.
            // We need to call the osu! API on these matches and persist them.
            Match? updatedEntity = await ProcessMatchAsync(match.MatchId, apiMatchRepository, matchesRepository);

            if (updatedEntity == null)
            {
                logger.LogWarning(
                    "Failed to process match {MatchId} because result from ApiMatchService processing was null",
                    match.MatchId
                );
            }
        }
        catch (Exception e)
        {
            logger.LogError(
                "Failed to automatically process match {MatchId}: {Message}",
                match.MatchId,
                e.Message
            );
        }
    }

    private async Task<Match?> ProcessMatchAsync(
        long osuMatchId,
        IApiMatchRepository apiMatchRepository,
        IMatchesRepository matchesRepository
    )
    {
        OsuApiMatchData? osuMatch = await apiService.GetMatchAsync(
            osuMatchId,
            $"{osuMatchId} was identified as a match that needs to be processed"
        );
        if (osuMatch is not null)
        {
            return await apiMatchRepository.CreateFromApiMatchAsync(osuMatch);
        }

        Match? existingEntity = await matchesRepository.GetByMatchIdAsync(osuMatchId);
        if (existingEntity is null)
        {
            return existingEntity;
        }

        await matchesRepository.UpdateVerificationStatusAsync(
            osuMatchId,
            MatchVerificationStatus.Failure,
            MatchVerificationSource.System,
            "Failed to fetch match from osu! API"
        );

        await matchesRepository.UpdateAsApiProcessed(existingEntity);
        return existingEntity;
    }
}
