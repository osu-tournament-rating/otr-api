namespace API.DTOs;

/// <summary>
/// Represents a note for an entity created by an admin
/// </summary>
public class AdminNoteDTO
{
    /// <summary>
    /// Id
    /// </summary>
    public int Id { get; init; }

    /// <summary>
    /// Timestamp of creation
    /// </summary>
    public DateTime Created { get; init; }

    /// <summary>
    /// Timestamp of the last update if available
    /// </summary>
    public DateTime? Updated { get; init; }

    /// <summary>
    /// Id of the parent entity
    /// </summary>
    public int ReferenceId { get; init; }

    /// <summary>
    /// Id of the admin user that created the note
    /// </summary>
    public int AdminUserId { get; set; }

    /// <summary>
    /// Username of the admin user that created the note
    /// </summary>
    public string? AdminUsername { get; init; }

    /// <summary>
    /// Content of the note
    /// </summary>
    public string Note { get; set; } = string.Empty;
}
