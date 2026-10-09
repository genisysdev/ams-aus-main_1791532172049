using DataAccessLayer.DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.DAL.CTCostCentreRepository
{
    public class CTCostCentreMapRepository : ICTCostCentreMapRepository
    {
        private readonly NovoNordiskEntities _dataContext;

        public CTCostCentreMapRepository(NovoNordiskEntities dataContext)
        {
            this._dataContext = dataContext;
        }

        public List<CTCostcentreMapViewModel> GetAllCostCentre_To_CT()
        {
            try
            {
                var ctCostCentreMapView = new List<CTCostcentreMapViewModel>();
                using (NovoNordiskEntities context = new NovoNordiskEntities())
                {
                    var result = (from cc in context.CostCentre_To_CT
                                  join u in context.Users on cc.User_Id equals u.ID
                                  join c in context.Cost_Centre on cc.CostCentre_Id equals c.ID
                    where cc.Status == true
                                  select new CTCostcentreMapViewModel
                                  {
                                      Id = cc.Id,
                                      CostCenterID = cc.CostCentre_Id,
                                      CostCenter_Code = c.Code,
                                      UserId = cc.User_Id,
                                      UserInitial = u.Initials
                                  }
                               ).OrderByDescending(o => o.Id).ToList();
                    context.Dispose();

                    var groupedResult = ctCostCentreMapView.GroupBy(s => s.UserId);
                    foreach (var obj in groupedResult)
                    {
                        StringBuilder names = new StringBuilder();
                        var sfm = obj.OrderByDescending(o => o.Id).FirstOrDefault();
                        foreach (CTCostcentreMapViewModel o in obj)
                        {
                            names.Append(o.CostCenter_Code + ", ");
                        }

                        var fm = new CTCostcentreMapViewModel
                        {
                            Id = sfm.Id,
                            UserId = sfm.UserId,
                            UserInitial = sfm.UserInitial,
                            CostCenter_Code = names.ToString().Remove(names.ToString().Length - 2)
                        };
                        result.Add(fm);
                    }
                    return result.OrderByDescending(o => o.Id).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void InsertCostCentreToCT(CostCentre_To_CT model)
        {
            _dataContext.CostCentre_To_CT.Add(model);
            _dataContext.SaveChanges();
        }

        public CostCentreMapView GetById(int UserId)
        {
            try
            {
                var result = new CostCentreMapView();
                using (NovoNordiskEntities context = new NovoNordiskEntities())
                {
                    CostCentre_To_CT ccDetail = context.CostCentre_To_CT.Where(s => s.User_Id == UserId).FirstOrDefault();
                    if (ccDetail != null)
                    {
                        result.SelectedUser = new List<string> { Convert.ToString(ccDetail.User_Id) };
                        result.selectedUserId = ccDetail.User_Id;
                        result.CTCCList = (from cc in context.CostCentre_To_CT
                                           join c in context.Cost_Centre on cc.CostCentre_Id equals c.ID
                                           join u in context.Users on cc.User_Id equals u.ID
                                           where cc.User_Id == UserId && cc.Status == true
                                           select new CTCostcentreMapViewModel
                                           {
                                               Id = cc.Id,
                                               CostCenterID = cc.CostCentre_Id,
                                               CostCenter_Code = c.Code,
                                               UserInitial = u.Initials,
                                               UserId = cc.User_Id
                                           }).ToList();
                    }
                }
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void DeactivateCTToUserMapping(int id)
        {
            var user = _dataContext.CostCentre_To_CT.Where(p => p.Id == id).FirstOrDefault();
            user.Status = false;
            _dataContext.SaveChanges();
        }

        public void DeleteCTToUserMapping(int id)
        {
            var users = _dataContext.CostCentre_To_CT.Where(p => p.User_Id == id).ToList();

            foreach (var p in users)
            {
                p.Status = false;
            }
            _dataContext.SaveChanges();
        }
    }
}
