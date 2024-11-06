using EPR.Payment.Portal.Common.Constants;
using Microsoft.AspNetCore.Localization;

namespace EPR.Payment.Portal.Sessions
{
    public class SessionRequestCultureProvider : RequestCultureProvider
    {
        public override async Task<ProviderCultureResult?> DetermineProviderCultureResult(HttpContext httpContext)
        {
            var culture = httpContext.Session.Get(Language.SessionLanguageKey) == null ? Language.English : httpContext.Session.GetString(Language.SessionLanguageKey);
            return await Task.FromResult(new ProviderCultureResult(culture));
        }
    }
}
