using AutoMapper;
using Common.Enums;
using Common.Enums.Verification;
using Database;
using Database.Entities;
using Database.Repositories.Interfaces;
using DWS.Messages;
using DWS.Services.Interfaces;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using OsuApiClient;
using OsuApiClient.Domain.Osu.Multiplayer;
using OsuApiClient.Enums;
using ApiGameScore = OsuApiClient.Domain.Osu.Multiplayer.GameScore;
using DbGameScore = Database.Entities.GameScore;

namespace DWS.Services.Implementations;

public class MatchFetchService(
    ILogger<MatchFetchService> logger,
    OtrContext context,
    IMatchesRepository matchesRepository,
    IGamesRepository gamesRepository,
    IGameScoresRepository gameScoresRepository,
    IPlayersRepository playersRepository,
    IBeatmapsRepository beatmapsRepository,
    IOsuClient osuClient,
    IPublishEndpoint publishEndpoint,
    ITournamentDataCompletionService dataCompletionService,
    IMapper mapper)
    : IMatchFetchService
{
    public async Task<bool> FetchAndPersistMatchAsync(long osuMatchId, CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Fetching match {OsuMatchId} from osu! API", osuMatchId);

        try
        {
            MultiplayerMatch? osuMatch = await osuClient.GetMatchAsync(osuMatchId, cancellationToken);

            if (osuMatch is null)
            {
                logger.LogWarning("Match {OsuMatchId} not found in osu! API", osuMatchId);

                IEnumerable<Match> existingMatches = await matchesRepository.GetAsync([osuMatchId]);
                Match? existingMatch = existingMatches.FirstOrDefault();

                if (existingMatch is not null)
                {
                    await dataCompletionService.UpdateMatchFetchStatusAsync(existingMatch.Id, DataFetchStatus.NotFound, cancellationToken);
                }

                return false;
            }

            int matchId = await ProcessMatchAsync(osuMatch, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);

            await dataCompletionService.UpdateMatchFetchStatusAsync(matchId, DataFetchStatus.Fetched, cancellationToken);

            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing match {OsuMatchId}", osuMatchId);

            IEnumerable<Match> existingMatches = await matchesRepository.GetAsync([osuMatchId]);
            Match? existingMatch = existingMatches.FirstOrDefault();

            if (existingMatch is not null)
            {
                await dataCompletionService.UpdateMatchFetchStatusAsync(existingMatch.Id, DataFetchStatus.Error, cancellationToken);
            }

            throw;
        }
    }

    /// <summary>
    /// Processes a match from the osu! API and persists it to the database.
    /// </summary>
    /// <param name="osuMatch">The match data from osu! API</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The database ID of the processed match</returns>
    private async Task<int> ProcessMatchAsync(MultiplayerMatch osuMatch, CancellationToken cancellationToken)
    {
        IEnumerable<Match> existingMatches = await matchesRepository.GetAsync([osuMatch.Match.Id]);
        Match? existingMatch = existingMatches.FirstOrDefault();

        Match match;
        if (existingMatch is not null)
        {
            match = existingMatch;
            mapper.Map(osuMatch, match);
            match.DataFetchStatus = DataFetchStatus.Fetching;
            await matchesRepository.UpdateAsync(match);
        }
        else
        {
            match = mapper.Map<Match>(osuMatch);
            match.DataFetchStatus = DataFetchStatus.Fetching;
            await matchesRepository.CreateAsync(match);
        }

        await ProcessPlayersAsync(osuMatch);

        await ProcessGamesAsync(match, osuMatch, cancellationToken);

        return match.Id;
    }


    /// <summary>
    /// Processes and creates players from a match if they don't exist.
    /// </summary>
    /// <param name="apiMatch">The match data containing player information.</param>
    private async Task ProcessPlayersAsync(MultiplayerMatch apiMatch)
    {
        var playerIds = apiMatch.Users
            .Select(u => u.Id)
            .Distinct()
            .ToList();

        IEnumerable<Player> existingPlayers = await playersRepository.GetAsync(playerIds);
        var existingPlayerIds = existingPlayers.Select(p => p.OsuId).ToHashSet();

        var newPlayers = apiMatch.Users
            .Where(u => !existingPlayerIds.Contains(u.Id))
            .Select(mapper.Map<Player>)
            .DistinctBy(p => p.OsuId)
            .ToList();

        if (newPlayers.Count != 0)
        {
            foreach (Player player in newPlayers)
            {
                await playersRepository.CreateAsync(player);
            }
        }
    }

    /// <summary>
    /// Processes games from the match events and persists them.
    /// </summary>
    /// <param name="match">The database match entity.</param>
    /// <param name="apiMatch">The API match data containing game events.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    private async Task ProcessGamesAsync(Match match, MultiplayerMatch apiMatch, CancellationToken cancellationToken)
    {
        var gameEvents = apiMatch.Events
            .Where(e => e.Detail.Type == MultiplayerEventType.Game && e.Game != null)
            .Select(e => e.Game!)
            .ToList();

        foreach (MultiplayerGame apiGame in gameEvents)
        {
            Game? existingGame = await context.Games
                .Where(g => g.OsuId == apiGame.Id)
                .FirstOrDefaultAsync(cancellationToken);

            Game game;
            if (existingGame is not null)
            {
                game = existingGame;
                mapper.Map(apiGame, game);
                await gamesRepository.UpdateAsync(game);
            }
            else
            {
                game = await CreateGameFromApiAsync(match, apiGame, cancellationToken);
                await gamesRepository.CreateAsync(game);
            }

            await ProcessGameScoresAsync(game, apiGame, cancellationToken);
        }
    }

    /// <summary>
    /// Creates a new game entity from API data.
    /// </summary>
    /// <param name="match">The parent match entity.</param>
    /// <param name="apiGame">The game data from the API.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>The created game entity.</returns>
    private async Task<Game> CreateGameFromApiAsync(Match match, MultiplayerGame apiGame, CancellationToken cancellationToken)
    {
        Beatmap? beatmap = null;

        if (apiGame.BeatmapId > 0)
        {
            beatmap = await beatmapsRepository.GetAsync(apiGame.BeatmapId);
            if (beatmap is null)
            {
                beatmap = new Beatmap
                {
                    OsuId = apiGame.BeatmapId,
                    DataFetchStatus = DataFetchStatus.NotFetched
                };
                await beatmapsRepository.CreateAsync(beatmap);

                var fetchBeatmapMessage = new FetchBeatmapMessage
                {
                    BeatmapId = apiGame.BeatmapId,
                    Priority = MessagePriority.Normal
                };

                await publishEndpoint.Publish(fetchBeatmapMessage, ctx =>
                {
                    ctx.SetPriority((byte)fetchBeatmapMessage.Priority);
                }, cancellationToken);

                logger.LogDebug("Queued beatmap {BeatmapId} for fetching", apiGame.BeatmapId);
            }
            else if (beatmap.DataFetchStatus == DataFetchStatus.NotFetched)
            {
                var fetchBeatmapMessage = new FetchBeatmapMessage
                {
                    BeatmapId = apiGame.BeatmapId,
                    Priority = MessagePriority.Normal
                };

                await publishEndpoint.Publish(fetchBeatmapMessage, ctx =>
                {
                    ctx.SetPriority((byte)fetchBeatmapMessage.Priority);
                }, cancellationToken);

                logger.LogDebug("Queued existing beatmap {BeatmapId} for fetching (status: {Status})",
                    apiGame.BeatmapId, beatmap.DataFetchStatus);
            }
            else
            {
                logger.LogDebug("Beatmap {BeatmapId} already has data (status: {Status}), no fetch required",
                    apiGame.BeatmapId, beatmap.DataFetchStatus);
            }
        }

        Game game = mapper.Map<Game>(apiGame);
        game.MatchId = match.Id;
        game.BeatmapId = beatmap?.Id;

        // If the parent match is rejected, cascade the rejection to the newly created game
        if (match.VerificationStatus != VerificationStatus.Rejected)
        {
            return game;
        }

        game.VerificationStatus = VerificationStatus.Rejected;
        game.RejectionReason |= GameRejectionReason.RejectedMatch;
        logger.LogInformation("Cascaded rejection to new game {GameId} from rejected match {MatchId}", game.OsuId, match.Id);

        return game;
    }


    /// <summary>
    /// Processes and persists game scores from the API.
    /// </summary>
    /// <param name="game">The game entity to process scores for.</param>
    /// <param name="apiGame">The API game data containing scores.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    private async Task ProcessGameScoresAsync(Game game, MultiplayerGame apiGame, CancellationToken cancellationToken)
    {
        // Fetch existing scores and map them by PlayerId for efficient lookup.
        // This aligns with the database unique constraint (player_id, game_id).
        var existingScoresMap = (await context.GameScores
            .Where(gs => gs.GameId == game.Id)
            .ToListAsync(cancellationToken))
            .ToDictionary(s => s.PlayerId);

        var playerOsuIds = apiGame.Scores.Select(s => s.UserId).Distinct().ToList();
        var players = await playersRepository.GetAsync(playerOsuIds);

        // Map osu! user ID to internal database Player ID.
        var playerMap = players.ToDictionary(p => p.OsuId, p => p.Id);

        foreach (ApiGameScore apiScore in apiGame.Scores)
        {
            if (!playerMap.TryGetValue(apiScore.UserId, out int playerId))
            {
                logger.LogWarning("Player with OsuId {OsuId} not found for score in game {GameId}", apiScore.UserId, game.OsuId);
                continue;
            }

            // Check for an existing score using only the PlayerId.
            if (existingScoresMap.TryGetValue(playerId, out DbGameScore? existingScore))
            {
                // Score exists: update it with the latest data.
                mapper.Map(apiScore, existingScore);
                existingScore.Ruleset = game.Ruleset;

                if (game.VerificationStatus == VerificationStatus.Rejected)
                {
                    existingScore.VerificationStatus = VerificationStatus.Rejected;
                    existingScore.RejectionReason |= ScoreRejectionReason.RejectedGame;
                }

                await gameScoresRepository.UpdateAsync(existingScore);
            }
            else
            {
                // Score does not exist: create a new one.
                DbGameScore newScore = mapper.Map<DbGameScore>(apiScore);
                newScore.GameId = game.Id;
                newScore.PlayerId = playerId;
                newScore.Ruleset = game.Ruleset;

                if (game.VerificationStatus == VerificationStatus.Rejected)
                {
                    newScore.VerificationStatus = VerificationStatus.Rejected;
                    newScore.RejectionReason |= ScoreRejectionReason.RejectedGame;
                    logger.LogInformation("Cascaded rejection to new score for player {PlayerId} in rejected game {GameId}", playerId, game.OsuId);
                }

                await gameScoresRepository.CreateAsync(newScore);
            }
        }
    }

}
