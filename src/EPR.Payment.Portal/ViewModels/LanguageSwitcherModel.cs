using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace EPR.Payment.Portal.ViewModels
{
    [ExcludeFromCodeCoverage]
    public class LanguageSwitcherModel
    {
        public CultureInfo CurrentCulture { get; set; } = null!;

        public List<CultureInfo> SupportedCultures { get; set; } = null!;

        public string ReturnUrl { get; set; } = null!;

        public bool ShowLanguageSwitcher { get; set; }
    }
}
