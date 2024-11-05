using EPR.Payment.Portal.ViewModels;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace EPR.Payment.Portal.ViewComponents
{
    public class LanguageSwitcherViewComponent : ViewComponent
    {
        private readonly IOptions<RequestLocalizationOptions> _localizationOptions;

        public LanguageSwitcherViewComponent(IOptions<RequestLocalizationOptions> localizationOptions)
        {
            _localizationOptions = localizationOptions;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var cultureFeature = HttpContext.Features.Get<IRequestCultureFeature>();
            var languageSwitcherModel = new LanguageSwitcherModel
            {
                SupportedCultures = _localizationOptions.Value.SupportedCultures!.ToList(),
                CurrentCulture = cultureFeature!.RequestCulture.Culture,
                ReturnUrl = $"~{Request.Path}{Request.QueryString}",
                ShowLanguageSwitcher = true // TODO: Set Feature Flag
            };

            return View(languageSwitcherModel);
        }
    }
}
