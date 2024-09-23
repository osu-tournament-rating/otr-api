namespace API.DTOs;

/// <summary>
/// Basic information for presenting an admin note
/// </summary>
public class AdminNoteDTO
{
    /// <summary>
    /// The content of the note
    /// </summary>
    public string Note { get; set; } = string.Empty;
    /// <summary>
    /// The author (admin user) which created this note
    /// </summary>
    public UserDTO Author { get; set; } = null!;
    /// <summary>
    /// When the note was created
    /// </summary>
    public DateTime Created { get; set; }
}
