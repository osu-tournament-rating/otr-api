namespace API.DTOs;

public class SearchResponseDTO
{
    /// <summary>
    /// Used to store the type of object being returned. This should probably be made into an enum later, but not super important.
    /// </summary>
    public string Type { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public string? Url { get; set; }
    public string? Thumbnail { get; set; }
}
