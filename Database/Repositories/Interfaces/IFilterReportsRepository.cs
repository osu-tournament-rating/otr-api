using Database.Entities;

namespace Database.Repositories.Interfaces;

/// <summary>
/// Repository for FilterReport entities
/// </summary>
public interface IFilterReportsRepository : IRepository<FilterReport>
{
    /// <summary>
    /// Gets a filter report by ID including all related player results
    /// </summary>
    /// <param name="id">The filter report ID</param>
    /// <returns>The filter report with player results, or null if not found</returns>
    Task<FilterReport?> GetWithPlayersAsync(int id);
}
