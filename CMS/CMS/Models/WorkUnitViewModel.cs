using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CME.Models
{
    public class WorkUnitViewModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public HttpPostedFile Material { get; set; }
        public bool Status { get; set; }
        public IEnumerable<WorkUnitViewModel> WorkUnit_List { get; set; }
        public List<HttpPostedFileBase> ImageFile { get; set; }
        public string DocName { get; set; }
        public string DocPath { get; set; }
    }
}