using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.DAL.CMENewRepository
{
    public class CMENewRepository : ICMENewRepository
    {
        private readonly NovoNordiskEntities _dataContext;
        public CMENewRepository(NovoNordiskEntities dataContext)
        {
            this._dataContext = dataContext;
        }

    }
}
