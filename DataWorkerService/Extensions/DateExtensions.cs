namespace DataWorkerService.Extensions;

public static class DateExtensions
{
    private static readonly DateTime s_placeholderDate = new(2007, 9, 17, 0, 0, 0);

    /// <summary>
    /// Denotes if the date is the database placeholder date
    /// </summary>
    public static bool IsPlaceholder(this DateTime date) =>
        date.Equals(s_placeholderDate) || date.Equals(DateTime.MinValue);
}
