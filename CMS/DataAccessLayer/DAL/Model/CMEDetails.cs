using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace DataAccessLayer.DAL.Model
{
    public class CMEDetails
    {
        public int Form_ID { get; set; }
        public string CME_Type { get; set; }
        public string DiseaseArea_Type { get; set; }
        public bool HospitalMeeting { get; set; }
        public bool? IsVirtual { get; set; }
        public bool IsPostPoned { get; set; }
        public int IssuedBy { get; set; }
        public string Issued_By_Initial { get; set; }
        public string Issuer_Role { get; set; }
        public string Name { get; set; }
        public string IssuedDate { get; set; }
        public string TherapyArea { get; set; }
        public string BrandName { get; set; }
        public string EventName { get; set; }
        public string LectureName { get; set; }
        public string ActivityName { get; set; }
        public string ActivityStartDate { get; set; }
        public string ActivityEndDate { get; set; }
        public int NumberofDays { get; set; }
        public string ChangeStartDate { get; set; }
        public string ChangeEndDate { get; set; }
        public int? Proposednumberofattendee { get; set; }
        public int? Minimumnumberofgaurantee { get; set; }
        [DisplayFormat(DataFormatString = "{0:N0}", ApplyFormatInEditMode = true)]
        public decimal? Approximatecost { get; set; }
        public decimal? Roomrate { get; set; }
        public decimal? Banquetroomrate { get; set; }
        public decimal? CostPerPAX { get; set; }
        public string CityName { get; set; }
        public string Venu_Hotel { get; set; }
        public List<PAFMiscellaneous> Miscellaneous { get; set; }
        public List<PAFTransaction> PAFTransactions { get; set; }
        public List<PAFHCPViewModel> HCPs { get; set; }
        public string Comment { get; set; }
        public List<PAFComment> Comments { get; set; }
        [DisplayFormat(DataFormatString = "{0:N0}", ApplyFormatInEditMode = true)]
        public decimal TotalCost { get; set; }
        public string RequestType { get; set; }
        public string Approver { get; set; }
        public List<HttpPostedFileBase> ICPImageFile { get; set; }
        [Required(ErrorMessage = "* Please upload document")]
        public List<HttpPostedFileBase> LMImageFile { get; set; }
        [Required(ErrorMessage = "* Please upload document")]
        public HttpPostedFileBase CCImageFile { get; set; }
        [Required(ErrorMessage = "* Please upload document")]
        public HttpPostedFileBase ASImageFile { get; set; }
        public IEnumerable<LectureMaterials> ImagePath { get; set; }
        public string CostCentre_Code { get; set; }
        public List<PAFAttachment> PAF_Filename { get; set; }
        public IEnumerable<LectureMaterials> LMFiles { get; set; }
        public string ActivityCloseDate { get; set; }
        public HttpPostedFileBase PAImageFile { get; set; }
        public HttpPostedFileBase HQImageFile { get; set; }
        public HttpPostedFileBase HERImageFile { get; set; }
        public List<HttpPostedFileBase> NOHImageFile { get; set; }
        public List<HttpPostedFileBase> PLMImageFile { get; set; }
        public List<HttpPostedFileBase> PEImageFile { get; set; }
        public bool Hotel_Star { get; set; }
        [DataType(DataType.Date)]
        public DateTime ModifiedDate { get; set; }
        public bool IsDownloadSapFile { get; set; }
        public bool? IsBTC { get; set; }
        public int Login_User_role_id { get; set; }
        public string Login_User_role_name { get; set; }
        public string Cost_Center { get; set; }
        public bool? Is_Assessment { get; set; }
        public string MeetingTypeName { get; set; }
        public string HCPtopic { get; set; }
        public string AssessmentDocName { get; set; }
        public string AssessmentDocPath { get; set; }
        public bool? Policy_Violation { get; set; }
        public bool IsDateDeviation { get; set; }
        public int Login_User_id { get; set; }
        public double Runrate { get; set; }
        public List<PAFParticipants> Participants { get; set; }
        public int IHCPId { get; set; }
        public decimal IHCPCost { get; set; }
        public string Invoice_No { get; set; }
        public string Invoice_Date { get; set; }
        public decimal? Total_Amount { get; set; }
        public decimal? Tax_Amount { get; set; }
        public decimal? Net_Amount { get; set; }
        public bool Is_Honorarium { get; set; }
        public string Inv_Comment { get; set; }
        public string AM_Comment { get; set; }
        public string CC_Comment { get; set; }
        public string AS_Comment { get; set; }
        public string PE_Comment { get; set; }
        public string PAM_Comment { get; set; }
        public string PAF_Comment { get; set; }
        public string HCPSJson { get; set; }
        public string SAP_ReportGeneration { get; set; }
        public int? Actual_Attendees { get; set; }
        public decimal? Final_CME_Expenses { get; set; }
    }
}
