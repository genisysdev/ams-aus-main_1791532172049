using CME.Models;
using System.ComponentModel.DataAnnotations;

namespace CME.Helpers
{
    public class PAFCommentValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var model = (CMEViewModel)validationContext.ObjectInstance;
            if (string.IsNullOrEmpty(model.Comment))
            {
                if (model.IsDateDeviation == true && model.Policy_Violation == true)
                {
                    return new ValidationResult("* please enter business justification for critical date violation");
                }
                else if (model.IsDateDeviation == true && model.Policy_Violation == true)
                {
                    return new ValidationResult("* please enter business justification for acceptable date violation");
                }
                else
                {
                    return new ValidationResult("* Please enter comment");
                }
            }
            else
            {
                return ValidationResult.Success;
            }
        }
    }
}