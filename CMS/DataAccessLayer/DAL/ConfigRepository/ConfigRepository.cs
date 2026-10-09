using DataAccessLayer.DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DataAccessLayer.DAL.ConfigRepository
{
    public class ConfigRepository : IConfigRepository
    {
        private readonly NovoNordiskEntities _dataContext;

        public ConfigRepository(NovoNordiskEntities dataContext)
        {
            this._dataContext = dataContext;
        }

        public List<AMSConfigViewModel> GetAll(string searchString = null, string sortOrder = null)
        {
            try
            {
                var result = new List<AMSConfigViewModel>();
                using (NovoNordiskEntities context = new NovoNordiskEntities())
                {
                    result = (from c in context.AMS_Config
                                select new AMSConfigViewModel
                                {
                                    Name = c.Name,
                                    Value = c.Value,
                                    Description = c.Description,
                                    Status = c.Status,
                                    Modified_Date = c.Modified_Date

                                }).ToList();

                    if (!string.IsNullOrEmpty(searchString))
                        result = result.Where(s => s.Name.ToLower().Contains(searchString.ToLower())).ToList();

                    #region Orderby List
                    switch (sortOrder)
                    {
                        case "Name":
                            result = result.OrderBy(o => o.Name).ToList();
                            break;
                        case "Name_Desc":
                            result = result.OrderByDescending(o => o.Name).ToList();
                            break;
                        case "Description":
                            result = result.OrderBy(o => o.Description).ToList();
                            break;
                        case "Description_Desc":
                            result = result.OrderByDescending(o => o.Description).ToList();
                            break;
                        default:
                            result = result.OrderByDescending(o => o.Modified_Date).ToList();
                            break;
                    }
                    #endregion
                    context.Dispose();
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void InsertAMS_Config(AMS_Config reqModel)
        {
            _dataContext.AMS_Config.Add(reqModel);
            _dataContext.SaveChanges();
        }

        public AMS_Config GetConfigByName(string name)
        {
            return _dataContext.AMS_Config.Where(p => p.Name == name).FirstOrDefault();
        }

        public void UpdateConfig(AMS_Config model)
        {
            var existingConfig = GetConfigByName(model.Name);

            if (existingConfig != null)
            {
                existingConfig.Value = model.Value;
                existingConfig.Description = model.Description;
                existingConfig.Status = model.Status;
                existingConfig.Modified_Date = DateTime.Now;
                _dataContext.SaveChanges();
            }
        }

        public List<AMSConfigViewModel> GetAMSconfig()
        {
            try
            {
                var response = new List<AMSConfigViewModel>();
                using (NovoNordiskEntities context = new NovoNordiskEntities())
                {
                    response = (from p in context.AMS_Config
                                select new AMSConfigViewModel
                                {
                                    Name = p.Name,
                                    Value = p.Value,
                                    Description = p.Description,
                                    Status = p.Status,
                                    Modified_Date = p.Modified_Date
                                }
                                ).ToList();

                    context.Dispose();
                    return response;
                }
            }
            catch(Exception ex) 
            {
                throw ex;
            }
        }
    }
}
