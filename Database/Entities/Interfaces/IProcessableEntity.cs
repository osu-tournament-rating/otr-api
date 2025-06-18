using Common.Enums.Verification;
using JetBrains.Annotations;

namespace Database.Entities.Interfaces;

/// <summary>
/// Interfaces an entity that is processed by the Data Worker Service
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.Members)]
public interface IProcessableEntity : IUpdateableEntity
{
    /// <summary>
    /// Verification status
    /// </summary>
    public VerificationStatus VerificationStatus { get; set; }

    /// <summary>
    /// The last time the entity was processed
    /// </summary>
    public DateTime LastProcessingDate { get; set; }

    /// <summary>
    /// Reset all properties which are managed by automation checks to the default value
    /// </summary>
    /// <example>
    /// Consider the <see cref="Tournament"/> entity. This entity has, among other related fields,
    /// a VerificationStatus field and a ProcessingStatus field. This method sets the tournament's VerificationStatus
    /// field to None and the ProcessingStatus field to NeedsAutomationChecks.
    /// </example>
    /// <param name="force">Whether to overwrite data which has a VerificationStatus of Verified or Rejected</param>
    public void ResetAutomationStatuses(bool force);
}
