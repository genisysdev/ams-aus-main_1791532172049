using DataAccessLayer.DAL.Model;
using System.Collections.Generic;
using System.Data;

namespace DataAccessLayer.DAL.UserRepository
{
    public interface IUserRepository
    {
        IEnumerable<User> GetAllUsers();       
        List<UserModel> GetUsers(string initial, string surName, string costCenter, string manager, string name, string sortOrder);
        int InsertUser(User reqModel);
        void InsertProfileToUser(Profiles_To_User reqModel);
        List<Profiles_To_User> GetProfilesToUserByUserId(int userId);
        void RemoveProfilesToUser(List<Profiles_To_User> profilesToUser);
        UserModel GetUserByInitial(string initial);
        UserDetailsViewModel GetUserDetailsById(int? id);
        string GetRole(string initial);
        void DeactivateProfiles(int profileId);
        void DeleteProfiles(byte fmId);
        int ImportExcelfile(DataTable dt);
        void SaveChanges();
        List<UserModel> GetAllCTToUserMapping();
        UserModel GetManagerStatus(string initial);
        UserModel GetManagerStatusById(int id);
    }
}
