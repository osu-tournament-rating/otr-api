using API.DTOs;
using API.Services.Interfaces;
using AutoMapper;
using Common.Enums;
using Common.Enums.Verification;
using Database.Entities;
using Database.Repositories.Interfaces;

namespace API.Services.Implementations;

public class GamesService(IGamesRepository gamesRepository, IMapper mapper) : IGamesService
{
    public async Task<GameDTO?> GetAsync(int id, bool verified)
    {
        GameDTO? game = mapper.Map<GameDTO?>(await gamesRepository.GetAsync(id, verified));

        return game ?? null;
    }

    public async Task<GameDTO?> UpdateAsync(int id, GameDTO game)
    {
        Game? existing = await gamesRepository.GetAsync(id);
        if (existing is null)
        {
            return null;
        }

        // Store the original ruleset to detect changes
        Ruleset originalRuleset = existing.Ruleset;

        mapper.Map(game, existing);

        // Check if we need to load scores for any operations
        bool rulesetChanged = originalRuleset != existing.Ruleset;
        bool needsRejection = game.VerificationStatus == VerificationStatus.Rejected;

        if (rulesetChanged || needsRejection)
        {
            await gamesRepository.LoadScoresAsync(existing);

            // Update ruleset for all child scores if changed
            if (rulesetChanged)
            {
                foreach (GameScore score in existing.Scores)
                {
                    score.Ruleset = existing.Ruleset;
                }
            }

            // Reject all children if verification status is rejected
            if (needsRejection)
            {
                existing.RejectAllChildren();
            }
        }

        await gamesRepository.UpdateAsync(existing);
        return mapper.Map<GameDTO>(existing);
    }

    public async Task DeleteAsync(int id) =>
        await gamesRepository.DeleteAsync(id);

    public async Task<GameDTO?> MergeScoresAsync(int targetGameId, IEnumerable<int> sourceGameIds)
    {
        Game? result = await gamesRepository.MergeScoresAsync(targetGameId, sourceGameIds);

        return result is null ? null : mapper.Map<GameDTO>(result);
    }
}
