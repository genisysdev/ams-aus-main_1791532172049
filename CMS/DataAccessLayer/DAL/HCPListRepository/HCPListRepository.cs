using DataAccessLayer.DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataAccessLayer.DAL.HCPListRepository
{
    public class HCPListRepository : IHCPListRepository
    {
        private readonly NovoNordiskEntities _dataContext;

        public HCPListRepository(NovoNordiskEntities dataContext)
        {
            this._dataContext = dataContext;
        }

        public List<HCPViewModel> GetAll(string searchString = null, string searchStringCN = null, string sortOrder = null, string CustomerID = null)
        {
            var result = new List<HCPViewModel>();

            using (NovoNordiskEntities context = new NovoNordiskEntities())
            {
                result = (from h in context.HCPs
                          join ta in context.Therapy_Area on h.TA_Id equals ta.ID
                          select new HCPViewModel
                          {
                              ID = h.ID,
                              VendorNo = h.Vendor_No,
                              FirstName = h.First_Name,
                              LastName = h.Last_Name,
                              TMFNo = h.TMF_No,
                              TAName = ta.Name,
                              CMSNo = h.Coupa_No,
                              HCPAddress = h.Address,
                              Email_Id = h.Email_Id,
                              Phone_Number = h.Phone_Number,
                              City = h.City,
                              ContractStartDate = h.Contract_Start_Date,
                              ContractEndDate = h.Contract_End_Date

                          }).ToList();

                if (!string.IsNullOrEmpty(searchString))
                    result = result.Where(s => s.FirstName.ToLower().Contains(searchString.ToLower())).ToList();
                if (!string.IsNullOrEmpty(searchStringCN))
                    result = result.Where(s => s.CMSNo.Contains(searchStringCN)).ToList();
                if (!string.IsNullOrEmpty(CustomerID))
                {
                    int customerID = Convert.ToInt32(CustomerID);
                    result = result.Where(s => s.ID == customerID).ToList();
                }

                #region Orderby List

                switch (sortOrder)
                {
                    case "Vendor":
                        result = result.OrderBy(o => o.VendorNo).ToList();
                        break;
                    case "Vendor_Desc":
                        result = result.OrderByDescending(o => o.VendorNo).ToList();
                        break;
                    case "TMFNo":
                        result = result.OrderBy(o => o.TMFNo).ToList();
                        break;
                    case "FirstName":
                        result = result.OrderBy(o => o.FirstName).ToList();
                        break;
                    case "LastName":
                        result = result.OrderBy(o => o.LastName).ToList();
                        break;
                    case "Unit":
                        result = result.OrderBy(o => o.TAName).ToList();
                        break;
                    case "Unit_Desc":
                        result = result.OrderByDescending(o => o.TAName).ToList();
                        break;
                    case "CoupaNumber":
                        result = result.OrderBy(o => o.LectCost).ToList();
                        break;
                    case "CoupaNumber_Desc":
                        result = result.OrderByDescending(o => o.LectCost).ToList();
                        break;
                    default:
                        result = result.OrderByDescending(o => o.ID).ToList();
                        break;
                }
                #endregion

                context.Dispose();
                return result;
            }
        }

        public List<CommonModel> GetTypeOfHCP()
        {
            var result = _dataContext.Type_Of_HCP.Select(u => new CommonModel { Id = u.ID, Name = u.Name }).OrderBy(u => u.Name).ToList();
            return result;
        }

        public void InsertHCP(HCP reqModel)
        {
            _dataContext.HCPs.Add(reqModel);
            _dataContext.SaveChanges();
        }

        public HCPViewModel GetHCPById(int? id)
        {
            try
            {
                var hcp = _dataContext.HCPs.Find(id);

                return hcp != null ? new HCPViewModel
                {
                    ID = hcp.ID,
                    VendorNo = hcp.Vendor_No,
                    CMSNo = hcp.Coupa_No,
                    FirstName = hcp.First_Name,
                    TAId = hcp.TA_Id,
                    HCPAddress = hcp.Address
                } : null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void UpdateHCP(int id, HCPViewModel model)
        {
            var existingHCP = _dataContext.HCPs.Find(model.ID);

            if (existingHCP != null)
            {
                existingHCP.Vendor_No = model.VendorNo;
                existingHCP.TMF_No = model.TMFNo;
                existingHCP.Coupa_No = model.CMSNo;
                existingHCP.First_Name = model.FirstName;
                existingHCP.Last_Name = model.LastName;
                existingHCP.TA_Id = model.TAId;
                existingHCP.Address = model.HCPAddress;
                existingHCP.Email_Id = model.Email_Id;
                existingHCP.Phone_Number = model.Phone_Number;
                existingHCP.City = model.City;
                existingHCP.Is_Contract = model.Is_Contract;
                existingHCP.Contract_Start_Date = (model.ContractStartDate == default(DateTime)) ? null : model.ContractStartDate;
                existingHCP.Contract_End_Date = (model.ContractEndDate == default(DateTime)) ? null : model.ContractEndDate;
                _dataContext.SaveChanges();
            }
        }

        public void DeactivateHCP(int id)
        {
            var hcp = _dataContext.HCPs.Find(id);
            if (hcp != null)
            {
                _dataContext.HCPs.Remove(hcp);
            }
            _dataContext.SaveChanges();
        }
    }
}
