using Database.Entities;

namespace Database.Repositories.Interfaces;

public interface IPlayerMatchStatsRepository
{
    /// <summary>
    ///  A list of all matches played by a player in a given mode between two dates. Ordered by match start time.
    /// </summary>
    /// <param name="playerId"></param>
    /// <param name="mode"></param>
    /// <param name="dateMin"></param>
    /// <param name="dateMax"></param>
    /// <returns></returns>
    Task<IEnumerable<PlayerMatchStats>> GetForPlayerAsync(
        int playerId,
        int mode,
        DateTime dateMin,
        DateTime dateMax
    );

    Task<IEnumerable<PlayerMatchStats>> TeammateStatsAsync(
        int playerId,
        int teammateId,
        int mode,
        DateTime dateMin,
        DateTime dateMax
    );

    Task<IEnumerable<PlayerMatchStats>> OpponentStatsAsync(
        int playerId,
        int opponentId,
        int mode,
        DateTime dateMin,
        DateTime dateMax
    );

    Task InsertAsync(IEnumerable<PlayerMatchStats> items);
    Task TruncateAsync();
    Task<int> CountMatchesPlayedAsync(
        int playerId,
        int mode,
        DateTime? dateMin = null,
        DateTime? dateMax = null
    );
    Task<int> CountMatchesWonAsync(
        int playerId,
        int mode,
        DateTime? dateMin = null,
        DateTime? dateMax = null
    );
    Task<double> GlobalWinrateAsync(
        int playerId,
        int mode,
        DateTime? dateMin = null,
        DateTime? dateMax = null
    );
}
