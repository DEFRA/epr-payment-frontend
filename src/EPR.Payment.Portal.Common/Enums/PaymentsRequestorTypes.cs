using System.ComponentModel;

namespace EPR.Payment.Portal.Common.Enums
{
    public enum PaymentsRequestorTypes
    {
        [Description("Producers")]
        Producers,
        [Description("ComplianceSchemes")]
        ComplianceSchemes,
        [Description("Exporters")]
        Exporters,
        [Description("Reprocessors")]
        Reprocessors,
    }
}
