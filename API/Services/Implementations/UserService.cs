using API.DTOs;
using API.Entities;
using API.Services.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace API.Services.Implementations;

public class UserService : ServiceBase<User>, IUserService
{
	private readonly OtrContext _context;
	private readonly IMapper _mapper;

	public UserService(ILogger<UserService> logger, IMapper mapper, OtrContext context) : base(logger, context)
	{
		_mapper = mapper;
		_context = context;
	}

	public async Task<UserDTO?> GetForPlayerAsync(int playerId) => _mapper.Map<UserDTO?>(await _context.Users.FirstOrDefaultAsync(u => u.PlayerId == playerId));
}