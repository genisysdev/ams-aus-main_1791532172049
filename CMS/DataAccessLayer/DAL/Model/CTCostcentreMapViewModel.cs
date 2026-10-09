using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace DataAccessLayer.DAL.Model
{
    public class CTCostcentreMapViewModel
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public string UserInitial { get; set; }
        public int? CostCenterID { get; set; }
        public string CostCenter_Code { get; set; }
        public List<string> CTCostCenterCode { get; set; }
    }

    public class CostCentreMapView
    {
        public IEnumerable<SelectListItem> CostCenter { get; set; }
        public IEnumerable<SelectListItem> User { get; set; }
        public IEnumerable<SelectListItem> CostCenterName { get; set; }
        [Required(ErrorMessage = "* Please select any Cost Center")]
        public List<string> SelectedCostCenter { get; set; }
        [Required(ErrorMessage = "* Please select any User Initial")]
        public int? selectedUserId { get; set; }
        public List<string> SelectedUser { get; set; }
        public SelectList UserList { get; set; }
        public List<CTCostcentreMapViewModel> CTCCList { get; set; }
        public int LogginUserId { get; set; }
    }
}
