using System;
using System.Collections.Generic;
using DataAccessLayer;
using DataAccessLayer.DAL.UserRepository;
using System.Web.Mvc;
using PagedList;
using CME.Models;
using System.Linq;
using DataAccessLayer.DAL.ProfileRepository;
using DataAccessLayer.DAL.Model;
using System.Web;
using System.IO;
using System.Data;
using ClosedXML.Excel;
using DataAccessLayer.DAL.CommonRespository;

namespace CME.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IProfileRepository _profileRepository;
        private readonly ICommonRespository _commonRespository;

        public UserController()
        {
            this._userRepository = new UserRepository(new NovoNordiskEntities());
            this._profileRepository = new ProfileRepository(new NovoNordiskEntities());
            this._commonRespository = new CommonRespository(new NovoNordiskEntities());
            ViewBag.AdminMenu = "active";
            ViewBag.UserMenu = "active";
        }

        // GET: User
        public ActionResult Index(string costCenter = null, string initial = null, string manager = null, string name = null, string surName = null, string sortOrder = "Id", int? page = 1, int pageSize = 10)
        {
            var userDetails = new List<UserViewModel>();
            try
            {
                ViewBag.SearchByinitial = initial;
                ViewBag.SearchBySurname = surName;
                ViewBag.SearchBycostCenter = costCenter;
                ViewBag.SearchByname = name;
                ViewBag.SearchByManager = manager;
                ViewBag.SortByName = sortOrder == "Name" ? "Name_Desc" : "Name";
                ViewBag.SortBySurName = sortOrder == "SurName" ? "SurName_Desc" : "SurName";
                ViewBag.SortByInitial = sortOrder == "Initial" ? "Initial_Desc" : "Initial";
                ViewBag.SortByMgr = sortOrder == "Mgr" ? "Mgr_Desc" : "Mgr";
                ViewBag.SortByposition = sortOrder == "position" ? "position_Desc" : "position";
                ViewBag.SortByIsAdmin = sortOrder == "IsAdmin" ? "IsAdmin_Desc" : "IsAdmin";
                ViewBag.SortBylocked = sortOrder == "locked" ? "locked_Desc" : "locked";
                ViewBag.SortByCC = sortOrder == "CostCenter" ? "CostCenter_Desc" : "CostCenter";
                ViewBag.SortOrder = sortOrder;

                var userModel = _userRepository.GetUsers(initial, surName, costCenter, manager,name, sortOrder);

                foreach (var user in userModel)
                {
                    var userDetail = new UserViewModel()
                    {
                        ID = user.ID,
                        Initial = user.Initial,
                        Name = user.Name,
                        Manager = user.Manager,
                        Status = user.Status.ToString(),
                        SelectedZone = user.ZoneName,
                        SelectedTherapyArea = user.TherapyArea,
                        SelectedRole = user.Role,
                        Position = user.Position,
                        CostCenter = user.CostCenter,
                        Surname = user.Surname,
                        IsAdmin=user.IsAdmin.ToString()
                    };
                    userDetails.Add(userDetail);
                }

                int pageNumber = (page ?? 1);
                var result = userDetails.ToPagedList(pageNumber, pageSize);
                return View(result);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // GET: User/Create
        public ActionResult Create()
        {
            try
            {
                var model = new UserViewModel()
                {
                    TherapyArea = _commonRespository.GetTherapyArea().Select(z => new SelectListItem { Text = z.Name.ToString(), Value = z.ID.ToString() }).OrderBy(s => s.Text).ToList(),
                    Role = _profileRepository.GetProfiles().Select(z => new SelectListItem { Text = z.Name.ToString(), Value = z.ID.ToString() }).OrderBy(s => s.Text).ToList(),
                    CostCenterList = _commonRespository.GetCostCentres().Select(z => new SelectListItem { Text = z.Name.ToString(), Value = z.ID.ToString() }).OrderBy(s => s.Text).ToList(),
                    ManagerList = _userRepository.GetAllUsers().Select(z => new SelectListItem { Text = z.Initials.ToString(), Value = z.ID.ToString() }).OrderBy(s => s.Text).ToList(),
                };
                return View(model);
            }
            catch (Exception ex) 
            {
                throw ex;
            }
        }

        // POST: User/Create
        [HttpPost]
        public ActionResult Create(string id, UserViewModel model)
        {
            try
            {
                var userDetails = _userRepository.GetAllUsers();

                model.TherapyArea = _commonRespository.GetTherapyArea().Select(z => new SelectListItem { Text = z.Name.ToString(), Value = z.ID.ToString() }).OrderBy(s => s.Text).ToList();
                model.Role = _profileRepository.GetProfiles().Select(z => new SelectListItem { Text = z.Name.ToString(), Value = z.ID.ToString() }).OrderBy(s => s.Text).ToList();
                model.CostCenterList = _commonRespository.GetCostCentres().Select(z => new SelectListItem { Text = z.Name.ToString(), Value = z.ID.ToString() }).OrderBy(s => s.Text).ToList();
                model.ManagerList = userDetails.Select(z => new SelectListItem { Text = z.Initials.ToString(), Value = z.ID.ToString() }).OrderBy(s => s.Text).ToList();

                if (model.Name != null && model.Surname != null && model.Initial != null && model.Manager != null &&
                   model.Position != null && model.SelectedTherapyArea != null &&
                   model.SelectedRole != null && model.CostCenter != null && model.EmailId != null)
                {
                    if (userDetails.Where(c => c.Initials.Equals(model.Initial, StringComparison.InvariantCultureIgnoreCase)).Count() == 0)
                    {
                        var insertUserDetail = new User
                        {
                            Name = model.Name,
                            SurName = model.Surname,
                            Initials = model.Initial,
                            Mgr_Id = Convert.ToInt32(model.Manager),
                            Position = model.Position,
                            Status = (model.Status == "Active") ? true : false,
                            IsAdmin = (model.IsAdmin == "No") ? false : true,
                            CC_Id = Convert.ToInt32(model.CostCenter),
                            TA_Id = Convert.ToByte(model.SelectedTherapyArea),
                            CreatedBy = Convert.ToInt32(Session["LoginUserId"]),
                            CreatedDate = DateTime.UtcNow,
                            ModifiedBy = Convert.ToInt32(Session["LoginUserId"]),
                            ModifiedDate = DateTime.UtcNow,
                            EmpId = model.EmpId,
                            Email = model.EmailId,
                        };

                        //insert user
                       int insertedId = _userRepository.InsertUser(insertUserDetail);

                        var insertProfileUser = new Profiles_To_User
                        {
                            Profile_Id = Convert.ToByte(model.SelectedRole),
                            User_Id = insertedId,
                            Status = (model.Status == "Active") ? true : false,
                            CreatedBy = Convert.ToInt32(Session["LoginUserId"]),
                            CreatedDate = DateTime.UtcNow,
                            ModifiedBy = Convert.ToInt32(Session["LoginUserId"]),
                            ModifiedDate = DateTime.UtcNow
                        };


                        //insert profile_to_user
                        _userRepository.InsertProfileToUser(insertProfileUser);
                        _userRepository.SaveChanges();

                        return RedirectToAction("Index");
                    }
                    else
                    {
                        TempData["userInitials"] = "*User Initial is Unique, Please select another Initial";
                        return View(model);
                    }
                }
                else
                {
                    return View(model);
                }
            }
            catch
            {
                return View();
            }
        }

        // GET: User/Edit/5
        public ActionResult Edit(int? id)        
        {
            try
            {
                var result = new UserDetailsViewModel();
                if (id == null)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    var userDetail = _userRepository.GetUserDetailsById(id);
                    if (userDetail != null)
                    {
                        result.Name = userDetail.Name;
                        result.Surname = userDetail.Surname;
                        result.Initial = userDetail.Initial;
                        result.Position = userDetail.Position;
                        result.EmailId = userDetail.EmailId;
                        result.EmpId = userDetail.EmpId;
                        result.Status = userDetail.Status;
                        result.TherapyArea = _commonRespository.GetTherapyArea().Select(s => new SelectListItem { Text = s.Name.ToString(), Value = s.ID.ToString(), Selected = s.ID == userDetail.TherapyAreaId }).OrderBy(s => s.Text).ToList();
                        result.Role = _profileRepository.GetProfiles().Select(s => new SelectListItem { Text = s.Name.ToString(), Value = s.ID.ToString(), Selected = s.ID == userDetail.RoleId }).OrderBy(s => s.Text).ToList();
                        result.CostCenterList = _commonRespository.GetCostCentres().Select(s => new SelectListItem { Text = s.Name.ToString(), Value = s.ID.ToString(), Selected = s.ID == userDetail.CostCenterId }).OrderBy(s => s.Text).ToList();
                        result.ManagerList = _userRepository.GetAllUsers().Select(s => new SelectListItem { Text = s.Name.ToString(), Value = s.ID.ToString(), Selected = s.ID == userDetail.ManagerId }).OrderBy(s => s.Text).ToList();
                    }
                    return View(result);
                }
            }
            catch (Exception ex) 
            {
                throw ex;
            }
        }

        // POST: User/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, UserDetailsViewModel model, UserViewModel user)
        {
            try
            {
                var userDetails = _userRepository.GetUserDetailsById(id);
                var alluserDetails = _userRepository.GetAllUsers();

                if (model.SelectedRole[0] != "" && model.SelectedTherapyArea[0] != "" &&
                     model.CostCenter != null && model.Manager != null && model.Name != null && model.Surname != null &&
                     model.Initial != null && model.EmailId != null)
                {
                    if (userDetails.Initial.Equals(model.Initial, StringComparison.InvariantCultureIgnoreCase) || alluserDetails.Where(s => s.Initials.Equals(model.Initial, StringComparison.InvariantCultureIgnoreCase)).Count() == 0)
                    {
                        model.Status = (user.Status == "Active") ? true : false;
                        model.IsAdmin = (user.IsAdmin == "No") ? false : true;
                        model.ModifiedBy = Convert.ToInt32(Session["LoginUserId"]);

                        //update
                        using (NovoNordiskEntities context = new NovoNordiskEntities())
                        {
                            var existingUser = context.Users.Where(s => s.ID == id).FirstOrDefault();
                            if (existingUser != null)
                            {
                                var existingProfilesToUser = _userRepository.GetProfilesToUserByUserId(id);
                                _userRepository.RemoveProfilesToUser(existingProfilesToUser);

                                var newProfileToUser = new Profiles_To_User
                                {
                                    Profile_Id = Convert.ToByte(model.SelectedRole[0]),
                                    User_Id = id,
                                    Status = true,
                                    CreatedBy = existingUser.CreatedBy,
                                    CreatedDate = DateTime.UtcNow,
                                    ModifiedBy = model.ModifiedBy,
                                    ModifiedDate = DateTime.UtcNow
                                };

                                _userRepository.InsertProfileToUser(newProfileToUser);

                                if (!string.IsNullOrEmpty(model.SelectedTherapyArea[0])
                            && !string.IsNullOrEmpty(model.CostCenter) && !string.IsNullOrEmpty(model.Manager))
                                {
                                    existingUser.CC_Id = Convert.ToInt32(model.CostCenter);
                                    existingUser.TA_Id = Convert.ToByte(model.SelectedTherapyArea[0]);
                                    existingUser.IsAdmin = model.IsAdmin;
                                    existingUser.Status = model.Status;
                                    existingUser.ModifiedBy = model.ModifiedBy;
                                    existingUser.ModifiedDate = DateTime.UtcNow;
                                    existingUser.Mgr_Id = Convert.ToInt32(model.Manager);
                                    existingUser.Name = model.Name;
                                    existingUser.SurName = model.Surname;
                                    existingUser.Initials = model.Initial;
                                    existingUser.Position = model.Position;
                                    existingUser.EmpId = Convert.ToInt32(model.EmpId);
                                    existingUser.Email = model.EmailId;

                                }
                                else
                                {
                                    existingUser.Mgr_Id = null;
                                    existingUser.TA_Id = null;

                                }
                                context.SaveChanges();
                            }
                        }
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        TempData["userInitials"] = "*User Initial are Unique, Please select another Initial";
                        return RedirectToAction("Edit");
                    }
                }
                else
                {
                    if (model.SelectedRole[0] == "" && userDetails.RoleId == 0) { TempData["eSrole"] = "*Please select Role"; }
                    if (model.SelectedTherapyArea[0] == "" && userDetails.TherapyAreaId == 0) { TempData["eSTA"] = "*Please select Therapy Area"; }
                    if (model.CostCenter == null && userDetails.CostCenterId == 0) { TempData["eSCC"] = "*Please select Cost Center"; }
                    if (model.Manager == null && userDetails.ManagerId == 0) { TempData["eSUM"] = "*Please select Cost Center"; }

                    return RedirectToAction("Edit");
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        // GET: User/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: User/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        public ActionResult AccountSwitch(string initial)
        {
            Session["LoginUserInitial"] = initial;
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Reset()
        {
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult ImportExcel()
        {
            var userDetail = _userRepository.GetUserByInitial(Session["LoginUserInitial"].ToString());
            ViewBag.Createddate = "Last updated on " + userDetail.CC_CreatedDate.ToShortDateString();
            return View();
        }

        [HttpPost]
        public ActionResult ImportExcel(HttpPostedFileBase FileUpload)
        {
            try
            {
                List<string> data = new List<string>();
                if (FileUpload != null)
                {
                    if (FileUpload.ContentType == "application/vnd.ms-excel" || FileUpload.ContentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                    {
                        var date = DateTime.Now.ToString("dd-MM-yyyy");
                        string[] ps = FileUpload.FileName.Split('.');
                        string filename = ps[0] + "_" + date + "." + ps[1];
                        if (!Directory.Exists(Server.MapPath("~/Documents/CostCenter/")))
                        {
                            Directory.CreateDirectory(Server.MapPath("~/Documents/CostCenter/"));
                        }
                        string targetpath = Server.MapPath("~/Documents/CostCenter/");
                        FileUpload.SaveAs(targetpath + filename);
                        string pathToExcelFile = targetpath + filename;

                        var dt = new DataTable();
                        using (var workbook = new XLWorkbook(pathToExcelFile))
                        {
                            var worksheet = workbook.Worksheet(1); 
                            bool firstRow = true;
                            foreach (var row in worksheet.Rows())
                            {
                                if (firstRow)
                                {
                                    foreach (var cell in row.Cells())
                                    {
                                        dt.Columns.Add(cell.Value.ToString());
                                    }
                                    firstRow = false;
                                }
                                else
                                {
                                    dt.Rows.Add();
                                    int i = 0;
                                    foreach (var cell in row.Cells())
                                    {
                                        dt.Rows[dt.Rows.Count - 1][i] = cell.Value.ToString();
                                        i++;
                                    }
                                }
                            }
                        }

                        int SPResult = _userRepository.ImportExcelfile(dt);
                        if (SPResult == 1)
                        {
                            TempData["MessageClass"] = "alert-success";
                            TempData["Message"] = "Cost center uploaded successfully";
                            return RedirectToAction("ImportExcel", "User");
                        }
                        else
                        {
                            TempData["MessageClass"] = "alert-danger";
                            TempData["Message"] = "Cost center not uploaded successfully";
                            return RedirectToAction("ImportExcel", "User");
                        }
                    }
                    else
                    {
                        TempData["MessageClass"] = "alert-danger";
                        TempData["Message"] = "Cost center not uploaded successfully";
                        return RedirectToAction("ImportExcel", "User");
                    }
                }
                else
                {
                    TempData["MessageClass"] = "alert-danger";
                    TempData["Message"] = "Cost center not uploaded successfully";
                    return RedirectToAction("ImportExcel", "User");
                }
            }
            catch (Exception ex)
            {
                TempData["MessageClass"] = "alert-danger";
                TempData["Message"] = ex;
                return RedirectToAction("ImportExcel", "User");
            }
        }

    }
}
