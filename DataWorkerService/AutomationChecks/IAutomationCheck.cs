using Database.Entities.Interfaces;

namespace DataWorkerService.AutomationChecks;

/// <summary>
/// Interfaces an automated verification check ran against a <typeparamref name="TEntity"/> as part of the processing flow
/// </summary>
/// <typeparam name="TEntity">Type of database entity to be checked</typeparam>
public interface IAutomationCheck<TEntity> where TEntity : class, IProcessableEntity
{
    /// <summary>
    /// The order in which the <see cref="IAutomationCheck{TEntity}"/> will be run
    /// </summary>
    /// <remarks>
    /// Higher values indicate the <see cref="IAutomationCheck{TEntity}"/> will be run after other
    /// <see cref="IAutomationCheck{TEntity}"/>s with a lesser <see cref="Order"/>
    /// </remarks>
    int Order => 0;

    /// <summary>
    /// Checks the given entity
    /// </summary>
    /// <param name="entity">Entity to be checked</param>
    /// <returns>True if the entity passed the automation check</returns>
    public bool Check(TEntity entity);
}
