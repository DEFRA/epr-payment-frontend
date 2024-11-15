using Microsoft.AspNetCore.Authentication.OAuth.Claims;
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using System.Text.Json;

namespace EPR.Payment.Portal.Extension
{
    [ExcludeFromCodeCoverage]
    public class CorrelationClaimAction : ClaimAction
    {
        public const string CorrelationClaimType = "correlationId";
        public CorrelationClaimAction()
            : base(CorrelationClaimType, ClaimValueTypes.String)
        {
        }
        public override void Run(JsonElement userData, ClaimsIdentity identity, string issuer)
        {
            if (identity.FindFirst(claim => claim.Type == CorrelationClaimType) == null)
            {
                identity.AddClaim(new Claim(CorrelationClaimType, Guid.NewGuid().ToString()));
            }
        }
    }
}