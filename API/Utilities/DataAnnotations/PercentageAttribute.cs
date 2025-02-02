using System.ComponentModel.DataAnnotations;

namespace API.Utilities.DataAnnotations;

/// <summary>
/// Data annotation to restrict a property to only a range of double precision values from 0.0 to 1.0,
/// mimicking a percentage restriction
/// </summary>
public class PercentageAttribute() : RangeAttribute(0.0D, 1.0D);
