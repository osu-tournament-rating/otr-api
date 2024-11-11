using System.ComponentModel.DataAnnotations.Schema;
using Database.Entities.Interfaces;
using Database.Enums.Verification;
using Database.Utilities;

namespace Database.Entities;

public abstract class ProcessableEntityBase : UpdateableEntityBase, IProcessableEntity
{
    [Column("verification_status")]
    public VerificationStatus VerificationStatus { get; set; }

    [Column("last_processing_date")]
    public DateTime LastProcessingDate { get; set; }

    public abstract void ResetAutomationStatuses(bool force);
    public void ConfirmPreVerificationStatus() => VerificationStatus = EnumUtils.ConfirmPreStatus(VerificationStatus);
}
