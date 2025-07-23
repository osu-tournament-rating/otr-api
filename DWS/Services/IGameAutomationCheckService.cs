using Database.Entities;

namespace DWS.Services;

/// <summary>
/// Service for processing automation checks on games.
/// </summary>
public interface IGameAutomationCheckService : IAutomationCheckService<Game>
{
}
