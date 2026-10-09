using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.DAL.Model
{
    public class PAFHCPViewModel
    {
        public int Id { get; set; }
        public int PAFId { get; set; }
        public byte Type { get; set; }
        public string TypeName { get; set; }
        public string HCPAddress { get; set; }
        public int? HCPId { get; set; }
        public string TMF_No { get; set; }
        public string First_Name { get; set; }
        public string Last_Name { get; set; }
        public decimal Cost { get; set; }
        public string Invitation { get; set; }
        public string CMSNo { get; set; }
        public string InvitationMail { get; set; }
        public string SM { get; set; }
        public string AttachedFile { get; set; }
        public bool Is_Honorarium { get; set; }
        public string HCPtopic { get; set; }
        public string Vendor_No { get; set; }
        public string Invoice_No { get; set; }
        public string Invoice_Date { get; set; }
        public decimal? Total_Amount { get; set; }
        public decimal? Tax_Amount { get; set; }
        public decimal? Net_Amount { get; set; }
    }
}
