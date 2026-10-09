using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CME.Models
{
    public class UserViewModel
    {
        public int ID { get; set; }
        [Required(ErrorMessage = "* Please enter User Name")]
        public string Name { get; set; }
        [Required(ErrorMessage = "* Please enter  Surname")]
        public string Surname { get; set; }
        [Required(ErrorMessage = "* Please select Cost Center")]
        public string CostCenter { get; set; }
        [Required(ErrorMessage = "* Please enter Initial")]
        public string Initial { get; set; }
        [Required(ErrorMessage = "* Please select Manager")]
        public string Manager { get; set; }
        [Required(ErrorMessage = "* Please enter Position")]
        public string Position { get; set; }
        public string IsAdmin { get; set; }
        public string Status { get; set; }
        [Required(ErrorMessage = "* Please select Therapy Area")]
        public string SelectedTherapyArea { get; set; }
        public IEnumerable<SelectListItem> TherapyArea { get; set; }
        [Required(ErrorMessage = "* Please select Zone")]
        public string SelectedZone { get; set; }
        public IEnumerable<SelectListItem> Zone { get; set; }
        [Required(ErrorMessage = "* Please enter Email Id")]
        public string EmailId { get; set; }
        [Required(ErrorMessage = "* Please select Role")]
        public string SelectedRole { get; set; }
        public IEnumerable<SelectListItem> Role { get; set; }
        public IEnumerable<SelectListItem> CostCenterList { get; set; }
        public IEnumerable<SelectListItem> ManagerList { get; set; }
        [Required(ErrorMessage = "Email id is required")]
        public int EmpId { get; set; }

    }

    public class Excel
    {
        [Required(ErrorMessage = "Please upload excel file")]
        public HttpPostedFileBase FileUpload { get; set; }
        public string initials { get; set; }
        public string Name_of_Emp { get; set; }
        public string Manager_Initials { get; set; }
        public string Department_Name { get; set; }
        public string Cost_Centre { get; set; }
        public string Cost_Centre_Name { get; set; }
        public string Job_title { get; set; }
    }
}