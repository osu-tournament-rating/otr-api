using API.DTOs;
using API.Entities;
using API.Repositories.Interfaces;
using API.Services.Interfaces;
using AutoMapper;

namespace API.Services.Implementations;

public class UserService(IUserRepository userRepository, IMapper mapper) : IUserService
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IMapper _mapper = mapper;

    public async Task<bool> ExistsAsync(int id) =>
        await _userRepository.ExistsAsync(id);

    public async Task<UserDTO?> GetAsync(int id)
    {
        User? user = await _userRepository.GetAsync(id);

        if (user is null)
        {
            return null;
        }

        return new UserDTO
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

    public async Task<IEnumerable<OAuthClientDTO>?> GetClientsAsync(int id) =>
        _mapper.Map<IEnumerable<OAuthClientDTO>?>((await _userRepository.GetAsync(id))?.Clients?.ToList());

    public async Task<UserDTO?> UpdateScopesAsync(int id, IEnumerable<string> scopes)
    {
        User? user = await _userRepository.GetAsync(id);
        if (user is null)
        {
            return null;
        }

        user.Scopes = scopes.ToArray();
        await _userRepository.UpdateAsync(user);

        return _mapper.Map<UserDTO>(user);
    }
}
