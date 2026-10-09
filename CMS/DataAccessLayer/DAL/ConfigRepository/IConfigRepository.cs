using DataAccessLayer.DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.DAL.ConfigRepository
{
    public interface IConfigRepository
    {
        List<AMSConfigViewModel> GetAll(string searchString = null, string sortOrder = null);
        void InsertAMS_Config(AMS_Config reqModel);
        AMS_Config GetConfigByName(string name);
        void UpdateConfig(AMS_Config model);
        List<AMSConfigViewModel> GetAMSconfig();
    }
}
