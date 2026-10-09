using CME.Models;
using DataAccessLayer;
using DataAccessLayer.DAL.CommonRespository;
using DataAccessLayer.DAL.HCPListRepository;
using DataAccessLayer.DAL.Model;
using DataAccessLayer.DAL.UserRepository;
using PagedList;
using System;
using System.Linq;
using System.Web.Mvc;

namespace CME.Controllers
{
    public class HCPListController : Controller
    {
        private readonly IHCPListRepository _hCPListRepository;
        private readonly IUserRepository _userRepository;
        private readonly ICommonRespository _commonRespository;

        public HCPListController()
        {
            this._hCPListRepository = new HCPListRepository(new NovoNordiskEntities());
            this._userRepository = new UserRepository(new NovoNordiskEntities());
            this._commonRespository = new CommonRespository(new NovoNordiskEntities());
            ViewBag.HCPListMenu = "active";
        }

        // GET: HCPList
        public ActionResult Index(string searchString, string searchStringCN, string sortOrder = "Id", int? page = 1, int pageSize = 10)
        {
            ViewBag.SearchString = searchString;
            ViewBag.SearchCN = searchStringCN;
            ViewBag.SortByVendor = sortOrder == "Vendor" ? "Vendor_Desc" : "Vendor";
            ViewBag.SortByTMF = sortOrder == "TMF" ? "TMF_Desc" : "TMF";
            ViewBag.SortByFName = sortOrder == "FName" ? "Name_Desc" : "Name";
            ViewBag.SortByLName = sortOrder == "LName" ? "Name_Desc" : "Name";
            ViewBag.SortByCoupaNumber = sortOrder == "CoupaNumber" ? "CoupaNumber_Desc" : "CoupaNumber";
            ViewBag.SortOrder = sortOrder;

            var allHcpList = _hCPListRepository.GetAll(searchString, searchStringCN, sortOrder, null);

            var hcpList = allHcpList.Select(h => new HCPViewModel
            {
                ID = h.ID,
                VendorNo = h.VendorNo,
                TMFNo = h.TMFNo,
                FirstName = h.FirstName,
                LastName = h.LastName,
                TAName = h.TAName,
                CMSNo = h.CMSNo,
                HCPAddress = h.HCPAddress,
                Email_Id = h.Email_Id,
                Phone_Number = h.Phone_Number,
                City = h.City,
                ContractStartDate = h.ContractStartDate,
                ContractEndDate = h.ContractEndDate
            }).ToList();

            int pageNumber = (page ?? 1);
            var result = hcpList.ToPagedList(pageNumber, pageSize);
            return View(result);
        }

