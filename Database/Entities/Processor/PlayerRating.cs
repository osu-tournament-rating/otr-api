﻿using Common.Enums;

namespace Database.Entities.Processor;

/// <summary>
/// Represents a summary of current rating data for a <see cref="Player"/> in a <see cref="Common.Enums.Ruleset"/>
/// </summary>
/// <remarks>
/// Generated by the <a href="https://docs.otr.stagec.xyz/o-tr-processor.html">o!TR Processor</a>
/// <br/><br/>
/// A <see cref="Player"/> may only have one <see cref="PlayerRating"/> per <see cref="Common.Enums.Ruleset"/> at any given time.
/// <br/><br/>
/// For more in depth documentation, see
/// <a href="https://docs.otr.stagec.xyz/rating-calculation.html#rating">
/// o!TR Rating Calculation Documentation
/// </a>
/// </remarks>
public class PlayerRating : EntityBase
{
    /// <summary>
    /// The <see cref="Common.Enums.Ruleset"/> that the <see cref="PlayerRating"/> was generated for
    /// </summary>
    public Ruleset Ruleset { get; init; }

    /// <summary>
    /// osu! Tournament Rating... The number we're all here for!
    /// </summary>
    public double Rating { get; init; }

    /// <summary>
    /// Measure of how "strong" a single change in <see cref="Rating"/> can be
    /// </summary>
    public double Volatility { get; init; }

    /// <summary>
    /// Global <see cref="Rating"/> percentile
    /// </summary>
    public double Percentile { get; init; }

    /// <summary>
    /// Global rank
    /// </summary>
    public int GlobalRank { get; init; }

    /// <summary>
    /// Country rank
    /// </summary>
    public int CountryRank { get; init; }

    /// <summary>
    /// Id of the <see cref="Player"/> that the <see cref="PlayerRating"/> was generated for
    /// </summary>
    public int PlayerId { get; init; }

    /// <summary>
    /// The <see cref="Player"/> that the <see cref="PlayerRating"/> was generated for
    /// </summary>
    public Player Player { get; init; } = null!;

    /// <summary>
    /// A collection of <see cref="RatingAdjustment"/>s that represent
    /// the individual changes to the <see cref="PlayerRating"/> over time
    /// </summary>
    public ICollection<RatingAdjustment> Adjustments { get; init; } = [];
}
