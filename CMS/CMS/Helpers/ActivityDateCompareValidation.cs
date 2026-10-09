using CME.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace CME.Helpers
{
    public class ActivityDateCompareValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null) return ValidationResult.Success;
            var model = (CMEViewModel)validationContext.ObjectInstance;
            if (Convert.ToDateTime(model.ActivityStartDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat) > Convert.ToDateTime(model.ActivityEndDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat))
            {
                return new ValidationResult("* The Activity Start Date must be before the Activity End Date");
            }
            else
            {
                return ValidationResult.Success;
            }
        }
    }
}