        // GET: HCPList/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: HCPList/Create
        public ActionResult Create(string searchString, string customerID)
        {
            try
            {
                if (string.IsNullOrEmpty(searchString) && string.IsNullOrEmpty(customerID))
                {
                    var model = CreateHCPViewModel();
                    return View(model);
                }
                else
                {
                    string searchStringCN = null;
                    string sortOrder = "Id";
                    var allHcpList = _hCPListRepository.GetAll(searchString, searchStringCN, sortOrder, customerID);

                    var hcpList = allHcpList.Select(h => new HCPViewModel
                    {
                        ID = h.ID,
                        VendorNo = h.VendorNo,
                        TMFNo = h.TMFNo,
                        FirstName = h.FirstName,
                        LastName = h.LastName,
                        TAName = h.TAName,
                        CMSNo = h.CMSNo,
                        HCPAddress = h.HCPAddress,
                        Email_Id = h.Email_Id,
                        Phone_Number = h.Phone_Number,
                        City = h.City,
                        ContractStartDate = h.ContractStartDate,
                        ContractEndDate = h.ContractEndDate
                    }).ToList();

                    if (hcpList == null || !hcpList.Any())
                    {
                        var model = CreateHCPViewModel();
                        return RedirectToAction("Create");
                    }
                    else if (hcpList.Count() != 1)
                    {
                        var model = CreateHCPViewModel();
                        model.SearchNameList = hcpList;
                        return View(model);
                    }
                    else
                    {
                        var singleHcp = hcpList.First();
                        var model = CreateHCPViewModel(singleHcp);
                        return View(model);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private HCPView CreateHCPViewModel(HCPViewModel singleHcp = null)
        {
            var model = new HCPView()
            {
                TherapyArea = _commonRespository.GetTherapyArea().Select(z => new SelectListItem
                {
                    Text = z.Name.ToString(),
                    Value = z.ID.ToString(),
                    Selected = singleHcp != null && z.Name == singleHcp.TAName
                }).OrderBy(s => s.Text).ToList(),
            };

            if (singleHcp != null)
            {
                model.VendorNo = singleHcp.VendorNo;
                model.TMFNo = singleHcp.TMFNo;
                model.CMSNo = !string.IsNullOrEmpty(singleHcp.CMSNo) ? Convert.ToInt32(singleHcp.CMSNo) : 0;
                model.FirstName = singleHcp.FirstName;
                model.LastName = singleHcp.LastName;
                model.HCPAddress = singleHcp.HCPAddress;
                model.Email_Id = singleHcp.Email_Id;
                model.Phone_Number = singleHcp.Phone_Number;
                model.City = singleHcp.City;
                model.Customer_ID = singleHcp.Customer_ID;
                model.Id = singleHcp.ID;
                model.ContractStartDate = singleHcp.ContractStartDate;
                model.ContractEndDate = singleHcp.ContractEndDate;
            }

            return model;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        // POST: HCPList/Create
        [HttpPost]
        public ActionResult Create(HCPView model, string command)
        {
            try
            {
                var errors = ModelState.Select(x => x.Value.Errors).Where(y => y.Count > 0).ToList();

                if (command != "update")
                {
                    if (errors.Count > 0)
                    {
                        model = CreateHCPViewModel();
                        return View(model);
                    }
                    else
                    {
                        var allHcpList = _hCPListRepository.GetAll();

                        if (allHcpList.Where(s => s.CMSNo == model.CMSNo.ToString()).Count() == 0)
                        {
                            var newHCP = new HCP()
                            {
                                Vendor_No = model.VendorNo,
                                TMF_No = model.TMFNo,
                                Coupa_No = model.CMSNo.ToString(),
                                First_Name = model.FirstName,
                                Last_Name = model.LastName,
                                TA_Id = Convert.ToByte(model.SelectedUnit[0]),
                                CreatedDate = DateTime.UtcNow,
                                ModifiedDate = DateTime.UtcNow,
                                Address = model.HCPAddress,
                                Email_Id = model.Email_Id,
                                Phone_Number = model.Phone_Number,
                                Is_Contract = model.Is_Contract,
                                City = model.City,
                                Contract_Start_Date = model.ContractStartDate,
                                Contract_End_Date = model.ContractEndDate

                            };

                            _hCPListRepository.InsertHCP(newHCP);
                        }
                        else
                        {
                            model = CreateHCPViewModel();
                            TempData["hcpCMENo"] = "* HCP Coupa Number is Unique, Please select another Coupa Number";
                            return View(model);
                        }
                    }
                }
                else
                {
                    if (errors.Count > 0 || model.SelectedUnit[0] == "")
                    {
                        model = CreateHCPViewModel();
                        return View(model);
                    }
                    else
                    {
                        if (model.Id != 0)
                        {
                            var allHcpList = _hCPListRepository.GetAll();
                            var hcpDetail = _hCPListRepository.GetHCPById(model.Id);

                            if (hcpDetail.CMSNo == model.CMSNo.ToString() || allHcpList.Where(s => s.CMSNo == model.CMSNo.ToString()).Count() == 0)
                            {
                                var newHcp = new HCPViewModel()
                                {
                                    ID = model.Id,
                                    VendorNo = model.VendorNo,
                                    TMFNo = model.TMFNo,
                                    FirstName = model.FirstName,
                                    LastName = model.LastName,
                                    TAId = Convert.ToByte(model.SelectedUnit[0]),
                                    CMSNo = Convert.ToString(model.CMSNo),
                                    Email_Id = model.Email_Id,
                                    Phone_Number = model.Phone_Number,
                                    Is_Contract = true,
                                    Customer_ID = model.Customer_ID,
                                    City = model.City,
                                    ContractStartDate = model.ContractStartDate,
                                    ContractEndDate = model.ContractEndDate
                                };

                                _hCPListRepository.UpdateHCP(model.Id, newHcp);
                                return RedirectToAction("Index");
                            }
                            else
                            {
                                model = CreateHCPViewModel();
                                TempData["hcpCMENo"] = "* HCP Coupa Number is Unique, Please select another Coupa Number";
                                return View(model);
                            }
                        }
                        else
                        {
                            model = CreateHCPViewModel();
                            return View(model);
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

        public ActionResult Reset()
        {
            return RedirectToAction("Index");
        }

        // GET: HCPList/Delete/5
        public ActionResult Delete(int id)
        {
            _hCPListRepository.DeactivateHCP(id);
            return RedirectToAction("Index");
        }
    }
}
