namespace API.DTOs;

/// <summary>
/// A member of a player's stats, this DTO represents 1 player, either a teammate or an opponent,
/// that the player has shared a match/game with.
/// </summary>
public class PlayerFrequencyDTO
{
	/// <summary>
	/// The id of the player whom we are interested in knowing how many games/matches
	/// they have shared with this player.
	/// </summary>
	public int PlayerId { get; set; }
	public long OsuId { get; set; }
	public string Username { get; set; } = null!;
	public int Frequency { get; set; }
}