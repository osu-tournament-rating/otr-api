using System.ComponentModel.DataAnnotations;

namespace DataWorkerService.Configurations;

public class OsuConfiguration
{
    public const string Position = "Osu";

    /// <summary>
    /// osu! OAuth client id
    /// </summary>
    [Required(ErrorMessage = "ClientId is required!")]
    [Range(1, long.MaxValue, ErrorMessage = "ClientId must be a positive, non-zero value!")]
    public long ClientId { get; set; }

    /// <summary>
    /// osu! OAuth client secret
    /// </summary>
    [Required(ErrorMessage = "ClientSecret is required!")]
    public string ClientSecret { get; set; } = string.Empty;

    /// <summary>
    /// Denotes if the worker should fetch <see cref="Player"/> data from the osu! API
    /// </summary>
    public bool ProcessPlayersOsu { get; set; }

    /// <summary>
    /// Denotes if the worker should fetch <see cref="Player"/> data from the osu!Track API
    /// </summary>
    public bool ProcessPlayersOsuTrack { get; set; }

    /// <summary>
    /// Denotes if the worker should fetch <see cref="Match"/> data from the osu! API
    /// </summary>
    public bool ProcessMatches { get; set; }
}
