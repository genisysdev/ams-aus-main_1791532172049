using DataAccessLayer.DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.DAL.CommonRespository
{
    public interface ICommonRespository
    {
        IEnumerable<Therapy_Area> GetTherapyArea();
        IEnumerable<Cost_Centre> GetCostCentres();
        IEnumerable<User> GetManagers();
        IEnumerable<Brand> GetBrands();
        IEnumerable<PAF_Meetingtype> GetPafMeetingtype();
        int InsertMiscellaneousType(Miscellaneous_Type miscellaneous);
        int InsertPAFMiscellaneous(PAF_Miscellaneous miscellaneous);
    }
}
