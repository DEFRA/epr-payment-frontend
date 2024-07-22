using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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

            if (!Regex.IsMatch(url, pattern))
            {
                return new ValidationResult("The URL is not valid.");
            }

            return ValidationResult.Success!;
        }
    }
}
