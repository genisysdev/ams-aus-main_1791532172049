using CME.Models;
using DataAccessLayer;
using DataAccessLayer.DAL.CMENewRepository;
using DataAccessLayer.DAL.CommonRespository;
using DataAccessLayer.DAL.ConfigRepository;
using DataAccessLayer.DAL.HCPListRepository;
using DataAccessLayer.DAL.LecturesRepository;
using DataAccessLayer.DAL.Model;
using DataAccessLayer.DAL.PAFRepository;
using DataAccessLayer.DAL.UserRepository;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.IO;
using System.Web.Mvc;
using PagedList;
using System.Web;
using Newtonsoft.Json;
using DataAccessLayer.DAL.CMERepository;

namespace CME.Controllers
{
    public class CMENewController : Controller
    {
        private readonly ICMENewRepository _cMENewRepository;
        private readonly IPAFRepository _pAFRepository;
        private readonly IHCPListRepository _hCPListRepository;
        private readonly IUserRepository _userRepository;
        private readonly ICommonRespository _commonRepository;
        private readonly IConfigRepository _configRepository;
        private readonly ILecturesRepository _lecturesRepository;
        private readonly ICMERepository _cMERepository;

        public CMENewController()
        {
            this._cMENewRepository = new CMENewRepository(new NovoNordiskEntities());
            this._pAFRepository = new PAFRepository(new NovoNordiskEntities());
            this._hCPListRepository = new HCPListRepository(new NovoNordiskEntities());
            this._userRepository = new UserRepository(new NovoNordiskEntities());
            this._commonRepository = new CommonRespository(new NovoNordiskEntities());
            this._configRepository = new ConfigRepository(new NovoNordiskEntities());
            this._lecturesRepository = new LecturesRepository(new NovoNordiskEntities());
            this._cMERepository = new CMERepository(new NovoNordiskEntities());
        }

