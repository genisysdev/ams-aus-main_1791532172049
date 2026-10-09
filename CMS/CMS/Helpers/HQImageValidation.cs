using CME.Models;
using System.ComponentModel.DataAnnotations;
namespace CME.Helpers
{
    public class HQImageValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var model = (CMEViewModel)validationContext.ObjectInstance;
            if (model.HQImageFile == null && model.HQImageFileType == null && model.IsVirtual == false)
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