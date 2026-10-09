using System;
using System.Collections.Generic;

namespace DataAccessLayer.DAL.Model
{
    public class CMEModel
    {
        public int ID { get; set; }
        public int IssuedBy { get; set; }
        public string PAFType { get; set; }
        public string CME_Type { get; set; }
        public string DiseaseArea_Type { get; set; }
        public bool HospitalMeeting { get; set; }
        public bool? IsVirtual { get; set; }
        public bool IsPostPoned { get; set; }
        public DateTime IssuedDate { get; set; }
        public byte TherapyAreaId { get; set; }
        public byte? BrandId { get; set; }
        public int? EventId { get; set; }
        public Nullable<int> LectureId { get; set; }
        public string ActivityName { get; set; }
        public DateTime ActivityStartDate { get; set; }
        public DateTime ActivityEndDate { get; set; }
        public int NumberofDays { get; set; }
        public DateTime? ChangeStartDate { get; set; }
        public DateTime? ChangeEndDate { get; set; }
        public int Proposednumberofattendee { get; set; }
        public int Minimumnumberofgaurantee { get; set; }
        public int Approximatecost { get; set; }
        public decimal Roomrate { get; set; }
        public decimal Banquetroomrate { get; set; }
        public decimal CostPerPAX { get; set; }
        public string City { get; set; }
        public string Venu_Hotel { get; set; }
        public bool Hotel_Star { get; set; }
        public List<PAFMiscellaneous> Miscellaneous { get; set; }
        public List<PAFHCPViewModel> HCPs { get; set; }
        public List<PAFComment> Comments { get; set; }
        public string Comment { get; set; }
        public decimal TotalCost { get; set; }
        public string RequestType { get; set; }
        public string ManagerInitial { get; set; }
        public string Approver { get; set; }
        public bool IsDeviationApproved { get; set; }
        public bool IsDateDeviation { get; set; }
        public IEnumerable<LectureMaterials> ImagePath { get; set; }
        public string IssuedByInitial { get; set; }
        public int? ManagerID { get; set; }
        public string Email { get; set; }
        public string ManagerEmail { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string IssuerZone { get; set; }
        public string IssuerRole { get; set; }
        public DateTime ActualStartDate { get; set; }
        public DateTime ActualEndDate { get; set; }
        public string Cost_Center { get; set; }
        public bool? IsBTC { get; set; }
        public byte? MeetingTypeId { get; set; }
        public bool? Policy_Violation { get; set; }
        public List<PAFParticipants> Participants { get; set; }
        public string Details_Comment { get; set; }
        public string Invoice_Comment { get; set; }
        public string HQ_Comment { get; set; }
        public string AM_Comment { get; set; }
        public string HE_Comment { get; set; }
        public string SAP_ReportGeneration { get; set; }
        public int? Actual_Attendees { get; set; }
        public decimal? Final_CME_Expenses { get; set; }
    }
}
