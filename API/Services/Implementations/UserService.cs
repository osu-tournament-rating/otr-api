using API.DTOs;
using API.Entities;
using API.Services.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace API.Services.Implementations;

public class UserService : ServiceBase<User>, IUserService
{
	private readonly IMapper _mapper;
	private readonly OtrContext _context;
	public UserService(ILogger<UserService> logger, IMapper mapper, OtrContext context) : base(logger, context)
	{
		_mapper = mapper;
		_context = context;
	}

	public async Task<UserDTO?> GetForPlayerAsync(int playerId)
	{
		using (_context)
		{
			return _mapper.Map<UserDTO?>(await _context.Users.FirstOrDefaultAsync(u => u.PlayerId == playerId));
		}
	}
}