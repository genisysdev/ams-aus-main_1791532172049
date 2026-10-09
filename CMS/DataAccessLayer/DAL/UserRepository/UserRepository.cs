using DataAccessLayer.DAL.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;

namespace DataAccessLayer.DAL.UserRepository
{
    public class UserRepository : IUserRepository
    {
        private readonly NovoNordiskEntities _dataContext;

        public UserRepository(NovoNordiskEntities dataContext)
        {
            this._dataContext = dataContext;
        }

        public IEnumerable<User> GetAllUsers()
        {
            return _dataContext.Users.OrderBy(z => z.ID).ToList();
        }

        public List<UserModel> GetUsers(string initial, string surName, string costCenter, string manager, string name, string sortOrder)
        {
            using (NovoNordiskEntities context = new NovoNordiskEntities())
            {
                var users = (from h in context.Users
                             join ta in context.Cost_Centre on h.CC_Id equals ta.ID
                             join z in context.Users on h.Mgr_Id equals z.ID into ps
                             from z in ps.DefaultIfEmpty()
                             select new UserModel
                             {
                                 ID = h.ID,
                                 Initial = h.Initials,
                                 CostCenter = ta.Code,
                                 Name = h.Name,
                                 Surname = h.SurName,
                                 Manager = !string.IsNullOrEmpty(z.Initials) ? z.Initials : "",
                                 Position = h.Position,
                                 IsAdmin = h.IsAdmin,
                                 Status = h.Status
                             }).ToList();

                // Filtering
                if (!string.IsNullOrEmpty(initial))
                {
                    users = users.Where(s => s.Initial.ToLower().Contains(initial.ToLower())).ToList();
                }
                if (!string.IsNullOrEmpty(surName))
                {
                    users = users.Where(s => s.Surname.ToLower().Contains(surName.ToLower())).ToList();
                }
                if (!string.IsNullOrEmpty(manager))
                {
                    users = users.Where(s => s.Manager.ToLower().Contains(manager.ToLower())).ToList();
                }
                if (!string.IsNullOrEmpty(costCenter))
                {
                    users = users.Where(s => s.CostCenter.ToLower().Contains(costCenter.ToLower())).ToList();
                }
                if (!string.IsNullOrEmpty(name))
                {
                    users = users.Where(s => s.Name.ToLower().Contains(name.ToLower())).ToList();
                }

                // Sorting
                switch (sortOrder)
                {
                    case "Initial":
                        users = users.OrderBy(o => o.Initial).ToList();
                        break;
                    case "Initial_Desc":
                        users = users.OrderByDescending(o => o.Initial).ToList();
                        break;
                    case "CostCenter":
                        users = users.OrderBy(o => o.CostCenter).ToList();
                        break;
                    case "CostCenter_Desc":
                        users = users.OrderByDescending(o => o.CostCenter).ToList();
                        break;
                    case "Mgr":
                        users = users.OrderBy(o => o.Manager).ToList();
                        break;
                    case "Mgr_Desc":
                        users = users.OrderByDescending(o => o.Manager).ToList();
                        break;
                    case "Name":
                        users = users.OrderBy(o => o.Name).ToList();
                        break;
                    case "Name_Desc":
                        users = users.OrderByDescending(o => o.Name).ToList();
                        break;
                    case "SurName":
                        users = users.OrderBy(o => o.Surname.Trim()).ToList();
                        break;
                    case "SurName_Desc":
                        users = users.OrderByDescending(o => o.Surname).ToList();
                        break;
                    case "Position":
                        users = users.OrderBy(o => o.Position).ToList();
                        break;
                    case "Position_Desc":
                        users = users.OrderByDescending(o => o.Position).ToList();
                        break;
                    case "IsAdmin":
                        users = users.OrderBy(o => o.IsAdmin).ToList();
                        break;
                    case "IsAdmin_Desc":
                        users = users.OrderByDescending(o => o.IsAdmin).ToList();
                        break;
                    case "Status":
                        users = users.OrderBy(o => o.Status).ToList();
                        break;
                    case "Status_Desc":
                        users = users.OrderByDescending(o => o.Status).ToList();
                        break;
                    default:
                        users = users.OrderByDescending(o => o.ID).ToList();
                        break;
                }

                return users;
            }
        }

