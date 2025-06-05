using API.DTOs;
using API.Services.Interfaces;
using AutoMapper;
using Common.Enums.Verification;
using Database.Entities;
using Database.Repositories.Interfaces;

namespace API.Services.Implementations;

public class UsersService(IUserRepository usersRepository, IPlayersRepository playersRepository,
    IMatchesRepository matchesRepository, IMapper mapper) : IUsersService
{
    public async Task<bool> ExistsAsync(int id) =>
        await usersRepository.ExistsAsync(id);

    public async Task<UserDTO?> GetAsync(int id) =>
        mapper.Map<UserDTO?>(await usersRepository.GetAsync(id));

    public async Task<int?> GetPlayerIdAsync(int id) =>
        await usersRepository.GetPlayerIdAsync(id);

    public async Task<IEnumerable<OAuthClientDTO>?> GetClientsAsync(int id) =>
        mapper.Map<IEnumerable<OAuthClientDTO>?>(await usersRepository.GetClientsAsync(id));

    public async Task<IEnumerable<MatchSubmissionStatusDTO>?> GetSubmissionsAsync(int id) =>
        mapper.Map<IEnumerable<MatchSubmissionStatusDTO>?>(await usersRepository.GetSubmissionsAsync(id));

    public async Task<User> LoginAsync(long osuId)
    {
        Player player = await playersRepository.GetOrCreateAsync(osuId);
        User user = await usersRepository.GetOrCreateByPlayerIdAsync(player.Id);

        user.LastLogin = DateTime.UtcNow;
        await usersRepository.UpdateAsync(user);

        return user;
    }

    public async Task<IEnumerable<PlayerCompactDTO>> GetFriendsAsync(int id) =>
        mapper.Map<IEnumerable<PlayerCompactDTO>>(await usersRepository.GetFriendsAsync(id));

    public async Task<bool> RejectSubmissionsAsync(int id, int? rejecterUserId)
    {
        IEnumerable<Match>? submissions = (await usersRepository.GetAsync(id))?.SubmittedMatches.ToList();
        if (submissions is null)
        {
            // Return in the affirmative if the user has no submissions
            return true;
        }

        foreach (Match match in submissions)
        {
            match.VerificationStatus = VerificationStatus.Rejected;
            match.VerifiedByUserId = rejecterUserId;
        }

        return await matchesRepository.UpdateAsync(submissions) == submissions.Count();
    }

    public async Task<UserDTO?> UpdateScopesAsync(int id, IEnumerable<string> scopes)
    {
        User? user = await usersRepository.GetAsync(id);
        if (user is null)
        {
            return null;
        }

        user.Scopes = [.. scopes];
        await usersRepository.UpdateAsync(user);

        return mapper.Map<UserDTO>(user);
    }
}
