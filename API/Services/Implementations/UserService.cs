using API.DTOs;
using API.Entities;
using API.Enums;
using API.Repositories.Interfaces;
using API.Services.Interfaces;
using AutoMapper;

namespace API.Services.Implementations;

public class UserService(IUserRepository userRepository, IMatchesRepository matchesRepository, IMapper mapper) : IUserService
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IMatchesRepository _matchesRepository = matchesRepository;
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

    public async Task<IEnumerable<MatchSubmissionStatusDTO>?> GetSubmissionsAsync(int id) =>
        _mapper.Map<IEnumerable<MatchSubmissionStatusDTO>?>((await _userRepository.GetAsync(id))?.SubmittedMatches);

    public async Task<bool> RejectSubmissionsAsync(int id, int? verifierId, int? verificationSource)
    {
        IEnumerable<Match>? submissions = (await _userRepository.GetAsync(id))?.SubmittedMatches?.ToList();
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

        return await _matchesRepository.UpdateAsync(submissions, verifierId) == submissions.Count();
    }

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
