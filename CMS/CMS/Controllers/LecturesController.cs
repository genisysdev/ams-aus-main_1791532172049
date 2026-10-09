using CME.Infrastructure;
using CME.Models;
using DataAccessLayer;
using DataAccessLayer.DAL.CommonRespository;
using DataAccessLayer.DAL.LecturesRepository;
using DataAccessLayer.DAL.Model;
using DataAccessLayer.DAL.UserRepository;
using PagedList;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace CME.Controllers
{
    //[CustomAuthenticationFilter]
    //[CustomAuthorize("Admin")]
    //[HandleError()]
    public class LecturesController : Controller
    {
        private readonly ILecturesRepository _lecturesRepository;
        private readonly IUserRepository _userRepository;
        private readonly ICommonRespository _commonRespository;

        public LecturesController()
        {
            this._lecturesRepository = new LecturesRepository(new NovoNordiskEntities());
            this._userRepository = new UserRepository(new NovoNordiskEntities());
            this._commonRespository = new CommonRespository(new NovoNordiskEntities());
            ViewBag.AdminMenu = "active";
            ViewBag.EventMenu = "active";
        }

        // GET: Lectures
        [HttpGet]
        public ActionResult Index(string searchString, LecturesViewModel viewModel, string sortOrder = "Id", int? page = 1, int pageSize = 10)
        {
            var model = new LecturesViewModel();

            try
            {
                ViewBag.SearchString = searchString;
                ViewBag.SortByBrand = sortOrder == "Brand" ? "Brand_Desc" : "Brand";
                ViewBag.SortByEvent = sortOrder == "Event" ? "Event_Desc" : "Event";
                ViewBag.SortByLecture = sortOrder == "Lecture" ? "Lecture_Desc" : "Lecture";
                ViewBag.SortByPackage = sortOrder == "PackageType" ? "PackageType_Desc" : "PackageType";
                ViewBag.SortByStatus = sortOrder == "Status" ? "Status_Desc" : "Status";
                ViewBag.SortOrder = sortOrder;

                model.TherapyArea = _commonRespository.GetTherapyArea().Select(z => new SelectListItem { Text = z.Name.ToString(), Value = z.ID.ToString() }).OrderBy(s => s.Text).ToList();
                model.Brand = _lecturesRepository.GetAllBrands().Select(z => new SelectListItem { Text = z.Name.ToString(), Value = z.ID.ToString() }).OrderBy(s => s.Text).ToList();
                model.Event = _lecturesRepository.GetAllEvents().Select(z => new SelectListItem { Text = z.Name.ToString(), Value = z.ID.ToString() }).OrderBy(s => s.Text).ToList();

                var allLectures = _lecturesRepository.GetAllLectures(searchString, sortOrder);

                if (viewModel.SelectedTherapyArea != null)
                {
                    if (!string.IsNullOrEmpty(viewModel.SelectedTherapyArea[0]) && viewModel.SelectedTherapyArea[0] != "0")
                    {
                        byte taId = Convert.ToByte(viewModel.SelectedTherapyArea[0]);
                        allLectures = allLectures.Where(s => s.TherapyAreaId == taId).ToList();
                    }
                }
                if (viewModel.SelectedBrand != null)
                {
                    if (!string.IsNullOrEmpty(viewModel.SelectedBrand[0]) && viewModel.SelectedBrand[0] != "0")
                    {
                        byte bId = Convert.ToByte(viewModel.SelectedBrand[0]);
                        allLectures = allLectures.Where(s => s.BrandId == bId).ToList();
                    }
                }
                if (viewModel.SelectedEvent != null)
                {
                    if (!string.IsNullOrEmpty(viewModel.SelectedEvent[0]) && viewModel.SelectedEvent[0] != "0")
                    {
                        int eId = Convert.ToInt32(viewModel.SelectedEvent[0]);
                        allLectures = allLectures.Where(s => s.EventId == eId).ToList();
                    }
                }
                model.Lectures = allLectures;
                int pageNumber = (page ?? 1);
                model.Lectures.ToPagedList(pageNumber, pageSize);
                return View(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult Index(string searchString, LecturesViewModel viewModel, string sortOrder = "Id", int? page = 1, int pageSize = 10, string action = "Post")
        {
            ViewBag.SearchString = searchString;
            ViewBag.SortByBrand = sortOrder == "Brand" ? "Brand_Desc" : "Brand";
            ViewBag.SortByEvent = sortOrder == "Event" ? "Event_Desc" : "Event";
            ViewBag.SortByLecture = sortOrder == "Lecture" ? "Lecture_Desc" : "Lecture";
            ViewBag.SortByPackage = sortOrder == "PackageType" ? "PackageType_Desc" : "PackageType";
            ViewBag.SortByStatus = sortOrder == "Status" ? "Status_Desc" : "Status";
            ViewBag.SortOrder = sortOrder;

            viewModel.TherapyArea = _commonRespository.GetTherapyArea().Select(z => new SelectListItem { Text = z.Name.ToString(), Value = z.ID.ToString() }).OrderBy(s => s.Text).ToList();
            viewModel.Brand = _lecturesRepository.GetAllBrands().Select(z => new SelectListItem { Text = z.Name.ToString(), Value = z.ID.ToString() }).OrderBy(s => s.Text).ToList();
            viewModel.Event = _lecturesRepository.GetAllEvents().Select(z => new SelectListItem { Text = z.Name.ToString(), Value = z.ID.ToString() }).OrderBy(s => s.Text).ToList();

            var allLectures = _lecturesRepository.GetAllLectures(searchString, sortOrder);
            if (viewModel.SelectedTherapyArea != null)
            {
                if (!string.IsNullOrEmpty(viewModel.SelectedTherapyArea[0]) && viewModel.SelectedTherapyArea[0] != "0")
                {
                    byte taId = Convert.ToByte(viewModel.SelectedTherapyArea[0]);
                    allLectures = allLectures.Where(s => s.TherapyAreaId == taId).ToList();
                }
            }
            if (viewModel.SelectedBrand != null)
            {
                if (!string.IsNullOrEmpty(viewModel.SelectedBrand[0]) && viewModel.SelectedBrand[0] != "0")
                {
                    byte bId = Convert.ToByte(viewModel.SelectedBrand[0]);
                    allLectures = allLectures.Where(s => s.BrandId == bId).ToList();
                }
            }
            if (viewModel.SelectedEvent != null)
            {
                if (!string.IsNullOrEmpty(viewModel.SelectedEvent[0]) && viewModel.SelectedEvent[0] != "0")
                {
                    int eId = Convert.ToInt32(viewModel.SelectedEvent[0]);
                    allLectures = allLectures.Where(s => s.EventId == eId).ToList();
                }
            }

            viewModel.Lectures = allLectures;
            int pageNumber = (page ?? 1);
            viewModel.Lectures.ToPagedList(pageNumber, pageSize);
            viewModel.SelectedTherapyArea[0] = "0";
            return View(viewModel);
        }

         /// <summary>
        /// New Lecture form view
        /// </summary>
        /// <returns></returns>       
        // GET: Lectures/Create
        public ActionResult Create()
        {
            var model = new Lectures()
            {
                TherapyArea = _commonRespository.GetTherapyArea().Select(z => new SelectListItem { Text = z.Name.ToString(), Value = z.ID.ToString() }).OrderBy(s => s.Text).ToList(),
                PackageType = _lecturesRepository.GetAllPackageType().Select(z => new SelectListItem { Text = z.Name.ToString(), Value = z.ID.ToString() }).OrderBy(s => s.Text).ToList()
            };
            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns> 
        // POST: Lectures/Create
        [HttpPost]
        public ActionResult Create(Lectures model)
        {
            string docPath = string.Empty;
            string docName = string.Empty;
            string docNamelm = string.Empty;
            int LecId = 0;

            try
            {
                if (model.Lecture != null)
                {
                    var insertLecture = new Lecture
                    {
                        Brand_Id = Convert.ToByte(model.SelectedBrand[0]),
                        Event_Id = Convert.ToInt32(model.SelectedEvent[0]),
                        Lecture1 = model.Lecture,
                        Status = model.Status,
                        Package_Type_Id = Convert.ToByte(model.SelectedPackageType[0]),
                        CreatedBy = Convert.ToInt32(Session["LoginUserId"]),
                        CreatedDate = DateTime.UtcNow,
                        ModifiedDate = DateTime.UtcNow,
                    };

                    LecId = _lecturesRepository.InsertLectures(insertLecture);
                }

                if (model.ImageFile != null && model.ImageFile[0] != null)
                {
                    foreach (var obj in model.ImageFile)
                    {
                        if (obj != null && obj.ContentLength > 0)
                        {
                            FileInfo fi = new FileInfo(obj.FileName);
                            try
                            {
                                docNamelm = Path.GetFileName(obj.FileName).Replace(" ", String.Empty);
                                docName = Path.GetFileName(Path.GetFileNameWithoutExtension(docNamelm) + DateTime.UtcNow.ToString(" - dd.MM.yyyy HH.mm.ss") + fi.Extension);
                                if (!System.IO.Directory.Exists(Server.MapPath("~/Documents/Materials/")))
                                    Directory.CreateDirectory(Server.MapPath("~/Documents/Materials/"));

                                docPath = Path.Combine(Server.MapPath("~/Documents/Materials/"), docName);
                                obj.SaveAs(docPath);
                            }
                            catch (Exception)
                            {
                                throw;
                            }
                        }

                        if (!string.IsNullOrEmpty(docNamelm) && !string.IsNullOrEmpty(docName))
                        {
                            var insertLecture = new Lecture_Materials
                            {
                                Lecture_Id = LecId,
                                Doc_Name = docName.Replace(" ", String.Empty),
                                Doc_Path = docNamelm,
                                CreatedDate = DateTime.UtcNow
                            };

                            _lecturesRepository.InsertLecturesMaterials(insertLecture);
                        }

                    }
                }
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult Edit(int id)
        {
            if (id == 0)
            {
                return RedirectToAction("Index");
            }
            else
            {
                var result = _lecturesRepository.GetLecturesById(id);

                if (result == null) { return RedirectToAction("Create"); }
                else
                {
                    var model = new Lectures()
                    {
                        ID = result.ID,
                        Lecture = result.Lecture,
                        Status = result.Status,
                        TherapyArea = _commonRespository.GetTherapyArea().Select(s => new SelectListItem { Text = s.Name.ToString(), Value = s.ID.ToString(), Selected = s.ID == result.TherapyAreaId }).OrderBy(s => s.Text).ToList(),
                        Brand = _lecturesRepository.GetAllBrands().Select(s => new SelectListItem { Text = s.Name.ToString(), Value = s.ID.ToString(), Selected = s.ID == result.BrandId }).OrderBy(s => s.Text).ToList(),
                        Event = _lecturesRepository.GetAllEvents().Select(s => new SelectListItem { Text = s.Name.ToString(), Value = s.ID.ToString(), Selected = s.ID == result.EventId }).OrderBy(s => s.Text).ToList(),
                        PackageType = _lecturesRepository.GetAllPackageType().Select(s => new SelectListItem { Text = s.Name.ToString(), Value = s.ID.ToString(), Selected = s.ID == result.PackageTypeId }).OrderBy(s => s.Text).ToList(),
                        ImagePath = result.Materials
                    };
                    return View(model);
                }
            }
        }

        // POST: Lectures/Edit
        [HttpPost]
        public ActionResult Edit(int id, Lectures model)
        {
            string docPath = string.Empty;
            string docName = string.Empty;
            string docNamelm = string.Empty;

            if (model.ImageFile != null && model.ImageFile[0] != null && model.ImageFile.Count() > 0)
            {
                foreach (var obj in model.ImageFile)
                {
                    FileInfo fi = new FileInfo(obj.FileName);
                    if (obj.ContentLength > 0)
                    {
                        try
                        {
                            docNamelm = Path.GetFileName(obj.FileName).Replace(" ", String.Empty);
                            docName = Path.GetFileName(Path.GetFileNameWithoutExtension(docNamelm) + DateTime.UtcNow.ToString(" - dd.MM.yyyy HH.mm.ss") + fi.Extension);
                            if (!System.IO.Directory.Exists(Server.MapPath("~/Documents/Materials/")))
                                Directory.CreateDirectory(Server.MapPath("~/Documents/Materials/"));

                            docPath = Path.Combine(Server.MapPath("~/Documents/Materials/"), docName);
                            obj.SaveAs(docPath);

                            var insertLecture = new Lecture
                            {
                                Lecture1 = model.Lecture,
                                Status = model.Status,
                                Package_Type_Id = Convert.ToByte(model.SelectedPackageType[0]),
                                ModifiedBy = Convert.ToInt32(Session["LoginUserId"]),
                                ModifiedDate = DateTime.UtcNow,
                            };
                            _lecturesRepository.UpdateLecture(id, insertLecture);


                            if (!string.IsNullOrEmpty(docNamelm) && !string.IsNullOrEmpty(docName))
                            {
                                var insertLectureMaterial = new Lecture_Materials
                                {
                                    Lecture_Id = id,
                                    Doc_Name = docName.Replace(" ", String.Empty),
                                    Doc_Path = docNamelm,
                                    CreatedDate = DateTime.UtcNow
                                };

                                _lecturesRepository.InsertLecturesMaterials(insertLectureMaterial);
                            }
                        }
                        catch (Exception)
                        {
                            throw;
                        }
                    }
                }
            }
            if (model.Lecture != null)
            {
                var insertLecture = new Lecture
                {
                    Lecture1 = model.Lecture,
                    Status = model.Status,
                    Package_Type_Id = Convert.ToByte(model.SelectedPackageType[0]),
                    ModifiedBy = Convert.ToInt32(Session["LoginUserId"]),
                    ModifiedDate = DateTime.UtcNow,
                };
                _lecturesRepository.UpdateLecture(id, insertLecture);
            }

            return RedirectToAction("Index");
        }

        // GET: Lectures/Delete/5
        public ActionResult Delete(int id)
        {
            var LecturesCount = _lecturesRepository.GetPAFCountByLectureId(id);

            if (LecturesCount > 0)
            {
                TempData["eventDeletecount"] = "*Could not delete the Lecture, PAFs are allocated for this Lecture.";
            }
            else
            {
                //get LectureMaterials
                var lectureMaterials = _lecturesRepository.GetLecturesMaterialsById(id);

                _lecturesRepository.DeleteLecture(id);

                if (lectureMaterials != null)
                {
                    foreach (var image in lectureMaterials)
                    {
                        if (System.IO.File.Exists(image.Doc_Path))
                        {
                            System.IO.File.Delete(image.Doc_Path);
                        }
                    }
                }
            }
            return RedirectToAction("Index");
        }

        public JsonResult GetBrandByTAId(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id)) { id = "0"; }
                var brands = _lecturesRepository.GetAllBrandsByTAId(Convert.ToByte(id)).Select(z => new SelectListItem { Text = z.Name.ToString(), Value = z.ID.ToString() }).OrderBy(s => s.Text).ToList();
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
                return Json(new SelectList(events.OrderBy(o => o.Value), "Value", "Text"));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #region Events Action Methods

        public ActionResult Event()
        {
            var model = new EventModel()
            {
                TherapyArea = _commonRespository.GetTherapyArea().Select(z => new SelectListItem { Text = z.Name.ToString(), Value = z.ID.ToString() }).OrderBy(s => s.Text).ToList()
            };
            return View(model);
        }

        [HttpPost]
        public ActionResult Event(EventModel model)
        {
            var lastEvents = _lecturesRepository.GetAllEvents().OrderByDescending(e => e.Sort_Order).ToList();

            var newEvent = new Event()
            {
                Name = model.Name,
                Brand_Id = Convert.ToByte(model.SelectedBrand[0]),
                Sort_Order = (byte)(lastEvents[0].Sort_Order + 1),
                CreatedBy = Convert.ToInt32(Session["LoginUserId"]),
                CreatedDate = DateTime.UtcNow,
                IsActive = true
            };

            _lecturesRepository.InsertEvent(newEvent);

            return RedirectToAction("Index");
        }

        // GET: Lectures/EditEvent/5
        public ActionResult EditEvent(LecturesViewModel viewModel)
        {
            if (viewModel.SelectedEvent[0] == "" || viewModel.SelectedBrand[0] == "" || viewModel.SelectedTherapyArea[0] == "")
            {
                TempData["selectEvent"] = "*Please select the Event to update.";
                return RedirectToAction("Index");
            }
            else
            {
                var eventResult = _lecturesRepository.GetEventById(Convert.ToInt32(viewModel.SelectedEvent[0]));

                if (eventResult == null) { return RedirectToAction("Index"); }
                else
                {
                    var model = new EventModel
                    {
                        ID = eventResult.ID,
                        Name = eventResult.Name,
                        IsActive = eventResult.IsActive,
                    };
                    return View(model);
                }
            }
        }

        // POST: Lectures/EditEvent/5
        [HttpPost]
        public ActionResult EditEvent(EventModel events, EventViewModel model, string IsActive)
        {
            try
            {
                if (events.Name != null)
                {
                    model.Id = events.ID;
                    model.Name = events.Name;
                    model.IsActive = (IsActive == "Active") ? true : false;

                    var eventReqModel = new Event
                    {
                        ID = model.Id,
                        Name = model.Name,
                        IsActive = model.IsActive,
                    };

                    _lecturesRepository.UpdateEvent(eventReqModel);

                    return RedirectToAction("Index");
                }
                else
                {
                    if (model.Name == "") { TempData["eventName"] = "*Event Name not be empty."; }
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult DeleteEvent(EventModel events)
        {
            try
            {
                if (events.SelectedEvent[0] == "" || events.SelectedBrand[0] == "" || events.SelectedTherapyArea[0] == "")
                {
                    TempData["eventDelete"] = "*Please select the Event to delete.";
                    return RedirectToAction("Index");
                }
                else
                {
                    var pafCount = _lecturesRepository.GetPAFCountByEventId(Convert.ToInt32(events.SelectedEvent[0]));
                    var lecturesCount = _lecturesRepository.GetLecturesCountByEventId(Convert.ToInt32(events.SelectedEvent[0]));

                    if (pafCount == 0 && lecturesCount == 0)
                    {
                        _lecturesRepository.DeleteEvent(Convert.ToInt32(events.SelectedEvent[0]));
                    }
                    else
                    {
                        if (pafCount > 0 && lecturesCount > 0)
                        {
                            TempData["eventDeletecount"] = "*Could not delete the Event, PAFs and Lectures are allocated for this Event.";
                        }
                        if (pafCount > 0 && lecturesCount == 0)
                        {
                            TempData["eventDeletecount"] = "*Could not delete the Event, PAFs are allocated for this Event.";
                        }
                        if (pafCount == 0 && lecturesCount > 0)
                        {
                            TempData["eventDeletecount"] = "*Could not delete the Event, Lectures are allocated for this Event.";
                        }
                    }
                    return RedirectToAction("Index");
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
        /// <returns></returns>
        [HttpGet]
        public ActionResult EventList(string sortOrder = "Id", string searchString = null, int? page = 1, int pageSize = 10)
        {
            ViewBag.EventMenu = "active";
            ViewBag.SearchString = searchString;
            ViewBag.SortByTherapyArea = sortOrder == "TherapyArea" ? "TherapyArea_Desc" : "TherapyArea";
            ViewBag.SortByBrand = sortOrder == "Brand" ? "Brand_Desc" : "Brand";
            ViewBag.SortByEvent = sortOrder == "Event" ? "Event_Desc" : "Event";

            var result = _lecturesRepository.GetAllEventStatus(sortOrder, searchString);
            int pageNumber = (page ?? 1);
            return View(result.ToPagedList(pageNumber, pageSize));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <param name="IsActive"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EventList(int id, bool active)
        {
            _lecturesRepository.UpdateEventStatus(id, active);
            return RedirectToAction("EventList");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Reset()
        {
            return RedirectToAction("Index");
        }
        #endregion
    }
}
