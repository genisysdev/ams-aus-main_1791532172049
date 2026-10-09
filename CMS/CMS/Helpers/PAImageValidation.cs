using CME.Models;
using System.ComponentModel.DataAnnotations;

namespace CME.Helpers
{
    public class PAImageValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var model = (CMEViewModel)validationContext.ObjectInstance;
            if (model.PAImageFile == null && model.PAImageFileType == null)
            {
                return new ValidationResult("* Please upload document");
            }
            else
            {
                return ValidationResult.Success;
            }
        }
    }
}