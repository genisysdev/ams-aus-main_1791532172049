using DataAccessLayer.DAL.Model;
using System.Collections.Generic;

namespace DataAccessLayer.DAL.HCPListRepository
{
    public interface IHCPListRepository
    {
        List<HCPViewModel> GetAll(string searchString = null, string searchStringCN = null, string sortOrder = null, string CustomerID = null);
        void InsertHCP(HCP reqModel);
        HCPViewModel GetHCPById(int? id);
        void UpdateHCP(int id, HCPViewModel model);
         void DeactivateHCP(int id);
        List<CommonModel> GetTypeOfHCP();
    }
}
