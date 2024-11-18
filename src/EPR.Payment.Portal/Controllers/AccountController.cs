namespace EPR.Payment.Portal.Controllers;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.Unicode;
using System.Web;

/// <summary>
/// Controller used in web apps to manage accounts.
/// </summary>
[Route("[controller]/[action]")]
public class AccountController : Controller
{
    /// <summary>
    /// Handles the user sign-out.
    /// </summary>
    /// <param name="scheme">Authentication scheme.</param>
    /// <returns>Sign out result.</returns>
    [ExcludeFromCodeCoverage(Justification = "Unable to mock authentication")]
    [HttpGet("{scheme?}")]
    public IActionResult SignOut(
        [FromRoute] string? scheme)
    {
        if (AppServicesAuthenticationInformation.IsAppServicesAadAuthenticationEnabled)
        {
            if (AppServicesAuthenticationInformation.LogoutUrl != null)
            {
                return LocalRedirect(AppServicesAuthenticationInformation.LogoutUrl);
            }

            return Ok();
        }

        scheme ??= OpenIdConnectDefaults.AuthenticationScheme;
        //string baseUrl = Configuration["BaseDomain"]; // Load from configuration
        //string relativeUrl = Url.Action("SignOut", "Account");
        //string fullUrl = new Uri(new Uri(baseUrl), "/report-data" + relativeUrl).ToString();
        var callbackUrl = Url.Action(action: "SignOut", controller: "Account", values: null, protocol: Request.Scheme).Replace("Account", "report-data/Account");

        return SignOut(
            new AuthenticationProperties
            {
                RedirectUri = callbackUrl,
            },
            CookieAuthenticationDefaults.AuthenticationScheme,
            scheme);
    }
}