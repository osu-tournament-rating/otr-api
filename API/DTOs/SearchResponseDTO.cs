namespace API.DTOs;

public class SearchResponseDTO
{
    public string Text { get; set; } = string.Empty;
    public string? Url { get; set; }
    public string? Thumbnail { get; set; }
}
