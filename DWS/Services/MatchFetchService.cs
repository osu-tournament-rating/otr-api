using AutoMapper;
using Common.Enums;
using Database;
using Database.Entities;
using Database.Repositories.Interfaces;
using DWS.Messages;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using OsuApiClient;
using OsuApiClient.Domain.Osu.Multiplayer;
using OsuApiClient.Enums;
using ApiGameScore = OsuApiClient.Domain.Osu.Multiplayer.GameScore;
using DbGameScore = Database.Entities.GameScore;

namespace DWS.Services;

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
    IMapper mapper)
    : IMatchFetchService
{
    public async Task<bool> FetchAndPersistMatchAsync(long osuMatchId, CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Fetching match {OsuMatchId} from osu! API", osuMatchId);

        try
        {
            // Fetch the match from osu! API
            MultiplayerMatch? osuMatch = await osuClient.GetMatchAsync(osuMatchId, cancellationToken);

            if (osuMatch is null)
            {
                logger.LogWarning("Match {OsuMatchId} not found in osu! API", osuMatchId);
                return false;
            }

            // Process the match data
            await ProcessMatchAsync(osuMatch, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);

            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing match {OsuMatchId}", osuMatchId);
            throw;
        }
    }

    private async Task ProcessMatchAsync(MultiplayerMatch osuMatch, CancellationToken cancellationToken)
    {
        // Check if match already exists
        IEnumerable<Match> existingMatches = await matchesRepository.GetAsync([osuMatch.Match.Id]);
        Match? existingMatch = existingMatches.FirstOrDefault();

        Match match;
        if (existingMatch is not null)
        {
            // Update existing match with fresh data
            match = existingMatch;
            mapper.Map(osuMatch, match);
            await matchesRepository.UpdateAsync(match);
        }
        else
        {
            // Create new match
            match = mapper.Map<Match>(osuMatch);
            await matchesRepository.CreateAsync(match);
        }

        // Process players first (needed for game scores)
        await ProcessPlayersAsync(osuMatch);

        // Process games
        await ProcessGamesAsync(match, osuMatch, cancellationToken);
    }


    private async Task ProcessPlayersAsync(MultiplayerMatch apiMatch)
    {
        // Get all unique player IDs from match users
        var playerIds = apiMatch.Users
            .Select(u => u.Id)
            .Distinct()
            .ToList();

        // Check which players already exist
        IEnumerable<Player> existingPlayers = await playersRepository.GetAsync(playerIds);
        var existingPlayerIds = existingPlayers.Select(p => p.OsuId).ToHashSet();

        // Create missing players
        var newPlayers = apiMatch.Users
            .Where(u => !existingPlayerIds.Contains(u.Id))
            .Select(u => mapper.Map<Player>(u))
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

    private async Task ProcessGamesAsync(Match match, MultiplayerMatch apiMatch, CancellationToken cancellationToken)
    {
        // Extract games from match events
        var gameEvents = apiMatch.Events
            .Where(e => e.Detail.Type == MultiplayerEventType.Game && e.Game != null)
            .Select(e => e.Game!)
            .ToList();

        foreach (MultiplayerGame apiGame in gameEvents)
        {
            // Check if game already exists
            // Note: We search by osu ID as games should be unique by their osu ID
            Game? existingGame = await context.Games
                .Where(g => g.OsuId == apiGame.Id)
                .FirstOrDefaultAsync(cancellationToken);

            Game game;
            if (existingGame is not null)
            {
                // Update existing game
                game = existingGame;
                mapper.Map(apiGame, game);
                await gamesRepository.UpdateAsync(game);
            }
            else
            {
                // Create new game
                game = await CreateGameFromApiAsync(match, apiGame, cancellationToken);
                await gamesRepository.CreateAsync(game);
            }

            // Process game scores
            await ProcessGameScoresAsync(game, apiGame, cancellationToken);
        }
    }

    private async Task<Game> CreateGameFromApiAsync(Match match, MultiplayerGame apiGame, CancellationToken cancellationToken)
    {
        Beatmap? beatmap = null;

        // Ensure beatmap exists
        if (apiGame.BeatmapId > 0)
        {
            beatmap = await beatmapsRepository.GetAsync(apiGame.BeatmapId);
            if (beatmap is null)
            {
                // Create placeholder beatmap
                beatmap = new Beatmap
                {
                    OsuId = apiGame.BeatmapId
                };
                await beatmapsRepository.CreateAsync(beatmap);

                // Save changes to persist the beatmap before creating the game
                await context.SaveChangesAsync(cancellationToken);

                // Queue it for processing
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
        }

        Game game = mapper.Map<Game>(apiGame);
        game.MatchId = match.Id;
        game.BeatmapId = beatmap?.Id;
        return game;
    }


    private async Task ProcessGameScoresAsync(Game game, MultiplayerGame apiGame, CancellationToken cancellationToken)
    {
        // Get existing scores for this game
        List<DbGameScore> existingScores = await context.GameScores
            .Include(gs => gs.Player)
            .Where(gs => gs.GameId == game.Id)
            .ToListAsync(cancellationToken);
        var existingScoreMap = new Dictionary<(int, Team), DbGameScore>();

        foreach (DbGameScore score in existingScores)
        {
            existingScoreMap[(score.PlayerId, score.Team)] = score;
        }

        foreach (ApiGameScore apiScore in apiGame.Scores)
        {
            IEnumerable<Player> players = await playersRepository.GetAsync([apiScore.UserId]);
            Player? player = players.FirstOrDefault();

            if (player is null)
            {
                logger.LogWarning("Player {PlayerId} not found for score in game {GameId}", apiScore.UserId, game.Id);
                continue;
            }

            (int Id, Team Team) key = (player.Id, apiScore.SlotInfo.Team);

            if (existingScoreMap.TryGetValue(key, out DbGameScore? existingScore))
            {
                // Update existing score
                mapper.Map(apiScore, existingScore);
                existingScore.Ruleset = game.Ruleset;
                await gameScoresRepository.UpdateAsync(existingScore);
            }
            else
            {
                // Create new score
                DbGameScore score = mapper.Map<DbGameScore>(apiScore);
                score.GameId = game.Id;
                score.PlayerId = player.Id;
                score.Ruleset = game.Ruleset;
                await gameScoresRepository.CreateAsync(score);
            }
        }
    }

}
