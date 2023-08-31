using API.Entities;
using API.Osu.Multiplayer;
using Newtonsoft.Json;

namespace API.Osu;

public class Converters
{
	public static List<PlayerMatchData> MatchDataFromApiResponse(MultiplayerLobbyData data)
	{
		var matchDatas = new List<PlayerMatchData>();

		foreach (var game in data.Games)
		{
			// TODO: Needs unit test
			foreach (var score in game.Scores)
			{
				// todo: convert
			}
		}

		return matchDatas;
	}
	
	public static Beatmap? BeatmapFromJson(string json)
	{
		return JsonConvert.DeserializeObject<Beatmap>(json);
	}
}