using System.Diagnostics.CodeAnalysis;
using Database.Entities;
using Database.Repositories.Interfaces;
using Database.Utilities.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

namespace Database.Repositories.Implementations;

/// <summary>
/// Repository for managing <see cref="Game"/> entities
/// </summary>
[SuppressMessage("Performance",
    "CA1862:Use the \'StringComparison\' method overloads to perform case-insensitive string comparisons")]
[SuppressMessage("ReSharper", "SpecifyStringComparison")]
public class GamesRepository(OtrContext context, ILogger<GamesRepository> logger) : RepositoryBase<Game>(context), IGamesRepository
{
    private readonly OtrContext _context = context;

    public async Task<Game?> GetAsync(int id, bool verified) =>
        await _context.Games
            .AsNoTracking()
            .IncludeChildren(verified)
            .FirstOrDefaultAsync(g => g.Id == id);

    public async Task LoadScoresAsync(Game game)
    {
        await _context.Entry(game)
            .Collection(g => g.Scores)
            .LoadAsync();
    }

    public async Task<Game?> MergeScoresAsync(int targetGameId, IEnumerable<int> sourceGameIds)
    {
        var gameIds = sourceGameIds.ToList();

        await using IDbContextTransaction transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            Game? targetGame = await _context.Games
                .Include(g => g.Match)
                .Include(g => g.Scores)
                .FirstOrDefaultAsync(g => g.Id == targetGameId);

            if (targetGame is null)
            {
                logger.LogDebug("Game merge failed: Target game {TargetGameId} not found", targetGameId);
                return null;
            }

            List<Game> sourceGames = await _context.Games
                .Include(g => g.Match)
                .Include(g => g.Scores)
                .Where(g => gameIds.Contains(g.Id))
                .ToListAsync();

            // Check if all requested source games were found
            if (sourceGames.Count != gameIds.Count)
            {
                var foundIds = sourceGames.Select(g => g.Id).ToHashSet();
                var missingIds = gameIds.Where(id => !foundIds.Contains(id)).ToList();
                logger.LogDebug("Game merge failed: Source games not found. Missing IDs: {MissingIds}", string.Join(", ", missingIds));
                return null;
            }

            // Verify all source games are from the same match as the target game
            if (sourceGames.Any(g => g.MatchId != targetGame.MatchId))
            {
                var differentMatchGames = sourceGames
                    .Where(g => g.MatchId != targetGame.MatchId)
                    .Select(g => $"Game {g.Id} (Match {g.MatchId})")
                    .ToList();
                logger.LogDebug("Game merge failed: Games from different matches. Target match: {TargetMatchId}, Mismatched games: {MismatchedGames}",
                    targetGame.MatchId, string.Join(", ", differentMatchGames));
                return null;
            }

            // Verify all source games have the same beatmap as the target game
            if (sourceGames.Any(g => g.BeatmapId != targetGame.BeatmapId))
            {
                var differentBeatmapGames = sourceGames
                    .Where(g => g.BeatmapId != targetGame.BeatmapId)
                    .Select(g => $"Game {g.Id} (Beatmap {g.BeatmapId})")
                    .ToList();
                logger.LogDebug("Game merge failed: Games have different beatmaps. Target beatmap: {TargetBeatmapId}, Mismatched games: {MismatchedGames}",
                    targetGame.BeatmapId, string.Join(", ", differentBeatmapGames));
                return null;
            }

            // Check for duplicate players
            var targetPlayerIds = targetGame.Scores.Select(s => s.PlayerId).ToHashSet();

            foreach (Game sourceGame in sourceGames)
            {
                foreach (GameScore score in sourceGame.Scores)
                {
                    if (targetPlayerIds.Add(score.PlayerId))
                    {
                        continue;
                    }

                    logger.LogDebug("Game merge failed: Duplicate player detected. Player {PlayerId} already has a score in target game {TargetGameId} and also in source game {SourceGameId}",
                        score.PlayerId, targetGameId, sourceGame.Id);

                    return null;
                }
            }

            // Move all scores from source games to target game
            foreach (GameScore score in sourceGames.SelectMany(sourceGame => sourceGame.Scores))
            {
                score.Game = targetGame;
            }

            // Save before deleting source games
            await _context.SaveChangesAsync();

            // Delete source games using tracked deletion to trigger auditing
            _context.Games.RemoveRange(sourceGames);
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();

            logger.LogDebug("Game merge successful: Merged {SourceGameCount} games into target game {TargetGameId}. Source game IDs: {SourceGameIds}",
                sourceGames.Count, targetGameId, string.Join(", ", sourceGames.Select(g => g.Id)));

            return await GetAsync(targetGameId, false);
        }
        catch (DbUpdateException ex)
        {
            // Rollback the transaction on failure
            await transaction.RollbackAsync();

            // Catch database exceptions (e.g., constraint violations)
            logger.LogDebug(ex, "Game merge failed: Database exception occurred while merging games. Target: {TargetGameId}, Sources: {SourceGameIds}",
                targetGameId, string.Join(", ", gameIds));
            return null;
        }
    }
}
