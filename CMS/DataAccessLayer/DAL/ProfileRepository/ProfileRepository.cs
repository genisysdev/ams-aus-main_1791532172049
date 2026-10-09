using DataAccessLayer.DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataAccessLayer.DAL.ProfileRepository
{
    public class ProfileRepository : IProfileRepository
    {
        private readonly NovoNordiskEntities _dataContext;
        public ProfileRepository(NovoNordiskEntities dataContext)
        {
            this._dataContext = dataContext;
        }

        public IEnumerable<Profile> GetProfiles(string searchString = null, string sortOrder = null)
        {
            var result = _dataContext.Profiles.OrderBy(o => o.ID).ToList();
            if (!string.IsNullOrEmpty(searchString))
                {
                    result = result.Where(s => s.Name.ToLower().Contains(searchString.ToLower())).ToList();
                }
                switch (sortOrder)
                {
                    case "name":
                        result = result.OrderBy(s => s.Name).ToList();
                        break;
                    case "name_desc":
                        result = result.OrderByDescending(s => s.Name).ToList();
                        break;
                    case "description":
                        result = result.OrderBy(s => s.Description).ToList();
                        break;
                    case "description_desc":
                        result = result.OrderByDescending(s => s.Description).ToList();
                        break;
                    default:
                        result = result.OrderByDescending(s => s.ID).ToList();
                        break;
                }
                return result;
        }

        public IEnumerable<ProfileMapModel> GetProfilesMap()
        {
            try
            {
                var response = new List<ProfileMapModel>();
                using (NovoNordiskEntities context = new NovoNordiskEntities())
                {
                    var result = (from pu in context.Profiles_To_User
                                  join p in context.Profiles on pu.Profile_Id equals p.ID
                                  join u in context.Users on pu.User_Id equals u.ID
                                  where pu.Status == true
                                  select new ProfileMapModel
                                  {
                                      Id = pu.ID,
                                      ProfileId = pu.Profile_Id,
                                      ProfileName = p.Name,
                                      UserInitial = u.Initials
                                  }
                               ).OrderByDescending(o => o.Id).ToList();
                    context.Dispose();

                    var groupedResult = result.GroupBy(s => s.ProfileId);
                    foreach (var obj in groupedResult)
                    {
                        StringBuilder names = new StringBuilder();
                        ProfileMapModel sfm = obj.OrderByDescending(o => o.Id).FirstOrDefault();
                        foreach (ProfileMapModel o in obj)
                        {
                            names.Append(o.UserInitial + ", ");
                        }

                        ProfileMapModel fm = new ProfileMapModel
                        {
                            Id = sfm.Id,
                            ProfileId = sfm.ProfileId,
                            ProfileName = sfm.ProfileName,
                            UserInitial = names.ToString().Remove(names.ToString().Length - 2)
                        };
                        response.Add(fm);
                    }
                    return response.OrderByDescending(o => o.Id).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Profile GetProfileById(int? id)
        {
            try
            {
                var result = _dataContext.Profiles.Find(id);
                return result != null ? new Profile { ID = result.ID, Name = result.Name, Description = result.Description } : null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Profiles_To_User GetProfileMapById(int? id)
        {
            try
            {
                var result = _dataContext.Profiles_To_User.Find(id);
                return result != null ? new Profiles_To_User { ID = result.ID, Profile_Id = result.Profile_Id, User_Id = result.User_Id, Status = result.Status } : null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<ProfileMapModel> GetProfileMapModelById(int? id)
        {
            try
            {
                var result = new List<ProfileMapModel>();
                using (NovoNordiskEntities context = new NovoNordiskEntities())
                {
                    var profileDetail = GetProfileMapById(id);
                    if (profileDetail != null)
                    {
                        result = (from fm in context.Profiles_To_User
                                      join f in context.Profiles on fm.Profile_Id equals f.ID
                                      join u in context.Users on fm.User_Id equals u.ID
                                      where fm.Profile_Id == id
                                      where fm.Status == true
                                      select new ProfileMapModel
                                      {
                                          Id = fm.ID,
                                          ProfileId = fm.Profile_Id,
                                          ProfileName = f.Name,
                                          UserInitial = u.Initials
                                      }).ToList();
                        context.Dispose();
                        
                    }                    
                }
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reqModel"></param>
        public void InsertProfiles(Profile reqModel)
        {
            _dataContext.Profiles.Add(reqModel);
        }

        public void UpdateProfile(int id, Profile updatedProfile)
        {
            var existingProfile = _dataContext.Profiles.Find(id);

            if (existingProfile != null)
            {
                existingProfile.Name = updatedProfile.Name;
                existingProfile.Description = updatedProfile.Description;
                _dataContext.SaveChanges();
            }
        }

        public void DeleteProfile(int id)
        {
            var userProfiles = _dataContext.Profiles_To_User.Where(up => up.ID == id).ToList();

            if (userProfiles.Any())
            {
                _dataContext.Profiles_To_User.RemoveRange(userProfiles);
            }

            var profile = _dataContext.Profiles.Find(id);

            if (profile != null)
            {
                _dataContext.Profiles.Remove(profile);
            }

            _dataContext.SaveChanges();
        }
        /// <summary>
        /// 
        /// </summary>
        public void SaveChanges()
        {
            _dataContext.SaveChanges();
        }
    }
}
