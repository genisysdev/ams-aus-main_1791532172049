using CME.Models;
using System.ComponentModel.DataAnnotations;

namespace CME.Helpers
{
    public class IntegerCompareValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null) return ValidationResult.Success;
            var model = (CMEViewModel)validationContext.ObjectInstance;
            if (model.Minimumnumberofgaurantee > model.Proposednumberofattendee)
            {
                return new ValidationResult("* Minimum number of guarantee should not be more than Proposed number of attendee(s)");
            }
            else
            {
                return ValidationResult.Success;
            }
        }
    }
}