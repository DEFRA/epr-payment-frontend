using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public const string ErrorEcternalPaymentIdEmpty = "Payment data not found.";
    }
}
