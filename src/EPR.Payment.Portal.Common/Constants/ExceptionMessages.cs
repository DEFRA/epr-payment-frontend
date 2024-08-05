namespace EPR.Payment.Portal.Common.Constants
{
    public static class ExceptionMessages
    {
        // HttpPaymentsFacade exceptions
        public const string PaymentFacadeBaseUrlMissing = "PaymentFacade BaseUrl configuration is missing";
        public const string PaymentFacadeEndPointNameMissing = "PaymentFacade EndPointName configuration is missing";
        public const string PaymentFacadeHttpClientNameMissing = "PaymentFacade HttpClientName configuration is missing";
        public const string ErrorRetrievingCompletePayment = "Error occured while retrieving complete payment.";
        public const string ErrorCompletePayment = "Error occured while completing payment.";
        public const string PaymentDataNotFound = "Payment data not found."; 
        public const string ErrorExternalPaymentIdEmpty = "Invalid payment ID.";
        public const string ErrorInvalidViewModel = "Invalid payment response view model.";
        public const string ErrorInitiatePayment = "Error occured while Initiating payment.";
        public const string ErrorInvalidPaymentRequestDto = "Invalid payment request model.";
    }
}
