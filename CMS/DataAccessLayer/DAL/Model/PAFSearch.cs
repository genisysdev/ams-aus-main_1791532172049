using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace DataAccessLayer.DAL.Model
{
    public class PAFSearch
    {
        public string PAFId { get; set; }
        public string Initial { get; set; }
        public string Manager { get; set; }

        public string SelectedTherapyArea { get; set; }
        [Display(Name = "Theraphy Area")]
        public IEnumerable<SelectListItem> TherapyArea { get; set; }


        public string SelectedBrand { get; set; }
        [Display(Name = "Brand")]
        public IEnumerable<SelectListItem> Brand { get; set; }


        public string SortOrder { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public int UserId { get; set; }
        public string UserRole { get; set; }
        public string UserZone { get; set; }
        public string HotelName { get; set; }

        public IEnumerable<HcpPaymentResult> Data { get; set; }

        [Required(ErrorMessage = "* Please select request status type.")]
        public string SelectedPafRequestType { get; set; }
        [Display(Name = "PafRequestType")]
        public IEnumerable<SelectListItem> PafRequestType { get; set; }

    }
}
