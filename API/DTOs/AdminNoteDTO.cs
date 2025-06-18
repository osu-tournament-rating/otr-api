using JetBrains.Annotations;

namespace API.DTOs;

/// <summary>
/// Represents a note for an entity created by an admin
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.Members)]
public class AdminNoteDTO
{
    /// <summary>
    /// The id of the admin note
    /// </summary>
    public int Id { get; init; }

    /// <summary>
    /// Timestamp of creation
    /// </summary>
    public DateTime Created { get; init; }

    /// <summary>
    /// Timestamp of the last update, if available
    /// </summary>
    public DateTime? Updated { get; init; }

    /// <summary>
    /// Id of the parent entity
    /// </summary>
    public int ReferenceId { get; init; }

    /// <summary>
    /// The admin user that created the note
    /// </summary>
    public UserCompactDTO AdminUser { get; init; } = null!;

    /// <summary>
    /// Content of the note
    /// </summary>
    public string Note { get; set; } = string.Empty;
}
