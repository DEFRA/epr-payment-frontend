using EPR.Payment.Portal.Common.Enums;

namespace EPR.Payment.Portal.Common.Dtos.Response
{
    public class CompletePaymentResponseDto
    {
        public PaymentStatus Status { get; set; }
        public string? Message { get; set; }
        public string? Reference { get; set; }
        public Guid? UserId { get; set; }
        public Guid? OrganisationId { get; set; }
        public string? Regulator { get; set; }
        public decimal? Amount { get; set; }
        public string? Email { get; set; }
        public string? Description { get; set; }
    }
}
