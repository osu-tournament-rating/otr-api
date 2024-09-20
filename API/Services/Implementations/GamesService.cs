using API.DTOs;
using API.Services.Interfaces;
using AutoMapper;
using Database.Entities;
using Database.Repositories.Interfaces;

namespace API.Services.Implementations;

public class GamesService(IGamesRepository gamesRepository, IMapper mapper) : IGamesService
{
    public async Task<GameDTO?> GetAsync(int id) =>
        mapper.Map<GameDTO?>(await gamesRepository.GetAsync(id));

    public async Task<GameDTO?> UpdateAsync(int id, GameDTO game)
    {
        Game? existing = await gamesRepository.GetAsync(id);
        if (existing is null)
        {
            return null;
        }

        existing.Ruleset = game.Ruleset;
        existing.ScoringType = game.ScoringType;
        existing.TeamType = game.TeamType;
        existing.Mods = game.Mods;
        existing.VerificationStatus = game.VerificationStatus;
        existing.ProcessingStatus = game.ProcessingStatus;
        existing.RejectionReason = game.RejectionReason;
        existing.StartTime = game.StartTime;
        existing.EndTime = game.EndTime ?? existing.EndTime;

        await gamesRepository.UpdateAsync(existing);
        return mapper.Map<GameDTO>(existing);
    }
}
