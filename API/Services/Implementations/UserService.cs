using API.DTOs;
using API.Entities;
using API.Repositories.Interfaces;
using API.Services.Interfaces;

namespace API.Services.Implementations;

public class UserService(IUserRepository repository) : IUserService
{
    private readonly IUserRepository _repository = repository;

    public async Task<UserInfoDTO?> GetAsync(int id)
    {
        User? user = await _repository.GetAsync(id);

        if (user == null)
        {
            return null;
        }

        return new UserInfoDTO
        {
            Id = user.PlayerId,
            UserId = user.Id,
            OsuCountry = user.Player.Country,
            OsuId = user.Player.OsuId,
            OsuPlayMode = 0, // TODO: Set to user's preferred mode
            Username = user.Player.Username,
            Roles = user.Scopes
        };
    }

    public async Task<UserInfoDTO?> GetForPlayerAsync(int playerId)
    {
        User? user = await _repository.GetForPlayerAsync(playerId);

        if (user == null)
        {
            return null;
        }

        return new UserInfoDTO
        {
            Id = user.PlayerId,
            UserId = user.Id,
            OsuCountry = user.Player.Country,
            OsuId = user.Player.OsuId,
            OsuPlayMode = 0, // TODO: Set to user's preferred mode
            Username = user.Player.Username,
            Roles = user.Scopes
        };
    }

    public async Task<User?> GetForPlayerAsync(long osuId) => await _repository.GetForPlayerAsync(osuId);

    public async Task<User?> GetOrCreateSystemUserAsync() => await _repository.GetOrCreateSystemUserAsync();

    public async Task<bool> HasRoleAsync(long osuId, string role) =>
        await _repository.HasRoleAsync(osuId, role);
}
