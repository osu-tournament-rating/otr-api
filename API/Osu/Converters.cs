using Newtonsoft.Json;

namespace API.Osu;

public class Converters
{
	// TODO: Convert from osu! API response to entities
	
	public static Beatmap? BeatmapFromJson(string json)
	{
		return JsonConvert.DeserializeObject<Beatmap>(json);
	}
}