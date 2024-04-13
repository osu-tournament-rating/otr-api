namespace API.DTOs;

/// <summary>
/// Represents the search result for an individual resource
/// </summary>
public class SearchResponseDTO
{
    /// <summary>
    /// Relevant name of the resource
    /// </summary>
    public required string Text { get; set; }

    /// <summary>
    /// Url to access the resource
    /// </summary>
    public required string Url { get; set; }

    /// <summary>
    /// Url of the thumbnail for the resource (specific to players at this time)
    /// </summary>
    public string? Thumbnail { get; set; }
}
