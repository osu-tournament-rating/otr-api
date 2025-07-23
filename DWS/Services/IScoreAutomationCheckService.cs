using Database.Entities;

namespace DWS.Services;

/// <summary>
/// Service for processing automation checks on scores.
/// </summary>
public interface IScoreAutomationCheckService : IAutomationCheckService<GameScore>
{
}
