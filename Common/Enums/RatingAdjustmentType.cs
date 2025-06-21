using JetBrains.Annotations;

namespace Common.Enums;

/// <summary>
/// Represents the different types of events that result in the generation of a <see cref="Database.Entities.Processor.RatingAdjustment"/>
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.Members)]
public enum RatingAdjustmentType
{
    /// <summary>
    /// The <see cref="Database.Entities.Processor.RatingAdjustment"/> is the initial rating
    /// </summary>
    Initial = 0,

    /// <summary>
    /// The <see cref="Database.Entities.Processor.RatingAdjustment"/> is the result of a period of inactivity (decay)
    /// </summary>
    Decay = 1,

    /// <summary>
    /// The <see cref="Database.Entities.Processor.RatingAdjustment"/> is the result of participation in a <see cref="System.Text.RegularExpressions.Match"/>
    /// </summary>
    Match = 2
}
