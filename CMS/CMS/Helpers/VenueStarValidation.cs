using CME.Models;
using System.ComponentModel.DataAnnotations;

namespace CME.Helpers
{
    public class VenueStarValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var model = (CMEViewModel)validationContext.ObjectInstance;
            if (model.Hotel_Star && model.HERImageFile == null && model.HERImageFileType == null)
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