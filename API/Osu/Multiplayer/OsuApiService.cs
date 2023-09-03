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
	private const int RATE_LIMIT_INTERVAL_SECONDS = 60;
	
	private int _rateLimitCounter;
	private DateTime _rateLimitResetTime = DateTime.UtcNow.AddSeconds(RATE_LIMIT_INTERVAL_SECONDS);
	
	private const string BaseUrl = "https://osu.ppy.sh/api/";
	private readonly HttpClient _client;
	private readonly ICredentials _credentials;
	private readonly ILogger<OsuApiService> _logger;

	public OsuApiService(ILogger<OsuApiService> logger, ICredentials credentials)
	{
		_logger = logger;
		_credentials = credentials;
		_client = new HttpClient
		{
			BaseAddress = new Uri(BaseUrl)
		};
	}

	public async Task<OsuApiMatchData?> GetMatchAsync(long matchId)
	{
		_logger.LogDebug("Attempting to fetch data for match {MatchId}", matchId);

		while (await IsRateLimited()) {}

		try
		{
			string response = await _client.GetStringAsync($"get_match?k={_credentials.OsuApiKey}&mp={matchId}");
			_rateLimitCounter++;

			_logger.LogDebug("Successfully received response from osu! API for match {MatchId}", matchId);
			_logger.LogTrace("Response: {Response}", response);
			return JsonConvert.DeserializeObject<OsuApiMatchData>(response);
		}
		catch (JsonSerializationException e)
		{
			_logger.LogWarning("The osu! API returned an invalid body for match {MatchId}, likely due to match deletion", matchId);
			return null;
		}
		catch (Exception e)
		{
			_logger.LogError(e, "Failure while fetching data for match {MatchId}", matchId);
			return null;
		}
	}

	public async Task<Beatmap?> GetBeatmapAsync(long beatmapId)
	{
		while (await IsRateLimited()) {}

		try
		{
			string response = await _client.GetStringAsync($"get_beatmaps?k={_credentials.OsuApiKey}&b={beatmapId}");
			_rateLimitCounter++;

			_logger.LogDebug("Successfully received response from osu! API for beatmap {BeatmapId}", beatmapId);
			return JsonConvert.DeserializeObject<Beatmap[]>(response)?[0];
		}
		catch (JsonSerializationException)
		{
			_logger.LogWarning("Failure while deserializing JSON for beatmap {BeatmapId} (map is likely deleted)", beatmapId);
			return null;
		}
		catch (IndexOutOfRangeException)
		{
			_logger.LogWarning("Failure while deserializing JSON for beatmap {BeatmapId} (map is likely deleted)", beatmapId);
			return null;
		}
		catch (Exception e)
		{
			_logger.LogError(e, "Failed to get beatmap data for beatmap {BeatmapId}", beatmapId);
			return null;
		}
	}

	public async Task<OsuApiUser?> GetUserAsync(long userId, OsuEnums.Mode mode)
	{
		while (await IsRateLimited()) {}

		try
		{
			string response = await _client.GetStringAsync($"get_user?k={_credentials.OsuApiKey}&u={userId}&m={(int)mode}&type=id");
			_rateLimitCounter++;
		
			_logger.LogDebug("Successfully received response from osu! API for user {UserId}", userId);
			return JsonConvert.DeserializeObject<OsuApiUser[]>(response)?[0];
		}
		catch (JsonSerializationException)
		{
			_logger.LogError("Failed to deserialize JSON for user {UserId}", userId);
			return null;
		}
		catch (IndexOutOfRangeException)
		{
			_logger.LogError("Failed to deserialize JSON for user {UserId}", userId);
			return null;
		}
		catch (Exception e)
		{
			_logger.LogError(e, "Failed to get user data for user {UserId}", userId);
			return null;
		}
	}
	
	private void CheckRatelimitReset()
	{
		_logger.LogDebug("osu! API ratelimit is currently at {Requests} (freq: {Capacity}req/{Seconds}s)", _rateLimitCounter, RATE_LIMIT_CAPACITY, RATE_LIMIT_INTERVAL_SECONDS);
		if (DateTime.UtcNow > _rateLimitResetTime)
		{
			_rateLimitCounter = 0; // Reset the counter when the reset time has passed
			_rateLimitResetTime = DateTime.UtcNow.AddSeconds(RATE_LIMIT_INTERVAL_SECONDS);
			
			_logger.LogDebug("Ratelimiter reset!");
		}
	}
	
	private async Task<bool> IsRateLimited()
	{
		CheckRatelimitReset();

		if (_rateLimitCounter >= RATE_LIMIT_CAPACITY && DateTime.UtcNow <= _rateLimitResetTime)
		{
			_logger.LogDebug("Rate limit reached, waiting for reset...");
			await Task.Delay(1000);
			return true;
		}

		return false;
	}
}