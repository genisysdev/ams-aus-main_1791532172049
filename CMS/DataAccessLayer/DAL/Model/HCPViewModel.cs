using System;

namespace DataAccessLayer.DAL.Model
{
    public class HCPViewModel
    {
        public int ID { get; set; }
        public string VendorNo { get; set; }
        public string TMFNo { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public byte TAId { get; set; }
        public string TAName { get; set; }
        public decimal LectCost { get; set; }
        public string CMSNo { get; set; }
        public string HCPAddress { get; set; }
        public string HCPtopic { get; set; }
        public bool Is_Contract { get; set; }
        public string Customer_ID { get; set; }
        public string Email_Id { get; set; }
        public string Phone_Number { get; set; }
        public string City { get; set; }
        public DateTime? ContractStartDate { get; set; }
        public DateTime? ContractEndDate { get; set; }

        public string FullName
        {
            get { return FirstName + " " + LastName; }
        }
    }
}
