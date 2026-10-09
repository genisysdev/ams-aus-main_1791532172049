using System.ComponentModel.DataAnnotations;

namespace CME.Models
{
    public class Login
    {
        [Required(ErrorMessage = "* Please enter user name")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "* Please enter password")]
        public string Password { get; set; }
    }
}