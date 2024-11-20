namespace EPR.Payment.Portal.Controllers;

using EPR.Payment.Portal.Common.Configuration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.FeatureManagement;
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
    private readonly string _rpdRootUrl;
    private readonly string _signOutUrl;
    public IFeatureManager FeatureManager { get; }

    public AccountController(IOptions<DashboardConfiguration> dashboardConfiguration, IFeatureManager featureManager)
    {
        if (dashboardConfiguration?.Value?.RPDRootUrl?.Url == null)
        {
            throw new ArgumentNullException(nameof(dashboardConfiguration.Value.RPDRootUrl));
        }

        if (dashboardConfiguration?.Value?.SignOutUrl?.Url == null)
        {
            throw new ArgumentNullException(nameof(dashboardConfiguration.Value.SignOutUrl));
        }

        _rpdRootUrl = dashboardConfiguration.Value.RPDRootUrl.Url;
        _signOutUrl = dashboardConfiguration.Value.SignOutUrl.Url;
        FeatureManager = featureManager;
    }
    

    /// <summary>
    /// Handles the user sign-out.
    /// </summary>
    /// <param name="scheme">Authentication scheme.</param>
    /// <returns>Sign out result.</returns>
    [ExcludeFromCodeCoverage(Justification = "Unable to mock authentication")]
    [HttpGet("{scheme?}")]
    public IActionResult SignOut([FromRoute] string? scheme)
    {
        if (FeatureManager.IsEnabledAsync("EnableAuthenticationFeature").GetAwaiter().GetResult())
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

            string callbackUrl = new Uri(new Uri(_rpdRootUrl), _signOutUrl).ToString();

            return SignOut(
                new AuthenticationProperties
                {
                    RedirectUri = callbackUrl,
                },
                CookieAuthenticationDefaults.AuthenticationScheme,
                scheme);
        }
        else {
            return Ok();
        }
    }
}