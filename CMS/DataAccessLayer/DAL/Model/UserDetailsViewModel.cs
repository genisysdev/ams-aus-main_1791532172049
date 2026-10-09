using System.Collections.Generic;
using System.Web.Mvc;

namespace DataAccessLayer.DAL.Model
{
    public class UserDetailsViewModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string CostCenter { get; set; }
        public string Initial { get; set; }
        public string Manager { get; set; }
        public int ManagerId { get; set; }
        public IEnumerable<SelectListItem> ManagerList { get; set; }
        public string Position { get; set; }
        public bool Status { get; set; }
        public string MManager { get; set; }

        public byte RoleId { get; set; }
        public List<string> SelectedRole { get; set; }
        public IEnumerable<SelectListItem> Role { get; set; }

        public int ZoneId { get; set; }
        public List<string> SelectedZone { get; set; }
        public IEnumerable<SelectListItem> Zone { get; set; }

        public int TherapyAreaId { get; set; }
        public List<string> SelectedTherapyArea { get; set; }
        public IEnumerable<SelectListItem> TherapyArea { get; set; }

        public int CostCenterId { get; set; }
        public IEnumerable<SelectListItem> CostCenterList { get; set; }

        public string EmailId { get; set; }
        public bool IsAdmin { get; set; }
        public int? EmpId { get; set; }
        public int ModifiedBy { get; set; }
    }
}
