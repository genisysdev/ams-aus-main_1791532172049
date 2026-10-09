using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.DAL.CostCentreRepository
{
    public interface ICostCentreRepository
    {
        IEnumerable<Cost_Centre> GetAllCostCentre();
    }
}
