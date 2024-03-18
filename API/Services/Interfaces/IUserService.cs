using API.DTOs;

namespace API.Services.Interfaces;

public interface IUserService
{
    /// <summary>
    /// Get a <see cref="UserInfoDTO"/> for user by id
    /// </summary>
    /// <param name="id">User id (primary key)</param>
    /// <returns></returns>
    Task<UserInfoDTO?> GetAsync(int id);
}
