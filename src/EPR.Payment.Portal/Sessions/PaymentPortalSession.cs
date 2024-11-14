using EPR.Common.Authorization.Interfaces;
using EPR.Common.Authorization.Models;
using System.Diagnostics.CodeAnalysis;

namespace EPR.Payment.Portal.Sessions
{
    [ExcludeFromCodeCoverage]
    public class PaymentPortalSession : IHasUserData
    {
        public UserData UserData { get; set; } = new();
    }
}