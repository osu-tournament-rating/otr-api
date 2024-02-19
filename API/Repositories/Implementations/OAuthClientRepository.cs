using API.Entities;
using API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories.Implementations;

public class OAuthClientRepository(OtrContext context) : RepositoryBase<OAuthClient>(context), IOAuthClientRepository
{
    public async Task<bool> SecretInUseAsync(string clientSecret)
    {
        return await context.OAuthClients.AnyAsync(x => x.Secret == clientSecret);
    }

    public async Task<bool> ValidateAsync(int clientId, string clientSecret)
    {
        var match = await context.OAuthClients
            .FirstOrDefaultAsync(x => x.Id == clientId && x.Secret == clientSecret);

        return match != null;
    }
}