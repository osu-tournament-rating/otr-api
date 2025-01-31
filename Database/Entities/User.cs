using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace Database.Entities;

/// <summary>
/// A user of the o!TR platform
/// </summary>
/// <remarks>
/// Not to be confused with <see cref="Player"/>.
/// Users only contain data tied to the o!TR platform and no data related to tournament participation or ratings
/// </remarks>
[Table("users")]
[SuppressMessage("ReSharper", "CollectionNeverUpdated.Global")]
[SuppressMessage("ReSharper", "PropertyCanBeMadeInitOnly.Global")]
public class User : UpdateableEntityBase
{
    /// <summary>
    /// Timestamp of the user's last login to the o!TR website
    /// </summary>
    [Column("last_login", TypeName = "timestamp with time zone")]
    public DateTime? LastLogin { get; set; }

    /// <summary>
    /// Timestamp of the most recent update to the <see cref="User"/>'s <see cref="User.Friends"/> list
    /// </summary>
    [Column("last_friends_list_update")]
    public DateTime? LastFriendsListUpdate { get; set; }

    /// <summary>
    /// A collection of string literals denoting special permissions granted to the user
    /// </summary>
    [Column("scopes")]
    public string[] Scopes { get; set; } = [];

    /// <summary>
    /// The <see cref="UserSettings"/> owned by the user
    /// </summary>
    public UserSettings Settings { get; set; } = null!;

    /// <summary>
    /// Id of the <see cref="Entities.Player"/> associated to the user
    /// </summary>
    [Column("player_id")]
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

    /// <summary>
    /// Players this user is following on osu!
    /// </summary>
    /// <remarks>
    /// We do not link to other <see cref="User"/>s because it is unlikely
    /// that all osu! users followed by this <see cref="User"/> will also
    /// be registered in our system. However, it is guaranteed that all osu! users followed
    /// will be <see cref="Player"/>s in our system (as they can be created easily).
    /// </remarks>
    public ICollection<Player> Friends { get; set; } = [];
}
