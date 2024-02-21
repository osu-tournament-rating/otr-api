using API.Entities;
using API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories.Implementations;

public class OAuthClientRepository : RepositoryBase<OAuthClient>, IOAuthClientRepository
{
    private readonly OtrContext _context;

    public OAuthClientRepository(OtrContext context) : base(context)
    {
        _context = context;
    }
    
    public async Task<bool> SecretInUseAsync(string clientSecret)
    {
        return await _context.OAuthClients.AnyAsync(x => x.Secret == clientSecret);
    }

    public async Task<bool> ValidateAsync(int clientId, string clientSecret)
    {
        var match = await _context.OAuthClients
            .FirstOrDefaultAsync(x => x.Id == clientId && x.Secret == clientSecret);

        return match != null;
    }
}