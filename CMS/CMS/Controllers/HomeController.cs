#region Namespace
using System.Web.Mvc;
using System;
using CME.Models;
using DataAccessLayer.DAL.UserRepository;
using DataAccessLayer;
using DataAccessLayer.DAL.Model;
using DataAccessLayer.DAL.PAFRepository;
using System.Collections.Generic;
using PagedList;
using DataAccessLayer.DAL.CommonRespository;
using System.Linq;
using DataAccessLayer.DAL.CMERepository;

#endregion

namespace CME.Controllers
{
    //[Authorize]
    //[HandleError()]
    [AllowAnonymous]
    public class HomeController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IPAFRepository _pAFRepository;
        private readonly ICommonRespository _commonRespository;
        private readonly ICMERepository _cMERepository;

        public HomeController()
        {
            this._userRepository = new UserRepository(new NovoNordiskEntities());
            this._pAFRepository = new PAFRepository(new NovoNordiskEntities());
            this._commonRespository = new CommonRespository(new NovoNordiskEntities());
            this._cMERepository = new CMERepository(new NovoNordiskEntities());
            ViewBag.HomeMenu = "active";
        }

        public ActionResult Index(PAFSearch model)
        {
            try
            {
                var domainUser = User.Identity.Name;
                string[] paramsLogin = domainUser.Split('\\');
                if (!string.IsNullOrEmpty(Convert.ToString(Session["LoginUserInitial"])))
                {
                    Session["LoginUserInitial"] = Convert.ToString(Session["LoginUserInitial"]);
                    Console.WriteLine("LoginUserInitial", Session["LoginUserInitial"]);
                }
                else
                {
                    if (paramsLogin.Length > 1)
                    {
                        Session["LoginUserInitial"] = paramsLogin[1].ToLower();
                    }
                    else
                    {
                        return RedirectToAction("Index", "LogIn");
                    }
                }

                string userInitial = Convert.ToString(Session["LoginUserInitial"]);
                var userDetail = _userRepository.GetUserByInitial(userInitial);

                if (string.IsNullOrEmpty(userInitial)) { userInitial = "RBDZ"; }

                if (userDetail != null)
                {
                    if (!userDetail.Status)
                    {
                        return RedirectToAction("Deactivate");
                    }
                    else
                    {
                        Session["LoginUserId"] = userDetail.ID;
                        Session["LoginUserInitial"] = userDetail.Initial;
                        Session["LoginUserName"] = userDetail.Name + " " + userDetail.Surname;
                        Session["LoginUserManager"] = userDetail.Manager;
                        Session["LoginUserStatus"] = userDetail.Status;
                        Session["LoginUserPosition"] = userDetail.Position;
                        Session["LoginUserRole"] = userDetail.Role;
                        Session["LoginUserMManager"] = userDetail.MManager;
                        Session["LoginUserZone"] = userDetail.ZoneName;
                        Session["LoginUserTherapyArea"] = userDetail.TherapyArea;
                        Session["TestingName"] = Convert.ToString(userDetail.Initial);
                        Session["TestingInitial"] = Convert.ToString(userDetail.Initial).Substring(0, 2);
                        if (Convert.ToString(Session["LoginUserRole"]).ToUpper() == "ADMIN")
                        { Session["IsSuperAdmin"] = "Yes"; }
                        if (Convert.ToString(Session["IsSuperAdmin"]).ToUpper() == "YES")
                        { Session["IsSuperAdmin"] = "Yes"; }
                        else
                        { Session["IsSuperAdmin"] = "No"; }

                        var pafList = _cMERepository.GetLatestviolation(userDetail.ID);
                        var percent = pafList.Count(n => n.IsDateDeviation == false);
                        var data = ((double)percent / 10) * 100;
                        Session["Runrate"] = data + "%";


                        var tempModel1 = new PAFSearch
                        {
                            UserId = userDetail.ID,
                            UserRole = userDetail.Role,
                            UserZone = userDetail.ZoneName
                        };

                        Session["Draftformcount"] = _pAFRepository.GetDraft(tempModel1).Count();

                        if (tempModel1.UserZone == "Rare Disease")
                        {
                            Session["Inprocessformcount"] = _pAFRepository.GetPendingBiopharm(tempModel1).Count;
                            Session["Rejectedformcount"] = _pAFRepository.GetRejectedBiopharm(tempModel1).Count;
                            Session["Approvedformcount"] = _pAFRepository.GetApprovedBiopharm(tempModel1).Count;
                            Session["Attendformcount"] = _pAFRepository.GetAttendBiopharm(tempModel1).Count;
                        }
                        else
                        {
                            Session["Inprocessformcount"] = _pAFRepository.GetPending(tempModel1).Count;
                            Session["Rejectedformcount"] = _pAFRepository.GetRejected(tempModel1).Count;
                            Session["Approvedformcount"] = _pAFRepository.GetApproved(tempModel1).Count;
                            Session["Attendformcount"] = _pAFRepository.GetAttend(tempModel1).Count;
                        }
                        Session["closedformcount"] = _pAFRepository.GetClosed(tempModel1).Count;
                        Session["removedformcount"] = _pAFRepository.GetRemoved(tempModel1).Count;
                        Session["Pendingverificationformcount"] = _pAFRepository.GetAllPendingVarificationForm(tempModel1).Count;
                    }

                }
                else
                {
                    Session.RemoveAll();
                    return RedirectToAction("Deactivate");
                }

                if (string.IsNullOrEmpty(model.PAFId) && string.IsNullOrEmpty(model.Initial) && string.IsNullOrEmpty(model.Manager)
                    && string.IsNullOrEmpty(model.SelectedTherapyArea)
                    && string.IsNullOrEmpty(model.SortOrder) && string.IsNullOrEmpty((model.StartDate).ToString()) && string.IsNullOrEmpty((model.EndDate).ToString()))
                { TempData.Remove("FilterValue"); }

                if (Session["SessionFilterValue"] is PAFSearch tempModel)
                {
                    if (!string.IsNullOrEmpty(tempModel.PAFId) && string.IsNullOrEmpty(model.PAFId)) { model.PAFId = tempModel.PAFId; }
                    if (!string.IsNullOrEmpty(tempModel.Initial) && string.IsNullOrEmpty(model.Initial)) { model.Initial = tempModel.Initial; }
                    if (!string.IsNullOrEmpty(tempModel.Manager) && string.IsNullOrEmpty(model.Manager)) { model.Manager = tempModel.Manager; }
                    if (!string.IsNullOrEmpty(tempModel.SelectedTherapyArea) && string.IsNullOrEmpty(model.SelectedTherapyArea)) { model.SelectedTherapyArea = tempModel.SelectedTherapyArea; }
                    if (!string.IsNullOrEmpty(tempModel.SortOrder) && string.IsNullOrEmpty(model.SortOrder)) { model.SortOrder = tempModel.SortOrder; }
                    if (!string.IsNullOrEmpty(tempModel.StartDate.ToString()) && string.IsNullOrEmpty(model.StartDate.ToString())) { model.StartDate = tempModel.StartDate; }
                    if (!string.IsNullOrEmpty(tempModel.EndDate.ToString()) && string.IsNullOrEmpty(model.EndDate.ToString())) { model.EndDate = tempModel.EndDate; }
                    if (!string.IsNullOrEmpty(tempModel.HotelName) && string.IsNullOrEmpty(model.HotelName)) { model.HotelName = tempModel.HotelName; }
                    TempData.Remove("FilterValue");
                }
                model.TherapyArea = _commonRespository.GetTherapyArea().Select(z => new SelectListItem { Text = z.Name.ToString(), Value = z.ID.ToString() }).OrderBy(s => s.Text).ToList();
                model.Brand = _commonRespository.GetBrands().Select(z => new SelectListItem { Text = z.Name.ToString(), Value = z.ID.ToString() }).OrderBy(s => s.Text).ToList();

                TempData.Add("FilterValue", model);
                Session.Add("SessionFilterValue", model);
                return View(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult Draft(string sortOrder = "Id", int? page = 1, int pageSize = 5)
        {
            var tempModel = TempData["FilterValue"] as PAFSearch;
            tempModel.UserZone = Convert.ToString(Session["LoginUserZone"]);

            return GetPaginatedPAFs(sortOrder, page, pageSize, _pAFRepository.GetDraft);
        }

        public ActionResult Pending(string sortOrder = "Id", int? page = 1, int pageSize = 5)
        {
            var tempModel = TempData["FilterValue"] as PAFSearch;
            tempModel.UserZone = Convert.ToString(Session["LoginUserZone"]);

            if (tempModel.UserZone == "Rare Disease")
            {
                return GetPaginatedPAFs(sortOrder, page, pageSize, _pAFRepository.GetPendingBiopharm);
            }
            else
            {
                return GetPaginatedPAFs(sortOrder, page, pageSize, _pAFRepository.GetPending);
            }
        }

        public ActionResult Rejected(string sortOrder = "Id", int? page = 1, int pageSize = 5)
        {
            var tempModel = TempData["FilterValue"] as PAFSearch;
            tempModel.UserZone = Convert.ToString(Session["LoginUserZone"]);

            if (tempModel.UserZone == "Rare Disease")
            {
                return GetPaginatedPAFs(sortOrder, page, pageSize, _pAFRepository.GetRejectedBiopharm);
            }
            else
            {
                return GetPaginatedPAFs(sortOrder, page, pageSize, _pAFRepository.GetRejected);
            }
        }

        public ActionResult Approved(string sortOrder = "Id", int? page = 1, int pageSize = 5)
        {
            var tempModel = TempData["FilterValue"] as PAFSearch;
            tempModel.UserZone = Convert.ToString(Session["LoginUserZone"]);

            if (tempModel.UserZone == "Rare Disease")
            {
                return GetPaginatedPAFs(sortOrder, page, pageSize, _pAFRepository.GetApprovedBiopharm);
            }
            else
            {
                return GetPaginatedPAFs(sortOrder, page, pageSize, _pAFRepository.GetApproved);
            }
        }

        public ActionResult Closed(string sortOrder = "Id", int? page = 1, int pageSize = 5)
        {
            var tempModel = TempData["FilterValue"] as PAFSearch;
            tempModel.UserZone = Convert.ToString(Session["LoginUserZone"]);

            return GetPaginatedPAFs(sortOrder, page, pageSize, _pAFRepository.GetClosed);
        }

        public ActionResult Attend(string sortOrder = "Id", int? page = 1, int pageSize = 5)
        {
            var tempModel = TempData["FilterValue"] as PAFSearch;
            tempModel.UserZone = Convert.ToString(Session["LoginUserZone"]);

            if (tempModel.UserZone == "Rare Disease")
            {
                return GetPaginatedPAFs(sortOrder, page, pageSize, _pAFRepository.GetAttendBiopharm);
            }
            else
            {
                return GetPaginatedPAFs(sortOrder, page, pageSize, _pAFRepository.GetAttend);
            }
        }

        public ActionResult Removed(string sortOrder = "Id", int? page = 1, int pageSize = 5)
        {
            var tempModel = TempData["FilterValue"] as PAFSearch;
            tempModel.UserZone = Convert.ToString(Session["LoginUserZone"]);

            return GetPaginatedPAFs(sortOrder, page, pageSize, _pAFRepository.GetRemoved);
        }

        public ActionResult Status(string sortOrder = "Id", int? page = 1, int pageSize = 5)
        {
            return GetPaginatedPAFs(sortOrder, page, pageSize, _pAFRepository.GetPreApprovalStatus);
        }

        public ActionResult AllForm(string sortOrder = "Id", int? page = 1, int pageSize = 5)
        {
            return GetPaginatedPAFs(sortOrder, page, pageSize, _pAFRepository.GetAllForm);
        }

        public ActionResult AllZoneForm(string sortOrder = "Id", int? page = 1, int pageSize = 5)
        {
            return GetPaginatedPAFs(sortOrder, page, pageSize, _pAFRepository.GetAllZoneForm);
        }

        public ActionResult GetForm(string sortOrder = "Id", int? page = 1, int pageSize = 5)
        {
            return GetPaginatedPAFs(sortOrder, page, pageSize, _pAFRepository.GetAllForm);
        }

        public ActionResult RejectedCloseForm(string sortOrder = "Id", int? page = 1, int pageSize = 5)
        {
            return GetPaginatedPAFs(sortOrder, page, pageSize, _pAFRepository.GetRejectedCloseBiopharm);
        }

        public ActionResult PendingVarificationForm(string sortOrder = "Id", int? page = 1, int pageSize = 5)
        {
            return GetPaginatedPAFs(sortOrder, page, pageSize, _pAFRepository.GetAllPendingVarificationForm);
        }

        public ActionResult Reset()
        {
            var model = new PAFSearch();
            Session.Remove("SessionFilterValue");
            return RedirectToAction("Index", model);
        }

        public ActionResult SwitchToAdmin()
        {
            Session["LoginUserInitial"] = "";
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult Deactivate()
        {
            return View();
        }

        [HttpGet]
        public ActionResult SessionExpire()
        {
            return View();
        }

        private void SetViewBagSortOrders(string sortOrder)
        {
            ViewBag.SortById = sortOrder == "Id" ? "Id_Desc" : "Id";
            ViewBag.SortByIssued = sortOrder == "Issued" ? "Issued_Desc" : "Issued";
            ViewBag.SortByStartDate = sortOrder == "StartDate" ? "StartDate_Desc" : "StartDate";
            ViewBag.SortByEndDate = sortOrder == "EndDate" ? "EndDate_Desc" : "EndDate";
            ViewBag.SortByRequest = sortOrder == "Request" ? "Request_Desc" : "Request";
            ViewBag.SortByActivityName = sortOrder == "ActivityName" ? "ActivityName_Desc" : "ActivityName";
            ViewBag.SortByCC = sortOrder == "CC" ? "CC_Desc" : "CC";
            ViewBag.SortByCost = sortOrder == "Cost" ? "Cost_Desc" : "Cost";
            ViewBag.SortByStatus = sortOrder == "Status" ? "Status_Desc" : "Status";
        }

        private ActionResult GetPaginatedPAFs(string sortOrder, int? page, int pageSize, Func<PAFSearch, List<PAFList>> getPafsFunc)
        {
            try
            {
                var tempModel = TempData["FilterValue"] as PAFSearch;
                TempData.Keep("FilterValue");

                SetViewBagSortOrders(sortOrder);

                tempModel.UserId = Convert.ToInt32(Session["LoginUserId"]);
                tempModel.UserRole = Convert.ToString(Session["LoginUserRole"]);
                tempModel.UserZone = Convert.ToString(Session["LoginUserZone"]);

                var pafs = getPafsFunc(tempModel);

                var pafLists = ConvertPafModelToViewModel(pafs);

                int pageNumber = (page ?? 1);
                return PartialView(pafLists.ToPagedList(pageNumber, pageSize));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private List<PAFList> ConvertPafModelToViewModel(List<PAFList> paf)
        {
            var pafLists = new List<PAFList>();

            foreach (var p in paf)
            {
                var pafList = new PAFList()
                {
                    Id = p.Id,
                    IssuedBy = p.IssuedBy,
                    CreatedBy = p.CreatedBy,
                    ManagerInitial = p.ManagerInitial,
                    StartDate = p.StartDate,
                    ViewStartDate = p.ViewStartDate,
                    EndDate = p.EndDate,
                    ViewEndDate = p.ViewEndDate,
                    Name = p.Name,
                    CMEType = p.CMEType,
                    ManagerRole = p.ManagerRole,
                    Zone = p.Zone,
                    ZoneId = p.ZoneId,
                    TherapyArea = p.TherapyArea,
                    TherapyAreaId = p.TherapyAreaId,
                    Brand = p.Brand,
                    Approver = p.Approver,
                    BrandId = p.BrandId,
                    TotalCost = p.TotalCost,
                    Activity_Start_Date = p.Activity_Start_Date,
                    Approved_Or_Reject = p.Approved_Or_Reject,
                    CCCode = p.CCCode,
                    IsDateDeviation = p.IsDateDeviation,
                    IsDateDeviationApproved = p.IsDateDeviationApproved,
                    HotelName = p.HotelName,
                    IsVirtual = p.IsVirtual,
                    Cost_Center = p.Cost_Center,
                    SAPFileGenerated = p.SAPFileGenerated
                };
                pafLists.Add(pafList);
            }
            return pafLists;
        }
    }
}