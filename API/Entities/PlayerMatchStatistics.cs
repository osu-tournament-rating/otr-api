namespace API.Entities;

/// <summary>
/// A set of statistics updated by the rating processor for each player.
/// This set of stats is for a single match. This is here to avoid duplicate data
/// in <see cref="PlayerStatistics"/>, as that data gets set on a per-game basis.
/// </summary>
public class PlayerMatchStatistics
{
	
}