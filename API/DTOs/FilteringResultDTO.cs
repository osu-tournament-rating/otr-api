using JetBrains.Annotations;

namespace API.DTOs;

/// <summary>
/// Represents a filtering result for a collection of players
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.Members)]
public class FilteringResultDTO
{
    /// <summary>
    /// The number of players who passed filtering
    /// </summary>
    public int PlayersPassed { get; set; }

    /// <summary>
    /// The number of players who failed filtering
    /// </summary>
    public int PlayersFailed { get; set; }

    /// <summary>
    /// A collection of filtering results, one per submitted player,
    /// in the same order as submitted in the <see cref="FilteringRequestDTO"/>
    /// </summary>
    public IList<PlayerFilteringResultDTO> FilteringResults { get; set; } = [];

    /// <summary>
    /// The ID of the filter report stored in the database
    /// </summary>
    public int FilterReportId { get; set; }
}
