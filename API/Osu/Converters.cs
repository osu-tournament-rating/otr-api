using API.Entities;
using API.Osu.Multiplayer;

namespace API.Osu;

public class Converters
{
	public static class OsuMultiplayerLobbyData
	{
		public static List<MatchData> FromMultiplayerLobbyData(MultiplayerLobbyData data)
		{
			var matchDatas = new List<MatchData>();

			foreach (var game in data.Games)
			{
				foreach (var score in game.Scores)
				{
					var matchData = new MatchData
					{
						OsuMatchId = long.TryParse(data.Match.MatchId, out var osuMatchId) ? osuMatchId : 0,
						GameId = long.TryParse(game.GameId, out var gameId) ? gameId : 0,
						ScoringType = game.ScoringType,
						OsuBeatmapId = long.TryParse(game.BeatmapId, out var beatmapId) ? beatmapId : 0,
						GameRawMods = int.TryParse(game.Mods, out var gameRawMods) ? gameRawMods : 0,
						RawMods = int.TryParse(score.EnabledMods, out var rawMods) ? rawMods : 0,
						MatchName = data.Match.Name,
						Mode = game.PlayMode,
						MatchStartDate = data.Match.StartTime,
						TeamType = game.TeamType,
						Team = score.Team,
						Score = double.TryParse(score.PlayerScore, out var playerScore) ? playerScore : 0,
						PlayerId = int.TryParse(score.UserId, out var playerId) ? playerId : 0
					};

					matchDatas.Add(matchData);
				}
			}

			return matchDatas;
		}
	}
}
