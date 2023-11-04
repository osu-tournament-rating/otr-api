namespace API.DTOs;

/// <summary>
/// DTO for comparing two players' stats, assuming they have been teammates
/// </summary>
public class PlayerTeammateComparisonDTO
{
	/// <summary>
	/// The id of the player who is being compared against the teammate
	/// </summary>
	public int PlayerId { get; set; }
	/// <summary>
	/// The id of the teammate to compare to
	/// </summary>
	public int TeammateId { get; set; }
	/// <summary>
	/// The number of matches the two have played together
	/// </summary>
	public int MatchesPlayed { get; set; }
	/// <summary>
	/// The number of matches the player has won with the teammate
	/// </summary>
	public int MatchesWon { get; set; }
	/// <summary>
	/// The number of matches the player has lost with the teammate
	/// </summary>
	public int MatchesLost { get; set; }
	/// <summary>
	/// The number of games played with the teammate
	/// </summary>
	public int GamesPlayed { get; set; }
	/// <summary>
	/// The win rate of the player with the teammate (how many matches have they won together / how many matches have they played together)
	/// </summary>
	public double WinRate { get; set; }
	/// <summary>
	/// The sum of the rating changes of all matches played with the teammate
	/// </summary>
	public double RatingDelta { get; set; }
}