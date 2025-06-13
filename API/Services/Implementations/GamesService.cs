using API.DTOs;
using API.Services.Interfaces;
using AutoMapper;
using Common.Enums.Verification;
using Database.Entities;
using Database.Repositories.Interfaces;

namespace API.Services.Implementations;

public class GamesService(IGamesRepository gamesRepository, IPlayersRepository playersRepository, IMapper mapper) : IGamesService
{
    public async Task<GameDTO?> GetAsync(int id, bool verified)
    {
        GameDTO? game = mapper.Map<GameDTO?>(await gamesRepository.GetAsync(id, verified));

        if (game is null)
        {
            return null;
        }

        game.Players = await GetPlayerCompactsAsync(game);
        return game;
    }

    public async Task<GameDTO?> UpdateAsync(int id, GameDTO game)
    {
        Game? existing = await gamesRepository.GetAsync(id);
        if (existing is null)
        {
            return null;
        }

        // Store the original ruleset to detect changes
        var originalRuleset = existing.Ruleset;

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

    public async Task<bool> ExistsAsync(int id) =>
        await gamesRepository.ExistsAsync(id);

    public async Task DeleteAsync(int id) =>
        await gamesRepository.DeleteAsync(id);

    public async Task<GameDTO?> MergeScoresAsync(int targetGameId, IEnumerable<int> sourceGameIds)
    {
        Game? result = await gamesRepository.MergeScoresAsync(targetGameId, sourceGameIds);

        if (result is not null)
        {
            GameDTO gameDto = mapper.Map<GameDTO>(result);
            gameDto.Players = await GetPlayerCompactsAsync(gameDto);
            return gameDto;
        }

        return null;
    }

    private async Task<ICollection<PlayerCompactDTO>> GetPlayerCompactsAsync(GameDTO game)
    {
        IEnumerable<int> playerIds = game.Scores.Select(s => s.PlayerId).Distinct();
        return mapper.Map<ICollection<PlayerCompactDTO>>(await playersRepository.GetAsync(playerIds));
    }
}
