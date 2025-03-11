using System.Collections;
using Common.Enums.Enums;
using DataWorkerService.AutomationChecks;
using Microsoft.Extensions.Logging;
using Serilog.Extensions.Logging;
using TestingUtils.SeededData;

namespace DataWorkerService.Tests;

/// <summary>
/// Test data shared by multiple tests
/// </summary>
public static class SharedTestData
{
    /// <summary>
    /// Creates an instance of an <see cref="IAutomationCheck{TEntity}"/>
    /// </summary>
    /// <typeparam name="TAutomationCheck">Concrete implementation of an <see cref="IAutomationCheck{TEntity}"/></typeparam>
    /// <returns>An instance of <typeparamref name="TAutomationCheck"/></returns>
    /// <exception cref="InvalidOperationException">Thrown if the instance could not be created</exception>
    public static TAutomationCheck GetAutomationCheck<TAutomationCheck>() where TAutomationCheck : class =>
        Activator.CreateInstance(
            typeof(TAutomationCheck),
            [new Logger<TAutomationCheck>(new SerilogLoggerFactory())]
        ) as TAutomationCheck
        ?? throw new InvalidOperationException($"Could not create AutomationCheck [Name: {nameof(TAutomationCheck)}]");

    /// <summary>
    /// <see cref="Mods"/> test cases for validity tests. <br/>
    /// Formatted { <see cref="Mods"/> mod, bool expectedPass }
    /// </summary>
    public class ModTestData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            foreach (Mods mod in Constants.InvalidMods)
            {
                yield return [mod, false];
            }
            yield return [Mods.SuddenDeath | Mods.SpunOut, false];
            yield return [Mods.Perfect | Mods.Relax, false];
            yield return [Mods.None, true];
            yield return [Mods.NoFail, true];
            yield return [Mods.Hidden, true];
            yield return [Mods.Easy, true];
            yield return [Mods.DoubleTime | Mods.HardRock, true];
            yield return [Mods.Hidden | Mods.Flashlight, true];
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    /// <summary>
    /// <see cref="DateTime"/> test cases for EndTime validity tests. <br/>
    /// Formatted { <see cref="DateTime"/> mod, bool expectedPass }
    /// </summary>
    public class EndTimeTestData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return [SeededDate.Placeholder, false];
            yield return [DateTime.MinValue, false];
            yield return [SeededDate.Generate(), true];
            yield return [DateTime.MaxValue, true];
            yield return [DateTime.Now, true];
            yield return [new DateTime(2009, 9, 16), true];
            yield return [new DateTime(2018, 5, 29), true];
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    /// <summary>
    /// <see cref="Nullable{DateTime}"/> test cases for EndTime validity tests. <br/>
    /// Formatted { <see cref="Nullable{DateTime}"/> mod, bool expectedPass }
    /// </summary>
    public class EndTimeNullableTestData : IEnumerable<object?[]>
    {
        public IEnumerator<object?[]> GetEnumerator()
        {
            yield return [SeededDate.Placeholder, true];
            yield return [DateTime.MinValue, true];
            yield return [SeededDate.Generate(), true];
            yield return [DateTime.MaxValue, true];
            yield return [DateTime.Now, true];
            yield return [new DateTime(2009, 9, 16), true];
            yield return [new DateTime(2018, 5, 29), true];
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    /// <summary>
    /// <see cref="Ruleset"/> test cases for validity tests. <br/>
    /// Formatted { <see cref="Ruleset"/> entityRuleset, <see cref="Ruleset"/> tournamentRuleset, bool expectedPass }
    /// </summary>
    public class RulesetTestData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            foreach (Ruleset ruleset in Enum.GetValues<Ruleset>())
            {
                yield return [ruleset, ruleset, true];
            }
            yield return [Ruleset.Osu, Ruleset.Taiko, false];
            yield return [Ruleset.Taiko, Ruleset.Catch, false];
            yield return [Ruleset.Catch, Ruleset.ManiaOther, false];
            yield return [Ruleset.ManiaOther, Ruleset.Osu, false];
            yield return [Ruleset.ManiaOther, Ruleset.Mania4k, false];
            yield return [Ruleset.ManiaOther, Ruleset.Mania7k, false];
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
