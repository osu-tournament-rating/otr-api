using API.Entities;

namespace API.Repositories.Interfaces;

public interface IUserSettingsRepository : IRepository<UserSettings>
{
    /// <summary>
    /// Generates default settings for a user using values from the associated player with the given playerId
    /// </summary>
    /// <remarks>
    /// Does not save the generated settings to the database. They are meant to be set to
    /// <see cref="User.Settings"/> when creating a new User entity.
    /// </remarks>
    Task<UserSettings> GenerateDefaultAsync(int playerId);
}
