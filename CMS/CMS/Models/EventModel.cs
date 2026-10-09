using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CME.Models
{
    public class EventModel
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
        [Display(Name = "Events")]
        public IEnumerable<SelectListItem> Events { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public byte Sort_Order { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public byte Brand_Id { get; set; }
        public bool IsActive { get; set; }
    }
}