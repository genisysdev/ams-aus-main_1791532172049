//using CME.Infrastructure;
//using CME.Models;
//using DataAccessLayer;
//using DataAccessLayer.DAL.ConfigRepository;
//using DataAccessLayer.DAL.Model;
//using PagedList;
//using System;
//using System.Linq;
//using System.Web.Mvc;

//namespace CME.Controllers
//{
//    [CustomAuthenticationFilter]
//    [CustomAuthorize("Admin")]
//    [HandleError()]
//    public class ConfigController : Controller
//    {
//        private readonly IConfigRepository _configRepository;

//        public ConfigController()
//        {
//            this._configRepository = new ConfigRepository(new NovoNordiskEntities());
//            ViewBag.AdminMenu = "active";
//            ViewBag.ConfigMenu = "active";
//        }

//        // GET: Config
//        public ActionResult Index(string sortOrder = "Id", string searchString = null, int? page = 1, int pageSize = 10)
//        {
//            try
//            {
//                ViewBag.SearchString = searchString;
//                ViewBag.SortByName = sortOrder == "Name" ? "Name_Desc" : "Name";
//                ViewBag.SortByDescription = sortOrder == "Description" ? "Description_Desc" : "Description";
//                int pageNumber = (page ?? 1);

//                var list = _configRepository.GetAll();

//                var configList = list.Select(config => new AMSConfigViewModel
//                {
//                    Name = config.Name,
//                    Value = config.Value,
//                    Description = config.Description,
//                    Status = config.Status
//                }).ToList();

//                var result = configList.ToPagedList(pageNumber, pageSize);
//                return View(result);
//            }
//            catch (Exception ex) 
//            {
//                throw ex;
//            }
//        }

//        // GET: Config/Create
//        public ActionResult Create()
//        {
//            return View();
//        }

//        // POST: Config/Create
//        [HttpPost]
//        public ActionResult Create(AMS_Config model)
//        {
//            try
//            {
//                var newConfig = new AMS_Config()
//                {
//                    Name = model.Name,
//                    Value = model.Value,
//                    Description = model.Description,
//                    Status = model.Status
//                };

//                _configRepository.InsertAMS_Config(model);
//                return RedirectToAction("Index");
//            }
//            catch(Exception ex)
//            {
//                ViewBag.ErrorMessage = "Please enter unique name";
//                throw ex;
//            }
//        }

//       /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="name"></param>
//        /// <returns></returns>
//        public ActionResult Edit(string name)
//        {
//            var viewModel = new ConfigViewModel();
//            if (string.IsNullOrEmpty(name))
//            {
//                return RedirectToAction("Index");
//            }
//            else
//            {
//                var configDetail = _configRepository.GetConfigByName(name);
//                viewModel = new ConfigViewModel()
//                {
//                    Name = configDetail.Name,
//                    Value = configDetail.Value,
//                    Description = configDetail.Description,
//                    Status = (configDetail.Status == null ? false : Convert.ToBoolean(configDetail.Status))
//                };
//            }
//            return View(viewModel);
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="model"></param>
//        /// <returns></returns>
//        [HttpPost]
//        public ActionResult Edit(ConfigViewModel model)
//        {
//            try
//            {
//                var configDetail = new AMS_Config()
//                {
//                    Name = model.Name,
//                    Value = model.Value,
//                    Description = model.Description,
//                    Status = model.Status
//                };

//                _configRepository.UpdateConfig(configDetail);
//                return RedirectToAction("Index");
//            }
//            catch(Exception ex)
//            {
//                throw ex;
//            }
//        }
//    }
//}
