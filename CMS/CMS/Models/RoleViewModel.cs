#region
using System.ComponentModel.DataAnnotations;
#endregion

namespace CME.Models
{
    public class RoleViewModel
    {
        [Required(ErrorMessage = "* Please Enter Name")]
        [MaxLength(50, ErrorMessage = "* Name length should not be more than 50")]
        public string Name { get; set; }
        [MaxLength(250, ErrorMessage = "* Description length should not be more than 250")]
        public string Description { get; set; }
        public byte SortOrder { get; set; }
    }
}