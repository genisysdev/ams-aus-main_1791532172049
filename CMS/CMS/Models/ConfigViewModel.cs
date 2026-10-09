using System.ComponentModel.DataAnnotations;

namespace CME.Models
{
    public class ConfigViewModel
    {
        [Required(ErrorMessage = "* Please Enter Name")]
        [Display(Name = "Name")]
        public string Name { get; set; }
        [Required(ErrorMessage = "* Please Enter Value")]
        [Display(Name = "Value")]
        public string Value { get; set; }
        [Required(ErrorMessage = "* Please Enter Description")]
        [Display(Name = "Description")]
        public string Description { get; set; }
        public bool Status { get; set; }
    }
}