        // GET: CMENew
        public ActionResult Index()
        {
            var emptyList = new List<SelectListItem>();
            var model = new CMEViewModel();
            try
            {
                ViewBag.CMSMenu = "active";
                Session.Remove("CMEFullDetail");
                Session.Remove("PAFLMList");

                //get paf id
                var newPAFId = _pAFRepository.GetPAFId();
                newPAFId++;
                Session["newPAFId"] = newPAFId;

                Session["PAFChar"] = Session["LoginUserTherapyArea"].ToString().Substring(0, 1);

                string approver = Convert.ToString(Session["LoginUserManager"].ToString());
                if (!string.IsNullOrEmpty(Session["LoginUserMManager"].ToString()))
                { approver = approver + " / " + Convert.ToString(Session["LoginUserMManager"].ToString()); }

                TempData.Remove("PAFTherapyArea"); TempData.Remove("PAFTherapyAreaName"); TempData.Remove("PAFBrand"); TempData.Remove("PAFEvent"); TempData.Remove("PAFTypeofHCP"); TempData.Remove("PAFCity"); TempData.Remove("PAFNameoftheHCP"); TempData.Remove("PAFHCPViewModel"); TempData.Remove("PAFMiscellaneous"); TempData.Remove("PAFMeetingType"); TempData.Remove("PAFAssessment"); TempData.Remove("PAFParticipants");

                //string zoneName = Convert.ToString(Session["LoginUserZone"]);


                model.ID = newPAFId;
                model.CME_Type = "Standard Meeting";
                model.Approver = approver;
                model.IssuedDate = DateTime.UtcNow.ToString("dd-MM-yyyy");
                string[] therapyAreaConfig = ConfigurationManager.AppSettings["TherapyArea"].Split(';');

                //if (Session["LoginUserZone"].ToString() == "Rare Disease")
                //{
                //    model.TherapyArea = _commonRepository.GetTherapyArea().Select(z => new SelectListItem { Text = z.Name.ToString(), Value = z.ID.ToString() }).OrderBy(s => s.Text).ToList();
                //    model.DiseaseArea_Type = "Hemophilia Medical";
                //    model.TherapyAreaName = model.TherapyArea.Where(s => s.Text == "Rare Disease").FirstOrDefault().Text;
                //    model.TherapyAreaId = Convert.ToByte(model.TherapyArea.Where(s => s.Text == "Rare Disease").FirstOrDefault().Value);
                //    model.TherapyArea = emptyList;
                //    model.Brand = _commonRepository.GetBrands().Select(s => new SelectListItem { Text = s.Name.ToString(), Value = s.ID.ToString(), Selected = s.Therapy_Area_Id == model.TherapyAreaId }).OrderBy(s => s.Text).ToList();
                //}
                //else
                //{
                //    model.TherapyArea = _commonRepository.GetTherapyArea().Where(a => a.Name != "Biopharm").Select(s => new SelectListItem { Text = s.Name.ToString(), Value = Convert.ToString(s.ID) }).ToList();
                //    model.Brand = emptyList;
                //}

                model.TherapyArea = _commonRepository.GetTherapyArea().Where(a => a.Name != "Biopharm").Select(s => new SelectListItem { Text = s.Name.ToString(), Value = Convert.ToString(s.ID) }).ToList();
                model.Brand = emptyList;


                model.Event = emptyList;
                model.Lectures = emptyList;
                model.TypeofHCP = _hCPListRepository.GetTypeOfHCP().Select(z => new SelectListItem { Text = z.Name.ToString(), Value = z.Id.ToString() }).OrderBy(s => s.Text).ToList();
                //model.NameoftheHCP = _hCPListRepository.GetAllZone().Select(s => new SelectListItem { Text = s.FirstName.ToString() + s.LastName.ToString(), Value = Convert.ToString(s.ID) }).ToList();
                model.MeetingType = _commonRepository.GetPafMeetingtype().Select(z => new SelectListItem { Text = z.Name.ToString(), Value = z.Id.ToString() }).OrderBy(s => s.Text).ToList();

                var amsConfigDetail = _configRepository.GetAMSconfig().Select(s => new SelectListItem { Text = s.Name, Value = s.Value }).ToList();
                foreach (var i in amsConfigDetail)
                {
                    if (i.Text == "Hospital_meeting_min_days") { model.HMinDays = Int32.Parse(i.Value); }
                    else if (i.Text == "Hospital_meeting_max_days") { model.HMaxDays = Int32.Parse(i.Value); }
                    else if (i.Text == "Diabetes_meeting_min_days") { model.DMinDays = Int32.Parse(i.Value); }
                    else if (i.Text == "Diabetes_meeting_max_days") { model.DMaxDays = Int32.Parse(i.Value); }
                }

                TempData.Add("PAFTherapyArea", model.TherapyArea);
                TempData.Add("PAFTherapyAreaName", model.TherapyAreaName);
                TempData.Add("PAFBrand", model.Brand);
                TempData.Add("PAFEvent", model.Event);
                TempData.Add("PAFCity", model.City);
                TempData.Add("PAFTypeofHCP", model.TypeofHCP);
                TempData.Add("PAFNameoftheHCP", model.NameoftheHCP);
                TempData.Add("PAFMeetingType", model.MeetingType);
                model.ImagePath = new List<LectureMaterials>();
                model.LMFiles = null;
                Session.Add("CMEFullDetail", model);

                return View(model);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult Index(CMEViewModel model, string command)
        {
            try
            {
                var sModel = (CMEViewModel)Session["CMEFullDetail"];
                var cme = new CMEModel();

                var cmeController = new CMEController();

                string[] therapyAreaConfig = ConfigurationManager.AppSettings["TherapyArea"].Split(';');
                var sLMModel = (List<LectureMaterials>)sModel.ImagePath ?? new List<LectureMaterials>();
                List<SelectListItem> emptyList = new List<SelectListItem>();
                if (!sModel.IsDeleteFile)
                {
                    if (model.PAImageFile != null && model.PAImageFile.ContentLength > 0 && sLMModel.SingleOrDefault(s => s.Type == "PA") == null)
                    {
                        var lmModel = SaveFile(model.PAImageFile, "PA");
                        sLMModel.Add(lmModel);
                    }
                    if (model.HQImageFile != null && model.HQImageFile.ContentLength > 0 && sLMModel.SingleOrDefault(s => s.Type == "HQ") == null)
                    {
                        var lmModel = SaveFile(model.HQImageFile, "HQ");
                        sLMModel.Add(lmModel);
                    }

                    //if (!therapyAreaConfig.Contains(Session["LoginUserZone"].ToString())) //checking for not Biopharm                     
                    //{
                    if (model.LMImageFile[0] != null && model.LMImageFile[0].ContentLength > 0 && sLMModel.SingleOrDefault(s => s.Type == "LM") == null)
                    {
                        foreach (var obj in model.LMImageFile)
                        {
                            if (obj.ContentLength > 0)
                            {
                                var lmModel = SaveFile(obj, "LM");
                                sLMModel.Add(lmModel);
                            }
                        }
                    }
                    //}
                    if (model.HERImageFile != null && model.HERImageFile.ContentLength > 0 && sLMModel.SingleOrDefault(s => s.Type == "HER") == null)
                    {
                        var lmModel = SaveFile(model.HERImageFile, "HER");
                        sLMModel.Add(lmModel);
                    }
                }
                var amsConfigData = _configRepository.GetAMSconfig().Select(s => new SelectListItem { Text = s.Name, Value = s.Value }).ToList();
                foreach (var i in amsConfigData)
                {
                    if (i.Text == "Hospital_meeting_min_days") { model.HMinDays = Int32.Parse(i.Value); }
                    else if (i.Text == "Hospital_meeting_max_days") { model.HMaxDays = Int32.Parse(i.Value); }
                    else if (i.Text == "Diabetes_meeting_min_days") { model.DMinDays = Int32.Parse(i.Value); }
                    else if (i.Text == "Diabetes_meeting_max_days") { model.DMaxDays = Int32.Parse(i.Value); }
                }


                if (sModel.IsDeleteFile) { sModel.IsDeleteFile = false; }
                sModel.ImagePath = sLMModel;
                model.ImagePath = sLMModel;
                model.PAImageFile = sModel.PAImageFile;
                model.HQImageFile = sModel.HQImageFile;
                model.LMImageFile = sModel.LMImageFile;
                model.HERImageFile = sModel.HERImageFile;
                model.LMFiles = (List<LectureMaterials>)Session["PAFLMList"];
                model.HCPs = (List<PAFHCPViewModel>)TempData["PAFHCPViewModel"];
                model.Participants = (List<PAFParticipants>)TempData["PAFParticipants"];
                Session.Remove("CMEFullDetail");
                Session.Add("CMEFullDetail", sModel);
                string approver = Convert.ToString(Session["LoginUserManager"].ToString());
                var userData = _userRepository.GetManagerStatus(approver);
                if (!string.IsNullOrEmpty(Session["LoginUserMManager"].ToString()))
                { approver = approver + " / " + Convert.ToString(Session["LoginUserMManager"].ToString()); }
                model.Approver = approver;
                if (command == "CostNext")
                {
                    model.Miscellaneous = TempData["PAFMiscellaneous"] as List<PAFMiscellaneous>;
                    TempData["TabDisplay"] = "Valid3";
                }
                if (command == "HCPNext")
                {
                    if ((model.HCPs != null && model.HCPs.Count == 0) || model.HCPs == null)
                    {
                        model.TherapyArea = TempData["PAFTherapyArea"] as IEnumerable<SelectListItem>;
                        TempData.Keep("PAFTherapyArea");
                        //if (therapyAreaConfig.Contains(Session["LoginUserZone"].ToString()))
                        //{
                        //    model.TherapyAreaName = TempData["PAFTherapyAreaName"] as String;
                        //    TempData.Keep("PAFTherapyAreaName");
                        //}

                        model.TherapyAreaName = TempData["PAFTherapyAreaName"] as String;
                        TempData.Keep("PAFTherapyAreaName");
                        model.Brand = TempData["PAFBrand"] as IEnumerable<SelectListItem>;
                        TempData.Keep("PAFBrand");
                        model.Event = TempData["PAFEvent"] as IEnumerable<SelectListItem>;
                        TempData.Keep("PAFEvent");
                        model.Lectures = TempData["PAFLecture"] as IEnumerable<SelectListItem>;
                        TempData.Keep("PAFLecture");
                        TempData.Keep("PAFCity");
                        model.TypeofHCP = TempData["PAFTypeofHCP"] as IEnumerable<SelectListItem>;
                        TempData.Keep("PAFTypeofHCP");
                        model.NameoftheHCP = TempData["PAFNameoftheHCP"] as IEnumerable<SelectListItem>;
                        TempData.Keep("PAFNameoftheHCP");
                        model.MeetingType = TempData["PAFMeetingType"] as IEnumerable<SelectListItem>;
                        TempData.Keep("PAFMeetingType");


                        //if (!therapyAreaConfig.Contains(Session["LoginUserZone"].ToString()))
                        //{
                        //    if (model.SelectedTherapyArea[0] == "") { TempData["sTherapyArea"] = "* Please select Therapy Area"; }
                        //}
                        if (model.SelectedTherapyArea[0] == "") { TempData["sTherapyArea"] = "* Please select Therapy Area"; }
                        if (model.SelectedBrand[0] == "") { TempData["sBrandArea"] = "* Please select Brand"; }
                        if (model.SelectedEvent[0] == "") { TempData["sEventArea"] = "* Please select Event"; }
                        if (model.SelectedLecture[0] == "") { TempData["sLecture"] = "* Please select Lecture"; }
                        if (model.Minimumnumberofgaurantee <= 0) { TempData["Minnuofgu"] = "* Please select a valid minimum number of guarantee"; }
                        if ((model.HCPs != null && model.HCPs.Count == 0) || model.HCPs == null) { TempData["eAddHCP"] = "*Please add HCP"; }
                        // if ((model.Participants != null && model.Participants.Count == 0) || model.Participants == null) { TempData["eAddParticipants"] = "*Please add Participant"; }
                        if (model.SelectedMeetingType[0] == "") { TempData["sMeetingType"] = "* Please select Meeting Type"; }
                        if (model.IsBTC == null && model.IsVirtual == false) { TempData["sBTC"] = "* Please select BTC"; }

                        model.Miscellaneous = TempData["PAFMiscellaneous"] as List<PAFMiscellaneous>;
                        TempData.Remove("PAFMiscellaneous");
                        TempData.Add("PAFMiscellaneous", model.Miscellaneous);
                        TempData.Keep("PAFMiscellaneous");

                        model.HCPs = TempData["PAFHCPViewModel"] as List<PAFHCPViewModel>;
                        TempData.Remove("PAFHCPViewModel");
                        TempData.Add("PAFHCPViewModel", model.HCPs);
                        TempData.Keep("PAFHCPViewModel");

                        model.Participants = TempData["PAFParticipants"] as List<PAFParticipants>;
                        TempData.Remove("PAFParticipants");
                        TempData.Add("PAFParticipants", model.Participants);
                        TempData.Keep("PAFParticipants");

                        TempData["TabDisplay"] = "Valid1";

                        return View(model);
                    }
                    else
                    {
                        model.Miscellaneous = TempData["PAFMiscellaneous"] as List<PAFMiscellaneous>;
                        TempData["TabDisplay"] = "Valid2";
                    }
                }

                if (command == "DetailsNext")
                {
                    if (model.SelectedEvent == null && model.CME_Type == "Non-Standard Meeting")
                    {
                        model.SelectedEvent = new List<string>
                        {
                            ""
                        };
                    }
                    if (model.SelectedLecture == null && model.CME_Type == "Non-Standard Meeting")
                    {
                        model.SelectedLecture = new List<string>
                        {
                            ""
                        };
                    }
                    if (Convert.ToDateTime(model.ActivityStartDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat) > Convert.ToDateTime(model.ActivityEndDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat)
                        || Convert.ToDateTime(model.ChangeStartDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat) > Convert.ToDateTime(model.ChangeEndDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat)
                        || model.Minimumnumberofgaurantee > model.Proposednumberofattendee
                        || (model.CME_Type == "Non-Standard Meeting" && string.IsNullOrEmpty(model.ActivityName))
                        || model.SelectedTherapyArea[0] == "" && (Session["LoginUserZone"].ToString() != "Rare Disease")
                        || (model.SelectedBrand[0] == "" && model.ActivityName == null)
                        || (model.SelectedEvent[0] == "" && model.ActivityName == null)
                        || (model.SelectedLecture[0] == "" && model.ActivityName == null)
                        || model.Minimumnumberofgaurantee <= 0
                        || model.Minimumnumberofgaurantee == null
                        || userData.Status == false
                        || (model.IsBTC == null && model.IsVirtual == false)
                        || model.SelectedMeetingType[0] == "")
                    {
                        model.TherapyArea = TempData["PAFTherapyArea"] as IEnumerable<SelectListItem>;
                        TempData.Keep("PAFTherapyArea");

                        //if (therapyAreaConfig.Contains(Session["LoginUserZone"].ToString()))
                        //{
                        //    model.TherapyAreaName = TempData["PAFTherapyAreaName"] as String;
                        //    TempData.Keep("PAFTherapyAreaName");
                        //}
                        model.TherapyAreaName = TempData["PAFTherapyAreaName"] as String;
                        TempData.Keep("PAFTherapyAreaName");
                        model.Brand = TempData["PAFBrand"] as IEnumerable<SelectListItem>;
                        TempData.Keep("PAFBrand");
                        model.Event = TempData["PAFEvent"] as IEnumerable<SelectListItem>;
                        TempData.Keep("PAFEvent");
                        model.Lectures = TempData["PAFLecture"] as IEnumerable<SelectListItem>;
                        TempData.Keep("PAFLecture");
                        //added to avoid error
                        model.Lectures = emptyList;
                        TempData.Keep("PAFCity");
                        model.TypeofHCP = TempData["PAFTypeofHCP"] as IEnumerable<SelectListItem>;
                        TempData.Keep("PAFTypeofHCP");
                        model.NameoftheHCP = TempData["PAFNameoftheHCP"] as IEnumerable<SelectListItem>;
                        TempData.Keep("PAFNameoftheHCP");
                        model.MeetingType = TempData["PAFMeetingType"] as IEnumerable<SelectListItem>;
                        TempData.Keep("PAFMeetingType");

                        //if (!therapyAreaConfig.Contains(Session["LoginUserZone"].ToString()))
                        //{
                        //    if (model.SelectedTherapyArea[0] == "") { TempData["sTherapyArea"] = "* Please select Therapy Area"; }
                        //}
                        if (model.SelectedTherapyArea[0] == "") { TempData["sTherapyArea"] = "* Please select Therapy Area"; }
                        if (model.SelectedBrand[0] == "") { TempData["sBrandArea"] = "* Please select Brand"; }
                        if (model.SelectedEvent[0] == "") { TempData["sEventArea"] = "* Please select Event"; }
                        if (model.SelectedLecture[0] == "") { TempData["sLecture"] = "* Please select Lecture"; }
                        if (model.Minimumnumberofgaurantee <= 0) { TempData["Minnuofgu"] = "* Please select a valid minimum number of guarantee"; }
                        if (model.SelectedMeetingType[0] == "") { TempData["sMeetingType"] = "* Please select Meeting Type"; }
                        if (model.IsBTC == null && model.IsVirtual == false) { TempData["sBTC"] = "* Please select BTC"; }

                        model.Miscellaneous = TempData["PAFMiscellaneous"] as List<PAFMiscellaneous>;
                        TempData.Remove("PAFMiscellaneous");
                        TempData.Add("PAFMiscellaneous", model.Miscellaneous);
                        TempData.Keep("PAFMiscellaneous");

                        model.HCPs = TempData["PAFHCPViewModel"] as List<PAFHCPViewModel>;
                        TempData.Remove("PAFHCPViewModel");
                        TempData.Add("PAFHCPViewModel", model.HCPs);
                        TempData.Keep("PAFHCPViewModel");

                        model.Participants = TempData["PAFParticipants"] as List<PAFParticipants>;
                        TempData.Remove("PAFParticipants");
                        TempData.Add("PAFParticipants", model.Participants);
                        TempData.Keep("PAFParticipants");

                        return View(model);
                    }
                    else
                    {
                        model.TherapyArea = TempData["PAFTherapyArea"] as IEnumerable<SelectListItem>;
                        TempData.Keep("PAFTherapyArea");
                        //if (therapyAreaConfig.Contains(Session["LoginUserZone"].ToString()))
                        //{
                        //    model.TherapyAreaName = TempData["PAFTherapyAreaName"] as String;
                        //    TempData.Keep("PAFTherapyAreaName");
                        //}
                        model.TherapyAreaName = TempData["PAFTherapyAreaName"] as String;
                        TempData.Keep("PAFTherapyAreaName");
                        model.Brand = TempData["PAFBrand"] as IEnumerable<SelectListItem>;
                        TempData.Keep("PAFBrand");
                        model.Event = TempData["PAFEvent"] as IEnumerable<SelectListItem>;
                        TempData.Keep("PAFEvent");
                        model.Lectures = TempData["PAFLecture"] as IEnumerable<SelectListItem>;
                        TempData.Keep("PAFLecture");
                        //added to avoid error
                        if (model.Lectures == null)
                        {
                            model.Lectures = emptyList;
                        }

                        TempData.Keep("PAFCity");
                        model.TypeofHCP = TempData["PAFTypeofHCP"] as IEnumerable<SelectListItem>;
                        TempData.Keep("PAFTypeofHCP");
                        model.NameoftheHCP = TempData["PAFNameoftheHCP"] as IEnumerable<SelectListItem>;
                        TempData.Keep("PAFNameoftheHCP");
                        model.MeetingType = TempData["PAFMeetingType"] as IEnumerable<SelectListItem>;
                        TempData.Keep("PAFMeetingType");

                        cme.PAFType = command;
                        cme.IssuedBy = Convert.ToInt32(Session["LoginUserId"]);
                        cme.ManagerInitial = Convert.ToString(Session["LoginUserManager"].ToString());
                        cme.Approver = approver;
                        cme.CME_Type = model.CME_Type;
                        //if (cme.CME_Type == "Non-Standard Meeting" && Session["LoginUserZone"].ToString() == "Rare Disease")
                        //{
                        //    cme.DiseaseArea_Type = model.DiseaseArea_Type;
                        //}
                        //else
                        //{
                        //    cme.DiseaseArea_Type = null;
                        //}
                        cme.IssuedDate = Convert.ToDateTime(DateTime.Now, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                        cme.ActivityName = model.ActivityName;

                        //if (!therapyAreaConfig.Contains(Session["LoginUserZone"].ToString()))
                        //{
                        //    if (model.SelectedTherapyArea[0] != "" && model.SelectedTherapyArea.Count > 0)
                        //    {
                        //        cme.TherapyAreaId = Convert.ToByte(model.SelectedTherapyArea[0]);
                        //    }
                        //}
                        //else { cme.TherapyAreaId = sModel.TherapyAreaId; }
                        if (model.SelectedTherapyArea[0] != "" && model.SelectedTherapyArea.Count > 0)
                        {
                            cme.TherapyAreaId = Convert.ToByte(model.SelectedTherapyArea[0]);
                        }
                        if (model.SelectedBrand[0] != "" && model.SelectedBrand.Count > 0)
                        {
                            cme.BrandId = Byte.Parse(model.SelectedBrand[0]);
                        }
                        if (model.SelectedEvent[0] != "" && model.SelectedEvent.Count > 0)
                        {
                            cme.EventId = int.Parse(model.SelectedEvent[0]);
                        }
                        if (model.SelectedLecture[0] != "" && model.SelectedLecture.Count > 0)
                        {
                            cme.LectureId = int.Parse(model.SelectedLecture[0]);
                        }
                        if (model.SelectedMeetingType[0] != "" && model.SelectedMeetingType.Count > 0)
                        {
                            cme.MeetingTypeId = Byte.Parse(model.SelectedMeetingType[0]);
                        }
                        cme.ActivityStartDate = Convert.ToDateTime(model.ActivityStartDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                        cme.ActivityEndDate = Convert.ToDateTime(model.ActivityEndDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                        cme.NumberofDays = Convert.ToInt32(model.NumberofDays);
                        cme.Proposednumberofattendee = Convert.ToInt32(model.Proposednumberofattendee);
                        cme.Minimumnumberofgaurantee = Convert.ToInt32(model.Minimumnumberofgaurantee);
                        cme.City = model.City;
                        cme.Venu_Hotel = model.Venu_Hotel;
                        cme.Hotel_Star = model.Hotel_Star;
                        cme.HospitalMeeting = model.HospitalMeeting;
                        cme.IsVirtual = model.IsVirtual;
                        cme.IsBTC = model.IsBTC;
                        cme.IsDateDeviation = model.IsDateDeviation;
                        cme.Policy_Violation = model.Policy_Violation;
                        cme.Comment = model.Comment;
                        TempData["TabDisplay"] = "Valid1";
                        return View(model);
                    }
                }

                if (command == "Draft" || command == "Send")
                {
                    if (Convert.ToDateTime(model.ActivityStartDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat) > Convert.ToDateTime(model.ActivityEndDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat)
                        || Convert.ToDateTime(model.ChangeStartDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat) > Convert.ToDateTime(model.ChangeEndDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat)
                        || model.Minimumnumberofgaurantee > model.Proposednumberofattendee
                        || (sLMModel.Where(s => s.Type == "PA").Count() <= 0 && Convert.ToString(Session["LoginUserRole"].ToString()) != "KAM")
                        || (sLMModel.Where(s => s.Type == "HQ").Count() <= 0 && model.IsVirtual == false)
                        || (model.Hotel_Star && sLMModel.Where(s => s.Type == "HER").Count() <= 0)
                        || string.IsNullOrEmpty(model.Comment)
                        || (model.CME_Type == "Non-Standard Meeting" && string.IsNullOrEmpty(model.ActivityName))
                        || (model.HCPs != null && model.HCPs.Count == 0)
                        || model.HCPs == null
                        || model.SelectedTherapyArea[0] == "" /*&& Session["LoginUserZone"].ToString() != "Rare Disease"*/
                        || (model.SelectedBrand[0] == "" && model.ActivityName == null)
                        || (model.SelectedEvent[0] == "" && model.ActivityName == null)
                        || (model.SelectedLecture[0] == "" && model.ActivityName == null)
                        || model.Minimumnumberofgaurantee <= 0
                        || model.Minimumnumberofgaurantee == null
                        || userData.Status == false
                        || (model.IsBTC == null && model.IsVirtual == false)
                        || model.SelectedMeetingType[0] == "")
                    {
                        model.TherapyArea = TempData["PAFTherapyArea"] as IEnumerable<SelectListItem>;
                        TempData.Keep("PAFTherapyArea");

                        //if (therapyAreaConfig.Contains(Session["LoginUserZone"].ToString()))
                        //{
                        //    model.TherapyAreaName = TempData["PAFTherapyAreaName"] as String;
                        //    TempData.Keep("PAFTherapyAreaName");
                        //}
                        model.TherapyAreaName = TempData["PAFTherapyAreaName"] as String;
                        TempData.Keep("PAFTherapyAreaName");
                        model.Brand = TempData["PAFBrand"] as IEnumerable<SelectListItem>;
                        TempData.Keep("PAFBrand");
                        model.Event = TempData["PAFEvent"] as IEnumerable<SelectListItem>;
                        TempData.Keep("PAFEvent");
                        model.Lectures = TempData["PAFLecture"] as IEnumerable<SelectListItem>;
                        TempData.Keep("PAFLecture");
                        //added to avoid error
                        //model.Lectures = emptyList;

                        TempData.Keep("PAFCity");
                        model.TypeofHCP = TempData["PAFTypeofHCP"] as IEnumerable<SelectListItem>;
                        TempData.Keep("PAFTypeofHCP");
                        model.NameoftheHCP = TempData["PAFNameoftheHCP"] as IEnumerable<SelectListItem>;
                        TempData.Keep("PAFNameoftheHCP");
                        //added for meeting type
                        model.MeetingType = TempData["PAFMeetingType"] as IEnumerable<SelectListItem>;
                        TempData.Keep("PAFMeetingType");

                        //if (!therapyAreaConfig.Contains(Session["LoginUserZone"].ToString()))
                        //{
                        //    if (model.SelectedTherapyArea[0] == "") { TempData["sTherapyArea"] = "* Please select Therapy Area"; }
                        //}
                        if (model.SelectedTherapyArea[0] == "") { TempData["sTherapyArea"] = "* Please select Therapy Area"; }
                        if (model.SelectedBrand[0] == "") { TempData["sBrandArea"] = "* Please select Brand"; }
                        if (model.SelectedEvent[0] == "") { TempData["sEventArea"] = "* Please select Event"; }
                        if (model.SelectedLecture[0] == "") { TempData["sLecture"] = "* Please select Lecture"; }
                        if (model.Minimumnumberofgaurantee <= 0) { TempData["Minnuofgu"] = "* Please select a valid minimum number of guarantee"; }
                        if ((model.HCPs != null && model.HCPs.Count == 0) || model.HCPs == null) { TempData["eAddHCP"] = "*Please add HCP"; }
                        if (userData.Status == false)
                        {
                            TempData["managerStatusFalse"] = "* Your Manager is inactive, Please contact Administrator.";
                        }
                        //validation for Meeting Type
                        if (model.SelectedMeetingType[0] == "") { TempData["sMeetingType"] = "* Please select Meeting Type"; }
                        //validation for BTC
                        if (model.IsBTC == null && model.IsVirtual == false) { TempData["sBTC"] = "* Please select BTC"; }

                        model.Miscellaneous = TempData["PAFMiscellaneous"] as List<PAFMiscellaneous>;
                        TempData.Remove("PAFMiscellaneous");
                        TempData.Add("PAFMiscellaneous", model.Miscellaneous);
                        TempData.Keep("PAFMiscellaneous");

                        model.HCPs = TempData["PAFHCPViewModel"] as List<PAFHCPViewModel>;
                        TempData.Remove("PAFHCPViewModel");
                        TempData.Add("PAFHCPViewModel", model.HCPs);
                        TempData.Keep("PAFHCPViewModel");

                        model.Participants = TempData["PAFParticipants"] as List<PAFParticipants>;
                        TempData.Remove("PAFParticipants");
                        TempData.Add("PAFParticipants", model.Participants);
                        TempData.Keep("PAFParticipants");

                        if (sLMModel.Where(s => s.Type == "PA").Count() <= 0 || (sLMModel.Where(s => s.Type == "HQ").Count() <= 0 && model.IsVirtual == false) || (model.Hotel_Star && sLMModel.Where(s => s.Type == "HER").Count() <= 0))
                        {
                            TempData["TabDisplay"] = "Valid3";
                        }
                        return View(model);
                    }
                    else
                    {
                        cme.PAFType = command;
                        cme.IssuedBy = Convert.ToInt32(Session["LoginUserId"]);
                        cme.ManagerInitial = Convert.ToString(Session["LoginUserManager"].ToString());
                        cme.IssuerZone = Convert.ToString(Session["LoginUserZone"].ToString());
                        cme.IssuerRole = Convert.ToString(Session["LoginUserRole"].ToString());
                        cme.Approver = approver;
                        cme.CME_Type = model.CME_Type;
                        //if (cme.CME_Type == "Non-Standard Meeting" && Session["LoginUserZone"].ToString() == "Rare Disease")
                        //{
                        //    cme.DiseaseArea_Type = model.DiseaseArea_Type;

                        //}
                        //else
                        //{
                        //    cme.DiseaseArea_Type = null;
                        //}

                        cme.IssuedDate = Convert.ToDateTime(DateTime.Now, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                        cme.ActivityName = model.ActivityName;
                        //if (!therapyAreaConfig.Contains(Session["LoginUserZone"].ToString()))
                        //{
                        //    if (model.SelectedTherapyArea[0] != "" && model.SelectedTherapyArea.Count > 0)
                        //    {
                        //        cme.TherapyAreaId = Convert.ToByte(model.SelectedTherapyArea[0]);
                        //    }
                        //}
                        //else { cme.TherapyAreaId = sModel.TherapyAreaId; }
                        if (model.SelectedTherapyArea[0] != "" && model.SelectedTherapyArea.Count > 0)
                        {
                            cme.TherapyAreaId = Convert.ToByte(model.SelectedTherapyArea[0]);
                        }
                        if (model.SelectedBrand[0] != "" && model.SelectedBrand.Count > 0)
                        {
                            cme.BrandId = Byte.Parse(model.SelectedBrand[0]);
                        }
                        if (model.SelectedEvent[0] != "" && model.SelectedEvent.Count > 0)
                        {
                            cme.EventId = int.Parse(model.SelectedEvent[0]);
                        }
                        if (model.SelectedLecture[0] != "" && model.SelectedLecture.Count > 0)
                        {
                            cme.LectureId = int.Parse(model.SelectedLecture[0]);
                        }
                        //added for the meeting type
                        if (model.SelectedMeetingType[0] != "" && model.SelectedMeetingType.Count > 0)
                        {
                            cme.MeetingTypeId = Byte.Parse(model.SelectedMeetingType[0]);
                        }
                        cme.ActivityStartDate = Convert.ToDateTime(model.ActivityStartDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                        cme.ActivityEndDate = Convert.ToDateTime(model.ActivityEndDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                        cme.NumberofDays = Convert.ToInt32(model.NumberofDays);
                        cme.ChangeStartDate = !string.IsNullOrEmpty(model.ChangeStartDate) ? Convert.ToDateTime(model.ChangeStartDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat) : (DateTime?)null;
                        cme.ChangeEndDate = !string.IsNullOrEmpty(model.ChangeEndDate) ? Convert.ToDateTime(model.ChangeEndDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat) : (DateTime?)null;
                        cme.Proposednumberofattendee = Convert.ToInt32(model.Proposednumberofattendee);
                        cme.Minimumnumberofgaurantee = Convert.ToInt32(model.Minimumnumberofgaurantee);
                        cme.Approximatecost = Convert.ToInt32(model.Approximatecost);
                        cme.Roomrate = Convert.ToInt32(model.Roomrate);
                        cme.Banquetroomrate = Convert.ToInt32(model.Banquetroomrate);
                        cme.CostPerPAX = Convert.ToInt32(model.CostPerPAX);
                        cme.City = model.City;
                        cme.Venu_Hotel = model.Venu_Hotel;
                        cme.Hotel_Star = model.Hotel_Star;
                        cme.Miscellaneous = TempData["PAFMiscellaneous"] as List<PAFMiscellaneous>;
                        cme.HCPs = TempData["PAFHCPViewModel"] as List<PAFHCPViewModel>;
                        cme.Participants = TempData["PAFParticipants"] as List<PAFParticipants>;
                        cme.Comment = model.Comment;
                        cme.ImagePath = model.ImagePath;
                        cme.HospitalMeeting = model.HospitalMeeting;
                        cme.IsVirtual = model.IsVirtual;
                        cme.IsBTC = model.IsBTC;
                        cme.IsDateDeviation = model.IsDateDeviation;
                        cme.Policy_Violation = model.Policy_Violation;

                        var userDetail = _userRepository.GetUserByInitial(Session["LoginUserInitial"].ToString());
                        cme.Cost_Center = userDetail.CostCenter;

                        cmeController.Insert(cme);

                        Session.Remove("CMEFullDetail");
                        if (command == "Draft") { return RedirectToAction("Index"); }
                        else { return RedirectToAction("Index", "Home"); }
                    }
                }
                else
                {
                    model.TherapyArea = TempData["PAFTherapyArea"] as IEnumerable<SelectListItem>;
                    TempData.Keep("PAFTherapyArea");
                    //if (therapyAreaConfig.Contains(Session["LoginUserZone"].ToString()))
                    //{
                    //    model.TherapyAreaName = TempData["PAFTherapyAreaName"] as String;
                    //    TempData.Keep("PAFTherapyAreaName");
                    //}
                    model.TherapyAreaName = TempData["PAFTherapyAreaName"] as String;
                    TempData.Keep("PAFTherapyAreaName");
                    model.Brand = TempData["PAFBrand"] as IEnumerable<SelectListItem>;
                    TempData.Keep("PAFBrand");
                    model.Event = TempData["PAFEvent"] as IEnumerable<SelectListItem>;
                    TempData.Keep("PAFEvent");
                    model.Lectures = TempData["PAFLecture"] as IEnumerable<SelectListItem>;
                    TempData.Keep("PAFLecture");
                    if (model.Lectures == null)
                    {
                        model.Lectures = emptyList;
                    }
                    TempData.Keep("PAFCity");
                    model.TypeofHCP = TempData["PAFTypeofHCP"] as IEnumerable<SelectListItem>;
                    TempData.Keep("PAFTypeofHCP");
                    model.NameoftheHCP = TempData["PAFNameoftheHCP"] as IEnumerable<SelectListItem>;
                    TempData.Keep("PAFNameoftheHCP");
                    model.MeetingType = TempData["PAFMeetingType"] as IEnumerable<SelectListItem>;
                    TempData.Keep("PAFMeetingType");
                    //string zoneName = Convert.ToString(Session["LoginUserZone"]);
                    //var hcpRepository = _hCPListRepository.GetAllZone();

                    if (command == "Add HCP")
                    {
                        if (string.IsNullOrEmpty(model.SelectedTypeofHCP[0]) || string.IsNullOrEmpty(model.SelectedNameoftheHCP[0]) || model.HCPCost == 0 || model.HCPCost == null)
                        {
                            if (string.IsNullOrEmpty(model.SelectedTypeofHCP[0])) { TempData["eHCPType"] = "*Please Select HCP Type"; }
                            if (string.IsNullOrEmpty(model.SelectedNameoftheHCP[0])) { TempData["eHCPName"] = "*Please Select HCP Name"; }
                            if (model.HCPCost == 0 || model.HCPCost == null) { TempData["eHCPNCost"] = "*Please Select Honorarium payable to HCP"; }
                            TempData["TabDisplay"] = "Valid1";
                        }
                        else
                        {
                            if (!therapyAreaConfig.Contains(Session["LoginUserZone"].ToString()))
                            {
                                if ((model.SelectedTherapyArea[0] != "" && model.SelectedBrand[0] != "" && model.SelectedEvent[0] != "") || (model.SelectedTherapyArea[0] != "" && model.ActivityName != null))
                                {
                                    model.HCPs = TempData["PAFHCPViewModel"] as List<PAFHCPViewModel>;
                                    if (model.HCPs == null)
                                    { model.HCPs = new List<PAFHCPViewModel>(); }

                                    if (!string.IsNullOrEmpty(model.SelectedTypeofHCP[0]) && !string.IsNullOrEmpty(model.SelectedNameoftheHCP[0]) && model.HCPCost != 0)
                                    {
                                        var hcp = _hCPListRepository.GetHCPById(Convert.ToInt32(model.SelectedNameoftheHCP[0]));

                                        if (hcp != null)
                                        {
                                            if (model.HCPs.SingleOrDefault(s => s.First_Name == hcp.FirstName && s.Last_Name == hcp.LastName && s.HCPAddress == hcp.HCPAddress) == null)
                                            {
                                                //if (hcp.NoLecture > hcp.NoLectureGiven)
                                                //{
                                                if (model.HCPtopic == null || model.HCPtopic == "")
                                                {
                                                    model.HCPtopic = "No Topic";
                                                }
                                                model.HCPs.Add(new PAFHCPViewModel
                                                {
                                                    Type = Convert.ToByte(model.SelectedTypeofHCP[0]),
                                                    HCPId = hcp.ID,
                                                    First_Name = hcp.FirstName,
                                                    Last_Name = hcp.LastName,
                                                    TMF_No = hcp.TMFNo,
                                                    Cost = Convert.ToDecimal(model.HCPCost),
                                                    HCPAddress = hcp.HCPAddress,
                                                    HCPtopic = model.HCPtopic
                                                });
                                                //}
                                                //else { TempData["eNolect"] = "* This doctor exceeds the limit of HCP"; }
                                            }
                                        }

                                    }
                                    else if (!string.IsNullOrEmpty(model.SelectedTypeofHCP[1]) && !string.IsNullOrEmpty(model.HCPFirstName) && !string.IsNullOrEmpty(model.HCPLastName) && (model.NOHImageFile[0] != null && model.NOHImageFile[0].ContentLength > 0))
                                    {
                                        if (model.SelectedTypeofHCP[1].ToLower() == "speaker" && (model.HCPtopic == null || model.HCPtopic == ""))
                                        {
                                            TempData["eHCPNTopic"] = "* Please Enter the HCP topic ";
                                        }
                                        else if (model.HCPs.SingleOrDefault(s => s.TypeName == model.HCPType && s.First_Name == model.HCPFirstName && s.Last_Name == model.HCPLastName && s.HCPtopic == model.HCPtopicNoH) == null)
                                        {
                                            foreach (var obj in model.NOHImageFile)
                                            {
                                                if (obj.ContentLength > 0)
                                                {
                                                    var lmModel = SaveFile(obj, "NOH");
                                                    lmModel.HCP_First_Name = model.HCPFirstName;
                                                    lmModel.HCP_Last_Name = model.HCPLastName;
                                                    sLMModel.Add(lmModel);
                                                    TempData["fileNAme"] += obj.FileName + "+";
                                                }
                                            }
                                            if (model.HCPtopicNoH == null || model.HCPtopicNoH == "")
                                            {
                                                model.HCPtopicNoH = "No Topic";
                                            }

                                            model.HCPs.Add(new PAFHCPViewModel
                                            {
                                                Type = Convert.ToByte(model.SelectedTypeofHCP[1]),
                                                First_Name = model.HCPFirstName,
                                                Last_Name = model.HCPLastName,
                                                TMF_No = model.TMFNo,
                                                AttachedFile = TempData["fileNAme"].ToString(),
                                                HCPtopic = model.HCPtopicNoH
                                            });
                                        }
                                    }
                                    else
                                    {
                                        if (model.HCPType == "Hon" && (string.IsNullOrEmpty(model.SelectedTypeofHCP[0]) || string.IsNullOrEmpty(model.SelectedNameoftheHCP[0])))
                                        {
                                            if (string.IsNullOrEmpty(model.SelectedTypeofHCP[0])) { TempData["eHCPType"] = "*Please Select HCP Type"; }
                                            if (string.IsNullOrEmpty(model.SelectedNameoftheHCP[0])) { TempData["eHCPName"] = "*Please Select HCP Name"; }
                                            if (model.HCPCost == 0) { TempData["eHCPNCost"] = "*Please Select Honorarium payable to HCP"; }

                                        }

                                        else if (model.HCPType == "NoHon" && (string.IsNullOrEmpty(model.SelectedTypeofHCP[1]) || string.IsNullOrEmpty(model.HCPFirstName) || string.IsNullOrEmpty(model.HCPLastName) || string.IsNullOrEmpty(model.TMFNo) || model.HCPCost == 0 || model.NOHImageFile == null))
                                        {
                                            if (string.IsNullOrEmpty(model.SelectedTypeofHCP[1])) { TempData["eHCPNType"] = "*Please Select HCP Type"; }
                                            if (string.IsNullOrEmpty(model.HCPFirstName)) { TempData["eHCPNFName"] = "*Please Select HCP First Name"; }
                                            if (string.IsNullOrEmpty(model.HCPLastName)) { TempData["eHCPNLName"] = "*Please Select HCP Last Name"; }
                                            if (model.NOHImageFile == null) { TempData["eHCPNfile"] = "*Please attach file"; }
                                        }
                                    }
                                    TempData["PAFAddP"] = "Addp";
                                    TempData.Remove("PAFHCPViewModel");
                                    TempData.Add("PAFHCPViewModel", model.HCPs);
                                    model.Miscellaneous = TempData["PAFMiscellaneous"] as List<PAFMiscellaneous>;

                                    TempData["TabDisplay"] = "Valid1";
                                }
                                else
                                {
                                    if (model.SelectedTherapyArea[0] == "" && Session["LoginUserZone"].ToString() != "Rare Disease")
                                    { TempData["sTherapyArea"] = "* Please select Therapy Area"; }
                                    TempData["sBrandArea"] = "* Please select Brand";
                                    TempData["sEventArea"] = "* Please select Event";
                                    TempData["sLectureArea"] = "* Please select Lecture";
                                    if (model.HCPType == "Hon" && (string.IsNullOrEmpty(model.SelectedTypeofHCP[0]) || string.IsNullOrEmpty(model.SelectedNameoftheHCP[0])))
                                    {
                                        if (string.IsNullOrEmpty(model.SelectedTypeofHCP[0])) { TempData["eHCPType"] = "*Please Select HCP Type"; }
                                        if (string.IsNullOrEmpty(model.SelectedNameoftheHCP[0])) { TempData["eHCPName"] = "*Please Select HCP Name"; }
                                        if (model.HCPCost == 0) { TempData["eHCPNCost"] = "*Please Select Honorarium payable to HCP"; }
                                        if (model.HCPtopic == null) { TempData["eHCPNTopic"] = "*Please Enter HCP topic"; }
                                    }
                                    else if (model.HCPType == "NoHon" && (string.IsNullOrEmpty(model.SelectedTypeofHCP[1]) || string.IsNullOrEmpty(model.HCPFirstName) || string.IsNullOrEmpty(model.HCPLastName) || string.IsNullOrEmpty(model.TMFNo) || model.HCPCost == 0 || model.NOHImageFile == null))
                                    {
                                        if (string.IsNullOrEmpty(model.SelectedTypeofHCP[1])) { TempData["eHCPNType"] = "*Please Select HCP Type"; }
                                        if (string.IsNullOrEmpty(model.HCPFirstName)) { TempData["eHCPNFName"] = "*Please Select HCP First Name"; }
                                        if (string.IsNullOrEmpty(model.HCPLastName)) { TempData["eHCPNLName"] = "*Please Select HCP Last Name"; }
                                        if (model.NOHImageFile == null || model.NOHImageFile[0] == null) { TempData["eHCPNfile"] = "*Please attach file"; }
                                        if (model.HCPtopic == null) { TempData["eHCPNNTopic"] = "*Please Enter HCP topic"; }
                                    }

                                    TempData["TabDisplay"] = "Valid1";
                                }
                            }
                            else
                            {
                                if ((model.SelectedBrand[0] != "" && model.SelectedEvent[0] != "") || (model.ActivityName != null))
                                {
                                    model.HCPs = TempData["PAFHCPViewModel"] as List<PAFHCPViewModel>;
                                    if (model.HCPs == null)
                                    { model.HCPs = new List<PAFHCPViewModel>(); }
                                    if (!string.IsNullOrEmpty(model.SelectedTypeofHCP[0]) && !string.IsNullOrEmpty(model.SelectedNameoftheHCP[0]) && (model.HCPCost != 0))
                                    {
                                        var hcp = _hCPListRepository.GetHCPById(Convert.ToInt32(model.SelectedNameoftheHCP[0]));
                                        if (hcp != null)
                                        {
                                            if (model.HCPs.SingleOrDefault(s => s.TMF_No == hcp.TMFNo && s.First_Name == hcp.FirstName) == null)
                                            {
                                                //if (hcp.NoLecture > hcp.NoLectureGiven)
                                                //{
                                                if (model.HCPtopic == null || model.HCPtopic == "")
                                                {
                                                    model.HCPtopic = "No Topic";
                                                }
                                                model.HCPs.Add(new PAFHCPViewModel
                                                {
                                                    Type = Convert.ToByte(model.SelectedTypeofHCP[0]),
                                                    HCPId = hcp.ID,
                                                    First_Name = hcp.FirstName,
                                                    Last_Name = hcp.LastName,
                                                    TMF_No = hcp.TMFNo,
                                                    HCPAddress = hcp.HCPAddress,
                                                    Cost = Convert.ToDecimal(model.HCPCost),
                                                    HCPtopic = model.HCPtopic
                                                });
                                                //}
                                                //else { TempData["eNolect"] = "* This doctor exceeds the limit of HCP"; }
                                            }
                                        }
                                    }
                                    else if (!string.IsNullOrEmpty(model.SelectedTypeofHCP[1]) && !string.IsNullOrEmpty(model.HCPFirstName) && !string.IsNullOrEmpty(model.HCPLastName) && (model.NOHImageFile[0] != null && model.NOHImageFile[0].ContentLength > 0))
                                    {
                                        if (model.HCPs.SingleOrDefault(s => s.TypeName == model.HCPType && s.First_Name == model.HCPFirstName && s.Last_Name == model.HCPLastName && s.TMF_No == model.TMFNo) == null)
                                        {
                                            bool has = model.HCPs.Any(cus => cus.First_Name == model.HCPFirstName || cus.Last_Name == model.HCPLastName || (cus.TMF_No == model.TMFNo && model.TMFNo != null));
                                            if (!has)
                                            {
                                                foreach (var obj in model.NOHImageFile)
                                                {
                                                    if (obj.ContentLength > 0)
                                                    {
                                                        var lmModel = SaveFile(obj, "NOH");
                                                        lmModel.HCP_First_Name = model.HCPFirstName;
                                                        lmModel.HCP_Last_Name = model.HCPLastName;
                                                        sLMModel.Add(lmModel);
                                                        TempData["fileNAme"] += obj.FileName + "+";
                                                    }
                                                }
                                                if (model.HCPtopicNoH == null || model.HCPtopicNoH == "")
                                                {
                                                    model.HCPtopicNoH = "No Topic";
                                                }
                                                model.HCPs.Add(new PAFHCPViewModel
                                                {
                                                    Type = Convert.ToByte(model.SelectedTypeofHCP[1]),
                                                    First_Name = model.HCPFirstName,
                                                    Last_Name = model.HCPLastName,
                                                    TMF_No = model.TMFNo,
                                                    AttachedFile = TempData["fileNAme"].ToString(),
                                                    HCPtopic = model.HCPtopicNoH
                                                });
                                            }
                                            else { TempData["SameUser"] = "*HCP has been already selected, Choose another"; }
                                        }
                                    }
                                    else
                                    {
                                        if (model.HCPType == "Hon" && (string.IsNullOrEmpty(model.SelectedTypeofHCP[0]) || string.IsNullOrEmpty(model.SelectedNameoftheHCP[0]) || model.HCPCost == 0))
                                        {
                                            if (string.IsNullOrEmpty(model.SelectedTypeofHCP[0])) { TempData["eHCPType"] = "*Please Select HCP Type"; }
                                            if (string.IsNullOrEmpty(model.SelectedNameoftheHCP[0])) { TempData["eHCPName"] = "*Please Select HCP Name"; }
                                            if (model.HCPCost == 0) { TempData["eHCPNCost"] = "*Please Select Honorarium payable to HCP"; }

                                        }

                                        else if (model.HCPType == "NoHon" && (string.IsNullOrEmpty(model.SelectedTypeofHCP[1]) || string.IsNullOrEmpty(model.HCPFirstName) || string.IsNullOrEmpty(model.HCPLastName) || string.IsNullOrEmpty(model.TMFNo) || model.NOHImageFile == null || model.NOHImageFile[0] == null))
                                        {
                                            if (string.IsNullOrEmpty(model.SelectedTypeofHCP[1])) { TempData["eHCPNType"] = "*Please Select HCP Type"; }
                                            if (string.IsNullOrEmpty(model.HCPFirstName)) { TempData["eHCPNFName"] = "*Please Select HCP First Name"; }
                                            if (string.IsNullOrEmpty(model.HCPLastName)) { TempData["eHCPNLName"] = "*Please Select HCP Last Name"; }
                                            if (model.NOHImageFile == null || model.NOHImageFile[0] == null) { TempData["eHCPNfile"] = "*Please attach file"; }
                                        }
                                    }
                                    TempData.Remove("PAFHCPViewModel");
                                    TempData.Add("PAFHCPViewModel", model.HCPs);
                                    model.Miscellaneous = TempData["PAFMiscellaneous"] as List<PAFMiscellaneous>;
                                    TempData["TabDisplay"] = "Valid1";
                                }
                                else
                                {
                                    TempData["sBrandArea"] = "* Please select Brand";
                                    TempData["sEventArea"] = "* Please select Event";
                                    if (model.HCPType == "Hon" && (string.IsNullOrEmpty(model.SelectedTypeofHCP[0]) || string.IsNullOrEmpty(model.SelectedNameoftheHCP[0])))
                                    {
                                        if (string.IsNullOrEmpty(model.SelectedTypeofHCP[0])) { TempData["eHCPType"] = "*Please Select HCP Type"; }
                                        if (string.IsNullOrEmpty(model.SelectedNameoftheHCP[0])) { TempData["eHCPName"] = "*Please Select HCP Name"; }
                                        if (model.HCPCost == 0) { TempData["eHCPNCost"] = "*Please Select Honorarium payable to HCP"; }
                                    }
                                    else if (model.HCPType == "NoHon" && (string.IsNullOrEmpty(model.SelectedTypeofHCP[1]) || string.IsNullOrEmpty(model.HCPFirstName) || string.IsNullOrEmpty(model.HCPLastName) || string.IsNullOrEmpty(model.TMFNo) || model.HCPCost == 0 || model.NOHImageFile == null))
                                    {
                                        if (string.IsNullOrEmpty(model.SelectedTypeofHCP[1])) { TempData["eHCPNType"] = "*Please Select HCP Type"; }
                                        if (string.IsNullOrEmpty(model.HCPFirstName)) { TempData["eHCPNFName"] = "*Please Select HCP First Name"; }
                                        if (string.IsNullOrEmpty(model.HCPLastName)) { TempData["eHCPNLName"] = "*Please Select HCP Last Name"; }
                                        if (model.NOHImageFile == null || model.NOHImageFile[0] == null) { TempData["eHCPNfile"] = "*Please attach file"; }
                                    }
                                }
                            }
                        }
                    }

                    if (command.Substring(0, 5) == "RMHCP")
                    {
                        model.HCPs = TempData["PAFHCPViewModel"] as List<PAFHCPViewModel>;
                        if (command.Substring(5, 2) == "--")
                        {
                            string mis = command.Substring(7, command.Length - 7);
                            String[] spearator = mis.Split('+');
                            string Name = spearator[0];
                            model.HCPs.RemoveAll(s => s.First_Name == Name);
                            for (int i = 0; i < spearator.Length - 1; i++)
                            {
                                if (spearator[i + 1] != "")
                                {
                                    string filename = spearator[i + 1].Replace(" ", String.Empty);
                                    var lmModel = sLMModel.Where(s => s.DocName == filename && s.HCP_First_Name == Name).FirstOrDefault();
                                    if (lmModel != null)
                                    {
                                        sLMModel.Remove(sLMModel.FirstOrDefault(s => s.SavedDocName == lmModel.SavedDocName));
                                        var fullPath = Path.Combine(Server.MapPath("~/Documents/PAF/"), lmModel.SavedDocName);
                                        if (System.IO.File.Exists(fullPath))
                                        {
                                            sModel.IsDeleteFile = true;
                                            System.IO.File.Delete(fullPath);
                                        }
                                        else { sModel.IsDeleteFile = false; }
                                    }
                                }
                            }
                        }
                        else
                        {
                            //string pan = command.Substring(6, 10);
                            //model.HCPs.RemoveAll(s => s.PAN == pan);

                            string mis = command.Substring(17);
                            String[] spearator = mis.Split('+');
                            string Name = spearator[0];
                            model.HCPs.RemoveAll(s => s.First_Name == Name);
                            for (int i = 0; i < spearator.Length - 1; i++)
                            {
                                if (spearator[i + 1] != "")
                                {
                                    string filename = spearator[i + 1].Replace(" ", String.Empty);
                                    var lmModel = sLMModel.Where(s => s.DocName == filename && s.HCP_First_Name == Name).FirstOrDefault();
                                    if (lmModel != null)
                                    {
                                        sLMModel.Remove(sLMModel.FirstOrDefault(s => s.SavedDocName == lmModel.SavedDocName));
                                        var fullPath = Path.Combine(Server.MapPath("~/Documents/PAF/"), lmModel.SavedDocName);
                                        if (System.IO.File.Exists(fullPath))
                                        {
                                            sModel.IsDeleteFile = true;
                                            System.IO.File.Delete(fullPath);
                                        }
                                        else { sModel.IsDeleteFile = false; }
                                    }
                                }
                            }
                        }

                        TempData.Remove("PAFHCPViewModel");
                        TempData.Add("PAFHCPViewModel", model.HCPs);
                        model.ImagePath = sLMModel;
                        model.Miscellaneous = TempData["PAFMiscellaneous"] as List<PAFMiscellaneous>;
                        TempData["TabDisplay"] = "Valid1";
                    }
                    if (command == "Search")
                    {

                        if ((model.SelectedTherapyArea[0] != "" && model.SelectedBrand[0] != "" && model.SelectedEvent[0] != "") || (model.SelectedTherapyArea[0] != "" && model.ActivityName != null))
                        {
                            if (model.PSearchName == null && model.PSearchCity == null && model.PSearchCustomerID == null)
                            {
                                TempData["Pname"] = "* Please Enter Name";
                                TempData["pcity"] = "* Please Enter City";
                                TempData["pcustomerid"] = "* Please Enter Customer id";
                            }
                            else
                            {
                                string searchString = model.PSearchName;
                                string searchStringCN = null;
                                string sortOrder = "Id";

                                model.SearchParticipants = _hCPListRepository.GetAll(searchString, searchStringCN, sortOrder, model.PSearchCustomerID);
                                TempData["PAFAddP"] = "Addp";
                            }
                        }
                        else
                        {
                            if (model.SelectedTherapyArea[0] == "" && Session["LoginUserZone"].ToString() != "Rare Disease")
                            { TempData["sTherapyArea"] = "* Please select Therapy Area"; }
                            TempData["sBrandArea"] = "* Please select Brand";
                            TempData["sEventArea"] = "* Please select Event";
                            TempData["sLectureArea"] = "* Please select Lecture";
                        }
                        TempData["TabDisplay"] = "Valid1";
                    }
                    if (command == "Add Participants")
                    {

                        if ((model.SelectedBrand[0] != "" && model.SelectedEvent[0] != "") || (model.ActivityName != null))
                        {
                            var participantlist = JsonConvert.DeserializeObject<List<PAFParticipants>>(model.JsonParticipantsdata);
                            if (participantlist == null)
                            {

                                TempData["Pname"] = "* Please Enter Name";
                                TempData["pcity"] = "* Please Enter City";
                                TempData["pcustomerid"] = "* Please Enter Customer id";
                            }
                            else
                            {
                                if (model.Participants == null)
                                {
                                    model.Participants = participantlist;
                                }
                                else
                                {
                                    model.Participants.AddRange(participantlist);
                                }
                                TempData["PAFAddP"] = "Addp";
                                TempData.Remove("PAFParticipants");
                                TempData.Add("PAFParticipants", model.Participants);
                            }
                        }
                        else
                        {
                            if (model.SelectedTherapyArea[0] == "" && Session["LoginUserZone"].ToString() != "Rare Disease")
                            { TempData["sTherapyArea"] = "* Please select Therapy Area"; }
                            TempData["sBrandArea"] = "* Please select Brand";
                            TempData["sEventArea"] = "* Please select Event";
                            TempData["sLectureArea"] = "* Please select Lecture";
                        }

                        TempData["TabDisplay"] = "Valid1";
                    }
                    else if (command == "Add Adoch Participants")
                    {
                        if (model.Participants == null)
                        { model.Participants = new List<PAFParticipants>(); }
                        if (model.Participant_Name != null && model.Participant_Email_ID != null && model.Participant_Phone_Number != null)
                        {
                            model.Participants.Add(new PAFParticipants
                            {
                                Name = model.Participant_Name,
                                Email_Id = model.Participant_Email_ID,
                                Phone_Number = model.Participant_Phone_Number
                            });
                        }
                        else
                        {
                            if (model.Participant_Name == null) { TempData["PAname"] = "* Please Enter Participant Name"; }
                            if (model.Participant_Email_ID == null) { TempData["PAemail"] = "* Please Enter Email Id"; }
                            if (model.Participant_Name == null) { TempData["PAphone"] = "* Please Enter Phone Number"; }
                        }
                        TempData["PAFAddP"] = "AddAdochp";
                        TempData.Remove("PAFParticipants");
                        TempData.Add("PAFParticipants", model.Participants);
                        TempData["TabDisplay"] = "Valid1";
                    }
                    if (command.Substring(0, 3) == "RMP")
                    {
                        model.Participants = TempData["PAFParticipants"] as List<PAFParticipants>;
                        if (command.Substring(3, 2) == "--")
                        {
                            var startindex = 5;
                            var endindex = command.IndexOf("-", startindex);
                            var length = endindex - startindex;
                            string mis = command.Substring(startindex, length);
                            String[] spearator = mis.Split('+');
                            string Name = spearator[0];
                            string Emailid = spearator[1];
                            model.Participants.RemoveAll(s => s.Name == Name && s.Email_Id == Emailid);
                        }
                        else
                        {
                            var startindex = 4;
                            var endindex = command.IndexOf("-", startindex);
                            var length = endindex - startindex;
                            string HCPID = command.Substring(startindex, length);
                            model.Participants.RemoveAll(s => s.ID == Int32.Parse(HCPID));
                        }

                        TempData.Remove("PAFParticipants");
                        TempData.Add("PAFParticipants", model.Participants);
                        TempData["TabDisplay"] = "Valid1";
                    }
                    if (command == "Add Miscellaneous")
                    {
                        if (!therapyAreaConfig.Contains(Session["LoginUserZone"].ToString()))
                        {
                            if ((model.SelectedTherapyArea[0] != "" && model.SelectedBrand[0] != "" && model.SelectedEvent[0] != "") || (model.SelectedTherapyArea[0] != "" && model.ActivityName != null))
                            {
                                model.Miscellaneous = TempData["PAFMiscellaneous"] as List<PAFMiscellaneous>;
                                if (model.Miscellaneous == null)
                                { model.Miscellaneous = new List<PAFMiscellaneous>(); }
                                if (!string.IsNullOrEmpty(model.MName))
                                {
                                    if (model.Miscellaneous.SingleOrDefault(s => s.Name == model.MName) == null)
                                    {
                                        model.Miscellaneous.Add(new PAFMiscellaneous { Name = model.MName, Cost = model.MCost });
                                        model.MName = string.Empty;
                                        model.MCost = 0;
                                    }
                                }
                                TempData.Remove("PAFMiscellaneous");
                                TempData.Add("PAFMiscellaneous", model.Miscellaneous);
                                model.HCPs = TempData["PAFHCPViewModel"] as List<PAFHCPViewModel>;
                            }
                            else
                            {
                                if (model.SelectedTherapyArea[0] == "") { TempData["sTherapyArea"] = "* Please select Therapy Area"; }
                                if (model.SelectedBrand[0] == "") { TempData["sBrandArea"] = "* Please select Brand"; }
                                if (model.SelectedEvent[0] == "") { TempData["sEventArea"] = "* Please select Event"; }
                                if (model.SelectedLecture[0] == "") { TempData["sLecture"] = "* Please select Lecture"; }
                            }
                        }
                        else
                        {
                            if ((model.SelectedBrand[0] != "" && model.SelectedEvent[0] != "") || (model.ActivityName != null))
                            {
                                model.Miscellaneous = TempData["PAFMiscellaneous"] as List<PAFMiscellaneous>;
                                if (model.Miscellaneous == null)
                                { model.Miscellaneous = new List<PAFMiscellaneous>(); }
                                if (!string.IsNullOrEmpty(model.MName))
                                {
                                    if (model.Miscellaneous.SingleOrDefault(s => s.Name == model.MName) == null)
                                    {
                                        model.Miscellaneous.Add(new PAFMiscellaneous { Name = model.MName, Cost = model.MCost });
                                        model.MName = string.Empty;
                                        model.MCost = 0;
                                    }
                                }
                                TempData.Remove("PAFMiscellaneous");
                                TempData.Add("PAFMiscellaneous", model.Miscellaneous);
                                model.HCPs = TempData["PAFHCPViewModel"] as List<PAFHCPViewModel>;
                            }
                            else
                            {
                                if (model.SelectedBrand[0] == "") { TempData["sBrandArea"] = "* Please select Brand"; }
                                if (model.SelectedEvent[0] == "") { TempData["sEventArea"] = "* Please select Event"; }
                                if (model.SelectedLecture[0] == "") { TempData["sLecture"] = "* Please select Lecture"; }
                            }
                        }

                        TempData["TabDisplay"] = "Valid2";
                    }
                    if (command.Substring(0, 5) == "RMMis")
                    {
                        model.Miscellaneous = TempData["PAFMiscellaneous"] as List<PAFMiscellaneous>;
                        string mis = command.Substring(6, command.Length - 6);
                        String[] spearator = mis.Split('+');
                        model.Miscellaneous.RemoveAll(s => s.Name == spearator[0] && s.Cost.Equals(decimal.Parse(spearator[1])));
                        TempData.Remove("PAFMiscellaneous");
                        TempData.Add("PAFMiscellaneous", model.Miscellaneous);
                        model.HCPs = TempData["PAFHCPViewModel"] as List<PAFHCPViewModel>;
                        TempData["TabDisplay"] = "Valid2";
                    }
                    TempData.Keep("PAFMiscellaneous");
                    TempData.Keep("PAFHCPViewModel");
                    TempData.Keep("PAFParticipants");

                    return View(model);
                }
            }

            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public void Insert(CMEModel model)
        {
            //string action = "Paf Create";
            try
            {
                using (NovoNordiskEntities context = new NovoNordiskEntities())
                {
                    User user = context.Users.Where(s => s.ID == model.IssuedBy).FirstOrDefault();
                    string managerEmail = string.Empty;
                    if (model.PAFType == "Send")
                    {
                        var managerUser = (from u in context.Users
                                           join ptu in context.Profiles_To_User on u.ID equals ptu.User_Id
                                           join p in context.Profiles on ptu.Profile_Id equals p.ID
                                           where u.Initials.ToLower() == model.ManagerInitial.ToLower() && ptu.Status
                                           orderby ptu.ID descending
                                           select new UserModel
                                           {
                                               ID = u.ID,
                                               Initial = u.Initials,
                                               Role = p.Name,
                                               EmailId = u.Email
                                           }).FirstOrDefault();
                        if (managerUser != null)
                        {
                            if (managerUser.Role.ToUpper() == "RBD")
                            {
                                model.PAFType = "Send to RBD";
                            }
                            else
                            {
                                model.PAFType = "Send";
                            }
                            managerEmail = managerUser.EmailId;
                        }

                        if (model.IssuerZone == "Rare Disease" && (model.IssuerRole == "SM" || model.IssuerRole == "CT"))
                        {
                            model.PAFType = "Send to RBD";
                        }
                    }

                    PAF_Request_Type pafRequest = context.PAF_Request_Type.Where(s => s.Name == model.PAFType).FirstOrDefault();

                    var newPaf = new PAF()
                    {
                        Issued_By_Id = model.IssuedBy,
                        Request_Type_Id = pafRequest.ID,
                        CME_Type = model.CME_Type,
                        Disease_Area = model.DiseaseArea_Type,
                        Issued_Date = model.IssuedDate,
                        Therapy_Area_Id = model.TherapyAreaId,
                        Brand_Id = model.BrandId,
                        Event_Id = model.EventId,
                        Lecture_Id = model.LectureId,
                        Activity_Name = model.ActivityName,
                        Activity_Start_Date = model.ActivityStartDate,
                        Activity_End_Date = model.ActivityEndDate,
                        No_Of_Days = model.NumberofDays,
                        Actual_Start_Date = model.ActivityEndDate,
                        Actual_End_Date = model.ActivityEndDate,
                        Change_Start_Date = model.ActivityStartDate,
                        Change_End_Date = model.ActivityEndDate,
                        Proposed_Attendee_Count = model.Proposednumberofattendee,
                        Min_Guarantee_Count = model.Minimumnumberofgaurantee,
                        Approximate_Count = model.Approximatecost,
                        Room_Rate = model.Roomrate,
                        Banquet_Room_Rate = model.Banquetroomrate,
                        Cost_Per_Pax = model.CostPerPAX,
                        City = model.City,
                        Hotel = model.Venu_Hotel,
                        Hotel_Star = model.Hotel_Star ? 5 : 4,
                        Hotel_Document_Name = null,
                        Hotel_Document_Path = null,
                        // IsDateDeviation = (model.ChangeStartDate.HasValue && model.ChangeEndDate.HasValue),
                        PAF_Status_Id = context.PAF_Status.Where(s => s.Name == "Active").FirstOrDefault().ID,
                        CreatedDate = DateTime.UtcNow,
                        ModifiedDate = DateTime.UtcNow,
                        ModifiedBy = model.IssuedBy,
                        Approver = model.Approver,
                        Hospital_Meeting = model.HospitalMeeting,
                        IsVirtual = model.IsVirtual,
                        //IsDelete = model.IsDelete,
                        Cost_Centre = model.Cost_Center,
                        Is_BTC = model.IsBTC,
                        //Is_Assessment=model.Is_Assessment,
                        Meetingtype_Id = model.MeetingTypeId,
                        IsDateDeviation = model.IsDateDeviation,
                        Policy_Violation = model.Policy_Violation
                    };

                    _pAFRepository.InsertPAF(newPaf);

                    decimal totalPAFCost = 0;
                    var latestPAF = context.PAFs.Where(p => p.Issued_By_Id == model.IssuedBy).OrderByDescending(o => o.ID).FirstOrDefault();
                    if (user.Therapy_Area.Name == "Rare Disease")
                    {
                        totalPAFCost = totalPAFCost + latestPAF.Room_Rate + latestPAF.Banquet_Room_Rate + (latestPAF.Cost_Per_Pax * model.Proposednumberofattendee);
                    }
                    else
                    {
                        //totalPAFCost = totalPAFCost + latestPAF.Approximate_Count + latestPAF.Room_Rate + latestPAF.Banquet_Room_Rate + latestPAF.Cost_Per_Pax;
                        totalPAFCost = totalPAFCost + latestPAF.Room_Rate + latestPAF.Banquet_Room_Rate + (latestPAF.Cost_Per_Pax * model.Proposednumberofattendee);
                    }

                    newPaf = new PAF
                    {
                        ID = latestPAF.ID,
                        Issued_By_Id = model.IssuedBy,
                        Request_Type_Id = pafRequest.ID,
                        CME_Type = model.CME_Type,
                        Disease_Area = model.DiseaseArea_Type,
                        Issued_Date = model.IssuedDate,
                        Therapy_Area_Id = model.TherapyAreaId,
                        Brand_Id = model.BrandId,
                        Event_Id = model.EventId,
                        Lecture_Id = model.LectureId,
                        Activity_Name = model.ActivityName,
                        Activity_Start_Date = model.ActivityStartDate,
                        Activity_End_Date = model.ActivityEndDate,
                        No_Of_Days = model.NumberofDays,
                        Actual_Start_Date = model.ActivityEndDate,
                        Actual_End_Date = model.ActivityEndDate,
                        Change_Start_Date = model.ActivityStartDate,
                        Change_End_Date = model.ActivityEndDate,
                        Cost_Per_Pax = model.CostPerPAX,
                        City = model.City,
                        Hotel = model.Venu_Hotel,
                        Hotel_Star = model.Hotel_Star ? 5 : 4,
                        PAF_Status_Id = context.PAF_Status.Where(s => s.Name == "Active").FirstOrDefault().ID,
                        CreatedDate = DateTime.UtcNow,
                        ModifiedDate = DateTime.UtcNow,
                        ModifiedBy = model.IssuedBy,
                        Approver = model.Approver,
                        Hospital_Meeting = model.HospitalMeeting,
                        IsVirtual = model.IsVirtual,
                        Cost_Centre = model.Cost_Center,
                        Is_BTC = model.IsBTC,
                        IsDateDeviation = model.IsDateDeviation,
                        Policy_Violation = model.Policy_Violation
                    };

                    var serializedLatestPaf = JsonConvert.SerializeObject(newPaf, Formatting.Indented, new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    });
                    //pafDetail.Append(Environment.NewLine + Environment.NewLine + serializedLatestPaf);
                    //pafDetail.Append(Environment.NewLine + Environment.NewLine + "Inserting PAF Detail started: " + newPaf.CreatedDate + Environment.NewLine);

                    #region Inserting Miscellaneous                    

                    if (model.Miscellaneous != null)
                    {
                        var newPafMisc = new PAF_Miscellaneous();
                        //pafDetail.Append(Environment.NewLine + Environment.NewLine + "Inserting Paf Miscellaneous started: " + Environment.NewLine);

                        foreach (var obj in model.Miscellaneous)
                        {
                            totalPAFCost += obj.Cost;
                            string MiscellaneousName = (context.Miscellaneous_Type.Where(s => s.Name == obj.Name).OrderByDescending(o => o.Name).FirstOrDefault() != null) ? context.Miscellaneous_Type.Where(s => s.Name == obj.Name).OrderByDescending(o => o.Name).FirstOrDefault().Name : "";
                            if (MiscellaneousName != obj.Name)
                            {
                                var newMiscellaneous = new Miscellaneous_Type()
                                {
                                    Name = obj.Name,
                                };
                                _commonRepository.InsertMiscellaneousType(newMiscellaneous);
                            }

                            var newPAFMiscellaneous = new PAF_Miscellaneous
                            {
                                PAF_Id = latestPAF.ID,
                                Type_Id = context.Miscellaneous_Type.Where(s => s.Name == obj.Name).OrderByDescending(o => o.ID).FirstOrDefault().ID,
                                Price = obj.Cost,
                                CreatedBy = model.IssuedBy,
                                CreatedDate = DateTime.UtcNow
                            };

                            _commonRepository.InsertPAFMiscellaneous(newPAFMiscellaneous);

                            newPafMisc.PAF_Id = latestPAF.ID;
                            newPafMisc.Type_Id = context.Miscellaneous_Type.Where(s => s.Name == obj.Name).OrderByDescending(o => o.ID).FirstOrDefault().ID;
                            newPafMisc.Price = obj.Cost;
                            newPafMisc.CreatedBy = model.IssuedBy;
                            newPafMisc.CreatedDate = DateTime.UtcNow;
                        };

                        var serializedPafCost = JsonConvert.SerializeObject(newPafMisc, Formatting.Indented, new JsonSerializerSettings
                        {
                            NullValueHandling = NullValueHandling.Ignore
                        });
                        //pafDetail.Append(serializedPafCost);
                        //pafDetail.Append(Environment.NewLine + Environment.NewLine + "Inserting Paf Miscellaneous is complete " + newPafMisc.CreatedDate + Environment.NewLine);
                    }


                    #endregion

                    #region Inserting HCP List                    

                    if (model.HCPs != null)
                    {
                        var newPafHCP = new PAF_HCP();
                        //pafDetail.Append(Environment.NewLine + Environment.NewLine + "Inserting HCP List started: " + Environment.NewLine);

                        foreach (var obj in model.HCPs)
                        {
                            var newPafHcp = new PAF_HCP
                            {
                                PAF_Id = latestPAF.ID,
                                Type_Of_HCP_Id = obj.Type,
                                HCP_Id = obj.HCPId,
                                HCP_First_Name = obj.First_Name,
                                HCP_Last_Name = obj.Last_Name,
                                HCP_Payable = obj.Cost,
                                Invitation_Email = obj.Invitation,
                                Is_Honorarium = obj.HCPId != null,
                                CreatedDate = DateTime.UtcNow,
                                ModifiedDate = DateTime.UtcNow,
                                HCP_Topic = obj.HCPtopic
                            };
                            totalPAFCost += obj.Cost;

                            _pAFRepository.InsertPAFHCP(newPafHcp);

                            newPafHCP.PAF_Id = latestPAF.ID;
                            newPafHCP.Type_Of_HCP_Id = obj.Type;
                            newPafHCP.HCP_Id = obj.HCPId;
                            newPafHCP.HCP_First_Name = obj.First_Name;
                            newPafHCP.HCP_Last_Name = obj.Last_Name;
                            newPafHCP.Is_Honorarium = obj.HCPId != null;

                        }
                        var serializedPAFHCPViewModel = JsonConvert.SerializeObject(newPafHCP, Formatting.Indented, new JsonSerializerSettings
                        {
                            NullValueHandling = NullValueHandling.Ignore
                        });
                        //pafDetail.Append(serializedPAFHCPViewModel);
                        //pafDetail.Append(Environment.NewLine + Environment.NewLine + "Inserting HCP List is complete " + newPAFHCPViewModel.CreatedDate + Environment.NewLine);
                    }

                    #endregion

                    #region Inserting Participants List

                    if (model.Participants != null)
                    {
                        var newPafParticipant = new PAF_Participants();

                        //pafDetail.Append(Environment.NewLine + Environment.NewLine + "Inserting Participants List started: " + Environment.NewLine);
                        foreach (PAFParticipants obj in model.Participants)
                        {
                            var newPafParticipants = new PAF_Participants
                            {
                                PAF_Id = latestPAF.ID,
                                HCP_Id = obj.ID,
                                Participants_Name = obj.Name,
                                PAN_No = obj.PAN,
                                Participants_Email_ID = obj.Email_Id,
                                Participants_Phone_Number = obj.Phone_Number,
                                Participants_Customer_ID = obj.Customer_ID
                            };


                            _pAFRepository.InsertPAFParticipants(newPafParticipants);

                            newPafParticipant.PAF_Id = latestPAF.ID;
                            newPafParticipant.Participants_Name = obj.Name;
                            newPafParticipant.Participants_Customer_ID = obj.Customer_ID;
                        }

                        var serializedPafPart = JsonConvert.SerializeObject(newPafParticipant, Formatting.Indented, new JsonSerializerSettings
                        {
                            NullValueHandling = NullValueHandling.Ignore
                        });
                        //pafDetail.Append(serializedPafPart);
                        //pafDetail.Append(Environment.NewLine + Environment.NewLine + "Inserting Participants List is complete " + DateTime.UtcNow + Environment.NewLine);
                    }

                    #endregion
                    latestPAF.TotalCost = Convert.ToInt32(totalPAFCost);
                    context.SaveChanges();


                    #region Save Comments
                    //pafDetail.Append(Environment.NewLine + Environment.NewLine + "Saving Paf Comments started." + Environment.NewLine);

                    context.PAF_Command.Add(new PAF_Command { PAF_Id = latestPAF.ID, CreatedBy = model.IssuedBy, CreatedDate = DateTime.UtcNow, Comment = model.Comment });
                    context.SaveChanges();

                    var newComments = new PAF_Command { PAF_Id = latestPAF.ID, CreatedBy = model.IssuedBy, CreatedDate = DateTime.UtcNow, Comment = model.Comment };

                    var deserializedComments = JsonConvert.SerializeObject(newComments, Formatting.Indented, new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    });
                    //pafDetail.Append(deserializedComments);
                    //pafDetail.Append(Environment.NewLine + Environment.NewLine + "Saving Paf Comments is complete " + newComments.CreatedDate + Environment.NewLine);

                    #endregion

                    #region PAF transaction
                    context.PAF_Transaction.Add(new PAF_Transaction { PAF_Id = latestPAF.ID, Request_Type_Id = pafRequest.ID, CreatedBy = model.IssuedBy, CreatedDate = DateTime.UtcNow });
                    context.SaveChanges();
                    //Helper.Log_ log = new Helper.Log_();
                    //try
                    //{
                    //    context.PAF_Transaction.Add(new PAF_Transaction { PAF_Id = latestPAF.ID, Request_Type_Id = pafRequest.ID, CreatedBy = model.IssuedBy, CreatedDate = DateTime.UtcNow });
                    //    context.SaveChanges();
                    //    StringBuilder s = new StringBuilder();
                    //    s.Append("PAF_Id:");
                    //    s.Append(latestPAF.ID);
                    //    s.Append(" Request_Type_Id:");
                    //    s.Append(pafRequest.ID);
                    //    s.Append(" CreatedBy:");
                    //    string Initials = context.Users.Where(u => u.ID == model.IssuedBy).FirstOrDefault().Initials;
                    //    s.Append(Initials);
                    //    s.Append(" CreatedDate:");
                    //    s.Append(DateTime.UtcNow);
                    //    //log.WriteIntoFile(s.ToString(), latestPAF.ID.ToString(), "PAF Insert Transaction");
                    //}
                    //catch (Exception ex)
                    //{
                    //    //log.WriteIntoFile(ex.ToString(), latestPAF.ID.ToString(), "PAF Insert Transaction error");
                    //}
                    #endregion

                    #region HCP count
                    //var hcps = new List<HCPViewModel>();
                    //hcps = (from hcp in context.HCPs
                    //        join phcp in context.PAF_HCP on hcp.ID equals phcp.HCP_Id
                    //        join p in context.PAFs on phcp.PAF_Id equals p.ID
                    //        where p.ID == latestPAF.ID
                    //        select new HCPViewModel
                    //        {
                    //            ID = hcp.ID,
                    //            //PAN = hcp.PAN_No,
                    //            //NoLecture = hcp.No_Of_Lect,
                    //            //NoLectureGiven = hcp.No_Of_Lect_Given
                    //        }).ToList();
                    //if (hcps != null)
                    //{
                    //    foreach (var hcp in hcps)
                    //    {
                    //        DataAccessLayer.HCP latestHCP = context.HCPs.Where(p => p.ID == hcp.ID).OrderByDescending(o => o.ID).FirstOrDefault();
                    //        if (latestHCP.No_Of_Lect_Given < latestHCP.No_Of_Lect)
                    //        {
                    //            latestHCP.No_Of_Lect_Given += 1;
                    //            context.SaveChanges();
                    //        }
                    //    }
                    //}
                    #endregion

                    #region Save PAF Attachment                 
                    if (model.ImagePath != null)
                    {
                        //pafDetail.Append(Environment.NewLine + Environment.NewLine + "Saving PAF Attachment started: " + Environment.NewLine);
                        foreach (var obj in model.ImagePath)
                        {
                            context.PAF_Attachment.Add(new PAF_Attachment
                            {
                                PAF_Id = latestPAF.ID,
                                File_Name = obj.DocName.Replace(" ", String.Empty),
                                File_Path = obj.DocPath.Replace(" ", String.Empty),
                                File_Type = obj.Extension,
                                Saved_File_Name = obj.SavedDocName,
                                Type = obj.Type,
                                CreatedDate = DateTime.UtcNow,
                                ModifiedDate = DateTime.UtcNow,
                                HCP_Name = obj.HCP_First_Name + obj.HCP_Last_Name
                            });
                            context.SaveChanges();

                            var newAttachment = new PAF_Attachment
                            {
                                PAF_Id = latestPAF.ID,
                                File_Name = obj.DocName.Replace(" ", String.Empty),
                                File_Path = obj.DocPath.Replace(" ", String.Empty),
                                File_Type = obj.Extension,
                                Saved_File_Name = obj.SavedDocName,
                                Type = obj.Type,
                                CreatedDate = DateTime.UtcNow,
                                ModifiedDate = DateTime.UtcNow,
                                HCP_Name = obj.HCP_First_Name + obj.HCP_Last_Name
                            };

                            var serializedImagePath = JsonConvert.SerializeObject(newAttachment, Formatting.Indented, new JsonSerializerSettings
                            {
                                NullValueHandling = NullValueHandling.Ignore
                            });
                            //pafDetail.Append(serializedImagePath);
                            //pafDetail.Append(Environment.NewLine + Environment.NewLine + "Saving PAF Attachment is complete " + newAttachment.CreatedDate + Environment.NewLine);
                        }
                    }
                    #endregion

                    #region Sending Email
                    if (model.PAFType.ToLower() == "send" || model.PAFType == "Send to RBD")
                    {
                        //if (model.ChangeStartDate != null && model.ChangeEndDate != null)
                        //{
                        //    PAFDDApproveEMail(latestPAF.ID, user.Initials, managerEmail);
                        //}
                        //else
                        //{
                        if (model.PAFType.ToLower() == "send" && model.IssuerRole == "SM" && model.CME_Type == "Standard Meeting" && user.Therapy_Area.Name == "Rare Disease")//&& model.IsDateDeviation == false
                        {
                            var RBD1user = (from u in context.Users
                                            join ptu in context.Profiles_To_User on u.ID equals ptu.User_Id
                                            join p in context.Profiles on ptu.Profile_Id equals p.ID
                                            where p.Name == "RBD1" || p.Name == "RBD" && u.Status == true
                                            orderby ptu.ID descending
                                            select new UserModel
                                            {
                                                ID = u.ID,
                                                Initial = u.Initials,
                                                Role = p.Name,
                                                EmailId = u.Email
                                            }).ToList();
                            if (RBD1user != null)
                            {
                                foreach (var us in RBD1user)
                                {
                                    //PAFApproveEMail(latestPAF.ID, user.Initials, us.EmailId);
                                }
                            }

                        }
                        else
                        {
                            //PAFApproveEMail(latestPAF.ID, user.Initials, managerEmail);
                        }
                        // }
                        if (user.Therapy_Area.Name == "GLP 1" || user.Therapy_Area.Name == "HO" || user.Therapy_Area.Name == "Institution Business")
                        {
                            //Cost_Centre cost_Centre = null;
                            //string cost_Centre_code = latestPAF.Cost_Centre;
                            //cost_Centre = context.Cost_Centre.Where(s => s.Code == cost_Centre_code).FirstOrDefault();
                            //int cost_Centre_id = cost_Centre.ID;

                            //List<Models.User> users_id = new List<Models.User>();
                            //users_id = (from ctc in context.CostCentre_To_CT
                            //            where ctc.CostCentre_Id == cost_Centre_id
                            //            select new Models.User
                            //            {
                            //                ID = (int)ctc.User_Id
                            //            }).ToList<Models.User>();

                            //List<Models.User> users = new List<Models.User>();
                            //for (int i = 0; i < users_id.Count(); i++)
                            //{
                            //    var id = users_id[i].ID;
                            //    var us = new Models.User();
                            //    us = (from u in context.Users
                            //          join ptu in context.Profiles_To_User on u.ID equals ptu.User_Id
                            //          //join p in context.Profiles on ptu.Profile_Id equals p.ID
                            //          where u.ID.Equals(id) && ptu.Status.Equals(true)
                            //          select new Models.User
                            //          {
                            //              ID = u.ID,
                            //              Initial = u.Initials,
                            //              //Role = p.Name,
                            //              EmailId = u.Email
                            //          }).SingleOrDefault();
                            //    users.Add(us);
                            //}

                            //List<Models.User> users = new List<Models.User>();
                            //users = (from u in context.Users
                            //         join ptu in context.Profiles_To_User on u.ID equals ptu.User_Id
                            //         //join p in context.Profiles on ptu.Profile_Id equals p.ID
                            //         where u.ID in users_id && ptu.Status == true
                            //         select new Models.User
                            //         {
                            //             ID = u.ID,
                            //             Initial = u.Initials,
                            //             Role = p.Name,
                            //             EmailId = u.Email
                            //         }).ToList<Models.User>();

                            //if (users != null)
                            //{
                            //    foreach (var us in users)
                            //    {
                            //        PAFSendEMail(latestPAF.ID, us.EmailId, user.Initials);
                            //    }
                            //}
                        }
                        //else
                        //{
                        Cost_Centre cost_Centre = null;
                        string cost_Centre_code = latestPAF.Cost_Centre;
                        cost_Centre = context.Cost_Centre.Where(s => s.Code == cost_Centre_code).FirstOrDefault();

                        //extra check to avoid updating User table/CostCentre_To_CT table as per story "New Cost Centre for an Event"
                        if (cost_Centre != null)
                        {
                            int cost_Centre_id = cost_Centre.ID;

                            var users_id = new List<UserModel>();
                            users_id = (from ctc in context.CostCentre_To_CT
                                        where ctc.CostCentre_Id == cost_Centre_id
                                        select new UserModel
                                        {
                                            ID = (int)ctc.User_Id
                                        }).ToList();

                            var users1 = new List<UserModel>();
                            for (int i = 0; i < users_id.Count(); i++)
                            {
                                var id = users_id[i].ID;
                                var us = new UserModel();
                                us = (from u in context.Users
                                      join ptu in context.Profiles_To_User on u.ID equals ptu.User_Id
                                      //join p in context.Profiles on ptu.Profile_Id equals p.ID
                                      where u.ID.Equals(id) && ptu.Status.Equals(true)
                                      select new UserModel
                                      {
                                          ID = u.ID,
                                          Initial = u.Initials,
                                          //Role = p.Name,
                                          EmailId = u.Email
                                      }).SingleOrDefault();
                                users1.Add(us);
                            }
                            //List<Models.User> users = new List<Models.User>();
                            //users = (from u in context.Users
                            //         join ptu in context.Profiles_To_User on u.ID equals ptu.User_Id
                            //         join p in context.Profiles on ptu.Profile_Id equals p.ID
                            //         where ptu.Status == true && (p.Name == "CT" || p.Name == "CTB") && u.Zone_Id == latestPAF.User.Zone_Id
                            //         select new Models.User
                            //         {
                            //             ID = u.ID,
                            //             Initial = u.Initials,
                            //             Role = p.Name,
                            //             EmailId = u.Email
                            //         }).ToList<Models.User>();
                            if (users1 != null)
                            {
                                foreach (var us in users1)
                                {
                                    //PAFSendEMail(latestPAF.ID, us.EmailId, user.Initials);
                                }
                            }
                            //}
                        }

                        if (user.Therapy_Area.Name == "Rare Disease" && model.CME_Type == "Non-Standard Meeting")
                        {
                            if (model.DiseaseArea_Type == "Hemophilia Medical")
                            {
                                var users = new List<UserModel>();
                                users = (from u in context.Users
                                         join ptu in context.Profiles_To_User on u.ID equals ptu.User_Id
                                         join p in context.Profiles on ptu.Profile_Id equals p.ID
                                         where (p.Name == "PMH" || p.Name == "RMA") && ptu.Status == true
                                         select new UserModel
                                         {
                                             ID = u.ID,
                                             Initial = u.Initials,
                                             EmailId = u.Email
                                         }).ToList();
                                if (users != null)
                                {
                                    foreach (var us in users)
                                    {
                                        //PAFSendEMail(latestPAF.ID, us.EmailId, user.Initials);
                                    }
                                }
                            }
                            if (model.DiseaseArea_Type == "Growth Hormone Medical")
                            {
                                var users = new List<UserModel>();
                                users = (from u in context.Users
                                         join ptu in context.Profiles_To_User on u.ID equals ptu.User_Id
                                         join p in context.Profiles on ptu.Profile_Id equals p.ID
                                         where (p.Name == "PMGH" || p.Name == "RMA") && ptu.Status == true
                                         select new UserModel
                                         {
                                             ID = u.ID,
                                             Initial = u.Initials,
                                             EmailId = u.Email
                                         }).ToList();
                                if (users != null)
                                {
                                    foreach (var us in users)
                                    {
                                        //PAFSendEMail(latestPAF.ID, us.EmailId, user.Initials);
                                    }
                                }
                            }

                        }
                    }


                    if (latestPAF.PAF_Request_Type.Name == "Approved Date Deviation")
                    {
                        //if (latestPAF.IsDateDeviation && !latestPAF.IsDeviationApproved)
                        //{
                        //    List<Models.User> users = new List<Models.User>();
                        //    users = (from u in context.Users
                        //             join ptu in context.Profiles_To_User on u.ID equals ptu.User_Id
                        //             join p in context.Profiles on ptu.Profile_Id equals p.ID
                        //             where (p.Name == "Admin" && ptu.Status == true && (u.Email != "zspy@novonordisk.com")) ||
                        //             ((p.Name == "CT" || p.Name == "CTB") && u.Zone_Id == latestPAF.User.Zone_Id)
                        //             select new Models.User
                        //             {
                        //                 ID = u.ID,
                        //                 Initial = u.Initials,
                        //                 Role = p.Name,
                        //                 EmailId = u.Email
                        //             }).ToList<Models.User>();
                        //    if (users != null)
                        //    {
                        //        foreach (var us in users)
                        //        {
                        //            DeviationPAFApproveEMail(latestPAF.ID, user.Initials, us.EmailId);
                        //        }
                        //    }
                        //}
                        if (user.Therapy_Area.Name == "Rare Disease" && model.CME_Type == "Non-Standard Meeting")
                        {
                            if (model.DiseaseArea_Type == "Hemophilia Medical")
                            {
                                var users = new List<UserModel>();
                                users = (from u in context.Users
                                         join ptu in context.Profiles_To_User on u.ID equals ptu.User_Id
                                         join p in context.Profiles on ptu.Profile_Id equals p.ID
                                         where (p.Name == "PMH" || p.Name == "RMA") && ptu.Status == true
                                         select new UserModel
                                         {
                                             ID = u.ID,
                                             Initial = u.Initials,
                                             EmailId = u.Email
                                         }).ToList();
                                if (users != null)
                                {
                                    foreach (var us in users)
                                    {
                                        //PAFSendEMail(latestPAF.ID, us.EmailId, user.Initials);
                                    }
                                }
                            }
                            if (model.DiseaseArea_Type == "Growth Hormone Medical")
                            {
                                var users = new List<UserModel>();
                                users = (from u in context.Users
                                         join ptu in context.Profiles_To_User on u.ID equals ptu.User_Id
                                         join p in context.Profiles on ptu.Profile_Id equals p.ID
                                         where (p.Name == "PMGH" || p.Name == "RMA") && ptu.Status == true
                                         select new UserModel
                                         {
                                             ID = u.ID,
                                             Initial = u.Initials,
                                             EmailId = u.Email
                                         }).ToList();
                                if (users != null)
                                {
                                    foreach (var us in users)
                                    {
                                        //PAFSendEMail(latestPAF.ID, us.EmailId, user.Initials);
                                    }
                                }
                            }

                        }
                    }
                    #endregion

                    #region Sending HCP Remainder email

                    //var hcps1 = new List<HCPViewModel>();
                    //hcps1 = (from hcp in context.HCPs
                    //         join phcp in context.PAF_HCP on hcp.ID equals phcp.HCP_Id
                    //         join p in context.PAFs on phcp.PAF_Id equals p.ID
                    //         where p.ID == latestPAF.ID
                    //         select new HCPViewModel
                    //         {
                    //             ID = hcp.ID,
                    //             //PAN = hcp.PAN_No,
                    //             NoLecture = hcp.No_Of_Lect,
                    //             NoLectureGiven = hcp.No_Of_Lect_Given
                    //         }).ToList();
                    //if (hcps1 != null)
                    //{
                    //    foreach (var hcp in hcps1)
                    //    {
                    //        var hcplist = context.HCPs.Where(x => x.ID == hcp.ID).OrderByDescending(x => x.ID).FirstOrDefault();
                    //        var pending_lecture = hcplist.No_Of_Lect - hcplist.No_Of_Lect_Given;
                    //        if (pending_lecture < 4)
                    //        {
                    //            //var zone = hcplist.Zone_Id;
                    //            var admin = new List<UserModel>();
                    //            admin = (from u in context.Users
                    //                     join ptu in context.Profiles_To_User on u.ID equals ptu.User_Id
                    //                     join p in context.Profiles on ptu.Profile_Id equals p.ID
                    //                     where (u.Initials.ToLower() == "zspy" || u.Initials.ToLower() == "ahtc")
                    //                     select new UserModel
                    //                     {
                    //                         ID = u.ID,
                    //                         Initial = u.Initials,
                    //                         Role = p.Name,
                    //                         EmailId = u.Email
                    //                     }).ToList();
                    //            foreach (var ad in admin)
                    //            {
                    //                //HCPPendingLectureEmail(hcplist.ID, ad.EmailId, hcplist.Name, hcplist.PAN_No, hcplist.Address, pending_lecture);
                    //            }
                    //            var ct_all_zone = new List<UserModel>();
                    //            ct_all_zone = (from u in context.Users
                    //                           join ptu in context.Profiles_To_User on u.ID equals ptu.User_Id
                    //                           join p in context.Profiles on ptu.Profile_Id equals p.ID
                    //                           where (ptu.Profile_Id == 3)
                    //                           select new UserModel
                    //                           {
                    //                               ID = u.ID,
                    //                               Initial = u.Initials,
                    //                               Role = p.Name,
                    //                               EmailId = u.Email
                    //                           }).ToList();
                    //            //foreach (var ctaz in ct_all_zone)
                    //            //{
                    //            //    if (zone == 6 || zone == 8)
                    //            //    {
                    //            //        //HCPPendingLectureEmail(hcplist.ID, ctaz.EmailId, hcplist.Name, hcplist.PAN_No, hcplist.Address, pending_lecture);
                    //            //    }
                    //            //}
                    //            //if (zone == 1 || zone == 2 || zone == 4 || zone == 5)
                    //            //{
                    //            //    var ct = new List<UserModel>();
                    //            //    ct = (from u in context.Users
                    //            //          join ptu in context.Profiles_To_User on u.ID equals ptu.User_Id
                    //            //          join p in context.Profiles on ptu.Profile_Id equals p.ID
                    //            //          where u.Zone_Id == zone && (ptu.Profile_Id == 3)
                    //            //          select new UserModel
                    //            //          {
                    //            //              ID = u.ID,
                    //            //              Initial = u.Initials,
                    //            //              Role = p.Name,
                    //            //              EmailId = u.Email
                    //            //          }).ToList();
                    //            //    foreach (var cts in ct)
                    //            //    {
                    //            //        //HCPPendingLectureEmail(hcplist.ID, cts.EmailId, hcplist.Name, hcplist.PAN_No, hcplist.Address, pending_lecture);
                    //            //    }
                    //            //    var ctb = context.Users.Where(i => i.Initials.ToLower() == "xrby").OrderByDescending(z => z.ID).FirstOrDefault();
                    //            //    var ctb_email = ctb.Email;
                    //            //    //HCPPendingLectureEmail(hcplist.ID, ctb_email, hcplist.Name, hcplist.PAN_No, hcplist.Address, pending_lecture);
                    //            //}
                    //            //if (zone == 6)
                    //            {
                    //                var ctb = context.Users.Where(i => i.Initials.ToLower() == "xrby").OrderByDescending(z => z.ID).FirstOrDefault();
                    //                var ctb_email = ctb.Email;
                    //                //HCPPendingLectureEmail(hcplist.ID, ctb_email, hcplist.Name, hcplist.PAN_No, hcplist.Address, pending_lecture);
                    //            }
                    //        }
                    //    }
                    //}
                    #endregion

                    //pafDetail.Append(Environment.NewLine + "PAF: " + latestPAF.ID + " " + latestPAF?.PAF_Request_Type?.Name + " - By: " + model.IssuerRole + Environment.NewLine);
                    //pafDetail.Append(Environment.NewLine + Environment.NewLine + "New Paf Creation is complete: " + Environment.NewLine);

                    //pafLog.FetchLogs(pafDetail, action, latestPAF.ID);

                    context.Dispose();
                }
            }
            catch (Exception ex)
            {
                throw ex;
                //pafLog.WriteIntoFile(pafDetail.ToString(), action, ex.ToString());
            }
        }

        public ActionResult Status(int id = 0)
        {
            try
            {
                var loginUserId = Convert.ToInt32(Session["LoginUserId"]);

                var model = _cMERepository.GetById(id);
                model.Login_User_role_name = Session["LoginUserRole"].ToString();
                var lmFiles = _lecturesRepository.GetMaterialsById(Convert.ToByte(id)).ToList();
                var cme = _cMERepository.GetPAFById(id);

                var userDetail = _userRepository.GetUserByInitial(cme.IssuedByInitial);
                Session["PAFChar"] = userDetail.TherapyArea.ToString().Substring(0, 1);

                var loginUserRoleId = _cMERepository.GetRoleID(loginUserId);

                ViewBag.Website = ConfigurationManager.AppSettings["WebSitePath"].ToString() + ConfigurationManager.AppSettings["Location"].ToString();

                if (model.Form_ID == 0) { return RedirectToAction("Index", "Home"); }
                else
                {
                    if (cme.LectureId == null)
                    {
                        var lmFilesevent = _lecturesRepository.GetMaterialsByEventId(Convert.ToByte(id)).ToList();
                        model.LMFiles = lmFilesevent;
                    }
                    else
                    {
                        model.LMFiles = lmFiles;
                    }
                }

                if (model.IsVirtual == null)
                {
                    model.IsVirtual = false;
                }
                if (cme.Policy_Violation == null)
                {
                    model.Policy_Violation = false;
                }
                else
                {
                    model.Policy_Violation = (bool)cme.Policy_Violation;
                }
                model.IssuedBy = cme.IssuedBy;
                model.Login_User_id = loginUserId;
                model.IsDateDeviation = cme.IsDateDeviation;
                model.Login_User_role_id = loginUserRoleId.Login_User_role_id;
                Session.Remove("CMEFullDetail");
                Session.Remove("sLMModel");
                Session.Add("CMEFullDetail", model);
                Session.Add("sLMModel", model.ImagePath);

                //log.WriteIntoTempFile(pafDetail.ToString(), "Paf Status:" + id.ToString());

                return View(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult Status(int id, string command)
        {
            try
            {
                if (id != 0)
                {
                    if (command.ToLower() == "cancel")
                    {
                        Session.Remove("CMEFullDetail");
                        Session.Remove("sLMModel");
                        return RedirectToAction("Index", "Home");
                    }
                }
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult Details(int id = 0)
        {
            try
            {
                var cme = _cMERepository.GetPAFById(id);
                var lmFiles = _lecturesRepository.GetMaterialsById(Convert.ToByte(id)).ToList();
                var userDetail = _userRepository.GetUserByInitial(cme.IssuedByInitial);
                var model = _cMERepository.GetById(id);
                var pafList = _cMERepository.GetLatestviolation(id);

                var percent = pafList.Count(n => n.IsDateDeviation == false);
                model.Runrate = ((double)percent / 10) * 100;
                ViewBag.Website = ConfigurationManager.AppSettings["WebSitePath"].ToString() + ConfigurationManager.AppSettings["Location"].ToString();

                if (model.Form_ID == 0) { return RedirectToAction("Index", "Home"); }
                else
                {
                    if (cme.LectureId == null)
                    {
                        var lmFilesevent = _lecturesRepository.GetMaterialsByEventId(Convert.ToByte(id)).ToList();
                        model.LMFiles = lmFilesevent;
                    }
                    else
                    {
                        model.LMFiles = lmFiles;
                    }
                }

                if (model.IsVirtual == null)
                {
                    model.IsVirtual = false;
                }
                if (cme.Policy_Violation == null)
                {
                    model.Policy_Violation = false;
                }
                else
                {
                    model.Policy_Violation = (bool)cme.Policy_Violation;
                }
                model.IssuedBy = cme.IssuedBy;
                model.Login_User_id = Convert.ToInt32(Session["LoginUserId"]);
                model.HCPSJson = JsonConvert.SerializeObject(model.HCPs);
                Session.Remove("CMEFullDetail");
                Session.Remove("sLMModel");
                Session.Add("CMEFullDetail", model);
                Session.Add("sLMModel", model.ImagePath);

                //log.WriteIntoTempFile(pafDetail.ToString(), "Paf Details:" + id.ToString());

                return View(model);
            }
            catch (Exception ex)
            {
                //log.WriteIntoTempFile(ex.Message.ToString(), "Paf Details: " + ex.StackTrace.ToString());
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult Details(int id, CMEDetails model, string command)
        {
            try
            {
                var cme = _cMERepository.GetPAFById(id);
                string[] therapyAreaConfig = ConfigurationManager.AppSettings["TherapyArea"].Split(';');

                if (id != 0)
                {
                    if (command.ToLower() == "cancel")
                    {
                        Session.Remove("CMEFullDetail");
                        Session.Remove("sLMModel");
                        return RedirectToAction("Index", "Home");
                    }
                    var sModel = (CMEDetails)Session["CMEFullDetail"];
                    var sLMModel = (List<LectureMaterials>)Session["sLMModel"] ?? new List<LectureMaterials>();

                    #region addgstinvoice
                    if (command.ToLower() == "addgstinvoice")
                    {
                        int a = 0;
                        if (sModel.HCPs[0].Invoice_No == model.Invoice_No)
                            if (model.Invoice_Date == null) { TempData["HCPIDate"] = "* Please Enter Invoice Date."; a++; }
                        if (model.Invoice_No == null || model.Invoice_No == "0") { TempData["HCPInvoice"] = "* Please Enter Invoice No."; a++; }
                        if (model.Net_Amount == 0 || model.Net_Amount == null) { TempData["HCPNet"] = "* Please Enter Net Amount"; a++; }
                        if (model.Tax_Amount == 0 || model.Tax_Amount == null) { TempData["HCPTax"] = "* Please Enter Tax Amount"; a++; }
                        if (model.Total_Amount == 0 || model.Total_Amount == null) { TempData["HCPTotal"] = "* Please Enter Total Amount"; a++; }

                        if (a == 0)
                        {
                            foreach (var obj in sModel.HCPs.Where(w => w.Id == model.IHCPId))
                            {
                                if (model.Invoice_No == null)
                                {
                                    model.Invoice_No = "NN/NGST" + Convert.ToString(sModel.Form_ID);
                                }
                                if (model.Total_Amount == null)
                                {
                                    model.Total_Amount = model.IHCPCost;
                                }
                                obj.Invoice_No = model.Invoice_No;
                                obj.Invoice_Date = model.Invoice_Date;
                                obj.Net_Amount = model.Net_Amount;
                                obj.Tax_Amount = model.Tax_Amount;
                                obj.Total_Amount = model.Total_Amount;
                            }
                        }
                        sModel.IHCPId = model.IHCPId;
                        sModel.Invoice_No = model.Invoice_No;
                        sModel.Invoice_Date = model.Invoice_Date;
                        sModel.Tax_Amount = model.Tax_Amount;
                        sModel.Net_Amount = model.Net_Amount;
                        sModel.Total_Amount = model.Total_Amount;
                        TempData["TabDisplay"] = "Valid1";
                        return View(sModel);
                    }
                    #endregion

                    #region addvirtualinvoice
                    if (command.ToLower() == "addvirtualinvoice")
                    {
                        int a = 0;
                        if (model.Invoice_Date == null) { TempData["HCPIDate"] = "* Please Enter Invoice Date."; a++; }
                        else if (a == 0)
                        {

                            foreach (var obj in sModel.HCPs.Where(w => w.Id == model.IHCPId))
                            {
                                if (model.Invoice_No == null)
                                {
                                    model.Invoice_No = "NN/NGST" + Convert.ToString(sModel.Form_ID);
                                }
                                if (model.Total_Amount == null)
                                {
                                    model.Total_Amount = model.IHCPCost;
                                }
                                obj.Invoice_No = model.Invoice_No;
                                obj.Invoice_Date = model.Invoice_Date;
                                obj.Net_Amount = 0;
                                obj.Tax_Amount = 0;
                                obj.Total_Amount = model.Total_Amount;
                            }
                        }
                        sModel.IHCPId = model.IHCPId;
                        sModel.Invoice_No = model.Invoice_No;
                        sModel.Invoice_Date = model.Invoice_Date;
                        sModel.Tax_Amount = model.Tax_Amount;
                        sModel.Net_Amount = model.Net_Amount;
                        sModel.Total_Amount = model.Total_Amount;
                        TempData["TabDisplay"] = "Valid1";
                        return View(sModel);
                    }
                    #endregion

                    #region addnongstinvoice
                    if (command.ToLower() == "addnongstinvoice")
                    {
                        int a = 0;
                        if (model.Invoice_Date == null) { TempData["HCPIDate"] = "* Please Enter Invoice Date."; a++; }
                        if (model.Invoice_No == null || model.Invoice_No == "0") { TempData["HCPInvoice"] = "* Please Enter Invoice No."; a++; }
                        if (a == 0)
                        {
                            foreach (var obj in sModel.HCPs.Where(w => w.Id == model.IHCPId))
                            {
                                if (model.Invoice_No == null)
                                {
                                    model.Invoice_No = "NN/NGST" + Convert.ToString(sModel.Form_ID);
                                }
                                if (model.Total_Amount == null)
                                {
                                    model.Total_Amount = model.IHCPCost;
                                }
                                obj.Invoice_No = model.Invoice_No;
                                obj.Invoice_Date = model.Invoice_Date;
                                obj.Net_Amount = 0;
                                obj.Tax_Amount = 0;
                                obj.Total_Amount = model.Total_Amount;
                            }
                        }
                        sModel.IHCPId = model.IHCPId;
                        sModel.Invoice_No = model.Invoice_No;
                        sModel.Invoice_Date = model.Invoice_Date;
                        sModel.Tax_Amount = model.Tax_Amount;
                        sModel.Net_Amount = model.Net_Amount;
                        sModel.Total_Amount = model.Total_Amount;
                        TempData["TabDisplay"] = "Valid1";
                        return View(sModel);
                    }
                    #endregion

                    #region addnonhonorarium
                    if (command.ToLower() == "addnonhonorarium")
                    {
                        foreach (var obj in sModel.HCPs.Where(w => w.Id == model.IHCPId))
                        {
                            obj.Invoice_No = " ";
                            obj.Invoice_Date = " ";
                            obj.Net_Amount = 0;
                            obj.Tax_Amount = 0;
                            obj.Total_Amount = 0;
                        }

                        sModel.IHCPId = model.IHCPId;
                        sModel.Invoice_No = model.Invoice_No;
                        sModel.Invoice_Date = model.Invoice_Date;
                        sModel.Tax_Amount = model.Tax_Amount;
                        sModel.Net_Amount = model.Net_Amount;
                        sModel.Total_Amount = model.Total_Amount;
                        TempData["TabDisplay"] = "Valid1";
                        return View(sModel);
                    }
                    #endregion

                    #region addHCP
                    if (sModel.HCPs != null)
                    {
                        for (int i = 0; i < sModel.HCPs.Count; i++)
                        {
                            string strFileUpload = "file_" + Convert.ToString(sModel.HCPs[i].Id);
                            HttpPostedFileBase file = Request.Files[strFileUpload];
                            if (file != null)
                            {

                                if (file.ContentLength > 0)
                                {
                                    var lmModel = SaveFile(file);
                                    lmModel.PostType = "IHCP";
                                    sLMModel.Add(lmModel);
                                }
                            }
                        }
                    }
                    #endregion

                    #region attachments
                    if (model.LMImageFile != null && model.LMImageFile[0] != null && sModel.RequestType != "Approved by SM" && sLMModel.Any(s => s.PostType == "PLM") == false)
                    {
                        foreach (var obj in model.LMImageFile)
                        {
                            if (obj.ContentLength > 0)
                            {
                                var lmModel = SaveFile(obj);
                                lmModel.PostType = "PLM";
                                sLMModel.Add(lmModel);
                            }
                        }
                        sModel.LMImageFile = model.LMImageFile;
                    }
                    if (model.CCImageFile != null && model.CCImageFile.ContentLength > 0 && sLMModel.Any(s => s.PostType == "CC") == false)
                    {
                        var lmModel = SaveFile(model.CCImageFile);
                        sModel.CCImageFile = model.CCImageFile;
                        lmModel.PostType = "CC";
                        sLMModel.Add(lmModel);
                    }

                    if (model.ASImageFile != null && model.ASImageFile.ContentLength > 0 && sLMModel.Any(s => s.PostType == "AS") == false)
                    {
                        var lmModel = SaveFile(model.ASImageFile);
                        sModel.ASImageFile = model.ASImageFile;
                        lmModel.PostType = "AS";
                        sLMModel.Add(lmModel);
                    }
                    if (model.PEImageFile != null && model.PEImageFile[0] != null && Session["LoginUserZone"].ToString() == "Rare Disease" && sLMModel.Any(s => s.PostType == "PE") == false)
                    {
                        foreach (var obj in model.PEImageFile)
                        {
                            if (obj.ContentLength > 0)
                            {
                                var lmModel = SaveFile(obj);
                                lmModel.PostType = "PE";
                                sLMModel.Add(lmModel);
                            }
                        }
                        sModel.PEImageFile = model.PEImageFile;
                    }

                    if (sModel.RequestType == "Approved by SM" && Session["LoginUserZone"].ToString() == "Rare Disease" && sModel.DiseaseArea_Type != null)
                    {
                        if (model.LMImageFile != null && model.LMImageFile[0] != null)
                        {
                            foreach (var obj in model.LMImageFile)
                            {
                                if (obj.ContentLength > 0)
                                {
                                    var lmModel = SaveFile(obj, "LM");
                                    sLMModel.Add(lmModel);
                                }
                            }
                        }
                        if (model.PAImageFile != null && model.PAImageFile.ContentLength > 0 && sLMModel.SingleOrDefault(s => s.Type == "PA") == null)
                        {
                            var lmModel = SaveFile(model.PAImageFile, "PA");
                            sLMModel.Add(lmModel);
                        }
                        else
                        {
                            if (model.PAImageFile == null)
                            {
                                if (cme.IssuerRole != "KAM")
                                {
                                    TempData["PARestriction"] = "* Please upload the Proposed Agenda and FMV.";
                                    TempData["TabDisplay"] = "Valid2";
                                    return RedirectToAction("Details", "CMENew");
                                }
                            }
                            else
                            {
                                TempData["PARestriction"] = "* Not able to upload the Proposed Agenda and FMV, Already contain a Proposed Agenda and FMV.";
                                TempData["TabDisplay"] = "Valid2";
                                return RedirectToAction("Details", "CMENew");
                            }

                        }
                    }
                    #endregion

                    sModel.Comment = model.Comment;
                    sModel.Actual_Attendees = model.Actual_Attendees;
                    sModel.Final_CME_Expenses = model.Final_CME_Expenses;
                    sModel.ImagePath = sLMModel;
                    Session.Remove("CMEFullDetail");
                    Session.Add("CMEFullDetail", sModel);
                    Session.Remove("sLMModel");
                    Session.Add("sLMModel", sLMModel);
                    model.IssuedBy = Convert.ToInt32(Session["LoginUserId"]);
                    model.Issuer_Role = Convert.ToString(Session["LoginUserRole"]);
                    model.IsVirtual = cme.IsVirtual;

                    #region approve
                    if (command.ToLower() == "approve")
                    {
                        if (therapyAreaConfig.Contains(Session["LoginUserZone"].ToString()))
                        {
                            if (sModel.RequestType == "Approved by SM" && sModel.CME_Type == "Non-Standard Meeting" && (model.LMImageFile[0] != null || model.PAImageFile != null))
                            {
                                var cmeAM = new CMEModel
                                {
                                    ImagePath = sModel.ImagePath
                                };
                                _cMERepository.UpdateDocument(id, cmeAM);

                                for (int i = 0; i < model.LMImageFile.Count; i++)
                                {
                                    model.LMImageFile[i] = null;
                                }
                                model.PAImageFile = null;
                            }
                            if (sModel.RequestType == "Approved by RBD" || sModel.RequestType == "Approved by CME Operation")
                            {
                                if (model.Comment != null)
                                {
                                    if (Convert.ToDateTime(cme.ActivityStartDate) > DateTime.Now.AddDays(-1))
                                    {
                                        _cMERepository.ApprovePAFBiopharm(id, model);
                                    }
                                    else
                                    {
                                        TempData["ApprovalRestriction"] = "* Not able to approve the activity, The approval date should be less than or equal to the activity date.";
                                        return RedirectToAction("Details", "CMENew");
                                    }
                                }
                                else
                                {
                                    TempData["RejectComment"] = "* Please enter comment";
                                    TempData["mcReject"] = "*";
                                    TempData["TabDisplay"] = "Valid3";
                                    return RedirectToAction("Details", "CMENew");
                                }
                            }
                            else
                            {
                                if ((cme.RequestType == "Send to cancel approve") || (Convert.ToDateTime(cme.ActivityStartDate) > DateTime.Now.AddDays(-1)))
                                {
                                    _cMERepository.ApprovePAFBiopharm(id, model);
                                }
                                else
                                {
                                    TempData["ApprovalRestriction"] = "* Not able to approve the activity, The approval date should be less than or equal to the activity date.";
                                    return RedirectToAction("Details", "CMENew");
                                }
                            }

                        }
                        else
                        {
                            if (sModel.RequestType == "Approved by RBD" || sModel.RequestType == "Approved by CME Operation")
                            {
                                if (model.Comment != null)
                                {
                                    if (Convert.ToDateTime(cme.ActivityStartDate) > DateTime.Now.AddDays(-1))
                                    {
                                        _cMERepository.ApprovePAF(id, model);
                                    }
                                    else
                                    {
                                        TempData["ApprovalRestriction"] = "* Not able to approve the activity, The approval date should be less than or equal to the activity date.";
                                        return RedirectToAction("Details", "CMENew");
                                    }
                                }
                                else
                                {
                                    TempData["RejectComment"] = "* Please enter comment";
                                    TempData["mcReject"] = "*";
                                    TempData["TabDisplay"] = "Valid3";
                                    return RedirectToAction("Details", "CMENew");
                                }
                            }
                            else
                            {
                                if ((cme.RequestType == "Send to cancel approve") || (Convert.ToDateTime(cme.ActivityStartDate) > DateTime.Now.AddDays(-1)))
                                {
                                    _cMERepository.ApprovePAF(id, model);
                                }
                                else
                                {
                                    TempData["ApprovalRestriction"] = "* Not able to approve the activity, The approval date should be less than or equal to the activity date.";
                                    return RedirectToAction("Details", "CMENew");
                                }
                            }
                        }
                    }
                    #endregion

                    #region reject
                    if (command.ToLower() == "reject")
                    {
                        if (therapyAreaConfig.Contains(Session["LoginUserZone"].ToString()))
                        {
                            var cmeR = new CMEModel
                            {
                                ImagePath = sModel.ImagePath
                            };
                            _cMERepository.UpdateDocument(id, cmeR);

                            if (model.LMImageFile != null)
                            {
                                for (int i = 0; i < model.LMImageFile.Count; i++)
                                {
                                    model.LMImageFile[i] = null;
                                }
                            }
                            if (cme.RequestType == "Send to cancel approve")
                            {
                                _cMERepository.RejectPAFBiopharm(id, model);
                            }
                            else
                            if (model.Comment != null)
                            {
                                _cMERepository.RejectPAFBiopharm(id, model);
                            }
                            else
                            {
                                TempData["RejectComment"] = "* Please enter comment";
                                TempData["mcReject"] = "*";
                                TempData["TabDisplay"] = "Valid3";
                                return RedirectToAction("Details", "CMENew");
                            }
                        }
                        else
                        {
                            if (cme.RequestType == "Send to cancel approve" || model.Comment != null)
                            {
                                _cMERepository.RejectPAF(id, model);
                            }
                            else
                            {
                                TempData["RejectComment"] = "* Please enter comment";
                                TempData["mcReject"] = "*";
                                TempData["TabDisplay"] = "Valid3";
                                return RedirectToAction("Details", "CMENew");
                            }
                        }
                    }
                    #endregion

                    #region sendtoclose
                    if (command.ToLower() == "sendtoclose")
                    {
                        if (sModel.LMImageFile == null || sModel.CCImageFile == null || sModel.ASImageFile == null || sModel.PEImageFile == null)
                        {
                            if (sModel.LMImageFile == null) { TempData["eLMImageFile"] = "* Please upload document"; }
                            if (sModel.PEImageFile == null) { TempData["ePEImageFile"] = "* Please upload document"; }
                            return View(sModel);
                        }
                        for (int i = 0; i < cme.HCPs.Count; i++)
                        {
                            if (sModel.HCPs[i].Invoice_No == null && sModel.HCPs[i].Invoice_Date == null)
                            {
                                TempData["TabDisplay"] = "Valid1";
                                TempData["HCPInvoiceValidation"] = "* Please fill HCP invoice details";
                                return View(sModel);
                            }
                        }
                        if (sLMModel.Any(s => s.PostType == "PLM") == true && sLMModel.Any(s => s.PostType == "CC") == true &&
                            sLMModel.Any(s => s.PostType == "AS") == true && sLMModel.Any(s => s.PostType == "PE") == true)
                        {
                            _cMERepository.SendtoClosePAFBiopharm(id, new CMEDetails
                            {
                                Comment = sModel.Comment,
                                ImagePath = sModel.ImagePath,
                                IssuedBy = model.IssuedBy,
                                Issued_By_Initial = model.Issued_By_Initial,
                                Issuer_Role = model.Issuer_Role
                            });
                            return RedirectToAction("Index", "Home");
                        }
                        else
                        {
                            if (sLMModel.Any(s => s.PostType == "IHCP") == false)
                            {
                                TempData["TabDisplay"] = "Valid2";
                                TempData["eSICPImageFile"] = "* Please upload document";
                            }
                            if (sLMModel.Any(s => s.PostType == "PLM") == false)
                            {
                                TempData["TabDisplay"] = "Valid2";
                                TempData["eSLMImageFile"] = "* Please upload document";
                            }
                            if (sLMModel.Any(s => s.PostType == "CC") == false)
                            {
                                TempData["TabDisplay"] = "Valid2";
                                TempData["eSCCImageFile"] = "* Please upload document";
                            }
                            if (sLMModel.Any(s => s.PostType == "AS") == false)
                            {
                                TempData["TabDisplay"] = "Valid2";
                                TempData["eSASImageFile"] = "* Please upload document";
                            }
                            if (sLMModel.Any(s => s.PostType == "PE") == false)
                            {
                                TempData["TabDisplay"] = "Valid2";
                                TempData["eSPEImageFile"] = "* Please upload document";
                            }
                            return View(sModel);
                        }
                    }
                    #endregion

                    #region close
                    if (therapyAreaConfig.Contains(Session["LoginUserZone"].ToString()))
                    {
                        if (command.ToLower() == "close")
                        {
                            if (sModel.LMImageFile == null || sModel.CCImageFile == null || sModel.ASImageFile == null || sModel.PEImageFile == null)
                            {
                                if (sModel.LMImageFile == null) { TempData["eLMImageFile"] = "* Please upload document"; }
                                if (sModel.PEImageFile == null) { TempData["ePEImageFile"] = "* Please upload document"; }
                                TempData["TabDisplay"] = "Valid2";
                                return View(sModel);
                            }
                            for (int i = 0; i < cme.HCPs.Count; i++)
                            {
                                if ((sModel.HCPs[i].Invoice_No == null || sModel.HCPs[i].Invoice_No == "") && (sModel.HCPs[i].Invoice_Date == null || sModel.HCPs[i].Invoice_Date == ""))
                                {
                                    if (sModel.HCPs[i].Is_Honorarium == true)
                                    {
                                        TempData["TabDisplay"] = "Valid1";
                                        TempData["HCPInvoiceValidation"] = "* Please add HCP invoice details";
                                        return View(sModel);
                                    }
                                }
                            }
                            if (sLMModel.Any(s => s.PostType == "PLM") == true && sLMModel.Any(s => s.PostType == "CC") == true &&
                                sLMModel.Any(s => s.PostType == "AS") == true && sLMModel.Any(s => s.PostType == "PE") == true)
                            {
                                _cMERepository.ClosePAFBiopharm(id, new CMEDetails
                                {
                                    Comment = sModel.Comment,
                                    ImagePath = sModel.ImagePath,
                                    IssuedBy = model.IssuedBy,
                                    Issued_By_Initial = model.Issued_By_Initial,
                                    Issuer_Role = model.Issuer_Role,
                                    HCPs = sModel.HCPs
                                });
                                return RedirectToAction("Index", "Home");
                            }

                            else
                            {
                                if (sLMModel.Any(s => s.PostType == "IHCP") == false)
                                {
                                    TempData["TabDisplay"] = "Valid2";
                                    TempData["eSICPImageFile"] = "* Please upload document";
                                }
                                if (sLMModel.Any(s => s.PostType == "PLM") == false)
                                {
                                    TempData["TabDisplay"] = "Valid2";
                                    TempData["eSLMImageFile"] = "* Please upload document";
                                }
                                if (sLMModel.Any(s => s.PostType == "CC") == false)
                                {
                                    TempData["TabDisplay"] = "Valid2";
                                    TempData["eSCCImageFile"] = "* Please upload document";
                                }
                                if (sLMModel.Any(s => s.PostType == "AS") == false)
                                {
                                    TempData["TabDisplay"] = "Valid2";
                                    TempData["eSASImageFile"] = "* Please upload document";
                                }
                                if (sLMModel.Any(s => s.PostType == "PE") == false)
                                {
                                    TempData["TabDisplay"] = "Valid2";
                                    TempData["eSPEImageFile"] = "* Please upload document";
                                }
                                return View(sModel);
                            }
                        }
                    }
                    else
                    {
                        if (command.ToLower() == "close")
                        {
                            if (sModel.Actual_Attendees == null || sModel.Actual_Attendees == 0)
                            {
                                TempData["TabDisplay"] = "Valid0";
                                if (sModel.Actual_Attendees == null || sModel.Actual_Attendees == 0) { TempData["eActualAttendees"] = "* Actual Attendees is required"; }
                                return View(sModel);
                            }
                            if (sModel.Final_CME_Expenses == null || sModel.Final_CME_Expenses == 0)
                            {
                                TempData["TabDisplay"] = "Valid2";
                                if (sModel.Final_CME_Expenses == null || sModel.Final_CME_Expenses == 0 || sModel.Final_CME_Expenses == 0) { TempData["eFinal_CME_Expenses"] = "* Please Final CME Expenses"; }
                                return View(sModel);
                            }
                            for (int i = 0; i < cme.HCPs.Count; i++)
                            {

                                if ((sModel.HCPs[i].Invoice_No == null || sModel.HCPs[i].Invoice_No == "") && (sModel.HCPs[i].Invoice_Date == null || sModel.HCPs[i].Invoice_Date == "") && (sModel.HCPs[i].Total_Amount == 0 || sModel.HCPs[i].Total_Amount == null) && (sModel.HCPs[i].Tax_Amount == 0 || sModel.HCPs[i].Tax_Amount == null) && (sModel.HCPs[i].Net_Amount == 0 || sModel.HCPs[i].Net_Amount == null))
                                {
                                    if (sModel.HCPs[i].Is_Honorarium == true)
                                    {
                                        TempData["TabDisplay"] = "Valid1";
                                        TempData["HCPInvoiceValidation"] = "* Please add HCP invoice details";
                                        return View(sModel);
                                    }

                                }
                            }
                            if (sModel.LMImageFile == null || sModel.CCImageFile == null || sModel.ASImageFile == null)
                            {
                                TempData["TabDisplay"] = "Valid2";
                                if (sModel.LMImageFile == null) { TempData["eLMImageFile"] = "* Please upload document"; }
                                return View(sModel);
                            }

                            _cMERepository.ClosePAF(id, new CMEDetails
                            {
                                Comment = sModel.Comment,
                                ImagePath = sModel.ImagePath,
                                IssuedBy = model.IssuedBy,
                                Issued_By_Initial = model.Issued_By_Initial,
                                Issuer_Role = model.Issuer_Role,
                                HCPs = sModel.HCPs,
                                Actual_Attendees = sModel.Actual_Attendees,
                                Final_CME_Expenses = sModel.Final_CME_Expenses
                            });
                            return RedirectToAction("Index", "Home");
                        }
                    }
                    #endregion

                    #region remove
                    if (command.ToLower() == "remove")
                    {
                        _cMERepository.RemovePAF(id, new CMEDetails
                        {
                            Comment = sModel.Comment,
                            ImagePath = sModel.ImagePath,
                            IssuedBy = model.IssuedBy,
                            Issued_By_Initial = model.Issued_By_Initial,
                            Issuer_Role = model.Issuer_Role
                        });
                        return RedirectToAction("Index", "Home");
                    }
                    #endregion

                }
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult Edit(int id = 0)
        {
            var model = new CMEViewModel();
            string userInitials = Convert.ToString(Session["LoginUserInitial"]);
            string[] therapyAreaConfig = ConfigurationManager.AppSettings["TherapyArea"].Split(';');

            Session.Remove("CMEFullDetail");
            Session.Remove("PAFLMList");
            Session["PAFChar"] = Session["LoginUserTherapyArea"].ToString().Substring(0, 1);
            TempData.Remove("PAFRequestType"); TempData.Remove("PAFTherapyArea"); TempData.Remove("PAFTherapyAreaName"); TempData.Remove("PAFBrand"); TempData.Remove("PAFEvent"); TempData.Remove("PAFLecture"); TempData.Remove("PAFTypeofHCP"); TempData.Remove("PAFCity"); TempData.Remove("PAFNameoftheHCP"); TempData.Remove("PAFHCPViewModel"); TempData.Remove("PAFMiscellaneous"); TempData.Remove("PAFMeetingType"); TempData.Remove("PAFAssessment"); TempData.Remove("PAFParticipants");

            var cme = _cMERepository.GetPAFById(id);
            var cmeDetails = _cMERepository.GetById(id);

            if (cme == null) { return RedirectToAction("Index"); }
            else
            {
                model.ID = cme.ID;
                model.ManagerInitial = Convert.ToString(Session["LoginUserManager"].ToString());
                model.ActivityName = cme.ActivityName;
                model.Approver = cme.Approver;
                model.IssuedDate = cme.IssuedDate.ToString("dd-MM-yyyy");

                if (cme.RequestType == "Reject by CT")
                {
                    DateTime todaysDate = DateTime.Now;
                    DateTime astartdate = cme.ActivityStartDate;
                    if (astartdate < todaysDate)
                    {
                        model.ActivityStartDate = null;
                        model.ActivityEndDate = null;
                    }
                    else
                    {
                        model.ActivityStartDate = cme.ActivityStartDate.ToString("dd-MM-yyyy");
                        model.ActivityEndDate = cme.ActivityEndDate.ToString("dd-MM-yyyy");
                    }

                }
                else
                {
                    model.ActivityStartDate = cme.ActivityStartDate.ToString("dd-MM-yyyy");
                    model.ActivityEndDate = cme.ActivityEndDate.ToString("dd-MM-yyyy");
                }

                model.ActualStartDate = cme.ActualStartDate.ToString("dd-MM-yyyy");
                model.ActualEndDate = cme.ActualEndDate.ToString("dd-MM-yyyy");
                model.CME_Type = cme.CME_Type;
                model.DiseaseArea_Type = cme.DiseaseArea_Type;
                model.NumberofDays = cme.NumberofDays;
                model.ChangeStartDate = (cme.ChangeStartDate != null) ? Convert.ToDateTime(cme.ChangeStartDate).ToString("dd-MM-yyyy") : null;
                model.ChangeEndDate = (cme.ChangeEndDate != null) ? Convert.ToDateTime(cme.ChangeEndDate).ToString("dd-MM-yyyy") : null;
                model.Proposednumberofattendee = cme.Proposednumberofattendee;
                model.Roomrate = Math.Round(cme.Roomrate, 2);
                model.Banquetroomrate = Math.Round(cme.Banquetroomrate, 2);
                model.CostPerPAX = Math.Round(cme.CostPerPAX, 2);
                model.LectureId = cme.LectureId;
                model.Approximatecost = cme.Approximatecost;
                model.Minimumnumberofgaurantee = cme.Minimumnumberofgaurantee;

                var value = ConfigurationManager.AppSettings["DateKey"];
                var Finaldate = DateTime.Parse(value);
                var ActivityStartDate = cmeDetails.ModifiedDate;

                if (therapyAreaConfig.Contains(Session["LoginUserZone"].ToString()))
                {
                    if (ActivityStartDate >= Finaldate)
                    {
                        model.TherapyArea = _commonRepository.GetTherapyArea().Where(a => a.Name == "Rare Disease").Select(s => new SelectListItem { Text = s.Name.ToString(), Value = Convert.ToString(s.ID), Selected = (Convert.ToString(s.ID) == Convert.ToString(cme.TherapyAreaId)) }).ToList();
                    }
                    else
                    {
                        model.TherapyArea = _commonRepository.GetTherapyArea().Where(a => (a.Name == "Biopharm" || a.Name == "Rare Disease")).Select(s => new SelectListItem { Text = s.Name.ToString(), Value = Convert.ToString(s.ID), Selected = (Convert.ToString(s.ID) == Convert.ToString(cme.TherapyAreaId)) }).ToList();

                    }
                    model.Brand = _commonRepository.GetBrands().Select(s => new SelectListItem { Text = s.Name.ToString(), Value = Convert.ToString(s.ID), Selected = (Convert.ToString(s.ID) == Convert.ToString(cme.BrandId)) }).ToList();
                    model.Event = _lecturesRepository.GetBiopharmEvent(cme.BrandId).Select(s => new SelectListItem { Text = s.Name.ToString(), Value = Convert.ToString(s.Id), Selected = (Convert.ToString(s.Id) == Convert.ToString(cme.EventId)) }).ToList();
                    model.Lectures = _lecturesRepository.GetLectures(Convert.ToByte(cme.EventId)).Select(s => new SelectListItem { Text = s.Lecture.ToString(), Value = Convert.ToString(s.ID), Selected = (Convert.ToString(s.ID) == Convert.ToString(cme.LectureId)) }).ToList();
                }
                else
                {
                    if (ActivityStartDate >= Finaldate)
                    {
                        model.TherapyArea = _commonRepository.GetTherapyArea().Where(a => a.Name != "Biopharm").Select(s => new SelectListItem { Text = s.Name.ToString(), Value = Convert.ToString(s.ID), Selected = (Convert.ToString(s.ID) == Convert.ToString(cme.TherapyAreaId)) }).ToList();
                    }
                    else
                    {
                        model.TherapyArea = _commonRepository.GetTherapyArea().Select(s => new SelectListItem { Text = s.Name.ToString(), Value = Convert.ToString(s.ID), Selected = (Convert.ToString(s.ID) == Convert.ToString(cme.TherapyAreaId)) }).ToList();
                    }
                    model.Brand = _commonRepository.GetBrands().Select(s => new SelectListItem { Text = s.Name.ToString(), Value = Convert.ToString(s.ID), Selected = (Convert.ToString(s.ID) == Convert.ToString(cme.BrandId)) }).ToList();
                    model.Event = _lecturesRepository.GetBiopharmEvent(cme.BrandId).Select(s => new SelectListItem { Text = s.Name.ToString(), Value = Convert.ToString(s.Id), Selected = (Convert.ToString(s.Id) == Convert.ToString(cme.EventId)) }).ToList();
                    model.Lectures = _lecturesRepository.GetLectures(Convert.ToByte(cme.EventId)).Select(s => new SelectListItem { Text = s.Lecture.ToString(), Value = Convert.ToString(s.ID), Selected = (Convert.ToString(s.ID) == Convert.ToString(cme.LectureId)) }).ToList();
                }
                model.Venu_Hotel = cme.Venu_Hotel;
                model.Hotel_Star = cme.Hotel_Star;
                model.City = cme.City;
                model.TypeofHCP = _hCPListRepository.GetTypeOfHCP().Select(z => new SelectListItem { Text = z.Name.ToString(), Value = z.Id.ToString() }).OrderBy(s => s.Text).ToList();
                //model.NameoftheHCP = _hCPListRepository.GetAllZone().Select(s => new SelectListItem { Text = s.FirstName.ToString() + s.LastName.ToString(), Value = Convert.ToString(s.ID) }).ToList();
                model.MeetingType = _commonRepository.GetPafMeetingtype().Select(s => new SelectListItem { Text = s.Name.ToString(), Value = Convert.ToString(s.Id), Selected = (Convert.ToString(s.Id) == Convert.ToString(cme.MeetingTypeId)) }).ToList();
                model.Miscellaneous = cme.Miscellaneous;
                model.HCPs = cmeDetails.HCPs;
                if (cme.Participants != null)
                {
                    model.Participants = cme.Participants;
                }
                if (cme.Comments.Where(s => s.Initial == userInitials).Count() > 0)
                {
                    model.Comment = (cme.Comments.Count > 0) ? cme.Comments.Where(s => s.Initial == userInitials).OrderByDescending(o => o.Id).FirstOrDefault().Comment : "";
                }
                model.Comments = cme.Comments;
                model.RequestType = cme.RequestType;
                model.IsDateDeviation = cme.IsDateDeviation;
                model.IsDeviationApproved = cme.IsDeviationApproved;
                model.ImagePath = cme.ImagePath;
                model.HospitalMeeting = cme.HospitalMeeting;
                if (cme.IsVirtual == null)
                {
                    model.IsVirtual = false;
                }
                else
                {
                    model.IsVirtual = (bool)cme.IsVirtual;
                }
                model.IsPostPoned = cme.IsPostPoned;
                if (model.LectureId == null)
                {
                    model.LMFiles = _lecturesRepository.GetMaterialsByEventId(Convert.ToByte(id)).ToList();
                }
                else
                {
                    model.LMFiles = _lecturesRepository.GetMaterialsById(Convert.ToByte(id)).ToList(); ;
                }

                model.IsBTC = cmeDetails.IsBTC;
                if (cme.Policy_Violation == null)
                {
                    model.Policy_Violation = false;
                }
                else
                {
                    model.Policy_Violation = (bool)cme.Policy_Violation;
                }
                var amsConfigDetail = _configRepository.GetAMSconfig().Select(s => new SelectListItem { Text = s.Name, Value = s.Value }).ToList();
                foreach (var i in amsConfigDetail)
                {
                    if (i.Text == "Hospital_meeting_min_days") { model.HMinDays = Int32.Parse(i.Value); }
                    else if (i.Text == "Hospital_meeting_max_days") { model.HMaxDays = Int32.Parse(i.Value); }
                    else if (i.Text == "Diabetes_meeting_min_days") { model.DMinDays = Int32.Parse(i.Value); }
                    else if (i.Text == "Diabetes_meeting_max_days") { model.DMaxDays = Int32.Parse(i.Value); }
                }
                TempData.Add("PAFTherapyArea", model.TherapyArea);
                TempData.Add("PAFTherapyAreaName", model.TherapyAreaName);
                TempData.Add("PAFBrand", model.Brand);
                TempData.Add("PAFEvent", model.Event);
                TempData.Add("PAFLecture", model.Lectures);
                TempData.Add("PAFCity", model.City);
                TempData.Add("PAFTypeofHCP", model.TypeofHCP);
                TempData.Add("PAFNameoftheHCP", model.NameoftheHCP);
                TempData.Add("PAFMeetingType", model.MeetingType);
                TempData["PAFMiscellaneous"] = cme.Miscellaneous;
                TempData["PAFHCPViewModel"] = cmeDetails.HCPs;
                if (cme.Participants != null)
                {
                    TempData["PAFParticipants"] = cme.Participants;
                }
                TempData["PAFRequestType"] = cme.RequestType;
                model.IsDeleteFile = false;
                Session.Add("CMEFullDetail", model);
                return View(model);
            }
        }

        [HttpPost]
        public ActionResult Edit(int id, CMEViewModel model, string command)
        {
            try
            {
                var cmeController = new CMEController();
                int i = 0;
                var cme = new CMEModel();
                var cmedata = _cMERepository.GetPAFById(id);
                var cmeDetails = _cMERepository.GetById(id);
                var sModel = (CMEViewModel)Session["CMEFullDetail"];
                var sLMModel = (List<LectureMaterials>)sModel.ImagePath;
                string[] therapyAreaConfig = ConfigurationManager.AppSettings["TherapyArea"].Split(';');

                if (sLMModel == null) { sLMModel = new List<LectureMaterials>(); }
                var amsConfigDetail = _configRepository.GetAMSconfig().Select(s => new SelectListItem { Text = s.Name, Value = s.Value }).ToList();
                foreach (var a in amsConfigDetail)
                {
                    if (a.Text == "Hospital_meeting_min_days") { model.HMinDays = Int32.Parse(a.Value); }
                    else if (a.Text == "Hospital_meeting_max_days") { model.HMaxDays = Int32.Parse(a.Value); }
                    else if (a.Text == "Diabetes_meeting_min_days") { model.DMinDays = Int32.Parse(a.Value); }
                    else if (a.Text == "Diabetes_meeting_max_days") { model.DMaxDays = Int32.Parse(a.Value); }
                }

                sModel.ImagePath = sLMModel;
                model.ImagePath = sLMModel;
                Session.Remove("CMEFullDetail");
                Session.Add("CMEFullDetail", sModel);
                model.RequestType = TempData["PAFRequestType"].ToString();
                TempData.Keep("PAFRequestType");
                model.LMFiles = (List<LectureMaterials>)Session["PAFLMList"];
                model.LectureId = cmedata.LectureId;
                model.IssuedDate = cmeDetails.IssuedDate;
                model.Approver = cmeDetails.Approver;
                var emptyList = new List<SelectListItem>();

                #region Cost
                if (command == "CostNext")
                {
                    model.HCPs = TempData["PAFHCPViewModel"] as List<PAFHCPViewModel>;
                    model.Participants = TempData["PAFParticipants"] as List<PAFParticipants>;
                    model.Miscellaneous = TempData["PAFMiscellaneous"] as List<PAFMiscellaneous>;
                    TempData["TabDisplay"] = "Valid3";
                }
                #endregion

                #region HCPNext

                if (command == "HCPNext")
                {
                    model.HCPs = TempData["PAFHCPViewModel"] as List<PAFHCPViewModel>;
                    model.Participants = TempData["PAFParticipants"] as List<PAFParticipants>;
                    model.Miscellaneous = TempData["PAFMiscellaneous"] as List<PAFMiscellaneous>;
                    if ((model.HCPs != null && model.HCPs.Count == 0) || model.HCPs == null)
                    {
                        model.TherapyArea = TempData["PAFTherapyArea"] as IEnumerable<SelectListItem>;
                        TempData.Keep("PAFTherapyArea");
                        if (therapyAreaConfig.Contains(Session["LoginUserZone"].ToString()))
                        {
                            model.TherapyAreaName = TempData["PAFTherapyAreaName"] as String;
                            TempData.Keep("PAFTherapyAreaName");
                        }

                        model.Brand = TempData["PAFBrand"] as IEnumerable<SelectListItem>;
                        TempData.Keep("PAFBrand");
                        model.Event = TempData["PAFEvent"] as IEnumerable<SelectListItem>;
                        TempData.Keep("PAFEvent");
                        model.Lectures = TempData["PAFLecture"] as IEnumerable<SelectListItem>;
                        TempData.Keep("PAFLecture");
                        TempData.Keep("PAFCity");
                        model.TypeofHCP = TempData["PAFTypeofHCP"] as IEnumerable<SelectListItem>;
                        TempData.Keep("PAFTypeofHCP");
                        model.NameoftheHCP = TempData["PAFNameoftheHCP"] as IEnumerable<SelectListItem>;
                        TempData.Keep("PAFNameoftheHCP");
                        model.MeetingType = TempData["PAFMeetingType"] as IEnumerable<SelectListItem>;
                        TempData.Keep("PAFMeetingType");

                        if (!therapyAreaConfig.Contains(Session["LoginUserZone"].ToString()))
                        {
                            if (model.SelectedTherapyArea[0] == "") { TempData["sTherapyArea"] = "* Please select Therapy Area"; }
                        }
                        if (model.SelectedBrand[0] == "") { TempData["sBrandArea"] = "* Please select Brand"; }
                        if (model.SelectedEvent[0] == "") { TempData["sEventArea"] = "* Please select Event"; }
                        if (model.SelectedLecture[0] == "") { TempData["sLecture"] = "* Please select Lecture"; }
                        if (model.Minimumnumberofgaurantee <= 0) { TempData["Minnuofgu"] = "* Please select a valid minimum number of guarantee"; }
                        if ((model.HCPs != null && model.HCPs.Count == 0) || model.HCPs == null) { TempData["eAddHCP"] = "*Please add HCP"; }
                        if (model.SelectedMeetingType[0] == "") { TempData["sMeetingType"] = "* Please select Meeting Type"; }
                        if (model.IsBTC == null && model.IsVirtual == false) { TempData["sBTC"] = "* Please select BTC"; }

                        model.Miscellaneous = TempData["PAFMiscellaneous"] as List<PAFMiscellaneous>;
                        TempData.Remove("PAFMiscellaneous");
                        TempData.Add("PAFMiscellaneous", model.Miscellaneous);
                        TempData.Keep("PAFMiscellaneous");

                        model.HCPs = TempData["PAFHCPViewModel"] as List<PAFHCPViewModel>;
                        TempData.Remove("PAFHCPViewModel");
                        TempData.Add("PAFHCPViewModel", model.HCPs);
                        TempData.Keep("PAFHCPViewModel");

                        model.Participants = TempData["PAFParticipants"] as List<PAFParticipants>;
                        TempData.Remove("PAFParticipants");
                        TempData.Add("PAFParticipants", model.Participants);
                        TempData.Keep("PAFParticipants");

                        TempData["TabDisplay"] = "Valid1";

                        return View(model);
                    }
                    else
                    {
                        TempData["TabDisplay"] = "Valid2";
                    }
                }
                #endregion

                #region DetailsNext
                if (command == "DetailsNext")
                {
                    if (model.SelectedEvent == null && model.CME_Type == "Non-Standard Meeting")
                    {
                        model.SelectedEvent = new List<string>
                        {
                            ""
                        };
                    }
                    if (model.SelectedLecture == null && model.CME_Type == "Non-Standard Meeting")
                    {
                        model.SelectedLecture = new List<string>
                        {
                            ""
                        };
                    }
                    if (Convert.ToDateTime(model.ActivityStartDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat) > Convert.ToDateTime(model.ActivityEndDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat)
                        || Convert.ToDateTime(model.ChangeStartDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat) > Convert.ToDateTime(model.ChangeEndDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat)
                        || model.Minimumnumberofgaurantee > model.Proposednumberofattendee
                        || (model.ActivityStartDate == "" || model.ActivityStartDate == null)
                        || (model.ActivityEndDate == "" || model.ActivityEndDate == null)
                        || (model.CME_Type == "Non-Standard Meeting" && string.IsNullOrEmpty(model.ActivityName))
                        || model.SelectedTherapyArea[0] == "" && Session["LoginUserZone"].ToString() != "Rare Disease"
                        || (model.SelectedBrand[0] == "" && model.ActivityName == null)
                        || (model.SelectedEvent[0] == "" && model.ActivityName == null)
                        || (model.SelectedLecture[0] == "" && model.ActivityName == null)
                        || model.Minimumnumberofgaurantee <= 0
                        || model.Minimumnumberofgaurantee == null
                        || (model.IsBTC == null && model.IsVirtual == false)
                        || model.SelectedMeetingType[0] == "")
                    {
                        model.TherapyArea = TempData["PAFTherapyArea"] as IEnumerable<SelectListItem>;
                        TempData.Keep("PAFTherapyArea");
                        if (therapyAreaConfig.Contains(Session["LoginUserZone"].ToString()))
                        {
                            model.TherapyAreaName = TempData["PAFTherapyAreaName"] as String;
                            TempData.Keep("PAFTherapyAreaName");
                        }

                        model.Brand = TempData["PAFBrand"] as IEnumerable<SelectListItem>;
                        TempData.Keep("PAFBrand");
                        model.Event = TempData["PAFEvent"] as IEnumerable<SelectListItem>;
                        TempData.Keep("PAFEvent");
                        model.Lectures = TempData["PAFLecture"] as IEnumerable<SelectListItem>;
                        TempData.Keep("PAFLecture");
                        //added to avoid error
                        model.Lectures = emptyList;
                        TempData.Keep("PAFCity");
                        model.TypeofHCP = TempData["PAFTypeofHCP"] as IEnumerable<SelectListItem>;
                        TempData.Keep("PAFTypeofHCP");
                        model.NameoftheHCP = TempData["PAFNameoftheHCP"] as IEnumerable<SelectListItem>;
                        TempData.Keep("PAFNameoftheHCP");
                        model.MeetingType = TempData["PAFMeetingType"] as IEnumerable<SelectListItem>;
                        TempData.Keep("PAFMeetingType");

                        if (!therapyAreaConfig.Contains(Session["LoginUserZone"].ToString()))
                        {
                            if (model.SelectedTherapyArea[0] == "") { TempData["sTherapyArea"] = "* Please select Therapy Area"; }
                        }
                        if (model.SelectedBrand[0] == "") { TempData["sBrandArea"] = "* Please select Brand"; }
                        if (model.SelectedEvent[0] == "") { TempData["sEventArea"] = "* Please select Event"; }
                        if (model.SelectedLecture[0] == "") { TempData["sLecture"] = "* Please select Lecture"; }
                        if (model.Minimumnumberofgaurantee <= 0) { TempData["Minnuofgu"] = "* Please select a valid minimum number of guarantee"; }
                        if (model.SelectedMeetingType[0] == "") { TempData["sMeetingType"] = "* Please select Meeting Type"; }
                        if (model.IsBTC == null && model.IsVirtual == false) { TempData["sBTC"] = "* Please select BTC"; }
                        if (model.ActivityStartDate == "" && model.ActivityStartDate == null) { TempData["ActivityEndDate"] = "* Please select Activity Start Date "; }
                        if (model.ActivityEndDate == "" && model.ActivityEndDate == null) { TempData["ActivityEndDate"] = "* Please select Activity End Date "; }

                        model.Miscellaneous = TempData["PAFMiscellaneous"] as List<PAFMiscellaneous>;
                        TempData.Remove("PAFMiscellaneous");
                        TempData.Add("PAFMiscellaneous", model.Miscellaneous);
                        TempData.Keep("PAFMiscellaneous");

                        model.HCPs = TempData["PAFHCPViewModel"] as List<PAFHCPViewModel>;
                        TempData.Remove("PAFHCPViewModel");
                        TempData.Add("PAFHCPViewModel", model.HCPs);
                        TempData.Keep("PAFHCPViewModel");

                        model.Participants = TempData["PAFParticipants"] as List<PAFParticipants>;
                        TempData.Remove("PAFParticipants");
                        TempData.Add("PAFParticipants", model.Participants);
                        TempData.Keep("PAFParticipants");

                        return View(model);
                    }
                    else
                    {
                        model.TherapyArea = TempData["PAFTherapyArea"] as IEnumerable<SelectListItem>;
                        TempData.Keep("PAFTherapyArea");
                        if (therapyAreaConfig.Contains(Session["LoginUserZone"].ToString()))
                        {
                            model.TherapyAreaName = TempData["PAFTherapyAreaName"] as String;
                            TempData.Keep("PAFTherapyAreaName");
                        }

                        model.Brand = TempData["PAFBrand"] as IEnumerable<SelectListItem>;
                        TempData.Keep("PAFBrand");
                        model.Event = TempData["PAFEvent"] as IEnumerable<SelectListItem>;
                        TempData.Keep("PAFEvent");
                        model.Lectures = TempData["PAFLecture"] as IEnumerable<SelectListItem>;
                        TempData.Keep("PAFLecture");
                        //added to avoid error
                        if (model.Lectures == null)
                        {
                            model.Lectures = emptyList;
                        }

                        TempData.Keep("PAFCity");
                        model.TypeofHCP = TempData["PAFTypeofHCP"] as IEnumerable<SelectListItem>;
                        TempData.Keep("PAFTypeofHCP");
                        model.NameoftheHCP = TempData["PAFNameoftheHCP"] as IEnumerable<SelectListItem>;
                        TempData.Keep("PAFNameoftheHCP");
                        model.MeetingType = TempData["PAFMeetingType"] as IEnumerable<SelectListItem>;
                        TempData.Keep("PAFMeetingType");

                        cme.PAFType = command;
                        cme.IssuedBy = Convert.ToInt32(Session["LoginUserId"]);
                        cme.ManagerInitial = Convert.ToString(Session["LoginUserManager"].ToString());
                        cme.CME_Type = model.CME_Type;
                        if (cme.CME_Type == "Non-Standard Meeting" && Session["LoginUserZone"].ToString() == "Rare Disease")
                        {
                            cme.DiseaseArea_Type = model.DiseaseArea_Type;
                        }
                        else
                        {
                            cme.DiseaseArea_Type = null;
                        }
                        cme.IssuedDate = Convert.ToDateTime(DateTime.Now, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                        cme.ActivityName = model.ActivityName;
                        if (!therapyAreaConfig.Contains(Session["LoginUserZone"].ToString()))
                        {
                            if (model.SelectedTherapyArea[0] != "" && model.SelectedTherapyArea.Count > 0)
                            {
                                cme.TherapyAreaId = Convert.ToByte(model.SelectedTherapyArea[0]);
                            }
                        }
                        else { cme.TherapyAreaId = sModel.TherapyAreaId; }
                        if (model.SelectedBrand[0] != "" && model.SelectedBrand.Count > 0)
                        {
                            cme.BrandId = Byte.Parse(model.SelectedBrand[0]);
                        }
                        if (model.SelectedEvent[0] != "" && model.SelectedEvent.Count > 0)
                        {
                            cme.EventId = int.Parse(model.SelectedEvent[0]);
                        }
                        if (model.SelectedLecture[0] != "" && model.SelectedLecture.Count > 0)
                        {
                            cme.LectureId = int.Parse(model.SelectedLecture[0]);
                        }
                        if (model.SelectedMeetingType[0] != "" && model.SelectedMeetingType.Count > 0)
                        {
                            cme.MeetingTypeId = Byte.Parse(model.SelectedMeetingType[0]);
                        }
                        cme.ActivityStartDate = Convert.ToDateTime(model.ActivityStartDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                        cme.ActivityEndDate = Convert.ToDateTime(model.ActivityEndDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                        cme.NumberofDays = Convert.ToInt32(model.NumberofDays);
                        cme.Proposednumberofattendee = Convert.ToInt32(model.Proposednumberofattendee);
                        cme.Minimumnumberofgaurantee = Convert.ToInt32(model.Minimumnumberofgaurantee);
                        cme.City = model.City;
                        cme.Venu_Hotel = model.Venu_Hotel;
                        cme.Hotel_Star = model.Hotel_Star;
                        cme.HospitalMeeting = model.HospitalMeeting;
                        cme.IsVirtual = model.IsVirtual;
                        cme.IsBTC = model.IsBTC;
                        cme.IsDateDeviation = model.IsDateDeviation;
                        cme.Policy_Violation = model.Policy_Violation;
                        cme.Comment = model.Comment;
                        if (model.HCPs == null) { model.HCPs = cmeDetails.HCPs; }
                        if (model.Participants == null) { model.Participants = cmeDetails.Participants; }
                        if (model.Miscellaneous == null) model.Miscellaneous = cmedata.Miscellaneous;
                        TempData["TabDisplay"] = "Valid1";
                        return View(model);
                    }
                }
                #endregion

                #region Draft or Send
                if (command == "Draft" || command == "Send")
                {
                    #region Attachements
                    if (!sModel.IsDeleteFile)
                    {
                        if (model.PAImageFile != null && model.PAImageFile.ContentLength > 0)
                        {
                            if (sLMModel.SingleOrDefault(s => s.Type == "PA") == null)
                            {
                                var lmModel = SaveFile(model.PAImageFile, "PA");
                                sLMModel.Add(lmModel);
                            }
                            else
                            {
                                i = 1;
                                TempData["ePAImageFile"] = "* Please remove then upload Proposed Agenda and FMV";
                            }
                        }
                        if (model.HQImageFile != null && model.HQImageFile.ContentLength > 0)
                        {
                            if (sLMModel.SingleOrDefault(s => s.Type == "HQ") == null)
                            {
                                var lmModel = SaveFile(model.HQImageFile, "HQ");
                                sLMModel.Add(lmModel);
                            }
                            else
                            {
                                i = 1;
                                TempData["eHQImageFile"] = "* Please remove then upload Hotel Quotation";
                            }
                        }
                        if (!therapyAreaConfig.Contains(Session["LoginUserZone"].ToString()))
                        {
                            if (model.LMImageFile[0] != null && model.LMImageFile[0].ContentLength > 0)
                            {
                                foreach (var obj in model.LMImageFile)
                                {
                                    if (obj.ContentLength > 0)
                                    {
                                        var lmModel = SaveFile(obj, "LM");
                                        sLMModel.Add(lmModel);
                                    }
                                }
                            }
                        }
                        if (model.HERImageFile != null && model.HERImageFile.ContentLength > 0)
                        {
                            if (sLMModel.SingleOrDefault(s => s.Type == "HER") == null)
                            {
                                var lmModel = SaveFile(model.HERImageFile, "HER");
                                sLMModel.Add(lmModel);
                            }
                            else
                            {
                                i = 1;
                                TempData["eHERImageFile"] = "* Please remove then upload Hotel Exception Request Form";
                            }
                        }
                    }
                    if (model.LectureId == null)
                    {
                        model.Lectures = emptyList;
                    }
                    #endregion

                    var userDetails = _userRepository.GetManagerStatusById(cmedata.IssuedBy);
                    if (sModel.IsDeleteFile == true) { sModel.IsDeleteFile = false; }
                    if (Convert.ToDateTime(model.ActivityStartDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat) > Convert.ToDateTime(model.ActivityEndDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat)
                        || Convert.ToDateTime(model.ChangeStartDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat) > Convert.ToDateTime(model.ChangeEndDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat)
                        || model.Minimumnumberofgaurantee > model.Proposednumberofattendee
                        || sLMModel.Where(s => s.Type == "PA").Count() <= 0
                        || (sLMModel.Where(s => s.Type == "HQ").Count() <= 0 && model.IsVirtual == false)
                        || (model.Hotel_Star && sLMModel.Where(s => s.Type == "HER").Count() <= 0)
                        || string.IsNullOrEmpty(model.Comment)
                        || (model.CME_Type == "Non-Standard Meeting" && string.IsNullOrEmpty(model.ActivityName))
                        || (sModel.HCPs.Count == 0)
                        || model.SelectedTherapyArea[0] == "" && Session["LoginUserZone"].ToString() != "Rare Disease"
                        || (model.SelectedBrand[0] == "" && model.ActivityName == null)
                        || (model.SelectedEvent[0] == "" && model.ActivityName == null)
                        || (model.SelectedLecture[0] == "" && model.ActivityName == null)
                        || (i == 1)
                        || model.Minimumnumberofgaurantee <= 0
                        || model.Minimumnumberofgaurantee == null
                        || userDetails.Status == false
                        || (model.IsBTC == null && model.IsVirtual == false)
                        || model.SelectedMeetingType[0] == "")
                    {
                        model.TherapyArea = TempData["PAFTherapyArea"] as IEnumerable<SelectListItem>;
                        TempData.Keep("PAFTherapyArea");
                        model.TherapyAreaName = TempData["PAFTherapyAreaName"] as String;
                        TempData.Keep("PAFTherapyAreaName");
                        model.Brand = TempData["PAFBrand"] as IEnumerable<SelectListItem>;
                        TempData.Keep("PAFBrand");
                        model.Event = TempData["PAFEvent"] as IEnumerable<SelectListItem>;
                        TempData.Keep("PAFEvent");
                        model.Lectures = TempData["PAFLecture"] as IEnumerable<SelectListItem>;
                        TempData.Keep("PAFLecture");
                        model.TypeofHCP = TempData["PAFTypeofHCP"] as IEnumerable<SelectListItem>;
                        TempData.Keep("PAFTypeofHCP");
                        model.NameoftheHCP = TempData["PAFNameoftheHCP"] as IEnumerable<SelectListItem>;
                        TempData.Keep("PAFNameoftheHCP");
                        model.MeetingType = TempData["PAFMeetingType"] as IEnumerable<SelectListItem>;
                        TempData.Keep("PAFMeetingType");

                        if (model.SelectedTherapyArea[0] == "") { TempData["sTherapyArea"] = "* Please select Therapy Area"; }
                        if (model.SelectedBrand[0] == "") { TempData["sBrandArea"] = "* Please select Brand"; }
                        if (model.SelectedEvent[0] == "") { TempData["sEventArea"] = "* Please select Event"; }
                        if (model.LectureId != null)
                        {
                            if (model.SelectedLecture[0] == "") { TempData["sLectureArea"] = "* Please select Lecture"; }
                        }
                        if (model.Minimumnumberofgaurantee <= 0) { TempData["Minnuofgu"] = "* Please select a valid minimum number of guarantee"; }
                        if (sModel.HCPs.Count == 0)
                        { TempData["eAddNoHCP"] = "*Please add HCP"; }
                        //validation for Meeting Type
                        if (model.SelectedMeetingType[0] == "") { TempData["sMeetingType"] = "* Please select Meeting Type"; }
                        //validation for Meeting Type
                        if (model.IsBTC == null && model.IsVirtual == false) { TempData["sBTC"] = "* Please select BTC"; }

                        if (userDetails.Status == false)
                        {
                            TempData["managerStatusFalse"] = "* Your Manager is inactive, Please contact Administrator.";
                        }
                        model.Miscellaneous = TempData["PAFMiscellaneous"] as List<PAFMiscellaneous>;
                        TempData.Remove("PAFMiscellaneous");
                        TempData.Add("PAFMiscellaneous", model.Miscellaneous);
                        TempData.Keep("PAFMiscellaneous");

                        model.HCPs = TempData["PAFHCPViewModel"] as List<PAFHCPViewModel>;
                        TempData.Remove("PAFHCPViewModel");
                        TempData.Add("PAFHCPViewModel", model.HCPs);
                        TempData.Keep("PAFHCPViewModel");

                        model.Participants = TempData["PAFParticipants"] as List<PAFParticipants>;
                        TempData.Remove("PAFParticipants");
                        TempData.Add("PAFParticipants", model.Participants);
                        TempData.Keep("PAFParticipants");
                        if (sLMModel.Where(s => s.Type == "PA").Count() <= 0 || (sLMModel.Where(s => s.Type == "HQ").Count() <= 0 && model.IsVirtual == false) || (model.Hotel_Star && sLMModel.Where(s => s.Type == "HER").Count() <= 0))
                        {
                            TempData["TabDisplay"] = "Valid3";
                        }

                        return View(model);
                    }
                    else
                    {
                        cme.PAFType = command;
                        cme.IssuedBy = Convert.ToInt32(Session["LoginUserId"]);
                        cme.ManagerInitial = Convert.ToString(Session["LoginUserManager"].ToString());
                        cme.IssuerZone = Convert.ToString(Session["LoginUserZone"].ToString());
                        cme.IssuerRole = Convert.ToString(Session["LoginUserRole"].ToString());
                        cme.Approver = model.Approver;
                        cme.CME_Type = model.CME_Type;
                        if (cme.CME_Type == "Non-Standard Meeting" && Session["LoginUserZone"].ToString() == "Rare Disease")
                        {
                            cme.DiseaseArea_Type = model.DiseaseArea_Type;

                        }
                        else
                        {
                            cme.DiseaseArea_Type = null;
                        }
                        cme.ActivityName = model.ActivityName;
                        if (!therapyAreaConfig.Contains(Session["LoginUserZone"].ToString()))
                        {
                            if (model.SelectedTherapyArea[0] != "" && model.SelectedTherapyArea.Count > 0)
                            {
                                cme.TherapyAreaId = Convert.ToByte(model.SelectedTherapyArea[0]);
                            }
                        }
                        else { cme.TherapyAreaId = Convert.ToByte(model.SelectedTherapyArea[0]); }
                        if (model.SelectedBrand[0] != "" && model.SelectedBrand.Count > 0)
                        {
                            cme.BrandId = Byte.Parse(model.SelectedBrand[0]);
                        }
                        if (model.SelectedEvent[0] != "" && model.SelectedEvent.Count > 0)
                        {
                            cme.EventId = int.Parse(model.SelectedEvent[0]);
                        }
                        if (model.SelectedLecture[0] != "" && model.SelectedLecture.Count > 0)
                        {
                            cme.LectureId = int.Parse(model.SelectedLecture[0]);
                        }
                        if (model.SelectedMeetingType[0] != "" && model.SelectedMeetingType.Count > 0)
                        {
                            cme.MeetingTypeId = Byte.Parse(model.SelectedMeetingType[0]);
                        }

                        cme.ActivityStartDate = Convert.ToDateTime(model.ActivityStartDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                        cme.ActivityEndDate = Convert.ToDateTime(model.ActivityEndDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                        cme.NumberofDays = Convert.ToInt32(model.NumberofDays);
                        cme.ChangeStartDate = !string.IsNullOrEmpty(model.ChangeStartDate) ? Convert.ToDateTime(model.ChangeStartDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat) : (DateTime?)null;
                        cme.ChangeEndDate = !string.IsNullOrEmpty(model.ChangeEndDate) ? Convert.ToDateTime(model.ChangeEndDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat) : (DateTime?)null;
                        cme.Proposednumberofattendee = Convert.ToInt32(model.Proposednumberofattendee);
                        cme.Minimumnumberofgaurantee = Convert.ToInt32(model.Minimumnumberofgaurantee);
                        cme.Approximatecost = Convert.ToInt32(model.Approximatecost);
                        cme.Roomrate = Convert.ToInt32(model.Roomrate);
                        cme.Banquetroomrate = Convert.ToInt32(model.Banquetroomrate);
                        cme.CostPerPAX = Convert.ToInt32(model.CostPerPAX);
                        cme.City = model.City;
                        cme.Venu_Hotel = model.Venu_Hotel;
                        cme.Hotel_Star = model.Hotel_Star;
                        cme.Miscellaneous = TempData["PAFMiscellaneous"] as List<PAFMiscellaneous>;
                        cme.HCPs = TempData["PAFHCPViewModel"] as List<PAFHCPViewModel>;
                        cme.Participants = TempData["PAFParticipants"] as List<PAFParticipants>;
                        cme.Comment = model.Comment;
                        cme.ImagePath = model.ImagePath;
                        cme.HospitalMeeting = model.HospitalMeeting;
                        cme.IsVirtual = model.IsVirtual;
                        cme.IsDeviationApproved = model.IsDeviationApproved;
                        cme.IsDateDeviation = model.IsDateDeviation;
                        cme.Policy_Violation = model.Policy_Violation;
                        cme.IsBTC = model.IsBTC;
                        var userDetail = _userRepository.GetUserByInitial(Session["LoginUserInitial"].ToString());
                        cme.Cost_Center = userDetail.CostCenter;
                        cmeController.Update(id, cme);
                        TempData.Remove("PAFRequestType"); TempData.Remove("PAFTherapyArea"); TempData.Remove("PAFTherapyAreaName"); TempData.Remove("PAFBrand"); TempData.Remove("PAFEvent"); TempData.Remove("PAFLecture"); TempData.Remove("PAFTypeofHCP"); TempData.Remove("PAFCity"); TempData.Remove("PAFNameoftheHCP"); TempData.Remove("PAFHCPViewModel"); TempData.Remove("PAFMiscellaneous"); TempData.Remove("PAFMeetingType"); TempData.Remove("PAFAssessment"); TempData.Remove("PAFParticipants");
                        return RedirectToAction("Index", "Home");
                    }


                }
                #endregion

                #region Cancel
                else if (command.ToLower() == "cancel")
                {
                    Session.Remove("CMEFullDetail");
                    Session.Remove("sLMModel");
                    TempData.Remove("PAFRequestType"); TempData.Remove("PAFTherapyArea"); TempData.Remove("PAFTherapyAreaName"); TempData.Remove("PAFBrand"); TempData.Remove("PAFEvent"); TempData.Remove("PAFLecture"); TempData.Remove("PAFTypeofHCP"); TempData.Remove("PAFCity"); TempData.Remove("PAFNameoftheHCP"); TempData.Remove("PAFHCPViewModel"); TempData.Remove("PAFMiscellaneous"); TempData.Remove("PAFMeetingType"); TempData.Remove("PAFAssessment"); TempData.Remove("PAFParticipants");
                    return RedirectToAction("Index", "Home");
                }
                #endregion

                #region Delete
                else if (command.ToLower() == "delete")
                {
                    cmeController.Delete(id);
                    return RedirectToAction("Index", "Home");
                }
                #endregion

                #region SubSteps
                else
                {
                    model.TherapyArea = TempData["PAFTherapyArea"] as IEnumerable<SelectListItem>;
                    TempData.Keep("PAFTherapyArea");
                    if (therapyAreaConfig.Contains(Session["LoginUserZone"].ToString()))
                    {
                        model.TherapyAreaName = TempData["PAFTherapyAreaName"] as String;
                        TempData.Keep("PAFTherapyAreaName");
                    }
                    model.Brand = TempData["PAFBrand"] as IEnumerable<SelectListItem>;
                    TempData.Keep("PAFBrand");
                    model.Event = TempData["PAFEvent"] as IEnumerable<SelectListItem>;
                    TempData.Keep("PAFEvent");
                    model.Lectures = TempData["PAFLecture"] as IEnumerable<SelectListItem>;
                    TempData.Keep("PAFLecture");
                    model.TypeofHCP = TempData["PAFTypeofHCP"] as IEnumerable<SelectListItem>;
                    TempData.Keep("PAFTypeofHCP");
                    model.NameoftheHCP = TempData["PAFNameoftheHCP"] as IEnumerable<SelectListItem>;
                    TempData.Keep("PAFNameoftheHCP");
                    model.MeetingType = TempData["PAFMeetingType"] as IEnumerable<SelectListItem>;
                    TempData.Keep("PAFMeetingType");

                    #region Add HCP
                    if (command == "Add HCP")
                    {
                        model.HCPs = TempData["PAFHCPViewModel"] as List<PAFHCPViewModel>;
                        if (model.HCPs == null)
                        { model.HCPs = new List<PAFHCPViewModel>(); }
                        if (!string.IsNullOrEmpty(model.SelectedTypeofHCP[0]) && !string.IsNullOrEmpty(model.SelectedNameoftheHCP[0]) && (model.HCPCost != 0))
                        {
                            var hcp = _hCPListRepository.GetHCPById(Convert.ToInt32(model.SelectedNameoftheHCP[0]));
                            if (hcp != null)
                            {
                                if (model.HCPs.SingleOrDefault(s => s.TMF_No == hcp.TMFNo && s.First_Name == hcp.FirstName && s.Last_Name == hcp.LastName && s.HCPAddress == hcp.HCPAddress) == null)
                                {
                                    //if (hcp.NoLecture > hcp.NoLectureGiven)
                                    //{
                                    model.HCPs.Add(new PAFHCPViewModel
                                    {
                                        Type = Convert.ToByte(model.SelectedTypeofHCP[0]),
                                        HCPId = hcp.ID,
                                        First_Name = hcp.FirstName,
                                        Last_Name = hcp.LastName,
                                        TMF_No = hcp.TMFNo,
                                        Cost = Convert.ToDecimal(model.HCPCost),
                                        HCPAddress = hcp.HCPAddress,
                                        HCPtopic = model.HCPtopic
                                    });
                                    //}
                                    //else { TempData["eNolect"] = "* This doctor exceeds the limit of HCP"; }
                                }
                                else { TempData["SameUser"] = "*HCP has been already selected, Choose another"; }
                            }
                        }
                        else if (!string.IsNullOrEmpty(model.SelectedTypeofHCP[1]) && !string.IsNullOrEmpty(model.HCPFirstName) && !string.IsNullOrEmpty(model.HCPLastName) && (model.NOHImageFile[0] != null && model.NOHImageFile[0].ContentLength > 0))
                        {
                            if (model.HCPs.SingleOrDefault(s => s.First_Name == model.HCPFirstName || s.Last_Name == model.HCPLastName || (s.TMF_No == model.TMFNo && model.TMFNo != null)) == null)
                            {
                                foreach (var obj in model.NOHImageFile)
                                {
                                    if (obj.ContentLength > 0)
                                    {
                                        var lmModel = SaveFile(obj, "NOH");
                                        lmModel.HCP_First_Name = model.HCPFirstName;
                                        lmModel.HCP_Last_Name = model.HCPLastName;
                                        sLMModel.Add(lmModel);
                                        TempData["fileNAme"] += obj.FileName + "+";
                                    }
                                }

                                model.HCPs.Add(new PAFHCPViewModel
                                {
                                    Type = Convert.ToByte(model.SelectedTypeofHCP[1]),
                                    First_Name = model.HCPFirstName,
                                    Last_Name = model.HCPLastName,
                                    TMF_No = model.TMFNo,
                                    AttachedFile = TempData["fileNAme"].ToString(),
                                    HCPtopic = model.HCPtopicNoH
                                });
                            }
                            else { TempData["SameUser"] = "*HCP has been already selected, Choose another"; }
                        }
                        else
                        {
                            if (model.HCPType == "Hon" && (string.IsNullOrEmpty(model.SelectedTypeofHCP[0]) || string.IsNullOrEmpty(model.SelectedNameoftheHCP[0]) || model.HCPCost == 0))
                            {
                                if (string.IsNullOrEmpty(model.SelectedTypeofHCP[0])) { TempData["eHCPType"] = "*Please Select HCP Type"; }
                                if (string.IsNullOrEmpty(model.SelectedNameoftheHCP[0])) { TempData["eHCPName"] = "*Please Select HCP Name"; }
                                if (model.HCPCost == 0) { TempData["eHCPNCost"] = "*Please Select Honorarium payable to HCP"; }

                            }
                            else if (model.HCPType == "NoHon" && (string.IsNullOrEmpty(model.SelectedTypeofHCP[1]) || string.IsNullOrEmpty(model.HCPFirstName) || string.IsNullOrEmpty(model.HCPLastName) || model.NOHImageFile == null || model.NOHImageFile[0] == null))
                            {
                                if (string.IsNullOrEmpty(model.SelectedTypeofHCP[1])) { TempData["eHCPNType"] = "*Please Select HCP Type"; }
                                if (string.IsNullOrEmpty(model.HCPFirstName)) { TempData["eHCPNFName"] = "*Please Select HCP First Name"; }
                                if (string.IsNullOrEmpty(model.HCPLastName)) { TempData["eHCPNLName"] = "*Please Select HCP Last Name"; }
                                if (model.NOHImageFile == null || model.NOHImageFile[0] == null) { TempData["eHCPNfile"] = "*Please attach file"; }
                            }
                        }
                        TempData.Remove("PAFHCPViewModel");
                        TempData.Add("PAFHCPViewModel", model.HCPs);
                        model.Miscellaneous = TempData["PAFMiscellaneous"] as List<PAFMiscellaneous>;
                        model.Participants = TempData["PAFParticipants"] as List<PAFParticipants>;
                        TempData["TabDisplay"] = "Valid1";
                    }
                    #endregion

                    #region Add Participants
                    if (command == "Add Participants")
                    {
                        if ((model.SelectedBrand[0] != "" && model.SelectedEvent[0] != "") || (model.ActivityName != null))
                        {
                            var participantlist = JsonConvert.DeserializeObject<List<PAFParticipants>>(model.JsonParticipantsdata);
                            if (participantlist == null)
                            {

                                TempData["Pname"] = "* Please Enter Name";
                                TempData["pcity"] = "* Please Enter City";
                                TempData["pcustomerid"] = "* Please Enter Customer id";
                            }
                            else
                            {
                                if (model.Participants == null)
                                {
                                    model.Participants = participantlist;
                                }
                                else
                                {
                                    model.Participants.AddRange(participantlist);
                                }

                                TempData.Remove("PAFParticipants");
                                TempData.Add("PAFParticipants", model.Participants);
                                model.HCPs = TempData["PAFHCPViewModel"] as List<PAFHCPViewModel>;
                                model.Miscellaneous = TempData["PAFMiscellaneous"] as List<PAFMiscellaneous>;
                            }
                        }
                        else
                        {
                            if (model.SelectedTherapyArea[0] == "" && Session["LoginUserZone"].ToString() != "Rare Disease")
                            { TempData["sTherapyArea"] = "* Please select Therapy Area"; }
                            TempData["sBrandArea"] = "* Please select Brand";
                            TempData["sEventArea"] = "* Please select Event";
                            TempData["sLectureArea"] = "* Please select Lecture";
                        }
                        TempData["TabDisplay"] = "Valid1";
                    }
                    #endregion

                    #region RMP
                    if (command.Substring(0, 3) == "RMP")
                    {
                        model.Participants = TempData["PAFParticipants"] as List<PAFParticipants>;
                        if (command.Substring(3, 2) == "--")
                        {
                            var startindex = 5;
                            var endindex = command.IndexOf("-", startindex);
                            var length = endindex - startindex;
                            string mis = command.Substring(startindex, length);
                            String[] spearator = mis.Split('+');
                            string Name = spearator[0];
                            string Emailid = spearator[1];
                            model.Participants.RemoveAll(s => s.Name == Name && s.Email_Id == Emailid);
                        }
                        else
                        {
                            var startindex = 4;
                            var endindex = command.IndexOf("-", startindex);
                            var length = endindex - startindex;
                            string HCPID = command.Substring(startindex, length);
                            model.Participants.RemoveAll(s => s.ID == Int32.Parse(HCPID));
                        }

                        TempData.Remove("PAFParticipants");
                        TempData.Add("PAFParticipants", model.Participants);
                        model.HCPs = TempData["PAFHCPViewModel"] as List<PAFHCPViewModel>;
                        model.Miscellaneous = TempData["PAFMiscellaneous"] as List<PAFMiscellaneous>;
                        TempData["TabDisplay"] = "Valid1";
                    }
                    #endregion

                    #region RMHCP
                    if (command.Substring(0, 5) == "RMHCP")
                    {
                        model.HCPs = TempData["PAFHCPViewModel"] as List<PAFHCPViewModel>;
                        if (command.Substring(5, 2) == "--")
                        {
                            string mis = command.Substring(7, command.Length - 7);
                            String[] spearator = mis.Split('+');
                            string Name = spearator[0];

                            var nohlist = sLMModel.Where(s => s.Type == "NOH" && s.HCP_First_Name == Name).ToList();
                            if (nohlist != null)
                            {
                                foreach (var obj in nohlist)
                                {
                                    var fullPath = Path.Combine(Server.MapPath("~/Documents/PAF/"), obj.SavedDocName);
                                    sModel.IsDeleteFile = true;
                                    var lmModel = sLMModel.Where(s => s.SavedDocName == obj.SavedDocName && s.HCP_First_Name == obj.HCP_First_Name).FirstOrDefault();
                                    sLMModel.Remove(lmModel);
                                    cmeController.RemoveFileNOH(id, obj.SavedDocName, obj.Type, obj.HCP_First_Name);

                                    if (System.IO.File.Exists(fullPath))
                                    {
                                        System.IO.File.Delete(fullPath);
                                    }
                                }
                            }

                            model.HCPs.RemoveAll(s => s.First_Name == Name);
                        }
                        else
                        {
                            string pan = command.Substring(6, 10);
                            string mis = command.Substring(17);
                            String[] spearator = mis.Split('+');
                            string Name = spearator[0];
                            var nohlist = sLMModel.Where(s => s.Type == "NOH" && s.HCP_First_Name == Name).ToList();
                            if (nohlist != null)
                            {
                                foreach (var obj in nohlist)
                                {
                                    var fullPath = Path.Combine(Server.MapPath("~/Documents/PAF/"), obj.SavedDocName);
                                    sModel.IsDeleteFile = true;
                                    var lmModel = sLMModel.Where(s => s.SavedDocName == obj.SavedDocName && s.HCP_First_Name == obj.HCP_First_Name).FirstOrDefault();
                                    sLMModel.Remove(lmModel);
                                    cmeController.RemoveFileNOH(id, obj.SavedDocName, obj.Type, obj.HCP_First_Name);
                                    if (System.IO.File.Exists(fullPath))
                                    {
                                        System.IO.File.Delete(fullPath);
                                    }
                                }
                            }
                            if (pan == "")
                            { model.HCPs.RemoveAll(s => s.First_Name == Name); }
                            //else
                            //{ model.HCPs.RemoveAll(s => s.PAN == pan); }
                        }

                        TempData.Remove("PAFHCPViewModel");
                        TempData.Add("PAFHCPViewModel", model.HCPs);
                        model.ImagePath = sLMModel;
                        model.Miscellaneous = TempData["PAFMiscellaneous"] as List<PAFMiscellaneous>;
                        model.Participants = TempData["PAFParticipants"] as List<PAFParticipants>;
                        TempData["TabDisplay"] = "Valid1";
                    }
                    #endregion

                    #region Add Miscellaneous
                    if (command == "Add Miscellaneous")
                    {
                        model.Miscellaneous = TempData["PAFMiscellaneous"] as List<PAFMiscellaneous>;
                        if (model.Miscellaneous == null)
                        { model.Miscellaneous = new List<PAFMiscellaneous>(); }
                        if (!string.IsNullOrEmpty(model.MName))
                        {
                            if (model.Miscellaneous.SingleOrDefault(s => s.Name == model.MName) == null)
                            {
                                model.Miscellaneous.Add(new PAFMiscellaneous { Name = model.MName, Cost = model.MCost });
                                model.MName = string.Empty;
                                model.MCost = 0;
                            }
                        }
                        TempData.Remove("PAFMiscellaneous");
                        TempData.Add("PAFMiscellaneous", model.Miscellaneous);
                        model.HCPs = TempData["PAFHCPViewModel"] as List<PAFHCPViewModel>;
                        model.Participants = TempData["PAFParticipants"] as List<PAFParticipants>;
                        TempData["TabDisplay"] = "Valid2";
                    }
                    #endregion

                    #region RMMis
                    if (command.Substring(0, 5) == "RMMis")
                    {
                        model.HCPs = TempData["PAFHCPViewModel"] as List<PAFHCPViewModel>;
                        model.Miscellaneous = TempData["PAFMiscellaneous"] as List<PAFMiscellaneous>;
                        string mis = command.Substring(6, command.Length - 6);
                        String[] spearator = mis.Split('+');
                        model.Miscellaneous.RemoveAll(s => s.Name == spearator[0] && s.Cost.Equals(decimal.Parse(spearator[1])));
                        TempData.Remove("PAFMiscellaneous");
                        TempData.Add("PAFMiscellaneous", model.Miscellaneous);
                        model.HCPs = TempData["PAFHCPViewModel"] as List<PAFHCPViewModel>;
                        model.Participants = TempData["PAFParticipants"] as List<PAFParticipants>;
                        TempData["TabDisplay"] = "Valid2";
                    }
                    #endregion

                    TempData.Keep("PAFMiscellaneous");
                    TempData.Keep("PAFHCPViewModel");
                    TempData.Keep("PAFParticipants");
                    return View(model);
                }
                #endregion                
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult RecallPAF(int id)
        {
            try
            {
                var cmeController = new CMEController();
                var cme = _cMERepository.GetPAFById(id);
                if (cme.RequestType == "Send" || (cme.RequestType == "Approved by RBD") || cme.RequestType == "Approved by SM" || cme.RequestType == "Send to RBD")
                {
                    cmeController.RecallPAF(id, new CMEDetails
                    {
                        IssuedBy = Convert.ToInt32(Session["LoginUserId"]),
                        Issuer_Role = Convert.ToString(Session["LoginUserRole"])
                    });

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return this.RedirectToAction("Status", new { id });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Change(int id = 0)
        {
            try
            {
                var model = new CMEViewModel();
                Session.Remove("CMEFullDetail");
                Session.Remove("PAFLMList");
                TempData.Remove("PAFRequestType"); TempData.Remove("PAFTherapyArea"); TempData.Remove("PAFTherapyAreaName"); TempData.Remove("PAFBrand"); TempData.Remove("PAFEvent"); TempData.Remove("PAFLecture"); TempData.Remove("PAFTypeofHCP"); TempData.Remove("PAFCity"); TempData.Remove("PAFNameoftheHCP"); TempData.Remove("PAFHCPViewModel"); TempData.Remove("PAFMiscellaneous"); TempData.Remove("PAFMeetingType"); TempData.Remove("PAFAssessment"); TempData.Remove("PAFParticipants");

                var cme = _cMERepository.GetPAFById(id);
                var cmeDetails = _cMERepository.GetById(id);
                var userDetail = _userRepository.GetUserByInitial(Session["LoginUserInitial"].ToString());
                Session["PAFChar"] = userDetail.TherapyArea.ToString().Substring(0, 1);
                string userInitials = cme.IssuedByInitial;
                if (cme == null) { return RedirectToAction("Index"); }
                else
                {


                    model.ID = cme.ID;
                    model.IssuedByInitial = cme.IssuedByInitial;
                    model.ManagerInitial = Convert.ToString(Session["LoginUserManager"].ToString());
                    model.ActivityName = cme.ActivityName;
                    model.Approver = cme.Approver;
                    model.ActivityStartDate = cme.ActivityStartDate.ToString("dd-MM-yyyy");
                    model.ActivityEndDate = cme.ActivityEndDate.ToString("dd-MM-yyyy");
                    model.CME_Type = cme.CME_Type;
                    model.DiseaseArea_Type = cme.DiseaseArea_Type;
                    model.IssuerZone = cme.IssuerZone;
                    model.NumberofDays = cme.NumberofDays;
                    model.ChangeStartDate = (cme.ChangeStartDate != null) ? Convert.ToDateTime(cme.ChangeStartDate).ToString("dd-MM-yyyy") : null;
                    model.ChangeEndDate = (cme.ChangeEndDate != null) ? Convert.ToDateTime(cme.ChangeEndDate).ToString("dd-MM-yyyy") : null;
                    model.Proposednumberofattendee = cme.Proposednumberofattendee;
                    model.Roomrate = Math.Round(cme.Roomrate, 2);
                    model.Banquetroomrate = Math.Round(cme.Banquetroomrate, 2);
                    model.CostPerPAX = Math.Round(cme.CostPerPAX, 2);
                    model.Minimumnumberofgaurantee = cme.Minimumnumberofgaurantee;
                    model.Approximatecost = cme.Approximatecost;
                    model.Venu_Hotel = cme.Venu_Hotel;
                    model.Hotel_Star = cme.Hotel_Star;
                    model.LectureId = cme.LectureId;

                    var value = ConfigurationManager.AppSettings["DateKey"];
                    var Finaldate = DateTime.Parse(value);
                    var ActivityStartDate = cmeDetails.ModifiedDate;

                    if (cme.IssuerZone == "Rare Disease")
                    {
                        if (ActivityStartDate >= Finaldate)
                        {
                            model.TherapyArea = _commonRepository.GetTherapyArea().Where(a => a.Name == "Rare Disease").Select(s => new SelectListItem { Text = s.Name.ToString(), Value = Convert.ToString(s.ID), Selected = (Convert.ToString(s.ID) == Convert.ToString(cme.TherapyAreaId)) }).ToList();
                        }
                        else
                        {
                            model.TherapyArea = _commonRepository.GetTherapyArea().Where(a => (a.Name == "Biopharm" || a.Name == "Rare Disease")).Select(s => new SelectListItem { Text = s.Name.ToString(), Value = Convert.ToString(s.ID), Selected = (Convert.ToString(s.ID) == Convert.ToString(cme.TherapyAreaId)) }).ToList();

                        }
                        model.Brand = _commonRepository.GetBrands().Select(s => new SelectListItem { Text = s.Name.ToString(), Value = Convert.ToString(s.ID), Selected = (Convert.ToString(s.ID) == Convert.ToString(cme.BrandId)) }).ToList();
                        model.Event = _lecturesRepository.GetBiopharmEvent(cme.BrandId).Select(s => new SelectListItem { Text = s.Name.ToString(), Value = Convert.ToString(s.Id), Selected = (Convert.ToString(s.Id) == Convert.ToString(cme.EventId)) }).ToList();
                        model.Lectures = _lecturesRepository.GetLectures(Convert.ToByte(cme.EventId)).Select(s => new SelectListItem { Text = s.Lecture.ToString(), Value = Convert.ToString(s.ID), Selected = (Convert.ToString(s.ID) == Convert.ToString(cme.LectureId)) }).ToList();
                    }
                    else
                    {
                        if (ActivityStartDate >= Finaldate)
                        {
                            model.TherapyArea = _commonRepository.GetTherapyArea().Where(a => a.Name != "Biopharm").Select(s => new SelectListItem { Text = s.Name.ToString(), Value = Convert.ToString(s.ID), Selected = (Convert.ToString(s.ID) == Convert.ToString(cme.TherapyAreaId)) }).ToList();
                        }
                        else
                        {
                            model.TherapyArea = _commonRepository.GetTherapyArea().Select(s => new SelectListItem { Text = s.Name.ToString(), Value = Convert.ToString(s.ID), Selected = (Convert.ToString(s.ID) == Convert.ToString(cme.TherapyAreaId)) }).ToList();
                        }
                        model.Brand = _commonRepository.GetBrands().Select(s => new SelectListItem { Text = s.Name.ToString(), Value = Convert.ToString(s.ID), Selected = (Convert.ToString(s.ID) == Convert.ToString(cme.BrandId)) }).ToList();
                        model.Event = _lecturesRepository.GetBiopharmEvent(cme.BrandId).Select(s => new SelectListItem { Text = s.Name.ToString(), Value = Convert.ToString(s.Id), Selected = (Convert.ToString(s.Id) == Convert.ToString(cme.EventId)) }).ToList();
                        model.Lectures = _lecturesRepository.GetLectures(Convert.ToByte(cme.EventId)).Select(s => new SelectListItem { Text = s.Lecture.ToString(), Value = Convert.ToString(s.ID), Selected = (Convert.ToString(s.ID) == Convert.ToString(cme.LectureId)) }).ToList();
                    }
                    if (model.LectureId == null)
                    {
                        model.LMFiles = _lecturesRepository.GetMaterialsByEventId(Convert.ToByte(id)).ToList();
                    }
                    else
                    {
                        model.LMFiles = _lecturesRepository.GetMaterialsById(Convert.ToByte(id)).ToList(); ;
                    }
                    model.City = cme.City;
                    model.TypeofHCP = _hCPListRepository.GetTypeOfHCP().Select(z => new SelectListItem { Text = z.Name.ToString(), Value = z.Id.ToString() }).OrderBy(s => s.Text).ToList();
                    //model.NameoftheHCP = _hCPListRepository.GetAllZone().Select(s => new SelectListItem { Text = s.FirstName.ToString() + s.LastName, Value = Convert.ToString(s.ID) }).ToList();
                    model.MeetingType = _commonRepository.GetPafMeetingtype().Select(s => new SelectListItem { Text = s.Name.ToString(), Value = Convert.ToString(s.Id), Selected = (Convert.ToString(s.Id) == Convert.ToString(cme.MeetingTypeId)) }).ToList();
                    model.Miscellaneous = cme.Miscellaneous;
                    model.HCPs = cmeDetails.HCPs;
                    model.Participants = cme.Participants;
                    if (cme.Comments.Where(s => s.Initial == userInitials).Count() > 0)
                    {
                        model.Comment = (cme.Comments.Count > 0) ? cme.Comments.Where(s => s.Initial == userInitials).OrderByDescending(o => o.Id).FirstOrDefault().Comment : "";
                    }
                    model.Comments = cme.Comments;
                    model.RequestType = cme.RequestType;
                    model.IsDateDeviation = cme.IsDateDeviation;
                    model.IsDeviationApproved = cme.IsDeviationApproved;
                    model.ImagePath = cme.ImagePath;
                    model.HospitalMeeting = cme.HospitalMeeting;
                    if (cme.IsVirtual == null)
                    {
                        model.IsVirtual = false;
                    }
                    else
                    {
                        model.IsVirtual = (bool)cme.IsVirtual;
                    }
                    model.IsPostPoned = cme.IsPostPoned;
                    model.IsBTC = cmeDetails.IsBTC;
                    if (cme.Policy_Violation == null)
                    {
                        model.Policy_Violation = false;
                    }
                    else
                    {
                        model.Policy_Violation = (bool)cme.Policy_Violation;
                    }
                    var amsConfigDetail = _configRepository.GetAMSconfig().Select(s => new SelectListItem { Text = s.Name, Value = s.Value }).ToList();
                    foreach (var i in amsConfigDetail)
                    {
                        if (i.Text == "Hospital_meeting_min_days") { model.HMinDays = Int32.Parse(i.Value); }
                        else if (i.Text == "Hospital_meeting_max_days") { model.HMaxDays = Int32.Parse(i.Value); }
                        else if (i.Text == "Diabetes_meeting_min_days") { model.DMinDays = Int32.Parse(i.Value); }
                        else if (i.Text == "Diabetes_meeting_max_days") { model.DMaxDays = Int32.Parse(i.Value); }
                    }
                    model.HCPSJson = JsonConvert.SerializeObject(model.HCPs);
                    model.IsDeleteFile = false;

                    TempData.Add("PAFTherapyArea", model.TherapyArea);
                    TempData.Add("PAFTherapyAreaName", model.TherapyAreaName);
                    TempData.Add("PAFBrand", model.Brand);
                    TempData.Add("PAFEvent", model.Event);
                    TempData.Add("PAFLecture", model.Lectures);
                    TempData.Add("PAFCity", model.City);
                    TempData.Add("PAFTypeofHCP", model.TypeofHCP);
                    TempData.Add("PAFNameoftheHCP", model.NameoftheHCP);
                    TempData.Add("PAFMeetingType", model.MeetingType);
                    TempData.Add("PAFMiscellaneous", cme.Miscellaneous);
                    TempData.Add("PAFHCPViewModel", cmeDetails.HCPs);
                    TempData.Add("PAFParticipants", cme.Participants);
                    TempData.Add("PAFRequestType", cme.RequestType);
                    Session.Add("CMEFullDetail", model);
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Updating CME details
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        // POST: CME/Change/5
        [HttpPost]
        public ActionResult Change(int id, CMEViewModel model, string command)
        {
            try
            {
                var cmeController = new CMEController();
                int i = 0;
                var cme = _cMERepository.GetPAFById(id);
                var cmeDetails = _cMERepository.GetById(id);
                var sModel = (CMEViewModel)Session["CMEFullDetail"];
                var sLMModel = (List<LectureMaterials>)sModel.ImagePath ?? new List<LectureMaterials>();
                sModel.ImagePath = sLMModel;
                model.ImagePath = sLMModel;
                Session.Remove("CMEFullDetail");
                Session.Add("CMEFullDetail", sModel);
                model.RequestType = TempData["PAFRequestType"].ToString();
                TempData.Keep("PAFRequestType");
                model.LMFiles = (List<LectureMaterials>)Session["PAFLMList"];
                model.LectureId = cme.LectureId;
                var emptyList = new List<SelectListItem>();
                string[] therapyAreaConfig = ConfigurationManager.AppSettings["TherapyArea"].Split(';');

                var amsConfigDetail = _configRepository.GetAMSconfig().Select(s => new SelectListItem { Text = s.Name, Value = s.Value }).ToList();
                foreach (var a in amsConfigDetail)
                {
                    if (a.Text == "Hospital_meeting_min_days") { model.HMinDays = Int32.Parse(a.Value); }
                    else if (a.Text == "Hospital_meeting_max_days") { model.HMaxDays = Int32.Parse(a.Value); }
                    else if (a.Text == "Diabetes_meeting_min_days") { model.DMinDays = Int32.Parse(a.Value); }
                    else if (a.Text == "Diabetes_meeting_max_days") { model.DMaxDays = Int32.Parse(a.Value); }
                }

                model.IssuedByInitial = sModel.IssuedByInitial;
                model.Approver = sModel.Approver;

                #region CostNext
                if (command == "CostNext")
                {
                    model.HCPs = TempData["PAFHCPViewModel"] as List<PAFHCPViewModel>;
                    model.Participants = TempData["PAFParticipants"] as List<PAFParticipants>;
                    model.Miscellaneous = TempData["PAFMiscellaneous"] as List<PAFMiscellaneous>;
                    TempData["TabDisplay"] = "Valid3";
                }
                #endregion

                #region HCPNext
                if (command == "HCPNext" || command.Contains("HCPNext"))
                {
                    model.HCPs = TempData["PAFHCPViewModel"] as List<PAFHCPViewModel>;

                    #region 
                    string[] parts = command.Split('-');
                    string topic = parts[1];
                    string cost = parts[2];
                    string hcpName = parts[3];

                    bool isSpeaker = model.HCPs.Select(item => item.TypeName == "Speaker").FirstOrDefault();

                    if (topic == "" && isSpeaker)
                    {
                        TempData["vsaveTopic"] = "* Please enter topic.";
                        TempData["TabDisplay"] = "Valid2";
                    }
                    else
                    {
                        model.HCPtopic = topic;
                        model.HCPCost = Convert.ToDecimal(cost);
                        model.HCPFirstName = hcpName;

                        model.HCPs.Where(item => item.First_Name == model.HCPFirstName).ToList().ForEach(item =>
                        {
                            item.HCPtopic = model.HCPtopic;
                            item.Cost = model.HCPCost;
                        });

                        TempData.Remove("PAFHCPViewModel");
                        TempData.Add("PAFHCPViewModel", model.HCPs);
                        TempData["TabDisplay"] = "Valid1";
                    }

                    #endregion

                    model.Participants = TempData["PAFParticipants"] as List<PAFParticipants>;
                    model.Miscellaneous = TempData["PAFMiscellaneous"] as List<PAFMiscellaneous>;
                    if ((model.HCPs != null && model.HCPs.Count == 0) || model.HCPs == null)
                    {
                        model.TherapyArea = TempData["PAFTherapyArea"] as IEnumerable<SelectListItem>;
                        TempData.Keep("PAFTherapyArea");
                        if (therapyAreaConfig.Contains(Session["LoginUserZone"].ToString()))
                        {
                            model.TherapyAreaName = TempData["PAFTherapyAreaName"] as String;
                            TempData.Keep("PAFTherapyAreaName");
                        }

                        model.Brand = TempData["PAFBrand"] as IEnumerable<SelectListItem>;
                        TempData.Keep("PAFBrand");
                        model.Event = TempData["PAFEvent"] as IEnumerable<SelectListItem>;
                        TempData.Keep("PAFEvent");
                        model.Lectures = TempData["PAFLecture"] as IEnumerable<SelectListItem>;
                        TempData.Keep("PAFLecture");
                        TempData.Keep("PAFCity");
                        model.TypeofHCP = TempData["PAFTypeofHCP"] as IEnumerable<SelectListItem>;
                        TempData.Keep("PAFTypeofHCP");
                        model.NameoftheHCP = TempData["PAFNameoftheHCP"] as IEnumerable<SelectListItem>;
                        TempData.Keep("PAFNameoftheHCP");
                        model.MeetingType = TempData["PAFMeetingType"] as IEnumerable<SelectListItem>;
                        TempData.Keep("PAFMeetingType");

                        if (!therapyAreaConfig.Contains(Session["LoginUserZone"].ToString()))
                        {
                            if (model.SelectedTherapyArea[0] == "") { TempData["sTherapyArea"] = "* Please select Therapy Area"; }
                        }
                        if (model.SelectedBrand[0] == "") { TempData["sBrandArea"] = "* Please select Brand"; }
                        if (model.SelectedEvent[0] == "") { TempData["sEventArea"] = "* Please select Event"; }
                        if (model.SelectedLecture[0] == "") { TempData["sLecture"] = "* Please select Lecture"; }
                        if (model.Minimumnumberofgaurantee <= 0) { TempData["Minnuofgu"] = "* Please select a valid minimum number of guarantee"; }
                        if ((model.HCPs != null && model.HCPs.Count == 0) || model.HCPs == null) { TempData["eAddHCP"] = "*Please add HCP"; }
                        if (model.SelectedMeetingType[0] == "") { TempData["sMeetingType"] = "* Please select Meeting Type"; }
                        if (model.IsBTC == null && model.IsVirtual == false) { TempData["sBTC"] = "* Please select BTC"; }

                        model.Miscellaneous = TempData["PAFMiscellaneous"] as List<PAFMiscellaneous>;
                        TempData.Remove("PAFMiscellaneous");
                        TempData.Add("PAFMiscellaneous", model.Miscellaneous);
                        TempData.Keep("PAFMiscellaneous");

                        model.HCPs = TempData["PAFHCPViewModel"] as List<PAFHCPViewModel>;
                        TempData.Remove("PAFHCPViewModel");
                        TempData.Add("PAFHCPViewModel", model.HCPs);
                        TempData.Keep("PAFHCPViewModel");

                        model.Participants = TempData["PAFParticipants"] as List<PAFParticipants>;
                        TempData.Remove("PAFParticipants");
                        TempData.Add("PAFParticipants", model.Participants);
                        TempData.Keep("PAFParticipants");

                        TempData["TabDisplay"] = "Valid1";

                        return View(model);
                    }
                    else
                    {
                        TempData["TabDisplay"] = "Valid2";
                    }
                }
                #endregion

                #region SVHCP
                if (command.Substring(0, 5) == "SVHCP")
                {
                    model.HCPs = (List<PAFHCPViewModel>)TempData["PAFHCPViewModel"];
                    string[] parts = command.Split('-');

                    string topic = parts[1];
                    string cost = parts[2];
                    string hcpName = parts[3];

                    model.HCPtopic = topic;
                    model.HCPCost = Convert.ToDecimal(cost);
                    model.HCPFirstName = hcpName;

                    model.HCPs.Where(item => item.First_Name == model.HCPFirstName).ToList().ForEach(item =>
                    {
                        item.HCPtopic = model.HCPtopic;
                        item.Cost = model.HCPCost;
                    });

                    TempData.Remove("PAFHCPViewModel");
                    TempData.Add("PAFHCPViewModel", model.HCPs);
                    TempData["TabDisplay"] = "Valid1";
                }
                #endregion

                #region DetailsNext
                if (command == "DetailsNext")
                {
                    if (model.SelectedEvent == null && model.CME_Type == "Non-Standard Meeting")
                    {
                        model.SelectedEvent = new List<string>
                        {
                            ""
                        };
                    }
                    if (model.SelectedLecture == null && model.CME_Type == "Non-Standard Meeting")
                    {
                        model.SelectedLecture = new List<string>
                        {
                            ""
                        };
                    }
                    if (Convert.ToDateTime(model.ActivityStartDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat) > Convert.ToDateTime(model.ActivityEndDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat)
                        || Convert.ToDateTime(model.ChangeStartDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat) > Convert.ToDateTime(model.ChangeEndDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat)
                        || model.Minimumnumberofgaurantee > model.Proposednumberofattendee
                        || (model.CME_Type == "Non-Standard Meeting" && string.IsNullOrEmpty(model.ActivityName))
                        || model.SelectedTherapyArea[0] == "" && Session["LoginUserZone"].ToString() != "Rare Disease"
                        || (model.SelectedBrand[0] == "" && model.ActivityName == null)
                        || (model.SelectedEvent[0] == "" && model.ActivityName == null)
                        || (model.SelectedLecture[0] == "" && model.ActivityName == null)
                        || model.Minimumnumberofgaurantee <= 0
                        || model.Minimumnumberofgaurantee == null
                        //|| userData.Status == false
                        || (model.IsBTC == null && model.IsVirtual == false)
                        || model.SelectedMeetingType[0] == "")
                    {
                        model.TherapyArea = TempData["PAFTherapyArea"] as IEnumerable<SelectListItem>;
                        TempData.Keep("PAFTherapyArea");
                        if (therapyAreaConfig.Contains(Session["LoginUserZone"].ToString()))
                        {
                            model.TherapyAreaName = TempData["PAFTherapyAreaName"] as String;
                            TempData.Keep("PAFTherapyAreaName");
                        }

                        model.Brand = TempData["PAFBrand"] as IEnumerable<SelectListItem>;
                        TempData.Keep("PAFBrand");
                        model.Event = TempData["PAFEvent"] as IEnumerable<SelectListItem>;
                        TempData.Keep("PAFEvent");
                        model.Lectures = TempData["PAFLecture"] as IEnumerable<SelectListItem>;
                        TempData.Keep("PAFLecture");
                        //added to avoid error
                        model.Lectures = emptyList;
                        TempData.Keep("PAFCity");
                        model.TypeofHCP = TempData["PAFTypeofHCP"] as IEnumerable<SelectListItem>;
                        TempData.Keep("PAFTypeofHCP");
                        model.NameoftheHCP = TempData["PAFNameoftheHCP"] as IEnumerable<SelectListItem>;
                        TempData.Keep("PAFNameoftheHCP");
                        model.MeetingType = TempData["PAFMeetingType"] as IEnumerable<SelectListItem>;
                        TempData.Keep("PAFMeetingType");

                        if (!therapyAreaConfig.Contains(Session["LoginUserZone"].ToString()))
                        {
                            if (model.SelectedTherapyArea[0] == "") { TempData["sTherapyArea"] = "* Please select Therapy Area"; }
                        }
                        if (model.SelectedBrand[0] == "") { TempData["sBrandArea"] = "* Please select Brand"; }
                        if (model.SelectedEvent[0] == "") { TempData["sEventArea"] = "* Please select Event"; }
                        if (model.SelectedLecture[0] == "") { TempData["sLecture"] = "* Please select Lecture"; }
                        if (model.Minimumnumberofgaurantee <= 0) { TempData["Minnuofgu"] = "* Please select a valid minimum number of guarantee"; }
                        if (model.SelectedMeetingType[0] == "") { TempData["sMeetingType"] = "* Please select Meeting Type"; }
                        if (model.IsBTC == null && model.IsVirtual == false) { TempData["sBTC"] = "* Please select BTC"; }

                        model.Miscellaneous = TempData["PAFMiscellaneous"] as List<PAFMiscellaneous>;
                        TempData.Remove("PAFMiscellaneous");
                        TempData.Add("PAFMiscellaneous", model.Miscellaneous);
                        TempData.Keep("PAFMiscellaneous");

                        model.HCPs = TempData["PAFHCPViewModel"] as List<PAFHCPViewModel>;
                        TempData.Remove("PAFHCPViewModel");
                        TempData.Add("PAFHCPViewModel", model.HCPs);
                        TempData.Keep("PAFHCPViewModel");

                        model.Participants = TempData["PAFParticipants"] as List<PAFParticipants>;
                        TempData.Remove("PAFParticipants");
                        TempData.Add("PAFParticipants", model.Participants);
                        TempData.Keep("PAFParticipants");

                        return View(model);
                    }
                    else
                    {
                        model.TherapyArea = TempData["PAFTherapyArea"] as IEnumerable<SelectListItem>;
                        TempData.Keep("PAFTherapyArea");
                        if (therapyAreaConfig.Contains(Session["LoginUserZone"].ToString()))
                        {
                            model.TherapyAreaName = TempData["PAFTherapyAreaName"] as String;
                            TempData.Keep("PAFTherapyAreaName");
                        }

                        model.Brand = TempData["PAFBrand"] as IEnumerable<SelectListItem>;
                        TempData.Keep("PAFBrand");
                        model.Event = TempData["PAFEvent"] as IEnumerable<SelectListItem>;
                        TempData.Keep("PAFEvent");
                        model.Lectures = TempData["PAFLecture"] as IEnumerable<SelectListItem>;
                        TempData.Keep("PAFLecture");
                        //added to avoid error
                        if (model.Lectures == null)
                        {
                            model.Lectures = emptyList;
                        }

                        TempData.Keep("PAFCity");
                        model.TypeofHCP = TempData["PAFTypeofHCP"] as IEnumerable<SelectListItem>;
                        TempData.Keep("PAFTypeofHCP");
                        model.NameoftheHCP = TempData["PAFNameoftheHCP"] as IEnumerable<SelectListItem>;
                        TempData.Keep("PAFNameoftheHCP");
                        model.MeetingType = TempData["PAFMeetingType"] as IEnumerable<SelectListItem>;
                        TempData.Keep("PAFMeetingType");

                        var cmeModel = new CMEModel
                        {
                            PAFType = command,
                            IssuedBy = Convert.ToInt32(Session["LoginUserId"]),
                            ManagerInitial = Convert.ToString(Session["LoginUserManager"].ToString()),
                            CME_Type = model.CME_Type
                        };
                        if (cmeModel.CME_Type == "Non-Standard Meeting" && Session["LoginUserZone"].ToString() == "Rare Disease")
                        {
                            cmeModel.DiseaseArea_Type = model.DiseaseArea_Type;
                        }
                        else
                        {
                            cmeModel.DiseaseArea_Type = null;
                        }
                        cmeModel.IssuedDate = Convert.ToDateTime(DateTime.Now, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                        cmeModel.ActivityName = model.ActivityName;
                        cmeModel.TherapyAreaId = Convert.ToByte(model.SelectedTherapyArea[0]);

                        if (model.SelectedBrand[0] != "" && model.SelectedBrand.Count > 0)
                        {
                            cmeModel.BrandId = Byte.Parse(model.SelectedBrand[0]);
                        }
                        if (model.SelectedEvent[0] != "" && model.SelectedEvent.Count > 0)
                        {
                            cmeModel.EventId = int.Parse(model.SelectedEvent[0]);
                        }
                        if (model.SelectedLecture[0] != "" && model.SelectedLecture.Count > 0)
                        {
                            cmeModel.LectureId = int.Parse(model.SelectedLecture[0]);
                        }
                        if (model.SelectedMeetingType[0] != "" && model.SelectedMeetingType.Count > 0)
                        {
                            cmeModel.MeetingTypeId = Byte.Parse(model.SelectedMeetingType[0]);
                        }
                        cmeModel.ActivityStartDate = Convert.ToDateTime(model.ActivityStartDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                        cmeModel.ActivityEndDate = Convert.ToDateTime(model.ActivityEndDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                        cmeModel.NumberofDays = Convert.ToInt32(model.NumberofDays);
                        cmeModel.Proposednumberofattendee = Convert.ToInt32(model.Proposednumberofattendee);
                        cmeModel.Minimumnumberofgaurantee = Convert.ToInt32(model.Minimumnumberofgaurantee);
                        cmeModel.City = model.City;
                        cmeModel.Venu_Hotel = model.Venu_Hotel;
                        cmeModel.Hotel_Star = model.Hotel_Star;
                        cmeModel.HospitalMeeting = model.HospitalMeeting;
                        cmeModel.IsVirtual = model.IsVirtual;
                        cmeModel.IsBTC = model.IsBTC;
                        cmeModel.IsDateDeviation = model.IsDateDeviation;
                        cmeModel.Policy_Violation = model.Policy_Violation;
                        cmeModel.Comment = model.Comment;
                        model.HCPs = sModel.HCPs;
                        model.Participants = sModel.Participants;
                        model.Miscellaneous = cmeModel.Miscellaneous;
                        TempData["TabDisplay"] = "Valid1";
                        return View(model);
                    }
                }
                #endregion


                #region Draft OR Send
                if (command == "Draft" || command == "Send")
                {
                    if (!sModel.IsDeleteFile)
                    {
                        if (model.PAImageFile != null && model.PAImageFile.ContentLength > 0)
                        {
                            if (sLMModel.SingleOrDefault(s => s.Type == "PA") == null)
                            {
                                var lmModel = SaveFile(model.PAImageFile, "PA");
                                sLMModel.Add(lmModel);
                            }
                            else
                            {
                                i = 1;
                                TempData["ePAImageFile"] = "* Please remove then upload Proposed Agenda and FMV";
                            }
                        }
                        if (model.HQImageFile != null && model.HQImageFile.ContentLength > 0)
                        {
                            if (sLMModel.SingleOrDefault(s => s.Type == "HQ") == null)
                            {
                                var lmModel = SaveFile(model.HQImageFile, "HQ");
                                sLMModel.Add(lmModel);
                            }
                            else
                            {
                                i = 1;
                                TempData["eHQImageFile"] = "* Please remove then upload Hotel Quotation";
                            }
                        }

                        if (model.LMImageFile != null && model.LMImageFile[0] != null && sModel.RequestType != "Approved by SM" && sLMModel.Any(s => s.PostType == "PLM") == false)
                        {
                            foreach (var obj in model.LMImageFile)
                            {
                                if (obj.ContentLength > 0)
                                {
                                    var lmModel = SaveFile(obj);
                                    lmModel.PostType = "PLM";
                                    sLMModel.Add(lmModel);
                                }
                            }
                            sModel.LMImageFile = model.LMImageFile;
                        }
                        if (model.CCImageFile != null && model.CCImageFile.ContentLength > 0 && sLMModel.Any(s => s.PostType == "CC") == false)
                        {
                            var lmModel = SaveFile(model.CCImageFile);
                            sModel.CCImageFile = model.CCImageFile;
                            lmModel.PostType = "CC";
                            sLMModel.Add(lmModel);
                        }
                        if (model.ASImageFile != null && model.ASImageFile.ContentLength > 0 && sLMModel.Any(s => s.PostType == "AS") == false)
                        {
                            var lmModel = SaveFile(model.ASImageFile);
                            sModel.ASImageFile = model.ASImageFile;
                            lmModel.PostType = "AS";
                            sLMModel.Add(lmModel);
                        }
                        if (model.PEImageFile != null && model.PEImageFile[0] != null && Session["LoginUserZone"].ToString() == "Rare Disease" && sLMModel.Any(s => s.PostType == "PE") == false)
                        {
                            foreach (var obj in model.PEImageFile)
                            {
                                if (obj.ContentLength > 0)
                                {
                                    var lmModel = SaveFile(obj);
                                    lmModel.PostType = "PE";
                                    sLMModel.Add(lmModel);
                                }
                            }
                            sModel.PEImageFile = model.PEImageFile;
                        }


                        if (cme.IssuerZone.ToString() != "Rare Disease")
                        {
                            if (model.LMImageFile[0] != null && model.LMImageFile[0].ContentLength > 0)
                            {
                                foreach (var obj in model.LMImageFile)
                                {
                                    if (obj.ContentLength > 0)
                                    {
                                        var lmModel = SaveFile(obj, "LM");
                                        sLMModel.Add(lmModel);
                                    }
                                }
                            }
                        }
                        if (cme.IssuerZone.ToString() != "Rare Disease")
                        {
                            if (model.PostLMImageFile[0] != null && model.PostLMImageFile[0].ContentLength > 0)
                            {
                                foreach (var obj in model.PostLMImageFile)
                                {
                                    if (obj.ContentLength > 0)
                                    {
                                        var lmModel = SaveFile(obj, "LM");
                                        sLMModel.Add(lmModel);
                                    }
                                }
                            }
                        }
                        if (model.HERImageFile != null && model.HERImageFile.ContentLength > 0)
                        {
                            if (sLMModel.SingleOrDefault(s => s.Type == "HER") == null)
                            {
                                var lmModel = SaveFile(model.HERImageFile, "HER");
                                sLMModel.Add(lmModel);
                            }
                            else
                            {
                                i = 1;
                                TempData["eHERImageFile"] = "* Please remove then upload Hotel Exception Request Form";
                            }
                        }
                        if (model.ICPImageFile != null && model.ICPImageFile[0] != null)
                        {
                            foreach (var obj in model.ICPImageFile)
                            {
                                if (obj.ContentLength > 0)
                                {
                                    var lmModel = SaveFile(obj);
                                    sLMModel.Add(lmModel);
                                }
                            }
                            sModel.ICPImageFile = model.ICPImageFile;
                        }
                        if (model.PLMImageFile != null && model.PLMImageFile[0] != null)
                        {
                            foreach (var obj in model.PLMImageFile)
                            {
                                var lmModel = SaveFile(obj);
                                sLMModel.Add(lmModel);
                            }
                            sModel.PLMImageFile = model.PLMImageFile;
                        }
                        if (model.CCImageFile != null && model.CCImageFile.ContentLength > 0)
                        {
                            var lmModel = SaveFile(model.CCImageFile);
                            sModel.CCImageFile = model.CCImageFile;
                            sLMModel.Add(lmModel);
                        }
                        if (model.ASImageFile != null && model.ASImageFile.ContentLength > 0)
                        {
                            var lmModel = SaveFile(model.ASImageFile);
                            sModel.ASImageFile = model.ASImageFile;
                            sLMModel.Add(lmModel);
                        }
                        if (model.HCPs != null)
                        {
                            for (int a = 0; a < model.HCPs.Count; a++)
                            {
                                string strFileUpload = "file_" + Convert.ToString(model.HCPs[a].Id);
                                HttpPostedFileBase file = Request.Files[strFileUpload];
                                if (file != null)
                                {

                                    if (file.ContentLength > 0)
                                    {
                                        var lmModel = SaveFile(file);
                                        lmModel.PostType = "IHCP";
                                        sLMModel.Add(lmModel);
                                    }
                                }
                            }
                        }
                    }
                    if (model.LectureId == null)
                    {
                        model.Lectures = emptyList;
                    }
                    if (sModel.IsDeleteFile == true) { sModel.IsDeleteFile = false; }
                    if (Convert.ToDateTime(model.ActivityStartDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat) > Convert.ToDateTime(model.ActivityEndDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat)
                        || Convert.ToDateTime(model.ChangeStartDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat) > Convert.ToDateTime(model.ChangeEndDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat)
                        || model.Minimumnumberofgaurantee > model.Proposednumberofattendee
                        || sLMModel.Where(s => s.Type == "PA").Count() <= 0
                        || (sLMModel.Where(s => s.Type == "HQ").Count() <= 0 && model.IsVirtual == false)
                        || (model.Hotel_Star && sLMModel.Where(s => s.Type == "HER").Count() <= 0)
                        || (model.CME_Type == "Non-Standard Meeting" && string.IsNullOrEmpty(model.ActivityName))
                        || (sModel.HCPs.Count == 0)
                        || model.SelectedTherapyArea[0] == "" && cme.IssuerZone.ToString() != "Rare Disease"
                        || (model.SelectedBrand[0] == "" && model.ActivityName == null)
                        || (model.SelectedEvent[0] == "" && model.ActivityName == null)
                        || (model.SelectedLecture[0] == "" && model.ActivityName == null)
                        || (i == 1)
                        || (DateTime.UtcNow.AddDays(09) >= Convert.ToDateTime(model.ActivityStartDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat) && command == "Send" && model.HospitalMeeting == false && cme.IsDateDeviation == false && model.RequestType == "Draft" && cme.IssuerZone.ToString() != "Rare Disease")
                        || (DateTime.UtcNow.AddDays(04) >= Convert.ToDateTime(model.ActivityStartDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat) && command == "Send" && model.HospitalMeeting == false && cme.IsDateDeviation == false && model.RequestType == "Draft" && cme.IssuerZone.ToString() == "Rare Disease")
                        || (model.IsBTC == null && model.IsVirtual == false)
                        || model.SelectedMeetingType[0] == "")
                    {
                        model.TherapyArea = TempData["PAFTherapyArea"] as IEnumerable<SelectListItem>;
                        TempData.Keep("PAFTherapyArea");
                        model.TherapyAreaName = TempData["PAFTherapyAreaName"] as String;
                        TempData.Keep("PAFTherapyAreaName");
                        model.Brand = TempData["PAFBrand"] as IEnumerable<SelectListItem>;
                        TempData.Keep("PAFBrand");
                        model.Event = TempData["PAFEvent"] as IEnumerable<SelectListItem>;
                        TempData.Keep("PAFEvent");
                        model.Lectures = TempData["PAFLecture"] as IEnumerable<SelectListItem>;
                        TempData.Keep("PAFLecture");
                        model.TypeofHCP = TempData["PAFTypeofHCP"] as IEnumerable<SelectListItem>;
                        TempData.Keep("PAFTypeofHCP");
                        model.NameoftheHCP = TempData["PAFNameoftheHCP"] as IEnumerable<SelectListItem>;
                        TempData.Keep("PAFNameoftheHCP");
                        model.MeetingType = TempData["PAFMeetingType"] as IEnumerable<SelectListItem>;
                        TempData.Keep("PAFMeetingType");

                        if (model.SelectedTherapyArea[0] == "") { TempData["sTherapyArea"] = "* Please select Therapy Area"; }
                        if (model.SelectedBrand[0] == "") { TempData["sBrandArea"] = "* Please select Brand"; }
                        if (model.SelectedEvent[0] == "") { TempData["sEventArea"] = "* Please select Event"; }
                        if (model.SelectedLecture[0] == "") { TempData["sLecture"] = "* Please select Lecture"; }
                        if (model.LectureId != null)
                        {
                            if (model.SelectedLecture[0] == "") { TempData["sLectureArea"] = "* Please select Lecture"; }
                        }
                        //validation for Meeting Type
                        if (model.SelectedMeetingType[0] == "") { TempData["sMeetingType"] = "* Please select Meeting Type"; }
                        //validation for Meeting Type
                        if (model.IsBTC == null && model.IsVirtual == false) { TempData["sBTC"] = "* Please select BTC"; }
                        if (sModel.HCPs.Count == 0)
                        { TempData["eAddNoHCP"] = "*Please add HCP"; }

                        if (DateTime.UtcNow.AddDays(09) >= Convert.ToDateTime(model.ActivityStartDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat) && command == "Send" && model.HospitalMeeting == false && cme.IsDateDeviation == false && model.RequestType == "Draft" && cme.IssuerZone.ToString() != "Rare Disease")
                        {
                            TempData["eSendRestrict"] = "* The submission date is over, Need to be ten days before the Activity.";
                        }
                        if (DateTime.UtcNow.AddDays(04) >= Convert.ToDateTime(model.ActivityStartDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat) && command == "Send" && model.HospitalMeeting == false && cme.IsDateDeviation == false && model.RequestType == "Draft" && cme.IssuerZone.ToString() != "Rare Disease")
                        {
                            TempData["eSendRestrict"] = "* The submission date is over, Need to be five days before the Activity.";
                        }
                        model.Miscellaneous = TempData["PAFMiscellaneous"] as List<PAFMiscellaneous>;
                        TempData.Remove("PAFMiscellaneous");
                        TempData.Add("PAFMiscellaneous", model.Miscellaneous);
                        TempData.Keep("PAFMiscellaneous");

                        model.HCPs = TempData["PAFHCPViewModel"] as List<PAFHCPViewModel>;
                        TempData.Remove("PAFHCPViewModel");
                        TempData.Add("PAFHCPViewModel", model.HCPs);
                        TempData.Keep("PAFHCPViewModel");

                        model.Participants = TempData["PAFParticipants"] as List<PAFParticipants>;
                        TempData.Remove("PAFParticipants");
                        TempData.Add("PAFParticipants", model.Participants);
                        TempData.Keep("PAFParticipants");

                        if (sLMModel.Where(s => s.Type == "PA").Count() <= 0 || (sLMModel.Where(s => s.Type == "HQ").Count() <= 0 && model.IsVirtual == false) || (model.Hotel_Star && sLMModel.Where(s => s.Type == "HER").Count() <= 0))
                        {
                            TempData["TabDisplay"] = "Valid3";
                        }

                        return View(model);
                    }
                    else
                    {
                        var cmeDetail = new CMEModel
                        {
                            PAFType = command,
                            IssuedBy = Convert.ToInt32(Session["LoginUserId"]),
                            ManagerInitial = Convert.ToString(Session["LoginUserManager"].ToString()),
                            Approver = model.Approver,
                            CME_Type = model.CME_Type
                        };
                        if (cmeDetail.CME_Type == "Non-Standard Meeting" && cme.IssuerZone.ToString() == "Rare Disease")
                        {
                            cmeDetail.DiseaseArea_Type = model.DiseaseArea_Type;
                        }
                        else
                        {
                            cmeDetail.DiseaseArea_Type = null;
                        }
                        cmeDetail.ActivityName = model.ActivityName;
                        cmeDetail.TherapyAreaId = Convert.ToByte(model.SelectedTherapyArea[0]);
                        if (model.SelectedBrand[0] != "" && model.SelectedBrand.Count > 0)
                        {
                            cmeDetail.BrandId = Byte.Parse(model.SelectedBrand[0]);
                        }
                        if (model.SelectedEvent[0] != "" && model.SelectedEvent.Count > 0)
                        {
                            cmeDetail.EventId = int.Parse(model.SelectedEvent[0]);
                        }
                        if (model.SelectedLecture[0] != "" && model.SelectedLecture.Count > 0)
                        {
                            cmeDetail.LectureId = int.Parse(model.SelectedLecture[0]);
                        }
                        if (model.SelectedMeetingType[0] != "" && model.SelectedMeetingType.Count > 0)
                        {
                            cmeDetail.MeetingTypeId = Byte.Parse(model.SelectedMeetingType[0]);
                        }
                        cmeDetail.ActivityStartDate = Convert.ToDateTime(model.ActivityStartDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                        cmeDetail.ActivityEndDate = Convert.ToDateTime(model.ActivityEndDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                        cmeDetail.NumberofDays = Convert.ToInt32(model.NumberofDays);
                        cmeDetail.ChangeStartDate = !string.IsNullOrEmpty(model.ChangeStartDate) ? Convert.ToDateTime(model.ChangeStartDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat) : (DateTime?)null;
                        cmeDetail.ChangeEndDate = !string.IsNullOrEmpty(model.ChangeEndDate) ? Convert.ToDateTime(model.ChangeEndDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat) : (DateTime?)null;
                        cmeDetail.Proposednumberofattendee = Convert.ToInt32(model.Proposednumberofattendee);
                        cmeDetail.Minimumnumberofgaurantee = Convert.ToInt32(model.Minimumnumberofgaurantee);
                        cmeDetail.Approximatecost = Convert.ToInt32(model.Approximatecost);
                        cmeDetail.Roomrate = Convert.ToInt32(model.Roomrate);
                        cmeDetail.Banquetroomrate = Convert.ToInt32(model.Banquetroomrate);
                        cmeDetail.CostPerPAX = Convert.ToInt32(model.CostPerPAX);
                        cmeDetail.City = model.City;
                        cmeDetail.Venu_Hotel = model.Venu_Hotel;
                        cmeDetail.Hotel_Star = model.Hotel_Star;
                        cmeDetail.Miscellaneous = TempData["PAFMiscellaneous"] as List<PAFMiscellaneous>;
                        cmeDetail.HCPs = TempData["PAFHCPViewModel"] as List<PAFHCPViewModel>;
                        cmeDetail.Participants = TempData["PAFParticipants"] as List<PAFParticipants>;
                        cmeDetail.Comment = model.Comment;
                        cmeDetail.ImagePath = model.ImagePath;
                        cmeDetail.HospitalMeeting = model.HospitalMeeting;
                        cmeDetail.IsVirtual = model.IsVirtual;
                        cmeDetail.IsBTC = model.IsBTC;
                        cmeDetail.IsDateDeviation = model.IsDateDeviation;
                        cmeDetail.Policy_Violation = model.Policy_Violation;

                        cmeController.Change(id, cmeDetail);
                        TempData.Remove("PAFRequestType"); TempData.Remove("PAFTherapyArea"); TempData.Remove("PAFTherapyAreaName"); TempData.Remove("PAFBrand"); TempData.Remove("PAFEvent"); TempData.Remove("PAFLecture"); TempData.Remove("PAFTypeofHCP"); TempData.Remove("PAFCity"); TempData.Remove("PAFNameoftheHCP"); TempData.Remove("PAFHCPViewModel"); TempData.Remove("PAFMiscellaneous"); TempData.Remove("PAFMeetingType"); TempData.Remove("PAFAssessment"); TempData.Remove("PAFParticipants");
                        return RedirectToAction("Index", "Home");
                    }
                }
                #endregion

                #region Cancel
                else if (command.ToLower() == "cancel")
                {
                    Session.Remove("CMEFullDetail");
                    Session.Remove("sLMModel");
                    TempData.Remove("PAFRequestType"); TempData.Remove("PAFTherapyArea"); TempData.Remove("PAFTherapyAreaName"); TempData.Remove("PAFBrand"); TempData.Remove("PAFEvent"); TempData.Remove("PAFLecture"); TempData.Remove("PAFTypeofHCP"); TempData.Remove("PAFCity"); TempData.Remove("PAFNameoftheHCP"); TempData.Remove("PAFHCPViewModel"); TempData.Remove("PAFMiscellaneous"); TempData.Remove("PAFMeetingType"); TempData.Remove("PAFAssessment"); TempData.Remove("PAFParticipants");
                    return RedirectToAction("Index", "Home");
                }
                #endregion

                #region Delete
                else if (command.ToLower() == "delete")
                {
                    cmeController.Delete(id);
                    return RedirectToAction("Index", "Home");
                }
                #endregion

                else
                {
                    model.TherapyArea = TempData["PAFTherapyArea"] as IEnumerable<SelectListItem>;
                    TempData.Keep("PAFTherapyArea");
                    if (cme.IssuerZone.ToString() == "Rare Disease")
                    {
                        model.TherapyAreaName = TempData["PAFTherapyAreaName"] as String;
                        TempData.Keep("PAFTherapyAreaName");
                    }
                    model.Brand = TempData["PAFBrand"] as IEnumerable<SelectListItem>;
                    TempData.Keep("PAFBrand");
                    model.Event = TempData["PAFEvent"] as IEnumerable<SelectListItem>;
                    TempData.Keep("PAFEvent");
                    model.Lectures = TempData["PAFLecture"] as IEnumerable<SelectListItem>;
                    TempData.Keep("PAFLecture");
                    model.TypeofHCP = TempData["PAFTypeofHCP"] as IEnumerable<SelectListItem>;
                    TempData.Keep("PAFTypeofHCP");
                    model.NameoftheHCP = TempData["PAFNameoftheHCP"] as IEnumerable<SelectListItem>;
                    TempData.Keep("PAFNameoftheHCP");
                    model.MeetingType = TempData["PAFMeetingType"] as IEnumerable<SelectListItem>;
                    TempData.Keep("PAFMeetingType");

                    #region Add HCP
                    if (command == "Add HCP")
                    {
                        model.HCPs = TempData["PAFHCPViewModel"] as List<PAFHCPViewModel>;
                        if (model.HCPs == null)
                        { model.HCPs = new List<PAFHCPViewModel>(); }
                        if (!string.IsNullOrEmpty(model.SelectedTypeofHCP[0]) && !string.IsNullOrEmpty(model.SelectedNameoftheHCP[0]) && (model.HCPCost != 0))
                        {
                            var hcp = _hCPListRepository.GetHCPById(Convert.ToInt32(model.SelectedNameoftheHCP[0]));
                            if (hcp != null)
                            {
                                if (model.HCPs.SingleOrDefault(s => s.TMF_No == hcp.TMFNo && s.First_Name == hcp.FirstName && s.Last_Name == hcp.LastName && s.HCPAddress == hcp.HCPAddress) == null)
                                {
                                    //if (hcp.NoLecture > hcp.NoLectureGiven)
                                    //{
                                    model.HCPs.Add(new PAFHCPViewModel
                                    {
                                        Type = Convert.ToByte(model.SelectedTypeofHCP[0]),
                                        HCPId = hcp.ID,
                                        First_Name = hcp.FirstName,
                                        Last_Name = hcp.LastName,
                                        TMF_No = hcp.TMFNo,
                                        Cost = Convert.ToDecimal(model.HCPCost),
                                        HCPAddress = hcp.HCPAddress,
                                        HCPtopic = model.HCPtopic
                                    });
                                    //}
                                    //else { TempData["eNolect"] = "* This doctor exceeds the limit of HCP"; }
                                }
                                else { TempData["SameUser"] = "*HCP has been already selected, Choose another"; }
                            }
                        }
                        else if (!string.IsNullOrEmpty(model.SelectedTypeofHCP[1]) && !string.IsNullOrEmpty(model.HCPFirstName) && !string.IsNullOrEmpty(model.HCPLastName) && (model.NOHImageFile[0] != null && model.NOHImageFile[0].ContentLength > 0))
                        {
                            if (model.HCPs.SingleOrDefault(s => s.First_Name == model.HCPFirstName && s.Last_Name == model.HCPLastName && s.HCPtopic == model.HCPtopicNoH) == null)
                            {
                                foreach (var obj in model.NOHImageFile)
                                {
                                    if (obj.ContentLength > 0)
                                    {
                                        var lmModel = SaveFile(obj, "NOH");
                                        lmModel.HCP_First_Name = model.HCPFirstName;
                                        sLMModel.Add(lmModel);
                                        TempData["fileNAme"] += obj.FileName + "+";
                                    }
                                }

                                model.HCPs.Add(new PAFHCPViewModel
                                {
                                    Type = Convert.ToByte(model.SelectedTypeofHCP[1]),
                                    First_Name = model.HCPFirstName,
                                    Last_Name = model.HCPLastName,
                                    TMF_No = model.TMFNo,
                                    AttachedFile = TempData["fileNAme"].ToString(),
                                    HCPtopic = model.HCPtopicNoH
                                });
                            }
                            else { TempData["SameUser"] = "*HCP has been already selected, Choose another"; }
                        }
                        else
                        {
                            if (model.HCPType == "Hon" && (string.IsNullOrEmpty(model.SelectedTypeofHCP[0]) || string.IsNullOrEmpty(model.SelectedNameoftheHCP[0]) || model.HCPCost == 0))
                            {
                                if (string.IsNullOrEmpty(model.SelectedTypeofHCP[0])) { TempData["eHCPType"] = "*Please Select HCP Type"; }
                                if (string.IsNullOrEmpty(model.SelectedNameoftheHCP[0])) { TempData["eHCPName"] = "*Please Select HCP Name"; }
                                if (model.HCPCost == 0) { TempData["eHCPNCost"] = "*Please Select Honorarium payable to HCP"; }

                            }
                            else if (model.HCPType == "NoHon" && (string.IsNullOrEmpty(model.SelectedTypeofHCP[1]) || string.IsNullOrEmpty(model.HCPFirstName) && !string.IsNullOrEmpty(model.HCPLastName) || model.NOHImageFile == null || model.NOHImageFile[0] == null))
                            {
                                if (string.IsNullOrEmpty(model.SelectedTypeofHCP[1])) { TempData["eHCPNType"] = "*Please Select HCP Type"; }
                                if (string.IsNullOrEmpty(model.HCPFirstName)) { TempData["eHCPNFName"] = "*Please Select HCP First Name"; }
                                if (string.IsNullOrEmpty(model.HCPLastName)) { TempData["eHCPNLName"] = "*Please Select HCP Last Name"; }
                                if (model.NOHImageFile == null || model.NOHImageFile[0] == null) { TempData["eHCPNfile"] = "*Please attach file"; }
                            }
                        }
                        TempData.Remove("PAFHCPViewModel");
                        TempData.Add("PAFHCPViewModel", model.HCPs);
                        model.Miscellaneous = TempData["PAFMiscellaneous"] as List<PAFMiscellaneous>;
                        model.Participants = TempData["PAFParticipants"] as List<PAFParticipants>;
                        TempData["TabDisplay"] = "Valid1";
                    }
                    #endregion

                    #region Add Participants
                    if (command == "Add Participants")
                    {
                        if ((model.SelectedTherapyArea[0] != "" && model.SelectedBrand[0] != "" && model.SelectedEvent[0] != "") || (model.SelectedTherapyArea[0] != "" && model.ActivityName != null))
                        {
                            List<PAFParticipants> participantlist = JsonConvert.DeserializeObject<List<PAFParticipants>>(model.JsonParticipantsdata);
                            if (participantlist == null)
                            {

                                TempData["Pname"] = "* Please Enter Name";
                                TempData["pcity"] = "* Please Enter City";
                                TempData["pcustomerid"] = "* Please Enter Customer id";
                            }
                            else
                            {
                                if (model.Participants == null)
                                {
                                    model.Participants = participantlist;
                                }
                                else
                                {
                                    model.Participants.AddRange(participantlist);
                                }

                                TempData.Remove("PAFParticipants");
                                TempData.Add("PAFParticipants", model.Participants);
                                model.HCPs = TempData["PAFHCPViewModel"] as List<PAFHCPViewModel>;
                                model.Miscellaneous = TempData["PAFMiscellaneous"] as List<PAFMiscellaneous>;
                            }
                        }
                        else
                        {
                            if (model.SelectedTherapyArea[0] == "" && Session["LoginUserZone"].ToString() != "Rare Disease")
                            { TempData["sTherapyArea"] = "* Please select Therapy Area"; }
                            TempData["sBrandArea"] = "* Please select Brand";
                            TempData["sEventArea"] = "* Please select Event";
                            TempData["sLectureArea"] = "* Please select Lecture";
                        }
                        TempData["TabDisplay"] = "Valid1";
                    }
                    #endregion

                    #region RMP
                    if (command.Substring(0, 3) == "RMP")
                    {
                        model.Participants = TempData["PAFParticipants"] as List<PAFParticipants>;
                        if (command.Substring(3, 2) == "--")
                        {
                            var startindex = 5;
                            var endindex = command.IndexOf("-", startindex);
                            var length = endindex - startindex;
                            string mis = command.Substring(startindex, length);
                            String[] spearator = mis.Split('+');
                            string Name = spearator[0];
                            string Emailid = spearator[1];
                            model.Participants.RemoveAll(s => s.Name == Name && s.Email_Id == Emailid);
                        }
                        else
                        {
                            var startindex = 4;
                            var endindex = command.IndexOf("-", startindex);
                            var length = endindex - startindex;
                            string HCPID = command.Substring(startindex, length);
                            model.Participants.RemoveAll(s => s.ID == Int32.Parse(HCPID));
                        }

                        TempData.Remove("PAFParticipants");
                        TempData.Add("PAFParticipants", model.Participants);
                        model.HCPs = TempData["PAFHCPViewModel"] as List<PAFHCPViewModel>;
                        model.Miscellaneous = TempData["PAFMiscellaneous"] as List<PAFMiscellaneous>;
                        TempData["TabDisplay"] = "Valid1";
                    }
                    #endregion

                    #region RMHCP
                    if (command.Substring(0, 5) == "RMHCP")
                    {
                        model.HCPs = TempData["PAFHCPViewModel"] as List<PAFHCPViewModel>;
                        if (command.Substring(5, 2) == "--")
                        {
                            string mis = command.Substring(7, command.Length - 7);
                            String[] spearator = mis.Split('+');
                            string Name = spearator[0];

                            var nohlist = sLMModel.Where(s => s.Type == "NOH" && s.HCP_First_Name == Name).ToList();
                            if (nohlist != null)
                            {
                                foreach (var obj in nohlist)
                                {
                                    var fullPath = Path.Combine(Server.MapPath("~/Documents/PAF/"), obj.SavedDocName);
                                    sModel.IsDeleteFile = true;
                                    var lmModel = sLMModel.Where(s => s.SavedDocName == obj.SavedDocName && s.HCP_First_Name == obj.HCP_First_Name).FirstOrDefault();
                                    sLMModel.Remove(lmModel);
                                    cmeController.RemoveFileNOH(id, obj.SavedDocName, obj.Type, obj.HCP_First_Name);
                                    if (System.IO.File.Exists(fullPath))
                                    {
                                        System.IO.File.Delete(fullPath);
                                    }
                                }
                            }

                            model.HCPs.RemoveAll(s => s.First_Name == Name);
                        }
                        else
                        {
                            string pan = command.Substring(6, 10);
                            string mis = command.Substring(17);
                            String[] spearator = mis.Split('+');
                            string Name = spearator[0];
                            var nohlist = sLMModel.Where(s => s.Type == "NOH" && s.HCP_First_Name == Name).ToList();
                            if (nohlist != null)
                            {
                                foreach (var obj in nohlist)
                                {
                                    var fullPath = Path.Combine(Server.MapPath("~/Documents/PAF/"), obj.SavedDocName);
                                    sModel.IsDeleteFile = true;
                                    var lmModel = sLMModel.Where(s => s.SavedDocName == obj.SavedDocName && s.HCP_First_Name == obj.HCP_First_Name).FirstOrDefault();
                                    sLMModel.Remove(lmModel);
                                    cmeController.RemoveFileNOH(id, obj.SavedDocName, obj.Type, obj.HCP_First_Name);
                                    if (System.IO.File.Exists(fullPath))
                                    {
                                        System.IO.File.Delete(fullPath);
                                    }
                                }
                            }
                            if (pan == "")
                            { model.HCPs.RemoveAll(s => s.First_Name == Name); }
                            //else
                            //{ model.HCPs.RemoveAll(s => s.PAN == pan); }
                        }

                        TempData.Remove("PAFHCPViewModel");
                        TempData.Add("PAFHCPViewModel", model.HCPs);
                        model.ImagePath = sLMModel;
                        model.Miscellaneous = TempData["PAFMiscellaneous"] as List<PAFMiscellaneous>;
                        model.Participants = TempData["PAFParticipants"] as List<PAFParticipants>;
                        TempData["TabDisplay"] = "Valid1";
                    }
                    #endregion

                    #region Add Miscellaneous
                    if (command == "Add Miscellaneous")
                    {
                        model.Miscellaneous = TempData["PAFMiscellaneous"] as List<PAFMiscellaneous>;
                        if (model.Miscellaneous == null)
                        { model.Miscellaneous = new List<PAFMiscellaneous>(); }
                        if (!string.IsNullOrEmpty(model.MName))
                        {
                            if (model.Miscellaneous.SingleOrDefault(s => s.Name == model.MName) == null)
                            {
                                model.Miscellaneous.Add(new PAFMiscellaneous { Name = model.MName, Cost = model.MCost });
                                model.MName = string.Empty;
                                model.MCost = 0;
                            }
                        }
                        TempData.Remove("PAFMiscellaneous");
                        TempData.Add("PAFMiscellaneous", model.Miscellaneous);
                        model.HCPs = TempData["PAFHCPViewModel"] as List<PAFHCPViewModel>;
                        model.Participants = TempData["PAFParticipants"] as List<PAFParticipants>;
                        TempData["TabDisplay"] = "Valid2";
                    }
                    #endregion

                    #region RMMis
                    if (command.Substring(0, 5) == "RMMis")
                    {
                        model.HCPs = TempData["PAFHCPViewModel"] as List<PAFHCPViewModel>;
                        model.Miscellaneous = TempData["PAFMiscellaneous"] as List<PAFMiscellaneous>;
                        string mis = command.Substring(6, command.Length - 6);
                        String[] spearator = mis.Split('+');
                        model.Miscellaneous.RemoveAll(s => s.Name == spearator[0] && s.Cost.Equals(decimal.Parse(spearator[1])));
                        TempData.Remove("PAFMiscellaneous");
                        TempData.Add("PAFMiscellaneous", model.Miscellaneous);
                        model.HCPs = TempData["PAFHCPViewModel"] as List<PAFHCPViewModel>;
                        model.Participants = TempData["PAFParticipants"] as List<PAFParticipants>;
                        TempData["TabDisplay"] = "Valid2";
                    }
                    #endregion

                    TempData.Keep("PAFMiscellaneous");
                    TempData.Keep("PAFHCPViewModel");
                    TempData.Keep("PAFParticipants");
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public JsonResult GetBrandByTAId(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id)) { id = "0"; }
                var brands = _lecturesRepository.GetAllBrandsByTAId(Convert.ToByte(id)).Select(z => new SelectListItem { Text = z.Name.ToString(), Value = z.ID.ToString() }).OrderBy(s => s.Text).ToList();
                brands.Add(new SelectListItem { Text = "Select", Value = "" });
                TempData.Remove("PAFBrand");
                TempData.Add("PAFBrand", brands);
                TempData.Keep("PAFBrand");
                return Json(new SelectList(brands.OrderBy(o => o.Value), "Value", "Text"));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public JsonResult GetEventsByBrandId(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id)) { id = "0"; }
                var events = _lecturesRepository.GetEventsByBrandId(Convert.ToByte(id)).Select(z => new SelectListItem { Text = z.Name.ToString(), Value = z.ID.ToString() }).OrderBy(s => s.Text).ToList();
                events.Add(new SelectListItem { Text = "Select", Value = "" });
                TempData.Remove("PAFEvent");
                TempData.Add("PAFEvent", events);
                TempData.Keep("PAFEvent");
                return Json(new SelectList(events.OrderBy(o => o.Value), "Value", "Text"));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public JsonResult GetLecturesById(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id)) { id = "0"; }
                var lectures = _lecturesRepository.GetLectures(Convert.ToByte(id)).Select(z => new SelectListItem { Text = z.Lecture.ToString(), Value = z.ID.ToString() }).OrderBy(s => s.Text).ToList();
                lectures.Add(new SelectListItem { Text = "Select", Value = "" });
                TempData.Remove("PAFLecture");
                TempData.Add("PAFLecture", lectures);
                TempData.Keep("PAFLecture");
                return Json(new SelectList(lectures.OrderBy(o => o.Value), "Value", "Text"), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public PartialViewResult GetMaterialsById(int? id, CMEViewModel model)
        {
            try
            {
                if (id == null) { id = 0; }
                var materials = _lecturesRepository.GetMaterialsById(Convert.ToByte(id)).ToList();
                var lmFiles = materials.Select(m => new LectureMaterials
                {
                    DocName = m.DocName,
                    SavedDocName = m.SavedDocName,
                    DocPath = m.DocPath,
                    Extension = m.Extension,
                    Type = m.Type,
                    IsNew = m.IsNew,
                    HCP_First_Name = m.HCP_First_Name,
                    HCP_Last_Name= m.HCP_Last_Name,
                    PostType = m.PostType,
                }).ToList();

                model.LMFiles = lmFiles;
                Session.Remove("PAFLMList");
                Session.Add("PAFLMList", materials);
                return PartialView("_Materials", model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //public PartialViewResult GetAssessmentById(int id)
        //{
        //    var model = new CMEViewModel();
        //    try
        //    {
        //        var assessmenfilename = _workUnitRepository.GetAllWorkUnit().Where(r => r.Id == id && r.Status == true).Select(s => new SelectListItem { Text = s.DocName.ToString(), Value = Convert.ToString(s.Id) }).FirstOrDefault().Text;
        //        var fullPath = Path.Combine(Server.MapPath("~/Documents/Assessment/"), assessmenfilename);
        //        if (System.IO.File.Exists(fullPath))
        //        {
        //            model.AssessmentDocName = assessmenfilename;
        //            model.AssessmentDocPath = fullPath;
        //            return PartialView("_Assessment", model);
        //        }
        //        else
        //            return null;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        public PartialViewResult SearchParticipants(string Name, string City, string CustomerID, int? page = 1)
        {
            try
            {
                string searchString = Name;
                string searchStringCN = null;
                string sortOrder = "Id";

                var allHcpList = _hCPListRepository.GetAll(searchString, searchStringCN, sortOrder, CustomerID);

                var hcpList = allHcpList.Select(h => new HCPViewModel
                {
                    ID = h.ID,
                    VendorNo = h.VendorNo,
                    TMFNo = h.TMFNo,
                    FirstName = h.FirstName,
                    LastName = h.LastName,
                    TAName = h.TAName,
                    LectCost = h.LectCost,
                    CMSNo = h.CMSNo,
                    HCPAddress = h.HCPAddress,
                    Customer_ID = h.Customer_ID,
                    Email_Id = h.Email_Id,
                    Phone_Number = h.Phone_Number,
                    City = h.City,
                    ContractStartDate = h.ContractStartDate,
                    ContractEndDate = h.ContractEndDate
                }).ToList();

                ViewBag.SearchString = searchString;
                ViewBag.SearchCity = City;
                ViewBag.SearchCustomerID = CustomerID;
                int pageSize = 5;
                int pageNumber = (page ?? 1);
                return PartialView("_Participants", hcpList.ToPagedList(pageNumber, pageSize));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private LectureMaterials SaveFile(HttpPostedFileBase imageFile, string type = "Post Activity")
        {
            var lmModel = new LectureMaterials();
            try
            {
                lmModel.DocName = Path.GetFileName(imageFile.FileName).Replace(" ", String.Empty);
                lmModel.Extension = Path.GetExtension(imageFile.FileName);
                lmModel.SavedDocName = Path.GetFileName(Path.GetFileNameWithoutExtension(imageFile.FileName).Replace(" ", String.Empty) + DateTime.UtcNow.ToString(" - dd.MM.yyyy HH.mm.ss") + lmModel.Extension);
                if (!Directory.Exists(Server.MapPath("~/Documents/PAF/")))
                    Directory.CreateDirectory(Server.MapPath("~/Documents/PAF/"));
                lmModel.DocPath = Path.Combine(Server.MapPath("~/Documents/PAF/"), lmModel.SavedDocName);
                lmModel.Type = type;
                lmModel.IsNew = true;
                imageFile.SaveAs(lmModel.DocPath);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return lmModel;
        }
    }
}
