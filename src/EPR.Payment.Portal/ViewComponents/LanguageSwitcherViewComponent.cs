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
            var requestPath = !Request.Path.HasValue ? "/" : Request.Path.Value!;
            var languageSwitcherModel = new LanguageSwitcherModel
            {
                SupportedCultures = _localizationOptions.Value.SupportedCultures!.ToList(),
                CurrentCulture = cultureFeature!.RequestCulture.Culture,
                ReturnUrl = $"~{requestPath}{Request.QueryString}",
                ShowLanguageSwitcher = await _featureManager.IsEnabledAsync(nameof(FeatureFlags.ShowLanguageSwitcher))
            };

            return View(languageSwitcherModel);
        }
    }
}
