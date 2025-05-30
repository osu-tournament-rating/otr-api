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
        Game? existing = await gamesRepository.GetAsync(id, false);
        if (existing is null)
        {
            return null;
        }

        // Store original verification status to detect changes
        VerificationStatus originalVerificationStatus = existing.VerificationStatus;

        mapper.Map(game, existing);

        // Check if verification status changed to Rejected and apply cascading logic
        if (originalVerificationStatus != VerificationStatus.Rejected &&
            existing.VerificationStatus == VerificationStatus.Rejected)
        {
            // Apply cascading rejection to all child scores
            foreach (GameScore score in existing.Scores)
            {
                score.VerificationStatus = VerificationStatus.Rejected;
                score.RejectionReason |= ScoreRejectionReason.RejectedGame;
            }
        }

        await gamesRepository.UpdateAsync(existing);
        return mapper.Map<GameDTO>(existing);
    }

    public async Task<bool> ExistsAsync(int id) =>
        await gamesRepository.ExistsAsync(id);

    public async Task DeleteAsync(int id) =>
        await gamesRepository.DeleteAsync(id);

    private async Task<ICollection<PlayerCompactDTO>> GetPlayerCompactsAsync(GameDTO game)
    {
        IEnumerable<int> playerIds = game.Scores.Select(s => s.PlayerId).Distinct();
        return mapper.Map<ICollection<PlayerCompactDTO>>(await playersRepository.GetAsync(playerIds));
    }
}