        public UserModel GetUserByInitial(string initial)
        {
            try
            {
                using (NovoNordiskEntities context = new NovoNordiskEntities())
                {
                    var result = (from u in context.Users
                                  join ta in context.Cost_Centre on u.CC_Id equals ta.ID
                                  join z in context.Users on u.Mgr_Id equals z.ID into ps
                                  from z in ps.DefaultIfEmpty()
                                  join ptu in context.Profiles_To_User on u.ID equals ptu.User_Id
                                  join p in context.Profiles on ptu.Profile_Id equals p.ID
                                  where u.Initials.ToLower() == initial.ToLower()
                                  orderby ptu.ID descending
                                  select new UserModel
                                  {
                                      ID = u.ID,
                                      Initial = u.Initials,
                                      CostCenter = ta.Code,
                                      Name = u.Name,
                                      Surname = u.SurName,
                                      Manager = !string.IsNullOrEmpty(z.Initials) ? z.Initials : "",
                                      Position = u.Position,
                                      IsAdmin = u.IsAdmin,
                                      Status = u.Status,
                                      Role = p.Name,
                                      MManager = (z.Mgr_Id != null) ? (context.Users.FirstOrDefault(s => s.ID == z.Mgr_Id).Initials) : "",
                                      //ZoneName = (u.Zone_Id != null) ? (context.Zones.FirstOrDefault(s => s.ID == u.Zone_Id).Name) : "",
                                      TherapyArea = (u.TA_Id != null) ? (context.Therapy_Area.FirstOrDefault(s => s.ID == u.TA_Id).Name) : "",
                                      CC_CreatedDate = ta.CreatedDate
                                  }).FirstOrDefault();
                    context.Dispose();
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public UserDetailsViewModel GetUserDetailsById(int? id)
        {
            try
            {
                using (NovoNordiskEntities context = new NovoNordiskEntities())
                {
                    var result = (from u in context.Users
                                  join ta in context.Cost_Centre on u.CC_Id equals ta.ID
                                  join z in context.Users on u.Mgr_Id equals z.ID into ps
                                  from z in ps.DefaultIfEmpty()
                                  where u.ID == id
                                  select new UserDetailsViewModel
                                  {
                                      ID = u.ID,
                                      Initial = u.Initials,
                                      Name = u.Name,
                                      Surname = u.SurName,
                                      Position = u.Position,
                                      Status = u.Status,
                                      RoleId = (from ptu in context.Profiles_To_User
                                                join p in context.Profiles on ptu.Profile_Id equals p.ID
                                                where ptu.User_Id == u.ID
                                                select p.ID
                                               ).FirstOrDefault<byte>(),
                                      MManager = (z.Mgr_Id != null) ? (context.Users.FirstOrDefault(s => s.ID == z.Mgr_Id).Initials) : "",
                                      //ZoneId = (u.Zone_Id != null) ? (context.Zones.FirstOrDefault(s => s.ID == u.Zone_Id).ID) : 0,
                                      TherapyAreaId = (u.TA_Id != null) ? (context.Therapy_Area.FirstOrDefault(s => s.ID == u.TA_Id).ID) : 0,
                                      CostCenterId = (u.CC_Id != 0) ? (context.Cost_Centre.FirstOrDefault(s => s.ID == u.CC_Id).ID) : 0,
                                      EmailId = u.Email,
                                      IsAdmin = u.IsAdmin,
                                      ManagerId = (u.Mgr_Id != 0) ? (context.Users.FirstOrDefault(s => s.ID == u.Mgr_Id).ID) : 0,
                                      EmpId = u.EmpId
                                  }).FirstOrDefault();
                    context.Dispose();
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string GetRole(string initial)
        {
            try
            {
                using (NovoNordiskEntities context = new NovoNordiskEntities())
                {
                    string role = (from u in context.Users
                                   join ptu in context.Profiles_To_User on u.ID equals ptu.User_Id
                                   join p in context.Profiles on ptu.Profile_Id equals p.ID
                                   where u.Initials.ToLower() == initial.ToLower()
                                   select p.Name
                                   ).FirstOrDefault();
                    context.Dispose();
                    return role;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int InsertUser(User reqModel)
        {
            _dataContext.Users.Add(reqModel);
            _dataContext.SaveChanges();
            return reqModel.ID;
        }

        public void InsertProfileToUser(Profiles_To_User reqModel)
        {
            _dataContext.Profiles_To_User.Add(reqModel);
            _dataContext.SaveChanges();
        }

        public List<Profiles_To_User> GetProfilesToUserByUserId(int userId)
        {
            return _dataContext.Profiles_To_User.Where(p => p.User_Id == userId).ToList();
        }

        public void RemoveProfilesToUser(List<Profiles_To_User> profilesToUser)
        {
            _dataContext.Profiles_To_User.RemoveRange(profilesToUser);
            _dataContext.SaveChanges();
        }

        public void DeactivateProfiles(int id)
        {
            var profile = _dataContext.Profiles_To_User.Where(p => p.ID == id).FirstOrDefault();
            profile.Status = false;
            _dataContext.SaveChanges();
        }

        public void DeleteProfiles(byte fmId)
        {
            var profile = _dataContext.Profiles_To_User.Where(s => s.Profile_Id.Equals(fmId)).FirstOrDefault();
            profile.Status = false;
            _dataContext.SaveChanges();
        }

        public int ImportExcelfile(DataTable dt)
        {
            try
            {
                foreach (DataRow row in dt.Rows)
                {
                    foreach (DataColumn column in dt.Columns)
                    {
                        if (row[column] != DBNull.Value && row[column].ToString() == "NULL")
                        {
                            row[column] = DBNull.Value;
                        }
                    }
                }

                string connectionString = _dataContext.Database.Connection.ConnectionString;

                using (var con = new SqlConnection(connectionString))
                {
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.CommandText = "PopulateCostCentre";
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 120;

                        var param = new SqlParameter
                        {
                            ParameterName = "@Datatable",
                            SqlDbType = SqlDbType.Structured,  
                            TypeName = "dbo.CostCentreTableType",  
                            Value = dt  
                        };
                        cmd.Parameters.Add(param);

                        var resultParam = new SqlParameter
                        {
                            ParameterName = "@Result",
                            DbType = DbType.Int32,
                            Direction = ParameterDirection.Output
                        };
                        cmd.Parameters.Add(resultParam);

                        con.Open();
                        cmd.ExecuteNonQuery(); 

                        int outputValue = Convert.ToInt32(resultParam.Value);
                        con.Close();

                        return outputValue;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public List<UserModel> GetAllCTToUserMapping()
        {
            try
            {
                var result = new List<UserModel>();
                using (NovoNordiskEntities context = new NovoNordiskEntities())
                {
                    result = (from h in context.Users
                              join ptu in context.Profiles_To_User on h.ID equals ptu.User_Id
                              join pl in context.Profiles on ptu.Profile_Id equals pl.ID
                              where pl.Name == "CT" && h.Status == true
                              select new UserModel
                              {
                                  ID = h.ID,
                                  Initial = h.Initials
                              }).ToList();
                    context.Dispose();
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void SaveChanges()
        {
            _dataContext.SaveChanges();
        }

        public UserModel GetManagerStatus(string initial)
        {
            try
            {
                using (NovoNordiskEntities context = new NovoNordiskEntities())
                {
                    var result = (from u in context.Users
                                  join ptu in context.Profiles_To_User on u.ID equals ptu.User_Id
                                  where u.Initials.ToLower() == initial.ToLower()
                                  orderby ptu.ID descending
                                  select new UserModel
                                  {
                                      ID = u.ID,
                                      Initial = u.Initials,
                                      Status = u.Status
                                  }).FirstOrDefault();
                    context.Dispose();
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public UserModel GetManagerStatusById(int id)
        {
            try
            {
                using (NovoNordiskEntities context = new NovoNordiskEntities())
                {
                    var result = (from u in context.Users
                                  where u.ID == id
                                  select new UserModel
                                  {
                                      ID = u.ID,
                                      Initial = u.Initials,
                                      Status = (from us in context.Users
                                                where us.ID == u.Mgr_Id
                                                select us.Status
                                               ).FirstOrDefault<bool>()
                                  }).FirstOrDefault();
                    context.Dispose();
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
