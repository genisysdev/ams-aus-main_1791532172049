using DataAccessLayer.DAL.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Linq;

namespace DataAccessLayer.DAL.PAFRepository
{
    public class PAFRepository : IPAFRepository
    {
        private readonly NovoNordiskEntities _dataContext;
        public PAFRepository(NovoNordiskEntities dataContext)
        {
            this._dataContext = dataContext;
        }

        public int GetPAFId()
        {
            var pafId = _dataContext.PAFs.OrderByDescending(p => p.ID).FirstOrDefault();
            return pafId != null ? pafId.ID : 0;
        }

        public void InsertPAF(PAF paf)
        {
            _dataContext.PAFs.Add(paf);
            _dataContext.SaveChanges();
        }

        public void InsertPAFHCP(PAF_HCP pafHcp)
        {
            _dataContext.PAF_HCP.Add(pafHcp);
            _dataContext.SaveChanges();
        }

        public void InsertPAFParticipants(PAF_Participants pafPart)
        {
            _dataContext.PAF_Participants.Add(pafPart);
            _dataContext.SaveChanges();
        }

        public List<PAFList> GetDraft(PAFSearch filterModel)
        {
            var result = new List<PAFList>();
            try
            {
                using (var context = new NovoNordiskEntities())
                {
                    var query = CreateBaseQuery(context);

                    result = query.Where(list => (list.Approved_Or_Reject == "Draft" || list.Approved_Or_Reject == "Send to postpone") && list.CreatedBy == filterModel.UserId).ToList();

                    result = ApplyFilters(result, filterModel).ToList();
                    result = ApplySorting(result, filterModel).ToList();

                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<PAFList> GetAllForm(PAFSearch filterModel)
        {
            var result = new List<PAFList>();
            try
            {
                using (var context = new NovoNordiskEntities())
                {
                    var query = CreateBaseQuery(context);

                    if (filterModel.UserRole == "Admin" || filterModel.UserRole == "CME Operation")
                    {
                        result = query.ToList();
                    }
                    else if (filterModel.UserRole == "CT")
                    {
                        result = query.Where(list=> list.CreatedBy == filterModel.UserId && list.CostCentreToCTStatus == true).ToList();
                    }
                    else if (filterModel.UserRole == "CTB")
                    {
                        result = query.Where(list => list.Zone == "Rare Disease").ToList();
                    }

                    result = ApplyFilters(result, filterModel).ToList();
                    result = ApplySorting(result, filterModel).ToList();

                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<PAFList> GetPreApprovalStatus(PAFSearch filterModel)
        {
            var result = new List<PAFList>();
            try
            {
                using (var context = new NovoNordiskEntities())
                {
                    var query = CreateBaseQuery(context);

                    result = query.Where(list => list.Approved_Or_Reject != "Draft").ToList();

                    result = ApplyFilters(result, filterModel).ToList();
                    result = ApplySorting(result, filterModel).ToList();

                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<PAFList> GetRejectedBiopharm(PAFSearch filterModel)
        {
            var result = new List<PAFList>();
            try
            {
                using (var context = new NovoNordiskEntities())
                {
                    var query = CreateBaseQuery(context);

                    result = query.Where(list => (list.Approved_Or_Reject == "Reject by SM" || list.Approved_Or_Reject == "Rejected by RBD1"
                    || list.Approved_Or_Reject == "Rejected by RBD2" || list.Approved_Or_Reject == "Rejected by Hemophilia Medical"
                    || list.Approved_Or_Reject == "Rejected by Growth Hormone Medical" || list.Approved_Or_Reject == "Reject by RBD"
                    || list.Approved_Or_Reject == "Reject by CME Operation") && list.CreatedBy == filterModel.UserId).ToList();

                    result = ApplyFilters(result, filterModel).ToList();
                    result = ApplySorting(result, filterModel).ToList();

                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<PAFList> GetApprovedBiopharm(PAFSearch filterModel)
        {
            var result = new List<PAFList>();
            try
            {
                using (var context = new NovoNordiskEntities())
                {
                    var query = CreateBaseQuery(context);
                    var mgrId = context.Users.Where(s => s.Initials == "RBDZ").FirstOrDefault().Initials;

                    result = query.Where(list => ((list.Approved_Or_Reject == "Approved by RBD1" && list.TotalCost <= 150000)
                                || (list.Approved_Or_Reject == "Approved by RBD2" && list.TotalCost > 150000)
                                || list.Approved_Or_Reject == "Approved by CME Operation" || list.Approved_Or_Reject == "Resend by CT"
                                || (list.Approved_Or_Reject == "Approved by RBD" && list.IsVirtual == true) || (list.Approved_Or_Reject == "Approved by RBD"
                                && list.IsBTC == false))
                                && (list.CreatedBy == filterModel.UserId || list.ManagerInitial == mgrId)).ToList();

                    result = ApplyFilters(result, filterModel).ToList();
                    result = ApplySorting(result, filterModel).ToList();

                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<PAFList> GetRejected(PAFSearch filterModel)
        {
            var result = new List<PAFList>();
            try
            {
                using (var context = new NovoNordiskEntities())
                {
                    var query = CreateBaseQuery(context);

                    result = query.Where(list => (list.Approved_Or_Reject == "Reject by SR" || list.Approved_Or_Reject == "Reject by SM"
                        || list.Approved_Or_Reject == "Reject by RBD" || list.Approved_Or_Reject == "Rejected by RBD1" || list.Approved_Or_Reject == "Rejected by RBD2"
                        || list.Approved_Or_Reject == "Reject by CT" || list.Approved_Or_Reject == "Reject by CME Operation") && list.CreatedBy == filterModel.UserId).ToList();

                    result = ApplyFilters(result, filterModel).ToList();
                    result = ApplySorting(result, filterModel).ToList();

                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<PAFList> GetApproved(PAFSearch filterModel)
        {
            var result = new List<PAFList>();
            try
            {
                using (var context = new NovoNordiskEntities())
                {
                    var query = CreateBaseQuery(context);
                    var mgrId = context.Users.Where(s => s.Initials == "RBDZ").FirstOrDefault().Initials;

                    result = query.Where(list => (list.Approved_Or_Reject == "Resend by CT" || list.Approved_Or_Reject == "Approved by CME Operation"
                    || (list.Approved_Or_Reject == "Approved by RBD" && list.IsVirtual == true) || (list.Approved_Or_Reject == "Approved by RBD" && list.IsBTC == false))
                    && (list.CreatedBy == filterModel.UserId || list.ManagerInitial == mgrId)).ToList();

                    result = ApplyFilters(result, filterModel).ToList();
                    result = ApplySorting(result, filterModel).ToList();

                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<PAFList> GetAttendBiopharm(PAFSearch filterModel)
        {
            var result = new List<PAFList>();
            try
            {
                using (var context = new NovoNordiskEntities())
                {
                    var query = CreateBaseQuery(context);

                    if (filterModel.UserRole == "SM")
                    {
                        result = query.Where(list => (list.Approved_Or_Reject == "Approved by SM" || list.Approved_Or_Reject == "Reject by SM" || list.Approved_Or_Reject == "Approved Date Deviation") && list.CreatedBy == filterModel.UserId).ToList();
                    }
                    else if (filterModel.UserRole == "RBD")
                    {
                        result = query.Where(list => (list.Approved_Or_Reject == "Approved by RBD" || list.Approved_Or_Reject == "Reject by RBD") && list.CreatedBy == filterModel.UserId).ToList();
                    }
                    else if (filterModel.UserRole == "RBD1")
                    {
                        result = query.Where(list => (list.Approved_Or_Reject == "Approved by RBD1" || list.Approved_Or_Reject == "Reject by RBD1") && list.CreatedBy == filterModel.UserId).ToList();
                    }
                    else if (filterModel.UserRole == "RBD2")
                    {
                        result = query.Where(list => (list.Approved_Or_Reject == "Approved by RBD2" || list.Approved_Or_Reject == "Reject by RBD2") && list.CreatedBy == filterModel.UserId).ToList();
                    }
                    else if (filterModel.UserRole == "Hemophilia Medical")
                    {
                        result = query.Where(list => (list.Approved_Or_Reject == "Approved by Hemophilia Medical" || list.Approved_Or_Reject == "Reject by Hemophilia Medical") && list.CreatedBy == filterModel.UserId).ToList();
                    }
                    else if (filterModel.UserRole == "Growth Hormone Medical")
                    {
                        result = query.Where(list => (list.Approved_Or_Reject == "Approved by Growth Hormone Medical" || list.Approved_Or_Reject == "Reject by Growth Hormone Medical") && list.CreatedBy == filterModel.UserId).ToList();
                    }
                    else if (filterModel.UserRole == "CTB")
                    {
                        result = query.Where(list => (list.Approved_Or_Reject == "Closed" || list.Approved_Or_Reject == "Payment requested" || list.Approved_Or_Reject == "Reject by CTB") && list.CreatedBy == filterModel.UserId).ToList();
                    }
                    result = ApplyFilters(result, filterModel).ToList();
                    result = ApplySorting(result, filterModel).ToList();

                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<PAFList> GetAttend(PAFSearch filterModel)
        {
            var result = new List<PAFList>();
            try
            {
                using (var context = new NovoNordiskEntities())
                {
                    var query = CreateBaseQuery(context);

                    if (filterModel.UserRole == "CME Operation")
                    {
                        result = query.Where(list => (list.Approved_Or_Reject == "Approved by CME Operation" || list.Approved_Or_Reject == "Reject by CME Operation" || list.Approved_Or_Reject == "Delete") && list.CreatedBy == filterModel.UserId).ToList();
                    }
                    else if (filterModel.UserRole == "SM")
                    {
                        result = query.Where(list => (list.Approved_Or_Reject == "Approved by SM" || list.Approved_Or_Reject == "Reject by SM" || list.Approved_Or_Reject == "Approved Date Deviation") && list.CreatedBy == filterModel.UserId).ToList();
                    }
                    else if (filterModel.UserRole == "RBD")
                    {
                        result = query.Where(list => (list.Approved_Or_Reject == "Approved by RBD" || list.Approved_Or_Reject == "Reject by RBD") && list.CreatedBy == filterModel.UserId).ToList();
                    }

                    result = ApplyFilters(result, filterModel).ToList();
                    result = ApplySorting(result, filterModel).ToList();

                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<PAFList> GetPending(PAFSearch filterModel)
        {
            var result = new List<PAFList>();
            try
            {
                using (var context = new NovoNordiskEntities())
                {
                    var query = CreateBaseQuery(context);
                    var mgrId = context.Users.Where(s => s.ID == filterModel.UserId).FirstOrDefault().Initials;

                    if (filterModel.UserRole == "SM")
                    {
                        result = query.Where(list => (list.Approved_Or_Reject == "Send" || list.Approved_Or_Reject == "Send to cancel approve") && list.ManagerInitial == mgrId).ToList();
                    }
                    else if (filterModel.UserRole == "RBD")
                    {
                        var res=query.ToList();
                        result = query.Where(list => (list.Approved_Or_Reject == "Approved by SM" || list.Approved_Or_Reject == "Send to RBD") && list.ManagerInitial == mgrId).ToList();
                    }
                    else if (filterModel.UserRole == "CME Operation")
                    {
                        result = query.Where(list => list.Approved_Or_Reject == "Approved by RBD" && list.IsVirtual != true && list.IsBTC == true).ToList();
                    }
                    result = ApplyFilters(result, filterModel).ToList();
                    result = ApplySorting(result, filterModel).ToList();

                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<PAFList> GetPendingBiopharm(PAFSearch filterModel)
        {
            var result = new List<PAFList>();
            try
            {
                using (var context = new NovoNordiskEntities())
                {
                    var query = CreateBaseQuery(context);
                    var mgrId = context.Users.Where(s => s.ID == filterModel.UserId).FirstOrDefault().Initials;
                    var profile = query.Select(list => list.ManagerRole).FirstOrDefault();

                    if (filterModel.UserRole == "SM")
                    {
                        result = query.Where(list => (list.Approved_Or_Reject == "Send" || list.Approved_Or_Reject == "Send to cancel approve" || list.Approved_Or_Reject == "Send to postpone") && list.ManagerInitial == mgrId).ToList();
                    }
                    else if (filterModel.UserRole == "RBD")
                    {
                        result = query.Where(list =>
                        (list.Approved_Or_Reject == "Approved by SM" || list.Approved_Or_Reject == "Send by RBD" || list.Approved_Or_Reject == "Approved by Hemophilia Medical"
                        || (list.Approved_Or_Reject == "Send" && list.ManagerInitial == mgrId && list.CMEType == "Standard Meeting"))).ToList();
                    }
                    else if (filterModel.UserRole == "RBD1")
                    {
                        result = query.Where(list =>
                        ((list.Approved_Or_Reject == "Approved by SM" && list.CMEType == "Standard Meeting")
                        || list.Approved_Or_Reject == "Send to RBD" || list.Approved_Or_Reject == "Approved by Hemophilia Medical" || list.Approved_Or_Reject == "Approved by Growth Hormone Medical"
                        || (list.Approved_Or_Reject == "Send" && list.ManagerInitial == mgrId) && list.CMEType == "Standard Meeting")
                                    || (list.Approved_Or_Reject == "Send" && list.CMEType == "Standard Meeting" && profile == "SM" && list.IsDateDeviation == false)
                                    || (list.Approved_Or_Reject == "Send" && list.CMEType == "Standard Meeting" && profile == "SM" && list.IsDateDeviation == true && list.IsDateDeviationApproved == true)
                                    && list.Zone == filterModel.UserZone).ToList();

                    }
                    else if (filterModel.UserRole == "RBD2")
                    {
                        result = query.Where(list => (list.Approved_Or_Reject == "Approved by RBD2" || list.TotalCost > 150000) && list.Zone == filterModel.UserZone).ToList();
                    }
                    else if (filterModel.UserRole == "CTB")
                    {
                        result = query.Where(list => list.Approved_Or_Reject == "Send to CTB" && list.Zone == filterModel.UserZone).ToList();
                    }
                    result = ApplyFilters(result, filterModel).ToList();
                    result = ApplySorting(result, filterModel).ToList();

                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<PAFList> GetClosed(PAFSearch filterModel)
        {
            var result = new List<PAFList>();
            try
            {
                using (var context = new NovoNordiskEntities())
                {
                    var query = CreateBaseQuery(context);

                    result = query.Where(list => list.Approved_Or_Reject == "Closed" || list.Approved_Or_Reject == "Payment requested").ToList();

                    result = ApplyFilters(result, filterModel).ToList();
                    result = ApplySorting(result, filterModel).ToList();

                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<PAFList> GetRemoved(PAFSearch filterModel)
        {
            var result = new List<PAFList>();
            try
            {
                using (var context = new NovoNordiskEntities())
                {
                    var query = CreateBaseQuery(context);

                    result = query.Where(list => list.Approved_Or_Reject == "Delete").ToList();

                    result = ApplyFilters(result, filterModel).ToList();
                    result = ApplySorting(result, filterModel).ToList();

                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<PAFList> GetAllPendingVarificationForm(PAFSearch filterModel)
        {
            var result = new List<PAFList>();
            try
            {
                using (var context = new NovoNordiskEntities())
                {
                    var query = CreateBaseQuery(context);

                    if (filterModel.UserRole == "CT")
                    {
                        result = query.Where(list => list.Approved_Or_Reject == "Payment requested" && list.CostCentreUserId == filterModel.UserId && list.CostCentreToCTStatus == true).ToList();
                    }

                    result = ApplyFilters(result, filterModel).ToList();
                    result = ApplySorting(result, filterModel).ToList();

                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<PAFList> GetAllZoneForm(PAFSearch filterModel)
        {
            var result = new List<PAFList>();
            try
            {
                using (var context = new NovoNordiskEntities())
                {
                    var query = CreateBaseQuery(context);
                    var mgrId = context.Users.Where(s => s.ID == filterModel.UserId).FirstOrDefault().Initials;
                    var profile = query.Select(list => list.ManagerRole).FirstOrDefault();

                    if (filterModel.UserRole == "CME Operation" || filterModel.UserRole == "CT")
                    {
                        result = query.Where(list => list.CreatedBy == filterModel.UserId && list.CostCentreToCTStatus == true).ToList();
                    }
                    else if (filterModel.UserRole == "RMA")
                    {
                        result = query.Where(list => list.Zone == "Rare Disease" && list.CMEType == "Non-Standard Meeting").ToList();
                    }
                    else if (filterModel.UserRole == "PMH")
                    {
                        result = query.Where(list => list.Zone == "Rare Disease" && list.Disease_Area == "Hemophilia Medical").ToList();
                    }
                    else if (filterModel.UserRole == "PMGH")
                    {
                        result = query.Where(list => list.Zone == "Rare Disease" && list.Disease_Area == "Growth Hormone Medical").ToList();
                    }
                    result = ApplyFilters(result, filterModel).ToList();
                    result = ApplySorting(result, filterModel).ToList();

                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<PAFList> GetRejectedCloseBiopharm(PAFSearch filterModel)
        {
            var result = new List<PAFList>();
            try
            {
                using (var context = new NovoNordiskEntities())
                {
                    var query = CreateBaseQuery(context);

                    result = query.Where(list => list.Approved_Or_Reject == "Rejected by CTB" && list.CreatedBy == filterModel.UserId).ToList();

                    result = ApplyFilters(result, filterModel).ToList();
                    result = ApplySorting(result, filterModel).ToList();

                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private IQueryable<PAFList> CreateBaseQuery(NovoNordiskEntities context)
        {
            var query = from p in context.PAFs
                        join ps in context.PAF_Status on p.PAF_Status_Id equals ps.ID
                        join prt in context.PAF_Request_Type on p.Request_Type_Id equals prt.ID
                        join u in context.Users on p.Issued_By_Id equals u.ID
                        join ptu in context.Profiles_To_User on u.ID equals ptu.User_Id
                        join pl in context.Profiles on ptu.Profile_Id equals pl.ID
                        join ta in context.Therapy_Area on p.Therapy_Area_Id equals ta.ID into taz
                        from taz1 in taz.DefaultIfEmpty()
                        join b in context.Brands on p.Brand_Id equals b.ID into bz
                        from bz1 in bz.DefaultIfEmpty()
                        join cc in context.Cost_Centre on p.Cost_Centre equals cc.Code into cci
                        from cci1 in cci.DefaultIfEmpty()
                        join c in context.CostCentre_To_CT on cci1.ID equals c.CostCentre_Id into ccm
                        from ccm1 in ccm.DefaultIfEmpty()
                        where ps.Name == "Active"
                        select new PAFList
                        {
                            Id = p.ID,
                            IssuedBy = u.Initials,
                            CreatedBy = u.ID,
                            ManagerInitial = context.Users.FirstOrDefault(s => s.ID == u.Mgr_Id).Initials ?? string.Empty,
                            ManagerRole = (from u1 in context.Users.Where(s => s.ID == u.Mgr_Id)
                                           join ptu in context.Profiles_To_User on u1.ID equals ptu.User_Id
                                           join p in context.Profiles on ptu.Profile_Id equals p.ID
                                           select p.Name).FirstOrDefault(),
                            Approver = p.Approver,
                            StartDate = p.Activity_Start_Date,
                            ViewStartDate = SqlFunctions.Replicate("0", 2 - SqlFunctions.DateName("dd", p.Activity_Start_Date).Trim().Length) + SqlFunctions.DateName("dd", p.Activity_Start_Date).Trim() + "-" + SqlFunctions.Replicate("", 2 - SqlFunctions.StringConvert((double)p.Activity_Start_Date.Month).TrimStart().Length) + SqlFunctions.Replicate("0", 2 - SqlFunctions.StringConvert((double)p.Activity_Start_Date.Month).TrimStart().Length) + SqlFunctions.StringConvert((double)p.Activity_Start_Date.Month).TrimStart() + SqlFunctions.Replicate("", 2 - SqlFunctions.StringConvert((double)p.Activity_Start_Date.Month).TrimStart().Length) + "-" + SqlFunctions.DateName("year", p.Activity_Start_Date),
                            EndDate = p.Actual_End_Date,
                            ViewEndDate = SqlFunctions.Replicate("0", 2 - SqlFunctions.DateName("dd", p.Activity_End_Date).Trim().Length) + SqlFunctions.DateName("dd", p.Activity_End_Date).Trim() + "-" + SqlFunctions.Replicate("", 2 - SqlFunctions.StringConvert((double)p.Activity_End_Date.Month).TrimStart().Length) + SqlFunctions.Replicate("0", 2 - SqlFunctions.StringConvert((double)p.Activity_End_Date.Month).TrimStart().Length) + SqlFunctions.StringConvert((double)p.Activity_End_Date.Month).TrimStart() + SqlFunctions.Replicate("", 2 - SqlFunctions.StringConvert((double)p.Activity_End_Date.Month).TrimStart().Length) + "-" + SqlFunctions.DateName("year", p.Activity_End_Date),
                            Name = p.Activity_Name ?? bz1.Name + "/" + context.Events.FirstOrDefault(e => e.ID == p.Event_Id).Name ?? "",
                            CMEType = p.CME_Type,
                            TherapyArea = taz1.Name,
                            TherapyAreaId = taz1.ID,
                            Brand = bz1.Name,
                            BrandId = bz1.ID,
                            TotalCost = p.TotalCost,
                            Activity_Start_Date = p.Activity_Start_Date,
                            Approved_Or_Reject = prt.Name,
                            CCCode = context.Cost_Centre.FirstOrDefault(s => s.ID == u.CC_Id).Name ?? string.Empty,
                            IsDateDeviation = p.IsDateDeviation,
                            IsDateDeviationApproved = p.IsDeviationApproved,
                            HotelName = p.Hotel,
                            IsBTC = p.Is_BTC,
                            IsVirtual = p.IsVirtual,
                            Cost_Center = p.Cost_Centre,
                            Disease_Area = p.Disease_Area,
                            IssuerRole = pl.Name,
                            SAPFileGenerated = p.SAP_ReportGeneration ?? "No",
                            CostCentreToCTStatus = ccm1.Status,
                            CostCentreUserId = ccm1.User_Id
                        };

            return query;
        }

        private List<PAFList> ApplyFilters(List<PAFList> query, PAFSearch filterModel)
        {
            if (!string.IsNullOrEmpty(filterModel.PAFId))
            {
                string pafId = filterModel.PAFId.ToLower();
                query = query.Where(a => a.Id.ToString() == pafId || a.IssuedBy.ToLower() == pafId || a.ManagerInitial.ToLower() == pafId || a.HotelName != null && a.HotelName.ToLower() == pafId).ToList();
            }

            if (!string.IsNullOrEmpty(filterModel.SelectedTherapyArea))
            {
                byte therapyAreaId = Convert.ToByte(filterModel.SelectedTherapyArea);
                query = query.Where(s => s.TherapyAreaId == therapyAreaId).ToList();
            }

            if (!string.IsNullOrEmpty(filterModel.SelectedBrand))
            {
                byte brandId = Convert.ToByte(filterModel.SelectedBrand);
                query = query.Where(s => s.BrandId == brandId).ToList();
            }

            if (filterModel.StartDate.HasValue)
            {
                query = query.Where(s => s.Activity_Start_Date >= filterModel.StartDate).ToList();
            }

            if (filterModel.EndDate.HasValue)
            {
                query = query.Where(s => s.Activity_Start_Date <= filterModel.EndDate).ToList();
            }

            return query;
        }

        private List<PAFList> ApplySorting(List<PAFList> query, PAFSearch filterModel)
        {
            switch (filterModel.SortOrder)
            {
                case "Id":
                    query = query.OrderBy(o => o.Id).ToList();
                    break;
                case "Issued":
                    query = query.OrderBy(o => o.IssuedBy).ToList();
                    break;
                case "Issued_Desc":
                    query = query.OrderByDescending(o => o.IssuedBy).ToList();
                    break;
                case "StartDate":
                    query = query.OrderBy(o => o.StartDate).ToList();
                    break;
                case "StartDate_Desc":
                    query = query.OrderByDescending(o => o.StartDate).ToList();
                    break;
                case "EndDate":
                    query = query.OrderBy(o => o.EndDate).ToList();
                    break;
                case "EndDate_Desc":
                    query = query.OrderByDescending(o => o.EndDate).ToList();
                    break;
                case "Request":
                    query = query.OrderBy(o => o.CMEType).ToList();
                    break;
                case "Request_Desc":
                    query = query.OrderByDescending(o => o.CMEType).ToList();
                    break;
                case "ActivityName":
                    query = query.OrderBy(o => o.Name).ToList();
                    break;
                case "ActivityName_Desc":
                    query = query.OrderByDescending(o => o.Name).ToList();
                    break;
                case "CC":
                    query = query.OrderBy(o => o.CCCode).ToList();
                    break;
                case "CC_Desc":
                    query = query.OrderByDescending(o => o.CCCode).ToList();
                    break;
                case "Cost":
                    query = query.OrderBy(o => o.TotalCost).ToList();
                    break;
                case "Cost_Desc":
                    query = query.OrderByDescending(o => o.TotalCost).ToList();
                    break;
                default:
                    query = query.OrderByDescending(o => o.Id).ToList();
                    break;
            }

            return query;
        }
    }
}
