using Database.Entities;

namespace Database.Repositories.Interfaces;

public interface IGamesRepository : IRepository<Game>
{
    /// <summary>
    /// Gets a game for the given id including all navigation properties.
    /// </summary>
    /// <remarks>The game and included navigations will not be tracked in the context</remarks>
    /// <param name="id">Game id</param>
    /// <param name="verified">Whether the game and all child navigations must be verified</param>
    public Task<Game?> GetAsync(int id, bool verified);

    /// <summary>
    /// Loads the scores for a game to enable cascading operations
    /// </summary>
    /// <param name="game">The game to load scores for</param>
    Task LoadScoresAsync(Game game);

    /// <summary>
    /// Merges game scores from source games into a target game.
    /// The source games must be from the same match and have the same beatmap as the target game.
    /// After merging, the source games are deleted.
    /// </summary>
    /// <param name="targetGameId">The ID of the game to merge scores into</param>
    /// <param name="sourceGameIds">The IDs of the games whose scores will be merged</param>
    /// <returns>The merged game if successful, null if failed</returns>
    Task<Game?> MergeScoresAsync(int targetGameId, IEnumerable<int> sourceGameIds);
}
