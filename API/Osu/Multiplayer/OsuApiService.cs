using API.Configurations;
using API.Entities;
using Newtonsoft.Json;

namespace API.Osu.Multiplayer;

public class OsuApiService : IOsuApiService
{
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

		try
		{
			string response = await _client.GetStringAsync($"get_match?k={_credentials.OsuApiKey}&mp={matchId}");
			_logger.LogDebug("Successfully received response from osu! API for match {MatchId}", matchId);
			_logger.LogTrace("Response: {Response}", response);
			return JsonConvert.DeserializeObject<OsuApiMatchData>(response);
		}
		catch (Exception e)
		{
			_logger.LogError(e, "Failure while fetching data for match {MatchId}", matchId);
			return null;
		}
	}

	public async Task<Beatmap?> GetBeatmapAsync(long beatmapId)
	{
		string? response = null;
		try
		{
			response = await _client.GetStringAsync($"get_beatmaps?k={_credentials.OsuApiKey}&b={beatmapId}");
			_logger.LogDebug("Successfully received response from osu! API for beatmap {BeatmapId}", beatmapId);
			return JsonConvert.DeserializeObject<Beatmap[]>(response)?[0];
		}
		catch (JsonSerializationException e)
		{
			_logger.LogError(e, "Failure while deserializing JSON for beatmap {BeatmapId} (response: {Response})", beatmapId, response);
			return null;
		}
		catch (Exception e)
		{
			_logger.LogError(e, "Failed to get beatmap data for beatmap {BeatmapId}", beatmapId);
			return null;
		}
	}
}