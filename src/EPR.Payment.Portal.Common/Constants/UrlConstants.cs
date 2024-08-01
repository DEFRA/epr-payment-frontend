namespace EPR.Payment.Portal.Common.Constants
{
    public static class UrlConstants
    {
        // Payments facade endpoints
        public const string PaymentsRetrieveData = "callback/retrieve-payment-data";
        public const string PaymentsComplete = "payments/{externalPaymentId}/complete";
    }
}
