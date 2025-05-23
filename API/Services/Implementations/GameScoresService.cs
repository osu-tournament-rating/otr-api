using API.DTOs;
using API.Services.Interfaces;
using AutoMapper;
using Database.Entities;
using Database.Repositories.Interfaces;

namespace API.Services.Implementations;

public class GameScoresService(IGameScoresRepository gameScoresRepository, IMapper mapper) : IGameScoresService
{
    public async Task<GameScoreDTO?> GetAsync(int id) =>
        mapper.Map<GameScoreDTO>(await gameScoresRepository.GetAsync(id));

    public async Task<GameScoreDTO?> UpdateAsync(int id, GameScoreDTO score)
    {
        GameScore? existing = await gameScoresRepository.GetAsync(id);
        if (existing is null)
        {
            return null;
        }

        mapper.Map(score, existing);

        await gameScoresRepository.UpdateAsync(existing);
        return mapper.Map<GameScoreDTO>(existing);
    }

    public async Task<bool> ExistsAsync(int id) =>
        await gameScoresRepository.ExistsAsync(id);

    public async Task DeleteAsync(int id) =>
        await gameScoresRepository.DeleteAsync(id);
}
