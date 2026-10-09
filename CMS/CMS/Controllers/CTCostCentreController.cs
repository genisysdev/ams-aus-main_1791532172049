using DataAccessLayer.DAL.Model;
using DataAccessLayer;
using DataAccessLayer.DAL.CostCentreRepository;
using DataAccessLayer.DAL.CTCostCentreRepository;
using DataAccessLayer.DAL.UserRepository;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace CME.Controllers
{
    public class CTCostCentreController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly ICostCentreRepository _costCentreRepository;
        private readonly ICTCostCentreMapRepository _cTCostCentreMapRepository;

        public CTCostCentreController()
        {
            this._userRepository = new UserRepository(new NovoNordiskEntities());
            this._costCentreRepository = new CostCentreRepository(new NovoNordiskEntities());
            this._cTCostCentreMapRepository = new CTCostCentreMapRepository(new NovoNordiskEntities());
            ViewBag.AdminMenu = "active";
            ViewBag.CTCostCenterMenu = "active";
        }

        // GET: CTToCostCentre
        public ActionResult Index()
        {
            var ccMapData = _costCentreRepository.GetAllCostCentre().Select(z => new SelectListItem { Text = z.Code.ToString(), Value = z.ID.ToString() }).OrderBy(s => s.Text).ToList();
            ccMapData.Insert(0, new SelectListItem() { Value = "0", Text = "Select all" });

            var model = new CostCentreMapView()
            {
                CostCenter = ccMapData.ToList(),
                User = _userRepository.GetAllCTToUserMapping().Select(z => new SelectListItem { Text = z.Initial.ToString(), Value = z.ID.ToString() }).OrderBy(s => s.Text).ToList(),
                CostCenterName = _costCentreRepository.GetAllCostCentre().Select(z => new SelectListItem { Text = z.Name.ToString(), Value = z.ID.ToString() }).OrderBy(s => s.Text).ToList(),
                CTCCList = _cTCostCentreMapRepository.GetAllCostCentre_To_CT().ToList(),
            };

            return View(model);
        }

        //Post CTToCostCentre
        [HttpPost]
        public ActionResult Index(CostCentreMapView model, string command)
        {
            try
            {
                var listView = new CostCentreMapView();
                if (command == "Save")
                {
                    model.LogginUserId = Convert.ToInt32(Session["LoginUserId"]);

                    CostCentre_To_CT puResult = null;
                    using (NovoNordiskEntities context = new NovoNordiskEntities())
                    {
                        Int32 CostCenterID = Convert.ToInt32(model.SelectedCostCenter[0]);
                        Int32 userId = Convert.ToInt32(model.SelectedUser[0]);
                        puResult = context.CostCentre_To_CT.Where(s => s.CostCentre_Id == CostCenterID && s.User_Id == userId).FirstOrDefault();
                        var user = context.Users.Where(s => s.ID == userId).FirstOrDefault();

                        if (userId != 0)
                        {
                            if (CostCenterID != 0)
                            {
                                var list = context.CostCentre_To_CT.Where(s => s.CostCentre_Id == CostCenterID && s.User_Id == userId && s.Status == true).ToList();
                                foreach (var obj in list)
                                {
                                    context.CostCentre_To_CT.Remove(obj);
                                    context.SaveChanges();
                                }
                                var saveCCDetails = new CostCentre_To_CT()
                                {
                                    CostCentre_Id = CostCenterID,
                                    User_Id = userId,
                                    Status = true,
                                    CreatedBy = model.LogginUserId,
                                    CreatedDate = DateTime.UtcNow,
                                    ModifiedBy = user.CreatedBy,
                                    ModifiedDate = DateTime.UtcNow
                                };

                                _cTCostCentreMapRepository.InsertCostCentreToCT(saveCCDetails);
                            }
                            else
                            {
                                var list = context.CostCentre_To_CT.Where(s => s.User_Id == userId && s.Status == true).ToList();
                                foreach (var a in list)
                                {
                                    context.CostCentre_To_CT.Remove(a);
                                    context.SaveChanges();
                                }
                                foreach (var obj in context.Cost_Centre)
                                {
                                    var saveCCDetails = new CostCentre_To_CT()
                                    {
                                        CostCentre_Id = obj.ID,
                                        User_Id = userId,
                                        Status = true,
                                        CreatedBy = model.LogginUserId,
                                        CreatedDate = DateTime.UtcNow,
                                        ModifiedBy = user.CreatedBy,
                                        ModifiedDate = DateTime.UtcNow
                                    };

                                    _cTCostCentreMapRepository.InsertCostCentreToCT(saveCCDetails);
                                }
                            }
                        }
                        context.Dispose();

                    }

                    var ccMapData = _cTCostCentreMapRepository.GetAllCostCentre_To_CT().Select(z => new SelectListItem { Text = z.CostCenter_Code.ToString(), Value = z.Id.ToString() }).OrderBy(s => s.Text).ToList();
                    ccMapData.Insert(0, new SelectListItem() { Value = "0", Text = "Select all" });
                    listView = new CostCentreMapView()
                    {
                        CostCenter = ccMapData.ToList(),
                        User = _userRepository.GetAllCTToUserMapping().Select(z => new SelectListItem { Text = z.Initial.ToString(), Value = z.ID.ToString() }).OrderBy(s => s.Text).ToList(),
                        CostCenterName = _costCentreRepository.GetAllCostCentre().Select(z => new SelectListItem { Text = z.Name.ToString(), Value = z.ID.ToString() }).OrderBy(s => s.Text).ToList(),
                        CTCCList = _cTCostCentreMapRepository.GetAllCostCentre_To_CT().ToList(),
                    };
                }
                else
                {
                    return View("Edit");
                }
                return View(listView);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public ActionResult Edit(int id)
        {
            try
            {
                var ccDetail = _cTCostCentreMapRepository.GetById(id);
                var userList = _userRepository.GetAllCTToUserMapping().Select(z => new SelectListItem { Text = z.Initial.ToString(), Value = z.ID.ToString() }).OrderBy(s => s.Text).ToList();
                ccDetail.UserList = new SelectList(userList.OrderBy(o => o.Value), "Id", "Initial");
                var ccMapData = _cTCostCentreMapRepository.GetAllCostCentre_To_CT().Select(z => new SelectListItem { Text = z.CostCenter_Code.ToString(), Value = z.Id.ToString() }).OrderBy(s => s.Text).ToList();
                ccMapData.Insert(0, new SelectListItem() { Value = "0", Text = "Select all" });
                ccDetail.CostCenter = ccMapData.ToList();
                ccDetail.CostCenterName = _costCentreRepository.GetAllCostCentre().Select(z => new SelectListItem { Text = z.Name.ToString(), Value = z.ID.ToString() }).OrderBy(s => s.Text).ToList();

                return View(ccDetail);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult Edit(int id, CostCentreMapView model)
        {
            try
            {
                model.LogginUserId = Convert.ToInt32(Session["LoginUserId"]);
                model.selectedUserId = Convert.ToInt32(id);

                using (NovoNordiskEntities context = new NovoNordiskEntities())
                {
                    Int32 costCenterID = Convert.ToInt32(model.SelectedCostCenter[0]);
                    var result = context.CostCentre_To_CT.Where(s => s.CostCentre_Id == costCenterID && s.User_Id == model.selectedUserId).FirstOrDefault();
                    var user = context.Users.Where(s => s.ID == model.selectedUserId).FirstOrDefault();

                    if (model.selectedUserId != 0)
                    {
                        if (costCenterID != 0)
                        {
                            List<CostCentre_To_CT> list = context.CostCentre_To_CT.Where(s => s.CostCentre_Id == costCenterID && s.User_Id == model.selectedUserId && s.Status == true).ToList<CostCentre_To_CT>();
                            foreach (CostCentre_To_CT obj in list)
                            {
                                context.CostCentre_To_CT.Remove(obj);
                                context.SaveChanges();
                            }
                            var saveCCDetails = new CostCentre_To_CT()
                            {
                                CostCentre_Id = costCenterID,
                                User_Id = model.selectedUserId,
                                Status = true,
                                CreatedBy = model.LogginUserId,
                                CreatedDate = DateTime.UtcNow,
                                ModifiedBy = user.CreatedBy,
                                ModifiedDate = DateTime.UtcNow
                            };

                            _cTCostCentreMapRepository.InsertCostCentreToCT(saveCCDetails);
                        }
                        else
                        {
                            var list = context.CostCentre_To_CT.Where(s => s.User_Id == model.selectedUserId && s.Status == true).ToList();
                            foreach (CostCentre_To_CT a in list)
                            {
                                context.CostCentre_To_CT.Remove(a);
                                context.SaveChanges();
                            }
                            foreach (var obj in context.Cost_Centre)
                            {
                                var saveCCDetails = new CostCentre_To_CT()
                                {
                                    CostCentre_Id = obj.ID,
                                    User_Id = model.selectedUserId,
                                    Status = true,
                                    CreatedBy = model.LogginUserId,
                                    CreatedDate = DateTime.UtcNow,
                                    ModifiedBy = user.CreatedBy,
                                    ModifiedDate = DateTime.UtcNow
                                };

                                _cTCostCentreMapRepository.InsertCostCentreToCT(saveCCDetails);
                            }
                        }
                    }
                    context.Dispose();
                }

                return RedirectToAction("Edit", new { id = id });
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ActionResult Deactivate(int id, int? UserId)
        {
            _cTCostCentreMapRepository.DeactivateCTToUserMapping(id);
            return RedirectToAction("Edit", new { id = UserId });
        }

        public ActionResult Delete(int id)
        {
            _cTCostCentreMapRepository.DeleteCTToUserMapping(id);
            return RedirectToAction("Index");
        }

        public JsonResult GetCostcenter(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id)) { id = "0"; }
                var ccdata = _costCentreRepository.GetAllCostCentre().Select(z => new SelectListItem { Text = z.Name.ToString(), Value = z.ID.ToString() }).OrderBy(s => s.Text).ToList();
                return Json(new SelectList(ccdata.OrderBy(o => o.Value), "Value", "Text"));
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}