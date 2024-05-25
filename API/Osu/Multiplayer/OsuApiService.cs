using API.Configurations;
using Database.Enums;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OsuSharp.Domain;
using OsuSharp.Interfaces;
using Beatmap = Database.Entities.Beatmap;

namespace API.Osu.Multiplayer;

/// <summary>
/// Represents a user obtained from the osu! api
/// </summary>
public class OsuApiUser
{
    /// <summary>
    /// Id of the user
    /// </summary>
    public long Id { get; init; }

    /// <summary>
    /// Username of the user
    /// </summary>
    public string? Username { get; init; }

    /// <summary>
    /// Rank of the user for the given ruleset
    /// </summary>
    public int? Rank { get; init; }

    /// <summary>
    /// Country code of the user
    /// </summary>
    public string Country { get; init; } = null!;

    /// <summary>
    /// Default ruleset of the user
    /// </summary>
    public Ruleset Ruleset { get; init; }
}

public class OsuApiService : IOsuApiService
{
    private const int RATE_LIMIT_CAPACITY = 1000;
    private const int RATE_LIMIT_INTERVAL_SECONDS = 60;
    private const int MAX_CONCURRENT_THREADS = 5;
    private const string BaseUrl = "https://osu.ppy.sh/api/";
    private readonly HttpClient _client;
    private readonly IOptions<OsuConfiguration> _osuConfiguration;
    private readonly IOsuClient _v2Client;
    private readonly ILogger<OsuApiService> _logger;
    private readonly SemaphoreSlim _semaphore = new(MAX_CONCURRENT_THREADS, MAX_CONCURRENT_THREADS);
    private int _rateLimitCounter;
    private DateTime _rateLimitResetTime = DateTime.Now.AddSeconds(RATE_LIMIT_INTERVAL_SECONDS);

    public OsuApiService(
        ILogger<OsuApiService> logger,
        IOptions<OsuConfiguration> osuConfiguration,
        IOsuClient v2Client
    )
    {
        _logger = logger;
        _osuConfiguration = osuConfiguration;
        _v2Client = v2Client;
        _client = new HttpClient { BaseAddress = new Uri(BaseUrl) };
    }

    public async Task<OsuApiMatchData?> GetMatchAsync(long matchId, string reason) =>
        await ExecuteApiCallAsync(
            async () =>
            {
                var response = await _client.GetStringAsync(
                    $"get_match?k={_osuConfiguration.Value.ApiKey}&mp={matchId}"
                );
                _logger.LogDebug(
                    "Successfully received response from osu! API for match {MatchId} [{Reason}]",
                    matchId,
                    reason
                );

                return JsonConvert.DeserializeObject<OsuApiMatchData>(response);
            },
            matchId
        );

    public async Task<IBeatmapDifficulty?> GetDifficultyAttributesAsync(long beatmapId) =>
        await ExecuteApiCallAsync(
            async () =>
            {
                IBeatmapDifficulty response = await _v2Client.GetBeatmapAttributesAsync(beatmapId);
                _logger.LogDebug(
                    "Successfully received response from osu! API for beatmap attributes {BeatmapId}",
                    beatmapId
                );

                return response;
            },
            beatmapId
        );

    public async Task<Beatmap?> GetBeatmapAsync(long beatmapId, string reason) =>
        await ExecuteApiCallAsync(
            async () =>
            {
                IBeatmap response = await _v2Client.GetBeatmapAsync(beatmapId);
                _logger.LogDebug(
                    "Successfully received response from osu! API for beatmap {BeatmapId} [{Reason}]",
                    beatmapId,
                    reason
                );

                IBeatmapDifficulty? attributes = await GetDifficultyAttributesAsync(beatmapId);

                var beatmap = new Beatmap
                {
                    Artist = response.Beatmapset.Artist,
                    BeatmapId = beatmapId,
                    Bpm = response.Bpm,
                    MapperId = response.Beatmapset.UserId,
                    Cs = response.CircleSize,
                    Ar = response.ApproachRate,
                    Hp = response.Drain,
                    Od = response.DifficultyRating,
                    DrainTime = response.Drain,
                    Length = response.Length.TotalSeconds,
                    Title = response.Beatmapset.Title,
                    GameMode = (int)response.Mode,
                    CircleCount = response.CircleCount,
                    SliderCount = response.SliderCount,
                    SpinnerCount = response.SpinnerCount,
                    MaxCombo = response.MaxCombo,
                    MapperName = response.Beatmapset.Creator
                };

                if (attributes != null)
                {
                    IBeatmapDifficultyAttributes attr = attributes.Attributes;
                    beatmap.Sr = attr.StarRating;
                    beatmap.AimDiff = attr.AimDifficulty;
                    beatmap.SpeedDiff = attr.SpeedDifficulty;
                }

                return beatmap;
            },
            beatmapId
        );

    public async Task<OsuApiUser?> GetUserAsync(long userId, Ruleset ruleset, string reason) =>
        await ExecuteApiCallAsync(
            async () =>
            {
                IGlobalUser response = await _v2Client.GetUserAsync(userId, (GameMode)ruleset);

                if (response.IsRestricted == true)
                {
                    _logger.LogDebug("User {UserId} is restricted, skipping", userId);
                    return null;
                }

                _logger.LogDebug(
                    "Successfully received response from osu! API for user {UserId} [{Reason}]",
                    userId,
                    reason
                );

                return new OsuApiUser
                {
                    Id = userId,
                    Username = response.Username,
                    Rank = (int?)response.Statistics.GlobalRank,
                    Country = response.Country.Code,
                    Ruleset = (Ruleset)response.GameMode
                };
            },
            userId
        );

    private async Task EnsureRateLimit()
    {
        CheckRatelimitReset();

        if (_rateLimitCounter >= RATE_LIMIT_CAPACITY)
        {
            await Task.Delay(_rateLimitResetTime - DateTime.Now); // Wait for the rate limit interval to pass
            CheckRatelimitReset();
        }

        _rateLimitCounter++;
    }

    private async Task<T?> ExecuteApiCallAsync<T>(Func<Task<T?>> apiCall, long id)
        where T : class
    {
        try
        {
            await _semaphore.WaitAsync();
            await EnsureRateLimit();
            return await apiCall();
        }
        catch (JsonSerializationException e)
        {
            _logger.LogWarning(
                e,
                "The osu! API returned an invalid body for id {Id} of type {T}, likely due to deletion",
                id,
                typeof(T)
            );
            return null;
        }
        catch (IndexOutOfRangeException e)
        {
            _logger.LogWarning(e, "The osu! API returned null for id {Id} of type {T}", id, typeof(T));
            return null;
        }
        catch (Exception e)
        {
            _logger.LogError(
                e,
                "Failure while fetching data for id {Id} (api call was made expecting type {T})",
                id,
                typeof(T)
            );
            return null;
        }
        finally
        {
            _logger.LogDebug(
                "osu! API ratelimit is currently at {Requests} (freq: {Capacity}req/{Seconds}s)",
                _rateLimitCounter,
                RATE_LIMIT_CAPACITY,
                RATE_LIMIT_INTERVAL_SECONDS
            );
            _semaphore.Release();
        }
    }

    private void CheckRatelimitReset()
    {
        if (DateTime.Now > _rateLimitResetTime)
        {
            ResetRateLimit();
        }
    }

    private void ResetRateLimit()
    {
        _rateLimitCounter = 0;
        _rateLimitResetTime = DateTime.Now.AddSeconds(RATE_LIMIT_INTERVAL_SECONDS);
        _logger.LogDebug("Ratelimiter reset!");
    }
}
