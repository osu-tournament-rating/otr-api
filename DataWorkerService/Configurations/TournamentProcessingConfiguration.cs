using JetBrains.Annotations;

namespace DataWorkerService.Configurations;

/// <summary>
/// Values that control the way <see cref="Database.Entities.Match"/> data is fetched from outside sources
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.Members)]
public class TournamentProcessingConfiguration
{
    public const string Position = "Tournaments";

    /// <summary>
    /// Denotes if the worker should fetch data for matches
    /// </summary>
    public bool Enabled { get; init; } = true;

    /// <summary>
    /// The number of matches to fetch in succession
    /// </summary>
    public int BatchSize { get; init; } = 1;

    /// <summary>
    /// Time to wait between batches in seconds
    /// </summary>
    public int BatchIntervalSeconds { get; init; } = 60;
}
