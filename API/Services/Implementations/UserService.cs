using API.DTOs;
using API.Repositories.Interfaces;
using API.Services.Interfaces;
using AutoMapper;
using Database.Entities;
using Database.Enums;
using Database.Repositories.Interfaces;

namespace API.Services.Implementations;

public class UserService(IUserRepository userRepository, IMatchesRepository matchesRepository, IMapper mapper) : IUserService
{
    public async Task<bool> ExistsAsync(int id) =>
        await userRepository.ExistsAsync(id);

    public async Task<UserDTO?> GetAsync(int id) =>
        mapper.Map<UserDTO?>(await userRepository.GetAsync(id));

    public async Task<int?> GetPlayerIdAsync(int id) =>
        await userRepository.GetPlayerIdAsync(id);

    public async Task<IEnumerable<OAuthClientDTO>?> GetClientsAsync(int id) =>
        mapper.Map<IEnumerable<OAuthClientDTO>?>(await userRepository.GetClientsAsync(id));

    public async Task<IEnumerable<MatchSubmissionStatusDTO>?> GetSubmissionsAsync(int id) =>
        mapper.Map<IEnumerable<MatchSubmissionStatusDTO>?>(await userRepository.GetSubmissionsAsync(id));

    public async Task<bool> RejectSubmissionsAsync(int id, int? rejecterUserId,
        Old_MatchVerificationSource verificationSource)
    {
        IEnumerable<Match>? submissions = (await userRepository.GetAsync(id))?.SubmittedMatches?.ToList();
        if (submissions is null)
        {
            // Return in the affirmative if the user has no submissions
            return true;
        }

        foreach (Match match in submissions)
        {
            match.VerificationStatus = Old_MatchVerificationStatus.Rejected;
            match.VerifierUserId = rejecterUserId;
            match.VerificationSource = verificationSource;
        }

        return await matchesRepository.UpdateAsync(submissions) == submissions.Count();
    }

    public async Task<UserDTO?> UpdateScopesAsync(int id, IEnumerable<string> scopes)
    {
        User? user = await userRepository.GetAsync(id);
        if (user is null)
        {
            return null;
        }

        user.Scopes = scopes.ToArray();
        await userRepository.UpdateAsync(user);

        return mapper.Map<UserDTO>(user);
    }
}
