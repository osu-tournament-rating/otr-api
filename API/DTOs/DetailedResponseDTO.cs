namespace API.DTOs;

/// <summary>
/// A wrapped response that includes error details if applicable
/// </summary>
/// <typeparam name="T">Type of inner response object</typeparam>
public class DetailedResponseDTO<T>
{
    /// <summary>
    /// Inner response data
    /// </summary>
    public T? Response { get; init; }

    /// <summary>
    /// Error detail
    /// </summary>
    public string? ErrorDetail { get; init; }
}
