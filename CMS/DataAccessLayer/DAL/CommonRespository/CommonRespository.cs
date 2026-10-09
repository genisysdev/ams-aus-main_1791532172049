using DataAccessLayer.DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.DAL.CommonRespository
{
    public class CommonRespository : ICommonRespository
    {
        private readonly NovoNordiskEntities _dataContext;
        public CommonRespository(NovoNordiskEntities dataContext)
        {
            this._dataContext = dataContext;
        }

        public IEnumerable<Therapy_Area> GetTherapyArea()
        {
            return _dataContext.Therapy_Area.OrderBy(z => z.ID).ToList();
        }

        public IEnumerable<Cost_Centre> GetCostCentres()
        {
            return _dataContext.Cost_Centre.OrderBy(z => z.ID).ToList();
        }

        public IEnumerable<User> GetManagers()
        {
            var result = _dataContext.Users.Select(u => new User { Initials = u.Initials, ID = u.ID }).OrderBy(u => u.ID).ToList();
            return result;
        }

        public IEnumerable<Brand> GetBrands()
        {
            return _dataContext.Brands.OrderBy(z => z.ID).ToList();
        }

        public IEnumerable<PAF_Meetingtype> GetPafMeetingtype()
        {
            return _dataContext.PAF_Meetingtype.OrderBy(z => z.Id).ToList();
        }

        public int InsertMiscellaneousType(Miscellaneous_Type miscellaneous)
        {
            _dataContext.Miscellaneous_Type.Add(miscellaneous);
            _dataContext.SaveChanges();
            return miscellaneous.ID;
        }

        public int InsertPAFMiscellaneous(PAF_Miscellaneous miscellaneous)
        {
            _dataContext.PAF_Miscellaneous.Add(miscellaneous);
            _dataContext.SaveChanges();
            return miscellaneous.ID;
        }

    }
}
