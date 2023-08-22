using API.Configurations;
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

	public async Task<MultiplayerLobbyData?> GetMatchAsync(long matchId)
	{
		_logger.LogDebug("Attempting to fetch data for match {MatchId}", matchId);

		try
		{
			string response = await _client.GetStringAsync($"get_match?k={_credentials.OsuApiKey}&mp={matchId}");
			_logger.LogDebug("Successfully received response from osu! API for match {MatchId}", matchId);
			_logger.LogTrace("Response: {Response}", response);
			return JsonConvert.DeserializeObject<MultiplayerLobbyData>(response);
		}
		catch (Exception e)
		{
			_logger.LogError(e, "Failure while fetching data for match {MatchId}", matchId);
			return null;
		}
	}
}