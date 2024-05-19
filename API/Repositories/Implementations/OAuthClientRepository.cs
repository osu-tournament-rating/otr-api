using System.Diagnostics.CodeAnalysis;
using API.Entities;
using API.Repositories.Interfaces;
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
        var hashedSecret = passwordHasher.HashPassword(entity, entity.Secret);
        entity.Secret = hashedSecret;

        return await base.CreateAsync(entity);
    }

    public async Task<bool> ExistsAsync(int id, int userId) =>
        await _context.OAuthClients.AnyAsync(c => c.Id == id && c.UserId == userId);

    public async Task<OAuthClient?> SetRatelimitOverridesAsync(int clientId, RateLimitOverrides rateLimitOverrides)
    {
        OAuthClient? match = await GetAsync(clientId);

        if (match == null)
        {
            return null;
        }

        match.RateLimitOverrides = rateLimitOverrides;
        await UpdateAsync(match);
        return match;
    }

    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    public string GenerateClientSecret()
    {
        const int length = 50;
        const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        var r = new Random();
        return new string(Enumerable.Repeat(chars, length).Select(s => s[r.Next(s.Length)]).ToArray());
    }
}
