using API.Entities;
using API.Enums;
using API.Osu.AutomationChecks;
using API.Osu.Multiplayer;
using API.Repositories.Interfaces;

namespace API.BackgroundWorkers;

public class OsuMatchDataWorker : BackgroundService
{
    private const int INTERVAL_SECONDS = 5;
    private readonly IOsuApiService _apiService;
    private readonly ILogger<OsuMatchDataWorker> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly bool _allowDataFetching;

    public OsuMatchDataWorker(
        ILogger<OsuMatchDataWorker> logger,
        IServiceProvider serviceProvider,
        IOsuApiService apiService,
        IConfiguration configuration
    )
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _apiService = apiService;
        _allowDataFetching = configuration.GetValue<bool>("Osu:AllowDataFetching");
    }

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
            _logger.LogInformation("Skipping osu! match data worker due to configuration");
            return;
        }

        _logger.LogInformation("Initialized osu! match data worker");

        while (!cancellationToken.IsCancellationRequested)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var matchesService = scope.ServiceProvider.GetRequiredService<IMatchesRepository>();
                var apiMatchService = scope.ServiceProvider.GetRequiredService<IApiMatchRepository>();
                var gamesRepository = scope.ServiceProvider.GetRequiredService<IGamesRepository>();
                var matchScoresService = scope.ServiceProvider.GetRequiredService<IMatchScoresRepository>();

                var apiMatch = await matchesService.GetFirstMatchNeedingApiProcessingAsync();
                var autoCheckMatch = await matchesService.GetFirstMatchNeedingAutoCheckAsync();

                if (apiMatch == null && autoCheckMatch == null)
                {
                    _logger.LogTrace(
                        "No matches need processing, sleeping for {Interval} seconds",
                        INTERVAL_SECONDS
                    );
                    await Task.Delay(TimeSpan.FromSeconds(INTERVAL_SECONDS), cancellationToken);
                    continue;
                }

                if (apiMatch != null)
                {
                    await ProcessMatchesOsuApiAsync(apiMatch, matchesService, apiMatchService);
                }

                if (autoCheckMatch != null)
                {
                    await ProcessMatchesNeedingAutomatedChecksAsync(
                        autoCheckMatch,
                        matchesService,
                        gamesRepository,
                        matchScoresService
                    );
                }
            }
        }
    }

    private async Task ProcessMatchesNeedingAutomatedChecksAsync(
        Match match,
        IMatchesRepository matchesRepository,
        IGamesRepository gamesRepository,
        IMatchScoresRepository matchScoresRepository
    )
    {
        _logger.LogInformation("Performing automated checks on match {Match}", match.MatchId);
        // Match verification checks
        if (!MatchAutomationChecks.PassesAllChecks(match))
        {
            match.VerificationStatus = (int)MatchVerificationStatus.Rejected;
            match.VerificationSource = (int)MatchVerificationSource.System;
            match.VerificationInfo = "Failed automation checks";

            match.NeedsAutoCheck = false;
            _logger.LogInformation("Match {Match} failed automation checks", match.MatchId);
        }
        else
        {
            if (match.VerificationStatus == (int)MatchVerificationStatus.Rejected)
            {
                // The match was previously rejected, but is now rectified of this status.

                if (match.VerifiedBy != null)
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
        }

        // Game verification checks
        foreach (var game in match.Games)
        {
            // NOTE: MUST be done in this order: Scores -> Game
            // The game checker has a sanity check to ensure the game scores are valid.
            // If a game has any invalid scores, it will fail.

            // Score verification checks
            foreach (var score in game.MatchScores)
            {
                if (!ScoreAutomationChecks.PassesAutomationChecks(score))
                {
                    if (score.IsValid != false)
                    {
                        // Avoid unnecessary db calls
                        score.IsValid = false;
                        await matchScoresRepository.UpdateAsync(score);
                    }

                    _logger.LogDebug(
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
                        await matchScoresRepository.UpdateAsync(score);
                    }

                    _logger.LogTrace(
                        "Score [Player: {Player} | Game: {Game}] passed automation checks",
                        score.PlayerId,
                        game.GameId
                    );
                }
            }

            if (!GameAutomationChecks.PassesAutomationChecks(game))
            {
                game.VerificationStatus = (int)GameVerificationStatus.Rejected;
                game.RejectionReason = (int)GameRejectionReason.FailedAutomationChecks;
                _logger.LogInformation("Game {Game} failed automation checks", game.GameId);

                await gamesRepository.UpdateAsync(game);
            }
            else
            {
                // Game has passed automation checks
                game.VerificationStatus = (int)GameVerificationStatus.PreVerified;
                if (match.VerificationStatus == (int)MatchVerificationStatus.Verified)
                {
                    game.VerificationStatus = (int)GameVerificationStatus.Verified;
                }

                _logger.LogDebug(
                    "Game {Game} passed automation checks and is marked as {Status}",
                    game.GameId,
                    (GameVerificationStatus)game.VerificationStatus
                );
            }
        }

        match.NeedsAutoCheck = false;
        await matchesRepository.UpdateAsync(match);

        _logger.LogInformation("Match {Match} has completed automated checks", match.MatchId);
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
            var updatedEntity = await ProcessMatchAsync(match.MatchId, apiMatchRepository, matchesRepository);

            if (updatedEntity == null)
            {
                _logger.LogWarning(
                    "Failed to process match {MatchId} because result from ApiMatchService processing was null",
                    match.MatchId
                );
            }
        }
        catch (Exception e)
        {
            _logger.LogError(
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
        var osuMatch = await _apiService.GetMatchAsync(
            osuMatchId,
            $"{osuMatchId} was identified as a match that needs to be processed"
        );
        if (osuMatch == null)
        {
            var existingEntity = await matchesRepository.GetByMatchIdAsync(osuMatchId);
            if (existingEntity != null)
            {
                await matchesRepository.UpdateVerificationStatusAsync(
                    osuMatchId,
                    MatchVerificationStatus.Failure,
                    MatchVerificationSource.System,
                    "Failed to fetch match from osu! API"
                );

                await matchesRepository.UpdateAsApiProcessed(existingEntity);
            }

            return null;
        }

        return await apiMatchRepository.CreateFromApiMatchAsync(osuMatch);
    }
}
