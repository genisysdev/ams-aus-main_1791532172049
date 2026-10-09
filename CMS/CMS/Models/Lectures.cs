using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Web;
using System.Web.Mvc;
using DataAccessLayer.DAL.Model;

namespace CME.Models
{
    public class Lectures
    {
        public int ID { get; set; }
        [Required(ErrorMessage = "* Please select Therapy Area")]
        public List<string> SelectedTherapyArea { get; set; }
        [Display(Name = "Therapy Area")]
        public IEnumerable<SelectListItem> TherapyArea { get; set; }
        [Required(ErrorMessage = "* Please select Brand")]
        public List<string> SelectedBrand { get; set; }
        [Display(Name = "Brand")]
        public IEnumerable<SelectListItem> Brand { get; set; }
        [Required(ErrorMessage = "* Please select Event")]
        public List<string> SelectedEvent { get; set; }
        [Display(Name = "Event")]
        public IEnumerable<SelectListItem> Event { get; set; }
        [Required(ErrorMessage = "* Please enter a Lecture")]
        [MaxLength(250, ErrorMessage = "Code cannot be longer than 250 characters")]
        public string Lecture { get; set; }
        [Required(ErrorMessage = "* Please select Package Type")]
        public List<string> SelectedPackageType { get; set; }
        [Display(Name = "Package Type")]
        public IEnumerable<SelectListItem> PackageType { get; set; }
        public HttpPostedFile Material { get; set; }
        public bool Status { get; set; }
        [DisplayName("Upload File")]
        public IEnumerable<LectureMaterials> ImagePath { get; set; }
        public List<HttpPostedFileBase> ImageFile { get; set; }
    }
}