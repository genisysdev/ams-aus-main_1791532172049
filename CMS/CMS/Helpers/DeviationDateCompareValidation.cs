using CME.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace CME.Helpers
{
    public class DeviationDateCompareValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null) return ValidationResult.Success;
            var model = (CMEViewModel)validationContext.ObjectInstance;
            if (Convert.ToDateTime(model.ChangeStartDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat) > Convert.ToDateTime(model.ChangeEndDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat))
            {
                return new ValidationResult("* The Deviation Start Date must be before the Deviation End Date");
            }
            else
            {
                return ValidationResult.Success;
            }
        }
    }
}