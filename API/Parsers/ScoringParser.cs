namespace API.Parsers;

public enum ScoringType
{
	Accuracy,
	Score,
	ScoreV2
}

public static class ScoringParser
{
	public static ScoringType Parse(string scoringType)
	{
		return scoringType switch
		{
			"Accuracy" => ScoringType.Accuracy,
			"Score" => ScoringType.Score,
			"Score v2" => ScoringType.ScoreV2,
			_ => throw new ArgumentException($"Invalid scoring type: {scoringType}")
		};
	}
}