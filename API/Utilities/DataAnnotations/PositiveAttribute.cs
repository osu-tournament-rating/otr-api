using System.ComponentModel.DataAnnotations;

namespace API.Utilities.DataAnnotations;

/// <summary>
/// Data annotation to restrict a property to only a range of positive integer values. By default, the range is
/// one to <see cref="int.MaxValue"/>
/// </summary>
/// <param name="allowZero">
/// If true, the beginning of the range is zero instead of one
/// </param>
public class PositiveAttribute(bool allowZero = false) : RangeAttribute(allowZero ? 0 : 1, int.MaxValue);
