using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.DAL.CostCentreRepository
{
    public class CostCentreRepository : ICostCentreRepository
    {
        private readonly NovoNordiskEntities _dataContext;

        public CostCentreRepository(NovoNordiskEntities dataContext)
        {
            this._dataContext = dataContext;
        }

        public IEnumerable<Cost_Centre> GetAllCostCentre()
        {
            return _dataContext.Cost_Centre.OrderBy(z => z.ID).ToList();
        }


    }
}
