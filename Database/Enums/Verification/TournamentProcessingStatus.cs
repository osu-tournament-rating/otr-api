namespace Database.Enums.Verification;

/// <summary>
/// The status of a <see cref="Entities.Tournament"/> in the processing flow
/// </summary>
public enum TournamentProcessingStatus
{
    /// <summary>
    /// The <see cref="Entities.Tournament"/> is awaiting approval from a
    /// <see cref="Entities.User"/> with verifier permission
    /// </summary>
    /// <remarks>
    /// Functions as the entry point to the processing flow. No entities owned by a <see cref="Entities.Tournament"/>
    /// will advance through the processing flow until approved.
    /// </remarks>
    NeedsApproval = 0,

    /// <summary>
    /// The <see cref="Entities.Tournament"/> has <see cref="Entities.Match"/>es with a
    /// <see cref="MatchProcessingStatus"/> of <see cref="MatchProcessingStatus.NeedsData"/>
    /// </summary>
    NeedsData = 1,

    /// <summary>
    /// The <see cref="Entities.Tournament"/> needs automation checks
    /// </summary>
    NeedsAutomationChecks = 2,

    /// <summary>
    /// The <see cref="Entities.Tournament"/> is awaiting verification from a
    /// <see cref="Entities.User"/> with verifier permission
    /// </summary>
    NeedsVerification = 3,

    /// <summary>
    /// The <see cref="Entities.Tournament"/> needs stat calculation
    /// </summary>
    NeedsStatCalculation = 4,

    /// <summary>
    /// The tournament has completed all processing steps
    /// </summary>
    Done = 5
}
