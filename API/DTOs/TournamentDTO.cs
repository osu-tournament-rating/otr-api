using System.Text.Json.Serialization;
using Database.Enums;
using Database.Enums.Verification;

namespace API.DTOs;

/// <summary>
/// Represents a tournament
/// </summary>
public class TournamentDTO
{
    /// <summary>
    /// Id
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Timestamp of submission
    /// </summary>
    public DateTime Created { get; set; }

    /// <summary>
    /// Name
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// Acronym / shortened name
    /// <example>For osu! World Cup 2023, this value would be "OWC2023"</example>
    /// </summary>
    public string Abbreviation { get; set; } = null!;

    /// <summary>
    /// The osu! forum post advertising this tournament
    /// </summary>
    public string ForumUrl { get; set; } = null!;

    /// <summary>
    /// Lowest rank a player can be to participate
    /// </summary>
    /// <example>For a 10,000-50,000 tournament, this value would be 10,000</example>
    public int RankRangeLowerBound { get; set; }

    /// <summary>
    /// The ruleset the tournament was played in
    /// </summary>
    public Ruleset Ruleset { get; set; }

    /// <summary>
    /// Expected in-match team size
    /// </summary>
    /// <example>For a 2v2 team size 4 tournament, this value would be 2</example>
    public int LobbySize { get; set; }

    /// <summary>
    /// Verification status
    /// </summary>
    public VerificationStatus VerificationStatus { get; set; }

    /// <summary>
    /// Rejection reason
    /// </summary>
    public TournamentRejectionReason RejectionReason { get; set; }

    /// <summary>
    /// Processing status
    /// </summary>
    public TournamentProcessingStatus ProcessingStatus { get; set; }

    /// <summary>
    /// The start date of the first match played in the tournament
    /// </summary>
    public DateTime StartTime { get; set; }

    /// <summary>
    /// The end date of the last match played in the tournament
    /// </summary>
    public DateTime EndTime { get; set; }

    /// <summary>
    /// Id of the user that submitted the tournament
    /// </summary>
    public int? SubmittedByUserId { get; set; }

    /// <summary>
    /// Id of the user that verified the tournament
    /// </summary>
    public int? VerifiedByUserId { get; set; }

    /// <summary>
    /// Associated match data
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public ICollection<MatchDTO> Matches { get; set; } = new List<MatchDTO>();
}
