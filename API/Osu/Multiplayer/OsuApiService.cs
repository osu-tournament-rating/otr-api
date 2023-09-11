using API.Configurations;
using API.Entities;
using Newtonsoft.Json;

namespace API.Osu.Multiplayer;

public class OsuApiUser
{
	[JsonProperty("user_id")]
	public long Id { get; set; }
	[JsonProperty("username")]
	public string? Username { get; set; }
	[JsonProperty("pp_rank")]
	public int? Rank { get; set; }
}

public class OsuApiService : IOsuApiService
{
	private const int RATE_LIMIT_CAPACITY = 1000;
	private const string BaseUrl = "https://osu.ppy.sh/api/";
	private readonly HttpClient _client;
	private readonly ICredentials _credentials;
	private readonly ILogger<OsuApiService> _logger;
	private readonly object _rateLimitLock = new();
	private readonly SemaphoreSlim _semaphore;
	private int _rateLimitCounter;
	private DateTime _rateLimitResetTime = DateTime.UtcNow.AddSeconds(RATE_LIMIT_CAPACITY);

	public OsuApiService(ILogger<OsuApiService> logger, ICredentials credentials)
	{
		_logger = logger;
		_credentials = credentials;
		_client = new HttpClient
		{
			BaseAddress = new Uri(BaseUrl)
		};

		_semaphore = new SemaphoreSlim(RATE_LIMIT_CAPACITY, RATE_LIMIT_CAPACITY);
	}

	public async Task<OsuApiMatchData?> GetMatchAsync(long matchId)
	{
		await EnsureRateLimit();
		return await ExecuteApiCall(async () =>
		{
			string response = await _client.GetStringAsync($"get_match?k={_credentials.OsuApiKey}&mp={matchId}");
			_logger.LogDebug("Successfully received response from osu! API for match {MatchId}", matchId);
			_logger.LogTrace("Response: {Response}", response);
			return JsonConvert.DeserializeObject<OsuApiMatchData>(response);
		}, matchId);
	}

	public async Task<Beatmap?> GetBeatmapAsync(long beatmapId, OsuEnums.Mods mods = OsuEnums.Mods.None)
	{
		await EnsureRateLimit();
		return await ExecuteApiCall(async () =>
		{
			string response = await _client.GetStringAsync($"get_beatmaps?k={_credentials.OsuApiKey}&b={beatmapId}&mods={(int)mods}");
			_logger.LogDebug("Successfully received response from osu! API for beatmap {BeatmapId}", beatmapId);
			return JsonConvert.DeserializeObject<Beatmap[]>(response)?[0];
		}, beatmapId);
	}

	public async Task<OsuApiUser?> GetUserAsync(long userId, OsuEnums.Mode mode)
	{
		await EnsureRateLimit();
		return await ExecuteApiCall(async () =>
		{
			string response = await _client.GetStringAsync($"get_user?k={_credentials.OsuApiKey}&u={userId}&m={(int)mode}&type=id");
			_logger.LogDebug("Successfully received response from osu! API for user {UserId}", userId);
			return JsonConvert.DeserializeObject<OsuApiUser[]>(response)?[0];
		}, userId);
	}

	private async Task EnsureRateLimit()
	{
		CheckRatelimitReset();

		if (_rateLimitCounter >= RATE_LIMIT_CAPACITY)
		{
			await _semaphore.WaitAsync();
		}
	}

	private async Task<T?> ExecuteApiCall<T>(Func<Task<T?>> apiCall, long id) where T : class
	{
		try
		{
			return await apiCall();
		}
		catch (JsonSerializationException e)
		{
			_logger.LogWarning("The osu! API returned an invalid body for id {Id}, likely due to deletion", id);
			return null;
		}
		catch (Exception e)
		{
			_logger.LogError(e, "Failure while fetching data for id {Id}", id);
			return null;
		}
		finally
		{
			lock (_rateLimitLock)
			{
				_rateLimitCounter++;
			}

			_semaphore.Release();
		}
	}

	private void CheckRatelimitReset()
	{
		lock (_rateLimitLock)
		{
			_logger.LogDebug("osu! API ratelimit is currently at {Requests} (freq: {Capacity}req/{Seconds}s)", _rateLimitCounter, RATE_LIMIT_CAPACITY, RATE_LIMIT_CAPACITY);
			if (DateTime.UtcNow > _rateLimitResetTime)
			{
				_rateLimitCounter = 0;
				_rateLimitResetTime = DateTime.UtcNow.AddSeconds(RATE_LIMIT_CAPACITY);

				_logger.LogDebug("Ratelimiter reset!");
			}
		}
	}
}