using CME.Helpers;
using DataAccessLayer.DAL.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;

namespace CME.Models
{
    public class CMEViewModel
    {
        public int ID { get; set; }
        public string CME_Type { get; set; }
        public bool HospitalMeeting { get; set; }
        public bool IsVirtual { get; set; }
        public bool IsPostPoned { get; set; }
        public string IssuedDate { get; set; }
        public string IssuedByInitial { get; set; }
        [Required(ErrorMessage = "* Please select Therapy Area")]
        public List<string> SelectedTherapyArea { get; set; }
        [Display(Name = "Therapy Area")]
        public IEnumerable<SelectListItem> TherapyArea { get; set; }
        [Required(ErrorMessage = "* Please select Brand")]
        public List<string> SelectedBrand { get; set; }
        [Display(Name = "Brand")]
        public IEnumerable<SelectListItem> Brand { get; set; }
        [Required(ErrorMessage = "* Please select Event")]
        public List<string> SelectedEvent { get; set; }
        [Display(Name = "Event")]
        public IEnumerable<SelectListItem> Event { get; set; }
        [Required(ErrorMessage = "* Please select Lecture")]
        public List<string> SelectedLecture { get; set; }
        [Display(Name = "Lecture")]
        public IEnumerable<SelectListItem> Lectures { get; set; }
        [Required(ErrorMessage = "* Please select activity name")]
        public string ActivityName { get; set; }
        [Helpers.ActivityDateCompareValidation]
        [Required(ErrorMessage = "* Please select activity start date")]
        public string ActivityStartDate { get; set; }
        [Required(ErrorMessage = "* Please select activity end date")]
        public string ActivityEndDate { get; set; }
        public int? NumberofDays { get; set; }
        [Helpers.DeviationDateCompareValidation]
        public string ChangeStartDate { get; set; }
        public string ChangeEndDate { get; set; }
        [Required(ErrorMessage = "* Please enter proposed number of attendee")]
        public int? Proposednumberofattendee { get; set; }
        [Helpers.IntegerCompareValidation]
        [Required(ErrorMessage = "* Please enter minimum number of guarantee")]
        public int? Minimumnumberofgaurantee { get; set; }
        public decimal? Approximatecost { get; set; }
        public decimal? Roomrate { get; set; }
        public decimal? Banquetroomrate { get; set; }
        public decimal? CostPerPAX { get; set; }
        [Required(ErrorMessage = "* Please enter city")]
        public string City { get; set; }
        [Required(ErrorMessage = "* Please enter venue details")]
        [DataType(DataType.MultilineText)]
        public string Venu_Hotel { get; set; }
        [Required(ErrorMessage = "* Please select venue star")]
        public bool Hotel_Star { get; set; }
        public string MName { get; set; }
        public decimal MCost { get; set; }
        public List<PAFMiscellaneous> Miscellaneous { get; set; }
        public List<string> SelectedTypeofHCP { get; set; }
        [Display(Name = "Type of HCP")]        
        public IEnumerable<SelectListItem> TypeofHCP { get; set; }
        public List<string> SelectedNameoftheHCP { get; set; }
        public List<string> SelectedNameoftheHCPNo { get; set; }
        [Display(Name = "Type of Name of the HCP")]
        public IEnumerable<SelectListItem> NameoftheHCP { get; set; }

        [StringLength(9, MinimumLength = 9, ErrorMessage = "* Invalid TMF Number")]
        [Display(Name = "TMF Number")]
        public string TMFNo { get; set; }
        public string HCPFirstName { get; set; }
        public string HCPLastName { get; set; }
        public decimal HCPCost { get; set; }
        public string HCPFileAttached { get; set; }
        public HttpPostedFileBase ImageFile { get; set; }
        public List<PAFHCPViewModel> HCPs { get; set; }
        [Helpers.PAFCommentValidation]
        [Required(ErrorMessage = "* Please enter comments")]
        public string Comment { get; set; }
        public List<PAFComment> Comments { get; set; }
        public string RequestType { get; set; }
        public string Approver { get; set; }
        public string ManagerInitial { get; set; }
        public bool IsDeviationApproved { get; set; }
        public bool IsDateDeviation { get; set; }
        [Helpers.PAImageValidation]
        public HttpPostedFileBase PAImageFile { get; set; }
        [Helpers.HQImageValidation]
        public HttpPostedFileBase HQImageFile { get; set; }
        public List<HttpPostedFileBase> LMImageFile { get; set; }
        [Required(ErrorMessage = "* Please upload document")]
        public List<HttpPostedFileBase> PostLMImageFile { get; set; }
        [Helpers.VenueStarValidation]
        public HttpPostedFileBase HERImageFile { get; set; }
        public IEnumerable<LectureMaterials> ImagePath { get; set; }
        public bool IsDeleteFile { get; set; }
        public IEnumerable<LectureMaterials> LMFiles { get; set; }
        public List<HttpPostedFileBase> NOHImageFile { get; set; }
        public string HCPType { get; set; }
        public string PAImageFileType { get; set; }
        public string HQImageFileType { get; set; }
        public string HERImageFileType { get; set; }
        public List<HttpPostedFileBase> ICPImageFile { get; set; }
        public List<HttpPostedFileBase> PLMImageFile { get; set; }
        public HttpPostedFileBase CCImageFile { get; set; }
        public HttpPostedFileBase ASImageFile { get; set; }
        public string DiseaseArea_Type { get; set; }
        public byte TherapyAreaId { get; set; }
        public string TherapyAreaName { get; set; }
        public string IssuerZone { get; set; }
        public Nullable<int> LectureId { get; set; }
        public string ActualStartDate { get; set; }
        public string ActualEndDate { get; set; }
        public string Cost_Center { get; set; }
        public bool? IsBTC { get; set; }
        [Display(Name = "Meeting Type")]
        public IEnumerable<SelectListItem> MeetingType { get; set; }
        [Required(ErrorMessage = "* Please select Meeting type")]
        public List<string> SelectedMeetingType { get; set; }
          public string AssessmentDocName { get; set; }
        public string AssessmentDocPath { get; set; }
        public string HCPtopic { get; set; }
        public string HCPtopicNoH { get; set; }
        public bool Policy_Violation { get; set; }
        public int HMinDays { get; set; }
        public int HMaxDays { get; set; }
        public int DMinDays { get; set; }
        public int DMaxDays { get; set; }
        public string Participant_Name { get; set; }
        public string Participant_Email_ID { get; set; }
        public string Participant_Phone_Number { get; set; }
        public string Participant_PAN { get; set; }
        public string Customer_ID { get; set; }
        public string PSearchCity { get; set; }
        public string PSearchName { get; set; }
        public string PSearchCustomerID { get; set; }
        public List<HCPViewModel> SearchParticipants { get; set; }

        public string JsonParticipantsdata { get; set; }

        public List<PAFParticipants> Participants { get; set; }
        public string Details_Comment { get; set; }
        public string Invoice_Comment { get; set; }
        public string HQ_Comment { get; set; }
        public string AM_Comment { get; set; }
        public string HE_Comment { get; set; }
        public string CC_Comment { get; set; }
        public string AS_Comment { get; set; }
        public List<HttpPostedFileBase> PEImageFile { get; set; }
        public string PE_Comment { get; set; }
        public string HCPSJson { get; set; }

        public string Inv_Comment { get; set; }
    }
}