using CME.Models;

using DataAccessLayer;
using DataAccessLayer.DAL.ProfileRepository;

using PagedList;

using System;
using System.Linq;
using System.Web.Mvc;

namespace CME.Controllers
{
    public class ProfileController : Controller
    {
        private readonly IProfileRepository _profileRepository;

        public ProfileController()
        {
            this._profileRepository = new ProfileRepository(new NovoNordiskEntities());
            ViewBag.AdminMenu = "active";
            ViewBag.FunctionalMenu = "active";
        }

        // GET: Profile
        public ActionResult Index(string searchString, string sortOrder = "Id", int? page = 1, int pageSize = 10)
        {
            try
            {
                ViewBag.SearchString = searchString;
                ViewBag.SortByName = sortOrder == "Name" ? "Name_Desc" : "Name";
                ViewBag.SortByDescription = sortOrder == "Description" ? "Description_Desc" : "Description";
                ViewBag.SortOrder = sortOrder;
                int pageNumber = (page ?? 1);

                var allProfiles = _profileRepository.GetProfiles(searchString, sortOrder);

                var profileViewModels = allProfiles.Select(profile => new ProfileViewModel
                {
                    Id = profile.ID,
                    Name = profile.Name,
                    Description = profile.Description
                }).ToList();

                var result = profileViewModels.ToPagedList(pageNumber, pageSize);
                return View(result);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // GET: Profile/Create
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Profile/Create
        [HttpPost]
        public ActionResult Create(ProfileViewModel model)
        {
            try
            {
                byte sortOrder;
                using (NovoNordiskEntities context = new NovoNordiskEntities())
                {
                    var obj = context.Profiles.OrderByDescending(o => o.Sort_Order).FirstOrDefault();
                    sortOrder = obj != null ? Convert.ToByte(obj.Sort_Order + 1) : Convert.ToByte(1);
                    context.Dispose();
                }

                if (ModelState.IsValid)
                {
                    Profile profile = new Profile()
                    {
                        Name = model.Name,
                        Description = model.Description,
                        Sort_Order = sortOrder,
                        CreatedDate = DateTime.Now,
                    };

                    _profileRepository.InsertProfiles(profile);
                    _profileRepository.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                    return View(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // GET: Profile/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }
            else
            {
                var userProfile = _profileRepository.GetProfileById(id);
                if (userProfile == null)
                {
                    return RedirectToAction("Create");
                }
                else
                {
                    return View(new ProfileViewModel { Id = Convert.ToByte(id), Name = userProfile.Name, Description = userProfile.Description });
                }
            }
        }

        // POST: Profile/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, ProfileViewModel model)
        {
            try
            {
                _profileRepository.UpdateProfile(id, new Profile
                {
                    Name = model.Name,
                    Description = model.Description,
                });

                return RedirectToAction("Index");
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
        public ActionResult Delete(int id)
        {
            try
            {
                _profileRepository.DeleteProfile(id);
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
    }
}
