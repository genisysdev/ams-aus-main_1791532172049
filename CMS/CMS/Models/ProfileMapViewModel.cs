using DataAccessLayer.DAL.Model;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace CME.Models
{
    public class ProfileMapViewModel
    {
        public IEnumerable<SelectListItem> Profile { get; set; }
        [Required(ErrorMessage = "* Please select any Role")]
        public List<string> SelectedProfile { get; set; }
        public byte SelectedProfileMapId { get; set; }
        public SelectList ProfileList { get; set; }


        public IEnumerable<SelectListItem> User { get; set; }
        [Required(ErrorMessage = "* Please select any User Initial")]
        public List<string> SelectedUser { get; set; }

        public List<ProfileMapModel> ProfileMapList { get; set; }
        public int LogginUserId { get; set; }
    }
}