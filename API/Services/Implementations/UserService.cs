using API.DTOs;
using API.Entities;
using API.Enums;
using API.Repositories.Interfaces;
using API.Services.Interfaces;
using AutoMapper;

namespace API.Services.Implementations;

public class UserService(IUserRepository userRepository, IMatchesRepository matchesRepository, IMapper mapper) : IUserService
{
    public async Task<bool> ExistsAsync(int id) =>
        await userRepository.ExistsAsync(id);

    public async Task<UserDTO?> GetAsync(int id) =>
        mapper.Map<UserDTO?>(await userRepository.GetAsync(id));

    public async Task<IEnumerable<OAuthClientDTO>?> GetClientsAsync(int id) =>
        mapper.Map<IEnumerable<OAuthClientDTO>?>(await userRepository.GetClientsAsync(id));

    public async Task<IEnumerable<MatchSubmissionStatusDTO>?> GetSubmissionsAsync(int id) =>
        mapper.Map<IEnumerable<MatchSubmissionStatusDTO>?>(await userRepository.GetSubmissionsAsync(id));

    public async Task<bool> RejectSubmissionsAsync(int id, int? rejecterUserId,
        MatchVerificationSource verificationSource)
    {
        IEnumerable<Match>? submissions = (await userRepository.GetAsync(id))?.SubmittedMatches?.ToList();
        if (submissions is null)
        {
            // Return in the affirmative if the user has no submissions
            return true;
        }

        foreach (Match match in submissions)
        {
            match.VerificationStatus = MatchVerificationStatus.Rejected;
            match.VerifierUserId = rejecterUserId;
            match.VerificationSource = verificationSource;
        }

        return await matchesRepository.UpdateAsync(submissions, rejecterUserId) == submissions.Count();
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
