namespace Database.Enums;

/// <summary>
/// Represents the different types of events that create a <see cref="Entities.Processor.RatingAdjustment"/>
/// </summary>
public enum RatingAdjustmentType
{
    /// <summary>
    /// The <see cref="Entities.Processor.RatingAdjustment"/> is caused by a period of inactivity (decay)
    /// </summary>
    Decay = 0,

    /// <summary>
    /// The <see cref="Entities.Processor.RatingAdjustment"/> is caused by participation in a <see cref="Entities.Match"/>
    /// </summary>
    Match = 1
}
