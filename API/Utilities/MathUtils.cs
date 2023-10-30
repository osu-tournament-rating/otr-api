namespace API.Utilities;

public static class MathUtils
{
	/// <summary>
	/// Gets the median from the list
	/// </summary>
	/// <typeparam name="T">The data type of the list</typeparam>
	/// <param name="values">The list of values</param>
	/// <returns>The median value</returns>
	public static T? Median<T>(List<T?> values)
	{
		if (values.Count == 0)
			return default;
		values.Sort();
		return values[values.Count / 2];
	}
}