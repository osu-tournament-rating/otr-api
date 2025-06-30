using JetBrains.Annotations;

namespace API.DTOs;

/// <summary>
/// Represents a complete filter report including metadata and results
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.Members)]
public class FilterReportDTO
{
    /// <summary>
    /// The unique identifier of the filter report
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The timestamp when the filter report was created
    /// </summary>
    public DateTime Created { get; set; }

    /// <summary>
    /// The ID of the user who created the filter report
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// The original filtering request
    /// </summary>
    public FilteringRequestDTO? Request { get; set; }

    /// <summary>
    /// The filtering results
    /// </summary>
    public FilteringResultDTO? Response { get; set; }
}
