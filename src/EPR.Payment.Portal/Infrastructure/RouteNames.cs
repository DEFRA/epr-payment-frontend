using System.Diagnostics.CodeAnalysis;

namespace EPR.Payment.Portal.Infrastructure
{
    [ExcludeFromCodeCoverage]
    public static class RouteNames
    {
        public static class GovPay
        {
            public const string PaymentError = nameof(PaymentError);
            public const string PaymentSuccess = nameof(PaymentSuccess);
            public const string Paymentfailure = nameof(Paymentfailure);
            public const string GovPayCallback = nameof(GovPayCallback);
        }

    }
}
