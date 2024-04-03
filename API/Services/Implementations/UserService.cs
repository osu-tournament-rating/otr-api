using API.DTOs;
using API.Entities;
using API.Enums;
using API.Repositories.Interfaces;
using API.Services.Interfaces;
using API.Utilities;
using AutoMapper;

namespace API.Services.Implementations;

public class UserService(IUserRepository userRepository, IMatchesRepository matchesRepository, IMapper mapper) : IUserService
{
    public async Task<bool> ExistsAsync(int id) =>
        await userRepository.ExistsAsync(id);

    public async Task<UserDTO?> GetAsync(int id)
    {
        User? user = await userRepository.GetAsync(id);

        if (user is null)
        {
            return null;
        }

        return new UserDTO
        {
            Id = user.Id,
            Scopes = user.Scopes,
            PlayerId = user.PlayerId,
            Country = user.Player.Country,
            OsuId = user.Player.OsuId, // TODO: Set to user's preferred mode
            Username = user.Player.Username
        };
    }

    public async Task<IEnumerable<OAuthClientDTO>?> GetClientsAsync(int id) =>
        mapper.Map<IEnumerable<OAuthClientDTO>?>((await userRepository.GetAsync(id))?.Clients?.ToList());

    public async Task<IEnumerable<MatchSubmissionStatusDTO>?> GetSubmissionsAsync(int id) =>
        mapper.Map<IEnumerable<MatchSubmissionStatusDTO>?>((await userRepository.GetAsync(id))?.SubmittedMatches);

    public async Task<bool> RejectSubmissionsAsync(int id, int? verifierId, int? verificationSource)
    {
        IEnumerable<Match>? submissions = (await userRepository.GetAsync(id))?.SubmittedMatches?.ToList();
        if (submissions is null)
        {
            // Return in the affirmative if the user has no submissions
            return true;
        }

        foreach (Match match in submissions)
        {
            match.VerificationStatus = (int)MatchVerificationStatus.Rejected;
            match.VerifierUserId = verifierId;
            match.VerificationSource = verificationSource;
        }

        return await matchesRepository.UpdateAsync(submissions, verifierId) == submissions.Count();
    }

    public async Task<UserDTO?> UpdateScopesAsync(int id, IEnumerable<string> scopes)
    {
        User? user = await userRepository.GetAsync(id);
        if (user is null)
        {
            return null;
        }

        scopes = scopes.ToList();
        // Ensure user scope is always present
        if (!scopes.Contains(OtrClaims.User))
        {
            scopes = scopes.Append(OtrClaims.User);
        }

        user.Scopes = scopes.ToArray();
        await userRepository.UpdateAsync(user);

        return mapper.Map<UserDTO>(user);
    }
}
