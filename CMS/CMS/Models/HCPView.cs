using DataAccessLayer;
using DataAccessLayer.DAL.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CME.Models
{
    public class HCPView
    {
        [Required(ErrorMessage = "* Please Enter Vendor Number")]
        [Display(Name = "Vendor Number")]
        public string VendorNo { get; set; }

        [Required(ErrorMessage = "* Please Enter TMF No")]
        [StringLength(9, MinimumLength = 9, ErrorMessage = "* Invalid TMF Number")]
        [Display(Name = "TMF Number")]
        public string TMFNo { get; set; }

        [Required(ErrorMessage = "* Please Enter First Name")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "* Please Enter Last Name")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "* Please Enter Coupa Number")]
        [Display(Name = "CMS Number")]
        public int? CMSNo { get; set; }

        [Required(ErrorMessage = "* Please Enter HCP Address")]
        [Display(Name = "HCP Address")]
        public string HCPAddress { get; set; }

        [Required(ErrorMessage = "* Please select Therapy Area")]
        public List<string> SelectedUnit { get; set; }
        [Display(Name = "Therapy Area")]

        public IEnumerable<SelectListItem> TherapyArea { get; set; }

        public string Customer_ID { get; set; }
        [MinLength(1), DataType(DataType.EmailAddress), EmailAddress, MaxLength(50)]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "Please enter a valid e-mail adress")]

        public string Email_Id { get; set; }
        public string Phone_Number { get; set; }
        public string City { get; set; }
        public bool Is_Contract { get; set; }
        public int Id { get; set; }
        public Nullable<System.DateTime> ContractStartDate { get; set; }
        public Nullable<System.DateTime> ContractEndDate { get; set; }
        public List<HCPViewModel> SearchNameList { get; set; }
    }
}