using API.DTOs;

namespace API.Services.Interfaces;

public interface IUserService
{
    /// <summary>
    /// Get a user by id
    /// </summary>
    /// <param name="id">id (primary key)</param>
    /// <returns></returns>
    Task<UserInfoDTO?> GetAsync(int id);
}
