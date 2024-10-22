namespace API.DTOs;

/// <summary>
/// Represents user information including optional data
/// </summary>
public class UserDTO : UserCompactDTO
{
    /// <summary>
    /// List of permissions granted to the user
    /// </summary>
    public string[] Scopes { get; init; } = [];

    /// <summary>
    /// Settings of the user
    /// </summary>
    public UserSettingsDTO Settings { get; init; } = null!;
}
