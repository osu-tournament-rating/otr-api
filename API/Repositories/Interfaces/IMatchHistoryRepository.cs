using API.Entities;
using API.Enums;

namespace API.Repositories.Interfaces;

public interface IMatchHistoryRepository : IRepository<MatchHistory>
{
    /// <summary>
    /// Creates a new <see cref="MatchHistory"/> mapped to the values of <paramref name="match"/> with action type <paramref name="action"/>
    /// </summary>
    /// <param name="match"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    Task<MatchHistory?> CreateAsync(Match match, HistoryActionType action);
}
