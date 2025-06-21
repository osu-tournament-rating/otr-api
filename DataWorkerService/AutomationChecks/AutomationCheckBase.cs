using Database.Entities.Interfaces;

namespace DataWorkerService.AutomationChecks;

/// <summary>
/// Generic implementation of a <see cref="IAutomationCheck{TEntity}"/> that handles simple logging
/// </summary>
/// <param name="logger">Logger</param>
/// <typeparam name="TEntity">Type of database entity to be checked. Must implement <see cref="IProcessableEntity"/></typeparam>
public abstract class AutomationCheckBase<TEntity>(
    ILogger logger
) : IAutomationCheck<TEntity> where TEntity : class, IProcessableEntity
{
    public virtual int Order => 0;

    private const LogLevel LogLevelOnPass = LogLevel.Trace;

    private const LogLevel LogLevelOnFail = LogLevel.Trace;

    public virtual bool Check(TEntity entity)
    {
        bool passed = OnChecking(entity);

        if (passed)
        {
            OnPass(entity);
        }
        else
        {
            OnFail(entity);
        }

        return passed;
    }

    /// <summary>
    /// Handles when the entity passes the automation check
    /// </summary>
    /// <param name="entity">Entity that was checked</param>
    protected virtual void OnPass(TEntity entity)
    {
        logger.Log(LogLevelOnPass, "Automation check passed [Id: {Id}]", entity.Id);
    }

    /// <summary>
    /// Handles when the entity fails the automation check
    /// </summary>
    /// <param name="entity">Entity that was checked</param>
    protected virtual void OnFail(TEntity entity)
    {
        logger.Log(LogLevelOnFail, "Automation check failed [Id: {Id}]", entity.Id);
    }

    /// <summary>
    /// Business logic of the automation check
    /// </summary>
    /// <param name="entity">Entity to be checked</param>
    /// <returns>True if the entity passes the check</returns>
    protected abstract bool OnChecking(TEntity entity);
}
