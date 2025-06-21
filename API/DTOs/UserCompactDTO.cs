using JetBrains.Annotations;

namespace API.DTOs;

/// <summary>
/// Represents user information
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.Members)]
public class UserCompactDTO
{
    /// <summary>
    /// Id
    /// </summary>
    public int Id { get; init; }

    /// <summary>
    /// Timestamp of the user's last login to the o!TR website
    /// </summary>
    public DateTime? LastLogin { get; init; }

    /// <summary>
    /// The associated player
    /// </summary>
    public PlayerCompactDTO Player { get; init; } = null!;
}
