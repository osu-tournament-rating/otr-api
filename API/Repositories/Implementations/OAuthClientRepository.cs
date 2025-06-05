using System.Diagnostics.CodeAnalysis;
using API.Repositories.Interfaces;
using Database;
using Database.Entities;
using Database.Repositories.Implementations;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories.Implementations;

[SuppressMessage("Performance", "CA1862:Use the \'StringComparison\' method overloads to perform case-insensitive string comparisons")]
[SuppressMessage("ReSharper", "SpecifyStringComparison")]
public class OAuthClientRepository(OtrContext context, IPasswordHasher<OAuthClient> passwordHasher) : RepositoryBase<OAuthClient>(context), IOAuthClientRepository
{
    private readonly OtrContext _context = context;

    public override async Task<OAuthClient> CreateAsync(OAuthClient entity)
    {
        string hashedSecret = passwordHasher.HashPassword(entity, entity.Secret);
        entity.Secret = hashedSecret;

        return await base.CreateAsync(entity);
    }

    public async Task<bool> ExistsAsync(int id, int userId) =>
        await _context.OAuthClients.AnyAsync(c => c.Id == id && c.UserId == userId);

    public async Task<OAuthClient?> SetRateLimitOverrideAsync(int clientId, int rateLimitOverride)
    {
        OAuthClient? client = await GetAsync(clientId);

        if (client is null)
        {
            return null;
        }

        client.RateLimitOverride = rateLimitOverride;
        await UpdateAsync(client);
        return client;
    }

    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    public string GenerateClientSecret()
    {
        const int length = 50;
        const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        var r = new Random();
        return new string([.. Enumerable.Repeat(chars, length).Select(s => s[r.Next(s.Length)])]);
    }
}
