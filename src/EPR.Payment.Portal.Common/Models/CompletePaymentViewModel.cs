﻿using EPR.Payment.Portal.Common.Enums;

namespace EPR.Payment.Portal.Common.Models
{
    public class CompletePaymentViewModel
    {
        public PaymentStatus Status { get; set; }
        public string? Message { get; set; }
        public string? Reference { get; set; }
        public Guid? UserId { get; set; }
        public Guid? OrganisationId { get; set; }
        public string? Regulator { get; set; }
        public decimal? Amount { get; set; }
    }
}
