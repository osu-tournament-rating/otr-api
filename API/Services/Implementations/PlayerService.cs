using API.Configurations;
using API.DTOs;
using API.Models;
using API.Services.Interfaces;
using AutoMapper;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Collections;

namespace API.Services.Implementations;

public class PlayerService : ServiceBase<Player>, IPlayerService
{
	private readonly OtrContext _context;
	private readonly IMapper _mapper;
	public PlayerService(ILogger<PlayerService> logger, OtrContext context, IMapper mapper) : base(logger, context)
	{
		_context = context;
		_mapper = mapper;
	}

	public async Task<IEnumerable<PlayerDTO>> GetAllAsync()
	{
		using(_context)
		{
			return _mapper.Map<IEnumerable<PlayerDTO>>(await _context.Players
			                                                         .Include(x => x.MatchScores)
			                                                         .Include(x => x.RatingHistories)
			                                                         .Include(x => x.Ratings)
			                                                         .ToListAsync());
		}
	}

	public async Task<PlayerDTO?> GetByOsuIdAsync(long osuId)
	{
		using (_context)
		{
			return _mapper.Map<PlayerDTO?>(await _context.Players
			                     .Include(x => x.MatchScores)
			                     .Include(x => x.RatingHistories)
			                     .Include(x => x.Ratings)
			                     .Include(x => x.User)
			                     .Where(x => x.OsuId == osuId)
			                     .FirstOrDefaultAsync());
		}
	}

	public async Task<IEnumerable<PlayerDTO>> GetByOsuIdsAsync(IEnumerable<long> osuIds)
	{
		using (_context)
		{
			return _mapper.Map<IEnumerable<PlayerDTO>>(await _context.Players.Where(p => osuIds.Contains(p.OsuId)).ToListAsync());
		}
	}

	public async Task<int> GetIdByOsuIdAsync(long osuId)
	{
		using (_context)
		{
			return await _context.Players.Where(p => p.OsuId == osuId).Select(p => p.Id).FirstOrDefaultAsync();
		}
	}

	public async Task<long> GetOsuIdByIdAsync(int id)
	{
		using (_context)
		{
			return await _context.Players.Where(p => p.Id == id).Select(p => p.OsuId).FirstOrDefaultAsync();
		}
	}

	public async Task<IEnumerable<Player>> GetOutdatedAsync()
	{
		using (_context)
		{
			return await _context.Players.Where(p => p.Updated < DateTime.Now.Subtract(TimeSpan.FromDays(14)) || p.Updated == null).ToListAsync();
		}
	}
}