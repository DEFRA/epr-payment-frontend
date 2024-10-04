using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace EPR.Payment.Portal.Common.Validators
{
    public class ValidUrlAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return new ValidationResult("The URL is required.");
            }

            string url = value.ToString()!;
            string pattern = @"^(http|https)://([\w-]+(\.[\w-]+)+)([/#?]?.*)$";
            var matchTimeout = TimeSpan.FromMilliseconds(100);

            if (!Regex.IsMatch(url, pattern, RegexOptions.None, matchTimeout))
            {
                return new ValidationResult("The URL is not valid.");
            }

            return ValidationResult.Success!;
        }
    }
}
