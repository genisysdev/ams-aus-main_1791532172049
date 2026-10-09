using System.Collections.Generic;
using System.Web.Mvc;

namespace DataAccessLayer.DAL.Model
{
    public class LecturesViewModel
    {
        public List<string> SelectedTherapyArea { get; set; }
        public IEnumerable<SelectListItem> TherapyArea { get; set; }

        public List<string> SelectedBrand { get; set; }
        public IEnumerable<SelectListItem> Brand { get; set; }

        public List<string> SelectedEvent { get; set; }
        public IEnumerable<SelectListItem> Event { get; set; }

        public List<LecturesView> Lectures { get; set; }
    }

    public class LecturesView
    {
        public int ID { get; set; }
        public string Lecture { get; set; }
        public byte? TherapyAreaId { get; set; }
        public string TherapyAreaName { get; set; }
        public byte BrandId { get; set; }
        public string BrandName { get; set; }
        public int? EventId { get; set; }
        public string EventName { get; set; }
        public bool IsActive { get; set; }
        public byte PackageTypeId { get; set; }
        public string PackageTypeName { get; set; }
        public bool Status { get; set; }
        public string DocName { get; set; }
        public string DocPath { get; set; }
        public IEnumerable<LectureMaterials> Materials { get; set; }
    }

}
