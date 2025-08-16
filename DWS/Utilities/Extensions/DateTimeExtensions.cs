namespace DWS.Utilities.Extensions;

public static class DateTimeExtensions
{
    /// <summary>
    /// Checks if a <see cref="DateTime"/> is a placeholder value, defined as anything before or on <see cref="DateTime.UnixEpoch"/>
    /// </summary>
    /// <param name="dt">The <see cref="DateTime"/> to check</param>
    /// <returns>Whether the given <see cref="DateTime"/> is a placeholder</returns>
    public static bool IsPlaceholder(this DateTime dt) =>
        dt <= DateTime.UnixEpoch;
}
