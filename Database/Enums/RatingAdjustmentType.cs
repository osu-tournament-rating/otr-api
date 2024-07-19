namespace Database.Enums;

/// <summary>
/// Represents the different types of events that result in the generation of a <see cref="Entities.Processor.RatingAdjustment"/>
/// </summary>
public enum RatingAdjustmentType
{
    /// <summary>
    /// The <see cref="Entities.Processor.RatingAdjustment"/> is the initial rating
    /// </summary>
    Initial = 0,

    /// <summary>
    /// The <see cref="Entities.Processor.RatingAdjustment"/> is the result of a period of inactivity (decay)
    /// </summary>
    Decay = 1,

    /// <summary>
    /// The <see cref="Entities.Processor.RatingAdjustment"/> is the result of participation in a <see cref="Entities.Match"/>
    /// </summary>
    Match = 2
}
