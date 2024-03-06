using Newtonsoft.Json;

namespace API.Osu;

public class OsuTrackHistoryStats
{
    [JsonProperty("pp_rank")]
    public int Rank { get; set; }

    [JsonProperty("timestamp")]
    public DateTime Timestamp { get; set; }
}
