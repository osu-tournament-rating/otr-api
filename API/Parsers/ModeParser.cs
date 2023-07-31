namespace API.Parsers;

public enum Mode
{
	Standard,
	Mania,
	Taiko,
	CatchTheBeat
}

public static class ModeParser
{
	public static Mode Parse(string mode)
	{
		return mode switch
		{
			"Standard" => Mode.Standard,
			"Mania" => Mode.Mania,
			"Taiko" => Mode.Taiko,
			"Catch the Beat" => Mode.CatchTheBeat,
			_ => throw new ArgumentException($"Invalid mode: {mode}")
		};
	}
}