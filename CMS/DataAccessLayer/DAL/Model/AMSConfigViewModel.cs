using System;

namespace DataAccessLayer.DAL.Model
{
    public class AMSConfigViewModel
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public string Description { get; set; }
        public bool? Status { get; set; }
        public DateTime? Modified_Date { get; set; }
    }
}
