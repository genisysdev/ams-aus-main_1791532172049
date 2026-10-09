using CME.Models;
using DataAccessLayer;
using DataAccessLayer.DAL.ProfileRepository;
using DataAccessLayer.DAL.UserRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CME.Infrastructure;

namespace CME.Controllers
{
    [CustomAuthenticationFilter]
    [CustomAuthorize("Admin")]
    [HandleError()]
    public class ProfileMapController : Controller
    {
        private readonly IProfileRepository _profileRepository;
        private readonly IUserRepository _userRepository;

        public ProfileMapController()
        {
            this._profileRepository = new ProfileRepository(new NovoNordiskEntities());
            this._userRepository = new UserRepository(new NovoNordiskEntities());
            ViewBag.AdminMenu = "active";
            ViewBag.FunctionalMenu = "active";
        }

        // GET: ProfileMap
        public ActionResult Index()
        {
            var profilesMapModel = new ProfileMapViewModel()
            {
                Profile = _profileRepository.GetProfiles().Select(z => new SelectListItem { Text = z.Name.ToString(), Value = z.ID.ToString() }).OrderBy(s => s.Text).ToList(),
                User = _userRepository.GetAllUsers().Select(z => new SelectListItem { Text = z.Initials.ToString(), Value = z.ID.ToString() }).OrderBy(s => s.Text).ToList(),
                ProfileMapList = _profileRepository.GetProfilesMap().ToList()
            };

            return View(profilesMapModel);
        }

        [HttpPost]
        public ActionResult Index(ProfileMapViewModel model, string command)
        {
            try
            {
                if (command == "Save")
                {
                    model.LogginUserId = Convert.ToInt32(Session["LoginUserId"]);

                    byte profileId = Convert.ToByte(model.SelectedProfile[0]);
                    int userId = Convert.ToInt32(model.SelectedUser[0]);

                    using (NovoNordiskEntities context = new NovoNordiskEntities())
                    {
                        var user = context.Users.FirstOrDefault(s => s.ID == userId);
                        if (user != null)
                        {
                            var existingProfilesToUser = _userRepository.GetProfilesToUserByUserId(userId);
                            _userRepository.RemoveProfilesToUser(existingProfilesToUser);

                            var newProfileToUser = new Profiles_To_User
                            {
                                Profile_Id = profileId,
                                User_Id = userId,
                                Status = true,
                                CreatedBy = user.CreatedBy,
                                CreatedDate = DateTime.UtcNow,
                                ModifiedBy = model.LogginUserId,
                                ModifiedDate = DateTime.UtcNow
                            };

                            _userRepository.InsertProfileToUser(newProfileToUser);
                        }
                    }


                    ProfileMapViewModel viewModel = new ProfileMapViewModel
                    {
                        Profile = _profileRepository.GetProfiles().Select(z => new SelectListItem { Text = z.Name.ToString(), Value = z.ID.ToString() }).OrderBy(s => s.Text).ToList(),
                        User = _userRepository.GetAllUsers().Select(z => new SelectListItem { Text = z.Initials.ToString(), Value = z.ID.ToString() }).OrderBy(s => s.Text).ToList(),
                        ProfileMapList = _profileRepository.GetProfilesMap().ToList()
                    };
                    return View(viewModel);
                }
                else
                {
                    return View("Edit");
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }            
        }

        // GET: ProfileMap/Edit/5
        public ActionResult Edit(int id)
        {
            try
            {
                var result = new ProfileMapViewModel();
                var profileDetail = _profileRepository.GetProfileMapById(id); //profile_to_user
                var ProfileMapList = _profileRepository.GetProfileMapModelById(id);//ProfileMapModel
                var profileList = _profileRepository.GetProfiles();

                result.SelectedProfile = new List<string> { Convert.ToString(profileDetail.Profile_Id) };
                result.SelectedProfileMapId = profileDetail.Profile_Id;
                result.ProfileMapList = ProfileMapList;

                result.ProfileList = new SelectList(profileList, "Id","Name");
                result.User = _userRepository.GetAllUsers().Select(z => new SelectListItem { Text = z.Initials.ToString(), Value = z.ID.ToString() }).OrderBy(s => s.Text).ToList();

                return View(result);
            }
            catch (Exception)
            {
                throw;
            }
        }

        // POST: ProfileMap/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, ProfileMapViewModel model)
        {
            try
            {
                model.LogginUserId = Convert.ToInt32(Session["LoginUserId"]);
                model.SelectedProfileMapId = Convert.ToByte(id);
                byte profileId = model.SelectedProfile != null ? Convert.ToByte(model.SelectedProfile[0]) : model.SelectedProfileMapId;
                int userId = Convert.ToInt32(model.SelectedUser[0]);

                using (NovoNordiskEntities context = new NovoNordiskEntities())
                {
                    var user = context.Users.FirstOrDefault(s => s.ID == userId);
                    if (user != null)
                    {
                        var existingProfilesToUser = _userRepository.GetProfilesToUserByUserId(userId);
                        _userRepository.RemoveProfilesToUser(existingProfilesToUser);

                        var newProfileToUser = new Profiles_To_User
                        {
                            Profile_Id = profileId,
                            User_Id = userId,
                            Status = true,
                            CreatedBy = user.CreatedBy,
                            CreatedDate = DateTime.UtcNow,
                            ModifiedBy = model.LogginUserId,
                            ModifiedDate = DateTime.UtcNow
                        };

                        _userRepository.InsertProfileToUser(newProfileToUser);
                    }
                }
                return RedirectToAction("Edit", new { id });
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Web API to deactivate profile map details
        /// </summary>
        /// <param name="id"></param>
        // POST: api/ProfileMap/Deactivate/1
        public ActionResult Deactivate(int id)
        {
            _userRepository.DeactivateProfiles(id);
            return RedirectToAction("Edit", new { id = id });
        }

        /// <summary>
        /// Web API to delete profile map details
        /// </summary>
        /// <param name="fmId"></param>
        // POST: api/ProfileMap/Delete/1
        public ActionResult Delete(byte fmId)
        {
            _userRepository.DeleteProfiles(fmId);
             return RedirectToAction("Index");
        }
    }
}

