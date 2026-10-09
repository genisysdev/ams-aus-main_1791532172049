using DataAccessLayer.DAL.Model;
using System.Collections.Generic;

namespace DataAccessLayer.DAL.ProfileRepository
{
    public interface IProfileRepository
    {
        IEnumerable<Profile> GetProfiles(string searchString = null, string sortOrder = null);
        IEnumerable<ProfileMapModel> GetProfilesMap();
        Profile GetProfileById(int? id);
        List<ProfileMapModel> GetProfileMapModelById(int? id);
        Profiles_To_User GetProfileMapById(int? id);
        void InsertProfiles(Profile reqModel);
        void DeleteProfile(int id);
        void UpdateProfile(int id, Profile updatedProfile);
        void SaveChanges();
    }
}
