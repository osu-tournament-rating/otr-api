namespace API.DTOs;

/// <summary>
/// A collection of booleans represeting which tiers to filter.
///
/// Null = No filter
/// False = Explictly excluded
/// True = Explicitly included
/// </summary>
public class LeaderboardTierFilterDTO
{
	public bool? FilterBronze { get; set; }
	public bool? FilterSilver { get; set; }
	public bool? FilterGold { get; set; }
	public bool? FilterPlatinum { get; set; }
	public bool? FilterDiamond { get; set; }
	public bool? FilterMaster { get; set; }
	public bool? FilterGrandmaster { get; set; }
	public bool? FilterEliteGrandmaster { get; set; }
}