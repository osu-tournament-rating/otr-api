using Newtonsoft.Json;

namespace APITests.Osu;

public class MatchScoreTests
{
	[Test]
	[TestCase(1100, 22, 0, 0, 98.69)]
	[TestCase(899, 25, 8, 21, 95.35)]
	public void StandardAccuracy_Computation_IsCorrect(int threeHundreds, int oneHundreds, int fifties, int misses, double expectedAccuracy)
	{
		var matchScore = new MatchScore
		{
			Count50 = fifties,
			Count100 = oneHundreds,
			Count300 = threeHundreds,
			CountMiss = misses
		};
		
		Assert.That(Math.Abs(expectedAccuracy - matchScore.AccuracyStandard), Is.LessThan(0.01));
	}
	
	[Test]
	[TestCase(2990, 157, 0, 0, 0, 0, 99.92)]
	[TestCase(2539, 237, 0, 2, 0, 2, 99.74)]
	public void ManiaAccuracy_Computation_IsCorrect(int max, int threeHundreds, int twoHundreds, int oneHundreds, int fifties, int misses, double expectedAccuracy)
	{
		var matchScore = new MatchScore
		{
			CountGeki = max,
			Count300 = threeHundreds,
			CountKatu = twoHundreds,
			Count100 = oneHundreds,
			Count50 = fifties,
			CountMiss = misses
		};
		
		Assert.That(Math.Abs(expectedAccuracy - matchScore.AccuracyMania), Is.LessThan(0.01));
	}

	[Test]
	[TestCase(1563, 62, 0, 9, 97.55)]
	[TestCase(1814, 70, 0, 0, 98.14)]
	public void TaikoAccuracy_Computation_IsCorrect(int threeHundreds, int oneHundreds, int fifties, int misses, double expectedAccuracy)
	{
		var matchScore = new MatchScore
		{
			Count50 = fifties,
			Count100 = oneHundreds,
			Count300 = threeHundreds,
			CountMiss = misses
		};
		
		Assert.That(Math.Abs(expectedAccuracy - matchScore.AccuracyTaiko), Is.LessThan(0.01));
	}

	[Test]
	[TestCase(1522, 4, 164, 21, 6, 98.43)]
	public void CatchAccuracy_Computation_IsCorrect(int threeHundreds, int oneHundreds, int fifties, int katu, int misses, double expectedAccuracy)
	{
		var matchScore = new MatchScore
		{
			Count50 = fifties,
			Count100 = oneHundreds,
			Count300 = threeHundreds,
			CountKatu = katu,
			CountMiss = misses
		};
		
		Assert.That(Math.Abs(expectedAccuracy - matchScore.AccuracyCatch), Is.LessThan(0.01));
	}

	[Test]
	public void Accuracy_IncludedInJsonSerialization()
	{
		var matchScore = new MatchScore()
		{
			Count300 = 500
		};

		string json = JsonConvert.SerializeObject(matchScore);
		Assert.That(json, Contains.Substring("Accuracy"));
		Assert.That(json.Contains("100.0"));
	}
}