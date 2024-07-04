using DataWorkerService.AutomationChecks;

namespace DataWorkerService.Tests.AutomationChecks;

/// <summary>
/// Base class for testing an implementation of an <see cref="IAutomationCheck{TEntity}"/>
/// </summary>
/// <typeparam name="TAutomationCheck">Concrete implementation of an <see cref="IAutomationCheck{TEntity}"/></typeparam>
public class AutomationChecksTestBase<TAutomationCheck> where TAutomationCheck : class
{
    /// <summary>
    /// Instance of the <see cref="IAutomationCheck{TEntity}"/> implementation being tested
    /// </summary>
    protected static readonly TAutomationCheck AutomationCheck = SharedTestData.GetAutomationCheck<TAutomationCheck>();
}
