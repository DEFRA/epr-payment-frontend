﻿namespace EPR.Payment.Portal.Controllers;

using EPR.Payment.Portal.Common.Configuration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Web;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.Unicode;
using System.Web;

/// <summary>
/// Controller used in web apps to manage accounts.
/// </summary>
[Route("[controller]/[action]")]
public class AccountController(IOptions<DashboardConfiguration> dashboardConfiguration) : Controller
{
    private readonly DashboardConfiguration _dashboardConfiguration = dashboardConfiguration?.Value
        ?? throw new ArgumentNullException(nameof(dashboardConfiguration));

    private readonly string _rpdRootUrl = dashboardConfiguration.Value?.RPDRootUrl?.Url 
        ?? throw new ArgumentNullException(nameof(dashboardConfiguration.Value.RPDRootUrl));

    private readonly string _signOutUrl = dashboardConfiguration.Value?.SignOutUrl?.Url 
        ?? throw new ArgumentNullException(nameof(dashboardConfiguration.Value.SignOutUrl));

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

        string callbackUrl = new Uri(new Uri(_rpdRootUrl), _signOutUrl).ToString();

        return SignOut(
            new AuthenticationProperties
            {
                RedirectUri = callbackUrl,
            },
            CookieAuthenticationDefaults.AuthenticationScheme,
            scheme);
    }
}