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

        if (user is null)
        {
            return null;
        }

        return new UserInfoDTO
        {
            Id = user.Id,
            Scopes = user.Scopes,
            PlayerId = user.PlayerId,
            OsuCountry = user.Player.Country,
            OsuId = user.Player.OsuId,
            OsuPlayMode = 0, // TODO: Set to user's preferred mode
            OsuUsername = user.Player.Username
        };
    }
}
