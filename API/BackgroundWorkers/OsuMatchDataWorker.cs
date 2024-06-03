using API.Osu.AutomationChecks;
using API.Osu.Multiplayer;
using API.Repositories.Interfaces;
using API.Services.Interfaces;
using Database;
using Database.Entities;
using Database.Enums;
using Database.Repositories.Interfaces;

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
            IApiMatchService apiMatchService = scope.ServiceProvider.GetRequiredService<IApiMatchService>();
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
        // Match verification checks
        if (MatchAutomationChecks.PassesAllChecks(match))
        {
            match.VerificationStatus = match.VerifierUserId is not null ? Old_MatchVerificationStatus.Verified : Old_MatchVerificationStatus.PreVerified;
            match.VerificationSource = Old_MatchVerificationSource.System;
            match.VerificationInfo = null;
        }
        else
        {
            match.VerificationStatus = Old_MatchVerificationStatus.Rejected;
            match.VerificationSource = Old_MatchVerificationSource.System;
            match.VerificationInfo = "Failed automation checks";

            match.NeedsAutoCheck = false;
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
                score.IsValid = ScoreAutomationChecks.PassesAutomationChecks(score);
            }

            Old_GameRejectionReason? rejectionReason = GameAutomationChecks.IdentifyRejectionReason(game);

            if (rejectionReason is not null)
            {
                game.VerificationStatus = Old_GameVerificationStatus.Rejected;
                game.RejectionReason = rejectionReason;
                logger.LogInformation("Game {Game} failed automation checks with reason {Reason}",
                    game.GameId, rejectionReason.ToString());
            }
            else
            {
                // Game has passed automation checks
                game.RejectionReason = null;
                game.VerificationStatus = match.VerificationStatus == Old_MatchVerificationStatus.Verified ? Old_GameVerificationStatus.Verified : Old_GameVerificationStatus.PreVerified;
            }

            gamesRepository.MarkUpdated(game);
        }

        match.NeedsAutoCheck = false;
        logger.LogInformation("Match {Match} has completed automated checks", match.MatchId);
    }

    private async Task ProcessMatchesOsuApiAsync(
        Match match,
        IMatchesRepository matchesRepository,
        IApiMatchService apiMatchService
    )
    {
        try
        {
            // Matches at this point should only contain data posted from the web interface.
            // We need to call the osu! API on these matches and persist them.
            Match? updatedEntity = await ProcessMatchAsync(match.MatchId, apiMatchService, matchesRepository);

            if (updatedEntity == null)
            {
                logger.LogInformation(
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
        IApiMatchService apiMatchService,
        IMatchesRepository matchesRepository
    )
    {
        OsuApiMatchData? osuMatch = await apiService.GetMatchAsync(
            osuMatchId,
            $"{osuMatchId} was identified as a match that needs to be processed"
        );
        if (osuMatch is not null)
        {
            return await apiMatchService.CreateFromApiMatchAsync(osuMatch);
        }

        Match? existingEntity = await matchesRepository.GetByMatchIdAsync(osuMatchId);
        if (existingEntity is null)
        {
            return existingEntity;
        }

        try
        {
            await matchesRepository.UpdateVerificationStatusAsync(
                existingEntity.Id,
                Old_MatchVerificationStatus.Failure,
                Old_MatchVerificationSource.System,
                "Failed to fetch match from osu! API"
            );

            await matchesRepository.UpdateAsApiProcessed(existingEntity);
            return existingEntity;
        }
        catch (Exception e)
        {
            // Something has seriously gone wrong.
            // It'll be marked as API processed, then it will get run for
            // automated checks. This exception gets thrown for bad
            // match links / invalid data. In testing, items caught here
            // will get handled by the automated checks just fine.
            await matchesRepository.UpdateAsApiProcessed(existingEntity);
            logger.LogError(
                "Failed to update match {MatchId} as processed: {Message}",
                existingEntity.MatchId,
                e.Message
            );

            return existingEntity;
        }
    }
}
