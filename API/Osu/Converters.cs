using API.Entities;
using Newtonsoft.Json;

namespace API.Osu;

public class Converters
{
	public static Beatmap? BeatmapFromJson(string json)
	{
		return JsonConvert.DeserializeObject<Beatmap>(json);
	}
}