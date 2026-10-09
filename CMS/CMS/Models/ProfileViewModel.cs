using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CME.Models
{
    public class ProfileViewModel
    {
        public byte Id { get; set; }
        [Required(ErrorMessage = "* Please Enter Name")]
        [MaxLength(50, ErrorMessage = "* Name length should not be more than 50")]
        public string Name { get; set; }
        [MaxLength(250, ErrorMessage = "* Description length should not be more than 250")]
        public string Description { get; set; }
    }
}