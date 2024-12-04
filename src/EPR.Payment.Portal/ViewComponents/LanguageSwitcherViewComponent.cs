using EPR.Payment.Portal.Common.Constants;
using EPR.Payment.Portal.ViewModels;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.FeatureManagement;

namespace EPR.Payment.Portal.ViewComponents
{
    public class LanguageSwitcherViewComponent : ViewComponent
    {
        private readonly IOptions<RequestLocalizationOptions> _localizationOptions;
        private readonly IFeatureManager _featureManager;

        public LanguageSwitcherViewComponent(IOptions<RequestLocalizationOptions> localizationOptions, IFeatureManager featureManager)
        {
            _localizationOptions = localizationOptions;
            _featureManager = featureManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var cultureFeature = HttpContext.Features.Get<IRequestCultureFeature>();

            // Construct the return URL, ensuring it includes the base path
            var rawReturnUrl = $"{Request.Path}{Request.QueryString}";
            var sanitizedReturnUrl = Url.Content(rawReturnUrl) ?? "/payment/"; // Default to base path

            var languageSwitcherModel = new LanguageSwitcherModel
            {
                SupportedCultures = _localizationOptions.Value.SupportedCultures!.ToList(),
                CurrentCulture = cultureFeature!.RequestCulture.Culture,
                ReturnUrl = sanitizedReturnUrl,
                ShowLanguageSwitcher = await _featureManager.IsEnabledAsync(nameof(FeatureFlags.ShowLanguageSwitcher))
            };

            return View(languageSwitcherModel);
        }
    }
}
