using System;

namespace DataAccessLayer.DAL.Model
{
    public class PAFList
    {
        public int Id { get; set; }
        public string IssuedBy { get; set; }
        public int CreatedBy { get; set; }
        public string ManagerInitial { get; set; }
        public string ManagerRole { get; set; }
        public int ManagerRoleID { get; set; }
        public DateTime StartDate { get; set; }
        public string ViewStartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string ViewEndDate { get; set; }
        public string CMEType { get; set; }
        public string Name { get; set; }
        public string CCCode { get; set; }
        public byte? TherapyAreaId { get; set; }
        public string TherapyArea { get; set; }
        public byte? ZoneId { get; set; }
        public string Zone { get; set; }
        public byte? BrandId { get; set; }
        public string Brand { get; set; }
        public string Approver { get; set; }
        public decimal TotalCost { get; set; }
        public DateTime Activity_Start_Date { get; set; }
        public string Approved_Or_Reject { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsDateDeviation { get; set; }
        public bool IsDateDeviationApproved { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string HotelName { get; set; }
        public bool? IsVirtual { get; set; }
        public bool? IsBTC { get; set; }
        public string Cost_Center { get; set; }
        public string Disease_Area { get; set; }
        public string HeadMgr { get; set; }
        public string IssuerRole { get; set; }
        public string SAPFileGenerated { get; set; }
        public DateTime IssuedDate { get; set; }
        public bool CostCentreToCTStatus { get; set; }
        public int? CostCentreUserId { get; set; }
    }
}
