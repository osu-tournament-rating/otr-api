using System.Security.Claims;
using API.Authorization;
using API.DTOs;
using API.Handlers.Interfaces;
using API.Repositories.Interfaces;
using API.Services.Interfaces;
using API.Utilities.Extensions;
using Database.Entities;
using Database.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using OsuApiClient;
using OsuApiClient.Configurations.Interfaces;
using OsuUser = OsuApiClient.Domain.Osu.Users.UserExtended;

namespace API.Handlers.Implementations;

/// <summary>
/// Handles serving access credentials
/// </summary>
public class OAuthHandler(
    ILogger<OAuthHandler> logger,
    IJwtService jwtService,
    IOAuthClientRepository clientRepository,
    IPlayersRepository playerRepository,
    IUserRepository userRepository,
    IOsuClient osuClient,
    IPasswordHasher<OAuthClient> clientSecretHasher,
    IOptions<IOsuClientConfiguration> osuConfiguration) : IOAuthHandler
{
    public async Task<AccessCredentialsDTO?> AuthorizeAsync(string osuAuthCode)
    {
        logger.LogDebug("Attempting authorization via osu! Auth Code");

        if (string.IsNullOrEmpty(osuAuthCode))
        {
            logger.LogDebug("osu! Auth Code null or empty, cannot authorize");
            return null;
        }

        OsuUser? osuUser = await AuthorizeOsuUserAsync(osuAuthCode);
        if (osuUser is null)
        {
            logger.LogDebug("Could not authorize user with the osu! API");
            return null;
        }

        // Create the Player if they don't exist in our system.
        // Then, create the User if needed.
        Player player = await playerRepository.GetOrCreateAsync(osuUser.Id);
        User user = await AuthenticateUserAsync(player.Id);

        await SyncFriendsAsync(user);

        logger.LogDebug("Authorized user with id {Id}", user.Id);

        return new AccessCredentialsDTO
        {
            AccessToken = jwtService.GenerateAccessToken(user),
            RefreshToken = jwtService.GenerateRefreshToken(user),
            AccessExpiration = jwtService.AccessDurationSeconds,
            RefreshExpiration = jwtService.RefreshDurationSeconds
        };
    }

    /// <summary>
    /// Sync the user's osu! friends list with the repository
    /// </summary>
    /// <param name="user">The user</param>
    private async Task SyncFriendsAsync(User user)
    {
        // Do not allow friends list syncs more often than every 24 hours
        if (user.LastFriendsListUpdate is not null &&
            (DateTime.UtcNow - user.LastFriendsListUpdate).Value <
            TimeSpan.FromHours(osuConfiguration.Value.LoginFriendsSyncFrequencyHours))
        {
            return;
        }

        // Sync the user's friends list
        // ReSharper disable once SuggestVarOrType_Elsewhere
        List<long> friendOsuIds = (await osuClient.GetUserFriendsAsync())?.Select(u => u.Id).ToList() ?? [];

        /**
         * Don't overwrite if the osu! api returns 0 friends.
         * We don't want to accidentally nuke someone's friends list from an API error.
         */
        if (friendOsuIds.Count > 0)
        {
            await userRepository.SyncFriendsAsync(user.Id, friendOsuIds);
        }
    }

    public async Task<DetailedResponseDTO<AccessCredentialsDTO>> AuthorizeAsync(int clientId, string clientSecret)
    {
        logger.LogDebug("Attempting authorization via client credentials");

        OAuthClient? client = await clientRepository.GetAsync(clientId);
        if (client is null)
        {
            return new DetailedResponseDTO<AccessCredentialsDTO> { ErrorDetail = "Invalid client credentials" };
        }

        // Validate secret
        PasswordVerificationResult
            result = clientSecretHasher.VerifyHashedPassword(client, client.Secret, clientSecret);
        if (result != PasswordVerificationResult.Success)
        {
            return new DetailedResponseDTO<AccessCredentialsDTO> { ErrorDetail = "Invalid client credentials" };
        }

        logger.LogDebug("Authorized client with id {Id}", clientId);

        return new DetailedResponseDTO<AccessCredentialsDTO>
        {
            Response = new AccessCredentialsDTO
            {
                AccessToken = jwtService.GenerateAccessToken(client),
                RefreshToken = jwtService.GenerateRefreshToken(client),
                AccessExpiration = jwtService.AccessDurationSeconds,
                RefreshExpiration = jwtService.RefreshDurationSeconds
            }
        };
    }

    public async Task<DetailedResponseDTO<AccessCredentialsDTO>> RefreshAsync(string refreshToken)
    {
        ClaimsPrincipal? claimsPrincipal = jwtService.ReadToken(refreshToken);

        if (claimsPrincipal is null)
        {
            return new DetailedResponseDTO<AccessCredentialsDTO> { ErrorDetail = "Invalid token" };
        }

        if (claimsPrincipal.GetTokenType() is not OtrClaims.TokenTypes.RefreshToken)
        {
            return new DetailedResponseDTO<AccessCredentialsDTO> { ErrorDetail = "Invalid token" };
        }

        // Validate the issuer is a user or client
        if (!claimsPrincipal.IsUser() && !claimsPrincipal.IsClient())
        {
            logger.LogWarning("Refresh token does not have the user or client role");
            return new DetailedResponseDTO<AccessCredentialsDTO> { ErrorDetail = "Invalid token" };
        }

        var accessToken = string.Empty;
        // Generate new access token
        if (claimsPrincipal.IsUser())
        {
            User? user = await userRepository.GetAsync(claimsPrincipal.GetSubjectId());
            if (user == null)
            {
                logger.LogWarning("Decrypted refresh token issuer is not a valid user");
                return new DetailedResponseDTO<AccessCredentialsDTO>
                {
                    ErrorDetail = "Refresh token does not belong to an existing user"
                };
            }

            accessToken = jwtService.GenerateAccessToken(user);
        }
        else if (claimsPrincipal.IsClient())
        {
            OAuthClient? client = await clientRepository.GetAsync(claimsPrincipal.GetSubjectId());
            if (client == null)
            {
                logger.LogWarning("Decrypted refresh token issuer is not a valid client");
                return new DetailedResponseDTO<AccessCredentialsDTO>
                {
                    ErrorDetail = "Refresh token does not belong to an existing client"
                };
            }

            accessToken = jwtService.GenerateAccessToken(client);
        }

        // Return a new OAuthResponseDTO containing only a new access token, NOT a new refresh token.
        return new DetailedResponseDTO<AccessCredentialsDTO>
        {
            Response = string.IsNullOrEmpty(accessToken)
                ? null
                : new AccessCredentialsDTO
                {
                    AccessToken = accessToken,
                    AccessExpiration = jwtService.AccessDurationSeconds
                },
            ErrorDetail = string.IsNullOrEmpty(accessToken)
                ? "Unable to refresh credentials"
                : null
        };
    }

    /// <summary>
    /// Authorizes the current user via osu! API v2
    /// </summary>
    /// <param name="osuCode">The authorization code for the user</param>
    /// <returns>The authorized user</returns>
    private async Task<OsuUser?> AuthorizeOsuUserAsync(string osuCode)
    {
        await osuClient.AuthorizeUserWithCodeAsync(osuCode);
        return await osuClient.GetCurrentUserAsync();
    }

    /// <summary>
    /// Gets and "logs in" the user for the given player id
    /// </summary>
    private async Task<User> AuthenticateUserAsync(int playerId)
    {
        User user = await userRepository.GetOrCreateByPlayerIdAsync(playerId);

        user.LastLogin = DateTime.UtcNow;
        await userRepository.UpdateAsync(user);

        return user;
    }
}
