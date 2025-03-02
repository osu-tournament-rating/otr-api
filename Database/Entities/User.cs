using System.Diagnostics.CodeAnalysis;

namespace Database.Entities;

/// <summary>
/// A user of the o!TR platform
/// </summary>
/// <remarks>
/// Not to be confused with <see cref="Player"/>.
/// Users only contain data tied to the o!TR platform and no data related to tournament participation or ratings
/// </remarks>
[SuppressMessage("ReSharper", "CollectionNeverUpdated.Global")]
[SuppressMessage("ReSharper", "PropertyCanBeMadeInitOnly.Global")]
public class User : UpdateableEntityBase
{
    /// <summary>
    /// Timestamp of the user's last login to the o!TR website
    /// </summary>
    public DateTime? LastLogin { get; set; }

    /// <summary>
    /// A collection of string literals denoting special permissions granted to the user
    /// </summary>
    public string[] Scopes { get; set; } = [];

    /// <summary>
    /// The <see cref="UserSettings"/> owned by the user
    /// </summary>
    public UserSettings Settings { get; set; } = null!;

    /// <summary>
    /// Id of the <see cref="Entities.Player"/> associated to the user
    /// </summary>
    public int PlayerId { get; set; }

    /// <summary>
    /// The <see cref="Entities.Player"/> associated to the user
    /// </summary>
    public Player Player { get; set; } = null!;

    /// <summary>
    /// A collection of <see cref="OAuthClient"/>s owned by the user
    /// </summary>
    public ICollection<OAuthClient> Clients { get; set; } = [];

    /// <summary>
    /// A collection of <see cref="Match"/>es submitted by the user
    /// </summary>
    public ICollection<Match> SubmittedMatches { get; set; } = [];

    /// <summary>
    /// A collection of <see cref="Tournament"/>s submitted by the user
    /// </summary>
    public ICollection<Tournament> SubmittedTournaments { get; set; } = [];

    /// <summary>
    /// A collection of <see cref="TournamentAdminNote"/>s created by the user
    /// </summary>
    public ICollection<TournamentAdminNote> TournamentAdminNotes { get; set; } = [];

    /// <summary>
    /// A collection of <see cref="PlayerAdminNote"/>s created by the user
    /// </summary>
    public ICollection<PlayerAdminNote> PlayerAdminNotes { get; set; } = [];

    /// <summary>
    /// A collection of <see cref="GameAdminNote"/>s created by the user
    /// </summary>
    public ICollection<GameAdminNote> GameAdminNotes { get; set; } = [];

    /// <summary>
    /// A collection of <see cref="GameScoreAdminNote"/>s created by the user
    /// </summary>
    public ICollection<GameScoreAdminNote> GameScoreAdminNotes { get; set; } = [];

    /// <summary>
    /// A collection of <see cref="MatchAdminNote"/>s created by the user
    /// </summary>
    public ICollection<MatchAdminNote> MatchAdminNotes { get; set; } = [];
}
