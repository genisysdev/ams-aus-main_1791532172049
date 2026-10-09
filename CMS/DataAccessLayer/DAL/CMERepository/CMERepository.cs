using DataAccessLayer.DAL.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Linq;

namespace DataAccessLayer.DAL.CMERepository
{
    public class CMERepository : ICMERepository
    {
        private readonly NovoNordiskEntities _dataContext;
        public CMERepository(NovoNordiskEntities dataContext)
        {
            this._dataContext = dataContext;
        }

        public CMEDetails GetById(int id)
        {
            //string action = "CME: GetById";
            //pafDetail.Append(Environment.NewLine + Environment.NewLine + "PAF ID" + id);
            try
            {
                var response = new CMEDetails();
                using (NovoNordiskEntities context = new NovoNordiskEntities())
                {
                    response = (from p in context.PAFs
                                join prt in context.PAF_Request_Type on p.Request_Type_Id equals prt.ID
                                join ta in context.Therapy_Area on p.Therapy_Area_Id equals ta.ID
                                join u in context.Users on p.Issued_By_Id equals u.ID
                                join x in context.Cost_Centre on u.CC_Id equals x.ID
                                where p.ID.Equals(id)
                                select new CMEDetails
                                {
                                    Form_ID = p.ID,
                                    CME_Type = p.CME_Type,
                                    DiseaseArea_Type = p.Disease_Area,
                                    Issued_By_Initial = u.Initials,
                                    Approver = p.Approver,
                                    IssuedDate = p.Issued_Date != null ? SqlFunctions.Replicate("0", 2 - SqlFunctions.DateName("dd", p.Issued_Date).Trim().Length) + SqlFunctions.DateName("dd", p.Issued_Date).Trim() + "-" + SqlFunctions.Replicate("", 2 - SqlFunctions.StringConvert((double)DbFunctions.TruncateTime(p.Issued_Date).Value.Month).TrimStart().Length) + SqlFunctions.Replicate("0", 2 - SqlFunctions.StringConvert((double)DbFunctions.TruncateTime(p.Issued_Date).Value.Month).TrimStart().Length) + SqlFunctions.StringConvert((double)DbFunctions.TruncateTime(p.Issued_Date).Value.Month).TrimStart() + SqlFunctions.Replicate("", 2 - SqlFunctions.StringConvert((double)DbFunctions.TruncateTime(p.Issued_Date).Value.Month).TrimStart().Length) + "-" + SqlFunctions.DateName("year", p.Issued_Date) : "",
                                    Name = u.Name + "  " + u.SurName,
                                    ActivityName = p.Activity_Name,
                                    TherapyArea = ta.Name,
                                    BrandName = (context.Brands.Where(b => b.ID == p.Brand_Id).FirstOrDefault() != null) ? context.Brands.Where(b => b.ID == p.Brand_Id).FirstOrDefault().Name : "",
                                    EventName = (context.Events.Where(e => e.ID == p.Event_Id).FirstOrDefault() != null) ? context.Events.Where(e => e.ID == p.Event_Id).FirstOrDefault().Name : "",
                                    LectureName = (context.Lectures.Where(l => l.ID == p.Lecture_Id).FirstOrDefault() != null) ? context.Lectures.Where(l => l.ID == p.Lecture_Id).FirstOrDefault().Lecture1 : "",
                                    ActivityStartDate = SqlFunctions.Replicate("0", 2 - SqlFunctions.DateName("dd", p.Activity_Start_Date).Trim().Length) + SqlFunctions.DateName("dd", p.Activity_Start_Date).Trim() + "-" + SqlFunctions.Replicate("", 2 - SqlFunctions.StringConvert((double)p.Activity_Start_Date.Month).TrimStart().Length) + SqlFunctions.Replicate("0", 2 - SqlFunctions.StringConvert((double)p.Activity_Start_Date.Month).TrimStart().Length) + SqlFunctions.StringConvert((double)p.Activity_Start_Date.Month).TrimStart() + SqlFunctions.Replicate("", 2 - SqlFunctions.StringConvert((double)p.Activity_Start_Date.Month).TrimStart().Length) + "-" + SqlFunctions.DateName("year", p.Activity_Start_Date),
                                    ActivityEndDate = SqlFunctions.Replicate("0", 2 - SqlFunctions.DateName("dd", p.Activity_End_Date).Trim().Length) + SqlFunctions.DateName("dd", p.Activity_End_Date).Trim() + "-" + SqlFunctions.Replicate("", 2 - SqlFunctions.StringConvert((double)p.Activity_End_Date.Month).TrimStart().Length) + SqlFunctions.Replicate("0", 2 - SqlFunctions.StringConvert((double)p.Activity_End_Date.Month).TrimStart().Length) + SqlFunctions.StringConvert((double)p.Activity_End_Date.Month).TrimStart() + SqlFunctions.Replicate("", 2 - SqlFunctions.StringConvert((double)p.Activity_End_Date.Month).TrimStart().Length) + "-" + SqlFunctions.DateName("year", p.Activity_End_Date),
                                    NumberofDays = p.No_Of_Days,
                                    ChangeStartDate = p.Change_Start_Date != null ? SqlFunctions.Replicate("0", 2 - SqlFunctions.DateName("dd", p.Change_Start_Date).Trim().Length) + SqlFunctions.DateName("dd", p.Change_Start_Date).Trim() + "-" + SqlFunctions.Replicate("", 2 - SqlFunctions.StringConvert((double)DbFunctions.TruncateTime(p.Change_Start_Date).Value.Month).TrimStart().Length) + SqlFunctions.Replicate("0", 2 - SqlFunctions.StringConvert((double)DbFunctions.TruncateTime(p.Change_Start_Date).Value.Month).TrimStart().Length) + SqlFunctions.StringConvert((double)DbFunctions.TruncateTime(p.Change_Start_Date).Value.Month).TrimStart() + SqlFunctions.Replicate("", 2 - SqlFunctions.StringConvert((double)DbFunctions.TruncateTime(p.Change_Start_Date).Value.Month).TrimStart().Length) + "-" + SqlFunctions.DateName("year", p.Change_Start_Date) : "",
                                    ChangeEndDate = p.Change_End_Date != null ? SqlFunctions.Replicate("0", 2 - SqlFunctions.DateName("dd", p.Change_End_Date).Trim().Length) + SqlFunctions.DateName("dd", p.Change_End_Date).Trim() + "-" + SqlFunctions.Replicate("", 2 - SqlFunctions.StringConvert((double)DbFunctions.TruncateTime(p.Change_End_Date).Value.Month).TrimStart().Length) + SqlFunctions.Replicate("0", 2 - SqlFunctions.StringConvert((double)DbFunctions.TruncateTime(p.Change_End_Date).Value.Month).TrimStart().Length) + SqlFunctions.StringConvert((double)DbFunctions.TruncateTime(p.Change_End_Date).Value.Month).TrimStart() + SqlFunctions.Replicate("", 2 - SqlFunctions.StringConvert((double)DbFunctions.TruncateTime(p.Change_End_Date).Value.Month).TrimStart().Length) + "-" + SqlFunctions.DateName("year", p.Change_End_Date) : "",
                                    Proposednumberofattendee = p.Proposed_Attendee_Count,
                                    Minimumnumberofgaurantee = p.Min_Guarantee_Count,
                                    Approximatecost = p.Approximate_Count,
                                    Roomrate = p.Room_Rate,
                                    Banquetroomrate = p.Banquet_Room_Rate,
                                    CostPerPAX = p.Cost_Per_Pax,
                                    CityName = p.City,
                                    Venu_Hotel = p.Hotel,
                                    TotalCost = p.TotalCost,
                                    RequestType = prt.Name,
                                    ActivityCloseDate = context.PAF_Transaction.Where(b => b.PAF_Id == p.ID && (b.PAF_Request_Type.Name == "Closed" || b.PAF_Request_Type.Name == "Payment requested")).FirstOrDefault() != null ? SqlFunctions.Replicate("0", 2 - SqlFunctions.DateName("dd", context.PAF_Transaction.Where(b => b.PAF_Id == p.ID && (b.PAF_Request_Type.Name == "Closed" || b.PAF_Request_Type.Name == "Payment requested")).FirstOrDefault().CreatedDate).Trim().Length) + SqlFunctions.DateName("dd", context.PAF_Transaction.Where(b => b.PAF_Id == p.ID && (b.PAF_Request_Type.Name == "Closed" || b.PAF_Request_Type.Name == "Payment requested")).FirstOrDefault().CreatedDate).Trim() + "-" + SqlFunctions.Replicate("", 2 - SqlFunctions.StringConvert((double)p.Activity_Start_Date.Month).TrimStart().Length) + SqlFunctions.Replicate("0", 2 - SqlFunctions.StringConvert((double)context.PAF_Transaction.Where(b => b.PAF_Id == p.ID && (b.PAF_Request_Type.Name == "Closed" || b.PAF_Request_Type.Name == "Payment requested")).FirstOrDefault().CreatedDate.Month).TrimStart().Length) + SqlFunctions.StringConvert((double)context.PAF_Transaction.Where(b => b.PAF_Id == p.ID && (b.PAF_Request_Type.Name == "Closed" || b.PAF_Request_Type.Name == "Payment requested")).FirstOrDefault().CreatedDate.Month).TrimStart() + SqlFunctions.Replicate("", 2 - SqlFunctions.StringConvert((double)context.PAF_Transaction.Where(b => b.PAF_Id == p.ID && (b.PAF_Request_Type.Name == "Closed" || b.PAF_Request_Type.Name == "Payment requested")).FirstOrDefault().CreatedDate.Month).TrimStart().Length) + "-" + SqlFunctions.DateName("year", context.PAF_Transaction.Where(b => b.PAF_Id == p.ID && (b.PAF_Request_Type.Name == "Closed" || b.PAF_Request_Type.Name == "Payment requested")).FirstOrDefault().CreatedDate) : "",
                                    CostCentre_Code = p.Cost_Centre,
                                    PAF_Filename = (from pa in context.PAF_Attachment
                                                    join p in context.PAFs on pa.PAF_Id equals p.ID
                                                    where pa.PAF_Id.Equals(id)
                                                    select new PAFAttachment
                                                    {
                                                        Id = pa.PAF_Id,
                                                        FileName = pa.File_Name
                                                    }).ToList(),
                                    ImagePath = (from pt in context.PAF_Attachment
                                                 where pt.PAF_Id.Equals(id)
                                                 select new LectureMaterials
                                                 {
                                                     DocName = pt.File_Name,
                                                     DocPath = pt.File_Path,
                                                     Extension = pt.File_Type,
                                                     SavedDocName = pt.Saved_File_Name,
                                                     Type = pt.Type,
                                                     PostType = pt.Post_Type
                                                 }
                                                 ).ToList(),
                                    Miscellaneous = (from pm in context.PAF_Miscellaneous
                                                     join m in context.Miscellaneous_Type on pm.Type_Id equals m.ID
                                                     where pm.PAF_Id.Equals(id)
                                                     select new PAFMiscellaneous
                                                     {
                                                         Id = pm.ID,
                                                         Name = m.Name,
                                                         Cost = pm.Price
                                                     }).ToList(),
                                    PAFTransactions = (from pt in context.PAF_Transaction
                                                       join prt in context.PAF_Request_Type on pt.Request_Type_Id equals prt.ID
                                                       where pt.PAF_Id.Equals(id)
                                                       select new PAFTransaction
                                                       {
                                                           RequestType = prt.Name,
                                                           CreatedDate = pt.CreatedDate != null ? SqlFunctions.Replicate("0", 2 - SqlFunctions.DateName("dd", pt.CreatedDate).Trim().Length) + SqlFunctions.DateName("dd", pt.CreatedDate).Trim() + "-" + SqlFunctions.Replicate("", 2 - SqlFunctions.StringConvert((double)DbFunctions.TruncateTime(pt.CreatedDate).Value.Month).TrimStart().Length) + SqlFunctions.Replicate("0", 2 - SqlFunctions.StringConvert((double)DbFunctions.TruncateTime(pt.CreatedDate).Value.Month).TrimStart().Length) + SqlFunctions.StringConvert((double)DbFunctions.TruncateTime(pt.CreatedDate).Value.Month).TrimStart() + SqlFunctions.Replicate("", 2 - SqlFunctions.StringConvert((double)DbFunctions.TruncateTime(pt.CreatedDate).Value.Month).TrimStart().Length) + "-" + SqlFunctions.DateName("year", pt.CreatedDate) : "",
                                                           CreatedBy = (from u in context.Users
                                                                        where u.ID.Equals(pt.CreatedBy)
                                                                        select new { u.Initials }).FirstOrDefault().Initials
                                                       }).ToList(),
                                    HCPs = (from ph in context.PAF_HCP
                                            join toh in context.Type_Of_HCP on ph.Type_Of_HCP_Id equals toh.ID
                                            where ph.PAF_Id.Equals(id)
                                            select new PAFHCPViewModel
                                            {

                                                Id = ph.ID,
                                                First_Name = ph.HCP_First_Name,
                                                Last_Name = ph.HCP_Last_Name,
                                                TMF_No = ph.TMF_No,
                                                Cost = ph.HCP_Payable,
                                                Type = toh.ID,
                                                TypeName = toh.Name,
                                                HCPId = ph.HCP_Id,
                                                CMSNo = (from hcp in context.HCPs
                                                         join pafh in context.PAF_HCP on hcp.ID equals pafh.HCP_Id
                                                         where pafh.ID.Equals(ph.ID)
                                                         select new { hcp.Coupa_No }).FirstOrDefault().Coupa_No,
                                                HCPAddress = (from pafh in context.PAF_HCP
                                                              from hcp in context.HCPs.Where(s => s.ID == pafh.HCP_Id).DefaultIfEmpty()
                                                              where pafh.ID.Equals(ph.ID)
                                                              select hcp.Address).FirstOrDefault(),
                                                Vendor_No = (from pafh in context.PAF_HCP
                                                             from hcp in context.HCPs.Where(s => s.ID == pafh.HCP_Id).DefaultIfEmpty()
                                                             where pafh.ID.Equals(ph.ID)
                                                             select hcp.Vendor_No).FirstOrDefault(),
                                                InvitationMail = ph.Invitation_Email,
                                                SM = p.Approver,
                                                Is_Honorarium = ph.Is_Honorarium,
                                                HCPtopic = ph.HCP_Topic,
                                                Invoice_No = ph.Invoice_No,
                                                Invoice_Date = ph.Invoice_Date.ToString(),
                                                Tax_Amount = ph.Tax_Amount,
                                                Total_Amount = ph.Total_Amount,
                                                Net_Amount = ph.Net_Amount

                                            }).ToList(),
                                    Comments = (from pc in context.PAF_Command
                                                join u in context.Users on pc.CreatedBy equals u.ID
                                                where pc.PAF_Id.Equals(id)
                                                select new PAFComment
                                                {
                                                    Id = pc.ID,
                                                    Initial = u.Initials,
                                                    Comment = pc.Comment,
                                                    Date = pc.CreatedDate != null ? SqlFunctions.Replicate("0", 2 - SqlFunctions.DateName("dd", pc.CreatedDate).Trim().Length) + SqlFunctions.DateName("dd", pc.CreatedDate).Trim() + "-" + SqlFunctions.Replicate("", 2 - SqlFunctions.StringConvert((double)DbFunctions.TruncateTime(pc.CreatedDate).Value.Month).TrimStart().Length) + SqlFunctions.Replicate("0", 2 - SqlFunctions.StringConvert((double)DbFunctions.TruncateTime(pc.CreatedDate).Value.Month).TrimStart().Length) + SqlFunctions.StringConvert((double)DbFunctions.TruncateTime(pc.CreatedDate).Value.Month).TrimStart() + SqlFunctions.Replicate("", 2 - SqlFunctions.StringConvert((double)DbFunctions.TruncateTime(pc.CreatedDate).Value.Month).TrimStart().Length) + "-" + SqlFunctions.DateName("year", pc.CreatedDate) : ""
                                                }).ToList(),
                                    Participants = (from pp in context.PAF_Participants
                                                    where pp.PAF_Id.Value.Equals(id)
                                                    select new PAFParticipants
                                                    {
                                                        ID = pp.HCP_Id,
                                                        Name = pp.Participants_Name,
                                                        PAN = pp.PAN_No,
                                                        Email_Id = pp.Participants_Email_ID,
                                                        Phone_Number = pp.Participants_Phone_Number,
                                                        Customer_ID = pp.Participants_Customer_ID
                                                    }).ToList(),
                                    HospitalMeeting = p.Hospital_Meeting,
                                    IsVirtual = (bool)p.IsVirtual,
                                    IsPostPoned = p.Is_Postponed,
                                    Hotel_Star = p.Hotel_Star == 5 ? true : false,
                                    ModifiedDate = p.ModifiedDate,
                                    IsDownloadSapFile = p.Is_DownloadSapFile,
                                    IsBTC = p.Is_BTC,
                                    Cost_Center = p.Cost_Centre,
                                    MeetingTypeName = (context.PAF_Meetingtype.Where(b => b.Id == p.Meetingtype_Id).FirstOrDefault() != null) ? context.PAF_Meetingtype.Where(b => b.Id == p.Meetingtype_Id).FirstOrDefault().Name : "",
                                    Policy_Violation = p.Policy_Violation,
                                    SAP_ReportGeneration = p.SAP_ReportGeneration ?? "no",
                                    Actual_Attendees = p.Actual_Attendees,
                                    Final_CME_Expenses = p.Final_CME_Expenses
                                }).SingleOrDefault();

                    context.Dispose();
                    return response;
                }
            }
            catch (Exception ex)
            {
                //pafLog.WriteIntoFile(pafDetail + "Error: " + ex.ToString(), action, id.ToString());
                throw ex;
            }
        }

        public CMEModel GetPAFById(int id)
        {
            //string action = "GetPAFById";
            //pafDetail.Append(Environment.NewLine + Environment.NewLine + "PAF ID" + id);
            try
            {
                var response = new CMEModel();
                using (NovoNordiskEntities context = new NovoNordiskEntities())
                {
                    response = (from p in context.PAFs
                                join prt in context.PAF_Request_Type on p.Request_Type_Id equals prt.ID
                                join ta in context.Therapy_Area on p.Therapy_Area_Id equals ta.ID
                                join u in context.Users on p.Issued_By_Id equals u.ID
                                where p.ID.Equals(id)
                                select new CMEModel
                                {
                                    ID = p.ID,
                                    IssuedBy = u.ID,
                                    IssuedByInitial = u.Initials,
                                    IssuedDate = (p.Issued_Date).Value,
                                    Approver = p.Approver,
                                    CME_Type = p.CME_Type,
                                    DiseaseArea_Type = p.Disease_Area,
                                    ActivityName = p.Activity_Name,
                                    TherapyAreaId = ta.ID,
                                    BrandId = context.Brands.Where(b => b.ID == p.Brand_Id).FirstOrDefault().ID,
                                    EventId = context.Events.Where(e => e.ID == p.Event_Id).FirstOrDefault().ID,
                                    LectureId = (p.Lecture_Id).Value,
                                    ActivityStartDate = p.Activity_Start_Date,
                                    ActivityEndDate = p.Activity_End_Date,
                                    ActualStartDate = p.Actual_Start_Date,
                                    ActualEndDate = p.Actual_End_Date,
                                    NumberofDays = p.No_Of_Days,
                                    ChangeStartDate = p.Change_Start_Date,
                                    ChangeEndDate = p.Change_End_Date,
                                    IsDeviationApproved = p.IsDeviationApproved,
                                    IsDateDeviation = p.IsDateDeviation,
                                    Proposednumberofattendee = p.Proposed_Attendee_Count,
                                    Minimumnumberofgaurantee = p.Min_Guarantee_Count,
                                    Approximatecost = p.Approximate_Count,
                                    Roomrate = p.Room_Rate,
                                    Banquetroomrate = p.Banquet_Room_Rate,
                                    CostPerPAX = p.Cost_Per_Pax,
                                    City = p.City,
                                    Venu_Hotel = p.Hotel,
                                    TotalCost = p.TotalCost,
                                    Cost_Center = p.Cost_Centre,
                                    MeetingTypeId = p.Meetingtype_Id,
                                    Policy_Violation = p.Policy_Violation,
                                    RequestType = prt.Name,
                                    Hotel_Star = (int)p.Hotel_Star == 5.0 ? true : false,
                                    ImagePath = (from pt in context.PAF_Attachment
                                                 where pt.PAF_Id.Equals(id)
                                                 select new LectureMaterials
                                                 {
                                                     DocName = pt.File_Name,
                                                     DocPath = pt.File_Path,
                                                     Extension = pt.File_Type,
                                                     SavedDocName = pt.Saved_File_Name,
                                                     Type = pt.Type,
                                                     HCP_First_Name = pt.HCP_Name.Split(' ')[0],
                                                     HCP_Last_Name = pt.HCP_Name.Split(' ')[1],
                                                 }).ToList(),
                                    Miscellaneous = (from pm in context.PAF_Miscellaneous
                                                     join m in context.Miscellaneous_Type on pm.Type_Id equals m.ID
                                                     where pm.PAF_Id.Equals(id)
                                                     select new PAFMiscellaneous
                                                     {
                                                         Id = pm.ID,
                                                         Name = m.Name,
                                                         Cost = pm.Price
                                                     }).ToList(),


                                    HCPs = (from ph in context.PAF_HCP
                                            join toh in context.Type_Of_HCP on ph.Type_Of_HCP_Id equals toh.ID
                                            join h in context.HCPs on ph.HCP_Id equals h.ID
                                            where ph.PAF_Id.Equals(id)
                                            select new PAFHCPViewModel
                                            {
                                                HCPId = ph.HCP_Id,
                                                First_Name = ph.HCP_First_Name,
                                                Last_Name = ph.HCP_Last_Name,
                                                TMF_No = ph.TMF_No,
                                                Cost = ph.HCP_Payable,
                                                Type = toh.ID,
                                                TypeName = toh.Name,
                                                HCPAddress = h.Address,
                                                HCPtopic = ph.HCP_Topic
                                            }).ToList(),
                                    Participants = (from pp in context.PAF_Participants
                                                    where pp.PAF_Id.Value.Equals(id)
                                                    select new PAFParticipants
                                                    {
                                                        ID = pp.HCP_Id,
                                                        Name = pp.Participants_Name,
                                                        PAN = pp.PAN_No,
                                                        Email_Id = pp.Participants_Email_ID,
                                                        Phone_Number = pp.Participants_Phone_Number,
                                                        Customer_ID = pp.Participants_Customer_ID
                                                    }).ToList(),
                                    Comments = (from pc in context.PAF_Command
                                                join u in context.Users on pc.CreatedBy equals u.ID
                                                where pc.PAF_Id.Equals(id)
                                                select new PAFComment
                                                {
                                                    Id = pc.ID,
                                                    Initial = u.Initials,
                                                    Comment = pc.Comment,
                                                    Date = pc.CreatedDate != null ? SqlFunctions.Replicate("0", 2 - SqlFunctions.DateName("dd", pc.CreatedDate).Trim().Length) + SqlFunctions.DateName("dd", pc.CreatedDate).Trim() + "-" + SqlFunctions.Replicate("", 2 - SqlFunctions.StringConvert((double)DbFunctions.TruncateTime(pc.CreatedDate).Value.Month).TrimStart().Length) + SqlFunctions.Replicate("0", 2 - SqlFunctions.StringConvert((double)DbFunctions.TruncateTime(pc.CreatedDate).Value.Month).TrimStart().Length) + SqlFunctions.StringConvert((double)DbFunctions.TruncateTime(pc.CreatedDate).Value.Month).TrimStart() + SqlFunctions.Replicate("", 2 - SqlFunctions.StringConvert((double)DbFunctions.TruncateTime(pc.CreatedDate).Value.Month).TrimStart().Length) + "-" + SqlFunctions.DateName("year", pc.CreatedDate) : ""
                                                }).ToList(),
                                    HospitalMeeting = p.Hospital_Meeting,
                                    IsVirtual = (bool)p.IsVirtual,
                                    IsPostPoned = p.Is_Postponed,
                                    //IssuerZone = (from p in context.PAFs
                                    //              join u in context.Users on p.Issued_By_Id equals u.ID
                                    //              where p.ID.Equals(id)
                                    //              select new ZoneModel
                                    //              {
                                    //                  Name = z.Name
                                    //              }).FirstOrDefault().Name,
                                    //IssuerRole = (from p in context.PAFs
                                    //              join u in context.Users on p.Issued_By_Id equals u.ID
                                    //              join ptu in context.Profiles_To_User on u.ID equals ptu.User_Id
                                    //              join l in context.Profiles on ptu.Profile_Id equals l.ID
                                    //              where p.ID.Equals(id)
                                    //              select new ZoneModel
                                    //              {
                                    //                  Name = l.Name
                                    //              }).FirstOrDefault().Name,
                                    SAP_ReportGeneration = p.SAP_ReportGeneration ?? "no",
                                    Actual_Attendees = p.Actual_Attendees,
                                    Final_CME_Expenses = p.Final_CME_Expenses
                                }
                                ).SingleOrDefault();

                    context.Dispose();
                    return response;
                }
            }
            catch (Exception ex)
            {
                //pafLog.WriteIntoFile(pafDetail + "Error: " + ex.ToString(), action, id.ToString());
                throw ex;
            }
        }

        public CMEDetails GetRoleID(int id)
        {
            //string action = "Paf GetRoleID";

            try
            {
                var response = new CMEDetails();
                using (NovoNordiskEntities context = new NovoNordiskEntities())
                {
                    response = (from ptu in context.Profiles_To_User
                                join u in context.Users on ptu.User_Id equals u.ID
                                where ptu.User_Id.Equals(id)
                                select new CMEDetails
                                {
                                    Login_User_role_id = context.Profiles_To_User.Where(p => p.User_Id == id).FirstOrDefault().Profile_Id
                                }).SingleOrDefault();

                    //pafLog.FetchLogs(pafDetail, action, id);

                    context.Dispose();
                    return response;
                }
            }
            catch (Exception ex)
            {
                //pafLog.WriteIntoFile(pafDetail.ToString(), action, ex.ToString());
                throw ex;
            }
        }

        public List<PAFList> GetLatestviolation(int Id)
        {
            try
            {
                //pafDetail.Append(Environment.NewLine + "GetLatestviolation started: ");
                //Stopwatch sw = new Stopwatch();
                //sw.Start();

                using (NovoNordiskEntities context = new NovoNordiskEntities())
                {
                    var response = (from p in context.PAFs
                                    join ps in context.PAF_Status on p.PAF_Status_Id equals ps.ID
                                    join prt in context.PAF_Request_Type on p.Request_Type_Id equals prt.ID
                                    join u in context.Users on p.Issued_By_Id equals u.ID
                                    orderby p.ID descending
                                    where ps.Name == "Active"
                                    && (p.Issued_By_Id == Id)
                                    && (prt.Name != "Draft")
                                    select new PAFList
                                    {
                                        Id = p.ID,
                                        IssuedBy = u.Initials,
                                        StartDate = p.Activity_Start_Date,
                                        EndDate = p.Actual_End_Date,
                                        Activity_Start_Date = p.Activity_Start_Date,
                                        Approved_Or_Reject = prt.Name,
                                        IsDateDeviation = p.IsDateDeviation
                                    }).Take(10).ToList();


                    context.Dispose();
                    //sw.Stop();
                    //pafDetail.Append(Environment.NewLine + "GetLatestviolation Ended " + sw.Elapsed);
                    //pafLog.WriteIntoFile(pafDetail.ToString(), "GetLatestviolation", "HCP Log");
                    return response;
                }
            }
            catch (Exception ex)
            {
                //PafLog log = new PafLog();
                //log.WriteIntoFile(ex.Message.ToString(), "GetLatestviolation", ex.InnerException.Message);
                throw ex;
            }
        }

        public void UpdateDocument(int id, CMEModel model)
        {
            try
            {
                using (NovoNordiskEntities context = new NovoNordiskEntities())
                {
                    if (model.ImagePath != null)
                    {
                        foreach (var obj in model.ImagePath.Where(s => s.IsNew))
                        {
                            context.PAF_Attachment.Add(new PAF_Attachment
                            {
                                PAF_Id = id,
                                File_Name = obj.DocName.Replace(" ", String.Empty),
                                File_Path = obj.DocPath.Replace(" ", String.Empty),
                                File_Type = obj.Extension,
                                Saved_File_Name = obj.SavedDocName,
                                Type = obj.Type,
                                CreatedDate = DateTime.UtcNow,
                                ModifiedDate = DateTime.UtcNow
                            });
                            context.SaveChanges();
                        }
                    }
                    context.Dispose();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void ApprovePAFBiopharm(int id, CMEDetails model)
        {
            try
            {
                using (NovoNordiskEntities context = new NovoNordiskEntities())
                {
                    DataAccessLayer.PAF latestPAF = context.PAFs.Where(p => p.ID == id).OrderByDescending(o => o.ID).FirstOrDefault();
                    PAF_Transaction pt = context.PAF_Transaction.Where(p => p.PAF_Id == id).OrderByDescending(o => o.ID).FirstOrDefault();
                    if (latestPAF != null)
                    {
                        if (pt.CreatedBy != model.IssuedBy)
                        {
                            byte requestTypeId = 0;
                            PAF_Request_Type cprt = context.PAF_Request_Type.Where(s => s.ID == latestPAF.Request_Type_Id).FirstOrDefault();
                            var userrole = (from u in context.Users
                                            join ptu in context.Profiles_To_User on u.ID equals ptu.User_Id
                                            join p in context.Profiles on ptu.Profile_Id equals p.ID
                                            where u.ID == latestPAF.Issued_By_Id
                                            select new UserModel
                                            {
                                                Role = p.Name
                                            }).FirstOrDefault();

                            /////////////////////
                            if (cprt.Name == "Send")
                            {
                                requestTypeId = context.PAF_Request_Type.Where(s => s.Name == "Approved by SM").FirstOrDefault().ID;
                            }
                            // CT --> RBD --> SCT (2 --> 21 --> 19), SM --> RBD (21 --> 19) Send mail to SCTs    
                            // 11 --> 19
                            if (cprt.Name == "Send to RBD" || cprt.Name == "Approved by SM")
                            {
                                requestTypeId = context.PAF_Request_Type.Where(s => s.Name == "Approved by RBD").FirstOrDefault().ID;
                            }
                            // 19 --> 16
                            if (cprt.Name == "Approved by RBD")
                            {
                                requestTypeId = context.PAF_Request_Type.Where(s => s.Name == "Approved by CME Operation").FirstOrDefault().ID;
                            }
                            //34 -->15 //email notification to SM is not updated
                            if (cprt.Name == "Send to cancel approve")
                            {
                                requestTypeId = context.PAF_Request_Type.Where(s => s.Name == "Delete").FirstOrDefault().ID;
                            }
                            //35 -->2 //email notification to SM is not updated
                            if (cprt.Name == "Send to postpone")
                            {
                                requestTypeId = context.PAF_Request_Type.Where(s => s.Name == "Draft").FirstOrDefault().ID;
                            }
                            /////////////////////
                            latestPAF.Request_Type_Id = requestTypeId;
                            latestPAF.ModifiedDate = DateTime.UtcNow;
                            context.SaveChanges();

                            if (latestPAF.PAF_Request_Type.Name == "Send to cancel approve" || latestPAF.PAF_Request_Type.Name == "Delete")
                            {
                                context.PAF_Command.Add(new PAF_Command { PAF_Id = latestPAF.ID, CreatedBy = model.IssuedBy, CreatedDate = DateTime.UtcNow, Comment = "Approve – Approved cancelation request. " + model.Comment });
                            }
                            else
                            if (!string.IsNullOrEmpty(model.Comment))
                            {
                                context.PAF_Command.Add(new PAF_Command { PAF_Id = latestPAF.ID, CreatedBy = model.IssuedBy, CreatedDate = DateTime.UtcNow, Comment = model.Comment });
                            }

                            #region PAF transaction
                            context.PAF_Transaction.Add(new PAF_Transaction { PAF_Id = latestPAF.ID, Request_Type_Id = requestTypeId, CreatedBy = model.IssuedBy, CreatedDate = DateTime.UtcNow });
                            context.SaveChanges();
                            //var log = new Helper.Log_();
                            //try
                            //{
                            //    context.PAF_Transaction.Add(new PAF_Transaction { PAF_Id = latestPAF.ID, Request_Type_Id = requestTypeId, CreatedBy = model.IssuedBy, CreatedDate = DateTime.UtcNow });
                            //    context.SaveChanges();
                            //    StringBuilder s = new StringBuilder();
                            //    s.Append("PAF_Id:");
                            //    s.Append(latestPAF.ID);
                            //    s.Append(" Request_Type_Id:");
                            //    s.Append(requestTypeId);
                            //    s.Append(" Approver:");
                            //    string Initials = context.Users.Where(u => u.ID == model.IssuedBy).FirstOrDefault().Initials;
                            //    s.Append(Initials);
                            //    s.Append(" CreatedDate:");
                            //    s.Append(DateTime.UtcNow);

                            //    log.WriteIntoFile(s.ToString(), latestPAF.ID.ToString(), "Approve PAF Biopharm Transaction");
                            //}
                            //catch (Exception ex)
                            //{
                            //    log.WriteIntoFile(ex.ToString(), latestPAF.ID.ToString(), "Approve PAF Biopharm Transaction error");
                            //}
                            #endregion

                            #region Email Notification
                            string managerEmail = string.Empty;
                            User user = context.Users.Where(s => s.ID == latestPAF.Issued_By_Id).FirstOrDefault();

                            #region Send
                            string myString = latestPAF.Approver;
                            string[] splitString = myString.Split('/');
                            string result = splitString[1];
                            if (cprt.Name == "Send")
                            {
                                var managerUser = (from u in context.Users
                                                   join ptu in context.Profiles_To_User on u.ID equals ptu.User_Id
                                                   join p in context.Profiles on ptu.Profile_Id equals p.ID
                                                   where u.Initials == result.Trim() && ptu.Status == true
                                                   orderby ptu.ID descending
                                                   select new UserModel
                                                   {
                                                       ID = u.ID,
                                                       Initial = u.Initials,
                                                       Role = p.Name,
                                                       EmailId = u.Email
                                                   }).FirstOrDefault();

                                if (managerUser != null)
                                {
                                    managerEmail = managerUser.EmailId;
                                }
                                else
                                {
                                    managerEmail = string.Empty;
                                }
                            }

                            if (cprt.Name == "Send")
                            {
                                PAFApproveEMail(latestPAF.ID, user.Initials, managerEmail);
                            }
                            #endregion

                            #region Send to RBD and Approved by SM
                            if (cprt.Name == "Send to RBD" || cprt.Name == "Approved by SM")
                            {
                                var SCTuser = (from u in context.Users
                                               join ptu in context.Profiles_To_User on u.ID equals ptu.User_Id
                                               join p in context.Profiles on ptu.Profile_Id equals p.ID
                                               where p.Name == "CME Operation" && u.Status == true
                                               orderby ptu.ID descending
                                               select new UserModel
                                               {
                                                   ID = u.ID,
                                                   Initial = u.Initials,
                                                   Role = p.Name,
                                                   EmailId = u.Email
                                               }).ToList();
                                if (SCTuser != null && latestPAF.IsVirtual != true)
                                {
                                    foreach (var us in SCTuser)
                                    {
                                        PAFApproveEMail(latestPAF.ID, user.Initials, us.EmailId);
                                    }
                                }
                            }
                            #endregion

                            #region Approved by RBD
                            if (cprt.Name == "Approved by RBD")
                            {
                                var managerUser = (from u in context.Users
                                                   join ptu in context.Profiles_To_User on u.ID equals ptu.User_Id
                                                   join p in context.Profiles on ptu.Profile_Id equals p.ID
                                                   join paf in context.PAFs on u.ID equals paf.Issued_By_Id
                                                   where u.ID == latestPAF.Issued_By_Id && ptu.Status == true
                                                   orderby ptu.ID descending
                                                   select new UserModel
                                                   {
                                                       ID = u.ID,
                                                       Initial = u.Initials,
                                                       Role = p.Name,
                                                       EmailId = u.Email
                                                   }).FirstOrDefault();
                                if (managerUser != null)
                                {
                                    managerEmail = managerUser.EmailId;
                                }
                                else
                                {
                                    managerEmail = string.Empty;
                                }
                            }
                            if (cprt.Name == "Approved by RBD")
                            {
                                PAFLastApproveEMail(latestPAF.ID, managerEmail);
                            }
                            #endregion

                            #endregion

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void ApprovePAF(int id, CMEDetails model)
        {
            //string action = "Paf Approve";
            try
            {
                using (NovoNordiskEntities context = new NovoNordiskEntities())
                {
                    DataAccessLayer.PAF latestPAF = context.PAFs.Where(p => p.ID == id).OrderByDescending(o => o.ID).FirstOrDefault();
                    if (latestPAF != null)
                    {
                        byte requestTypeId = 0;
                        PAF_Request_Type cprt = context.PAF_Request_Type.Where(s => s.ID == latestPAF.Request_Type_Id).FirstOrDefault();

                        PAF_Transaction pt = context.PAF_Transaction.Where(p => p.PAF_Id == id).OrderByDescending(o => o.ID).FirstOrDefault(); //Where(s => s.PAF_Id == id).ToList();//OrderByDescending(o => o.ID).FirstOrDefault();


                        //SR --> SM  to RBD(4 to 11) 
                        if (cprt.Name == "Send")
                        {
                            requestTypeId = context.PAF_Request_Type.Where(s => s.Name == "Approved by SM").FirstOrDefault().ID;
                        }
                        // CT --> RBD --> SCT (2 --> 21 --> 19), SM --> RBD (21 --> 19) Send mail to SCTs    
                        // 11 --> 19
                        if (cprt.Name == "Send to RBD" || cprt.Name == "Approved by SM")
                        {
                            if (latestPAF.IsDateDeviation && !latestPAF.IsDeviationApproved)
                            {
                                requestTypeId = context.PAF_Request_Type.Where(s => s.Name == "Approved by RBD").FirstOrDefault().ID;
                            }
                            else
                            {
                                requestTypeId = context.PAF_Request_Type.Where(s => s.Name == "Approved by RBD").FirstOrDefault().ID;
                            }
                        }
                        // 19 --> 16
                        if (cprt.Name == "Approved by RBD")
                        {
                            requestTypeId = context.PAF_Request_Type.Where(s => s.Name == "Approved by CME Operation").FirstOrDefault().ID;
                        }
                        //34 -->15 //email notification to SM is not updated
                        if (cprt.Name == "Send to cancel approve")
                        {
                            requestTypeId = context.PAF_Request_Type.Where(s => s.Name == "Delete").FirstOrDefault().ID;
                        }
                        //35 -->2 //email notification to SM is not updated
                        if (cprt.Name == "Send to postpone")
                        {
                            requestTypeId = context.PAF_Request_Type.Where(s => s.Name == "Draft").FirstOrDefault().ID;
                        }

                        if (pt.CreatedBy != model.IssuedBy)
                        {
                            #region PAF 
                            latestPAF.Request_Type_Id = requestTypeId;
                            latestPAF.ModifiedDate = DateTime.UtcNow;
                            if (cprt.Name == "Approved by RBD") { latestPAF.Is_BTC = latestPAF.Is_BTC; }
                            context.SaveChanges();
                            #endregion

                            #region PAF transaction
                            context.PAF_Transaction.Add(new PAF_Transaction { PAF_Id = latestPAF.ID, Request_Type_Id = requestTypeId, CreatedBy = model.IssuedBy, CreatedDate = DateTime.UtcNow });
                            context.SaveChanges();
                            //Helper.Log_ log = new Helper.Log_();
                            //try
                            //{
                            //    context.PAF_Transaction.Add(new PAF_Transaction { PAF_Id = latestPAF.ID, Request_Type_Id = requestTypeId, CreatedBy = model.IssuedBy, CreatedDate = DateTime.UtcNow });
                            //    context.SaveChanges();
                            //    StringBuilder s = new StringBuilder();
                            //    s.Append("PAF_Id:");
                            //    s.Append(latestPAF.ID);
                            //    s.Append(" Request_Type_Id:");
                            //    s.Append(requestTypeId);
                            //    s.Append(" Approver:");
                            //    string Initials = context.Users.Where(u => u.ID == model.IssuedBy).FirstOrDefault().Initials;
                            //    s.Append(Initials);
                            //    s.Append(" CreatedDate:");
                            //    s.Append(DateTime.UtcNow);
                            //    log.WriteIntoFile(s.ToString(), latestPAF.ID.ToString(), "Approve PAF Transaction");
                            //}
                            //catch (Exception ex)
                            //{
                            //    log.WriteIntoFile(ex.ToString(), latestPAF.ID.ToString(), "Approve PAF Transaction error");
                            //}
                            #endregion

                            #region Comment
                            if (latestPAF.PAF_Request_Type.Name == "Send to cancel approve" || latestPAF.PAF_Request_Type.Name == "Delete")
                            {
                                context.PAF_Command.Add(new PAF_Command { PAF_Id = latestPAF.ID, CreatedBy = model.IssuedBy, CreatedDate = DateTime.UtcNow, Comment = "Approve – Approved cancelation request. " + model.Comment });
                                context.SaveChanges();
                            }
                            else
                            if (!string.IsNullOrEmpty(model.Comment))
                            {
                                context.PAF_Command.Add(new PAF_Command { PAF_Id = latestPAF.ID, CreatedBy = model.IssuedBy, CreatedDate = DateTime.UtcNow, Comment = model.Comment });
                                context.SaveChanges();
                            }
                            #endregion

                            #region PAF deviation
                            PAF_Transaction latestPAFTrn = context.PAF_Transaction.Where(s => s.PAF_Id == latestPAF.ID).OrderByDescending(o => o.ID).FirstOrDefault();
                            if ((DateTime.UtcNow - latestPAFTrn.CreatedDate).TotalDays >= 3)
                            {
                                byte deviationTypeId = 0;
                                if (model.Issuer_Role.ToLower() == "rm") { deviationTypeId = context.Deviation_Type.Where(s => s.Name.ToLower() == "rm approval").FirstOrDefault().ID; }
                                if (model.Issuer_Role.ToLower() == "ct") { deviationTypeId = context.Deviation_Type.Where(s => s.Name.ToLower() == "ct approval").FirstOrDefault().ID; }
                                if (model.Issuer_Role.ToLower() == "rbd") { deviationTypeId = context.Deviation_Type.Where(s => s.Name.ToLower() == "rbd approval").FirstOrDefault().ID; }
                                if (model.Issuer_Role == "CME Operation") { deviationTypeId = context.Deviation_Type.Where(s => s.Name.ToLower() == "sct approval").FirstOrDefault().ID; }

                                context.PAF_Deviation.Add(new PAF_Deviation { PAF_Id = latestPAF.ID, Deviation_Type_Id = deviationTypeId, DeviationBy = model.IssuedBy, CreatedDate = DateTime.UtcNow });
                                context.SaveChanges();
                            }
                            #endregion

                            #region Email

                            string managerEmail = string.Empty;
                            User user = context.Users.Where(s => s.ID == latestPAF.Issued_By_Id).FirstOrDefault();

                            #region Send
                            string myString = latestPAF.Approver;
                            string[] splitString = myString.Split('/');
                            string result = splitString[1];
                            if (cprt.Name == "Send")
                            {
                                var managerUser = (from u in context.Users
                                                   join ptu in context.Profiles_To_User on u.ID equals ptu.User_Id
                                                   join p in context.Profiles on ptu.Profile_Id equals p.ID
                                                   where u.Initials == result.Trim() && ptu.Status == true
                                                   orderby ptu.ID descending
                                                   select new UserModel
                                                   {
                                                       ID = u.ID,
                                                       Initial = u.Initials,
                                                       Role = p.Name,
                                                       EmailId = u.Email
                                                   }).FirstOrDefault();

                                if (managerUser != null)
                                {
                                    managerEmail = managerUser.EmailId;
                                }
                                else
                                {
                                    managerEmail = string.Empty;
                                }
                            }

                            if (cprt.Name == "Send")
                            {
                                PAFApproveEMail(latestPAF.ID, user.Initials, managerEmail);
                            }
                            #endregion

                            #region Send to RBD and Approved by SM
                            if (cprt.Name == "Send to RBD" || cprt.Name == "Approved by SM")
                            {

                                var SCTuser = (from u in context.Users
                                               join ptu in context.Profiles_To_User on u.ID equals ptu.User_Id
                                               join p in context.Profiles on ptu.Profile_Id equals p.ID
                                               where p.Name == "CME Operation" && u.Status == true
                                               orderby ptu.ID descending
                                               select new UserModel
                                               {
                                                   ID = u.ID,
                                                   Initial = u.Initials,
                                                   Role = p.Name,
                                                   EmailId = u.Email
                                               }).ToList();
                                if (SCTuser != null && latestPAF.IsVirtual != true)
                                {
                                    foreach (var us in SCTuser)
                                    {
                                        PAFApproveEMail(latestPAF.ID, user.Initials, us.EmailId);
                                    }
                                }
                            }
                            #endregion

                            #region Approved by RBD
                            if (cprt.Name == "Approved by RBD" || (cprt.Name == "Approved by SM" && latestPAF.Is_BTC == true))
                            {
                                var managerUser = (from u in context.Users
                                                   join ptu in context.Profiles_To_User on u.ID equals ptu.User_Id
                                                   join p in context.Profiles on ptu.Profile_Id equals p.ID
                                                   join paf in context.PAFs on u.ID equals paf.Issued_By_Id
                                                   where u.ID == latestPAF.Issued_By_Id && ptu.Status == true
                                                   orderby ptu.ID descending
                                                   select new UserModel
                                                   {
                                                       ID = u.ID,
                                                       Initial = u.Initials,
                                                       Role = p.Name,
                                                       EmailId = u.Email
                                                   }).FirstOrDefault();
                                if (managerUser != null)
                                {
                                    managerEmail = managerUser.EmailId;
                                }
                                else
                                {
                                    managerEmail = string.Empty;
                                }
                            }
                            #endregion

                            #region Final Approval Mail to SCT, CT                        
                            if (cprt.Name == "Approved by RBD" || cprt.Name == "Approved by CME Operation" || (cprt.Name == "Approved by SM" && latestPAF.Is_BTC == true))
                            {
                                PAFLastApproveEMail(latestPAF.ID, managerEmail);

                                #region Final Approval Mail to SCT
                                if (latestPAF.Is_BTC == true)
                                {
                                    var SCTuser = (from u in context.Users
                                                   join ptu in context.Profiles_To_User on u.ID equals ptu.User_Id
                                                   join p in context.Profiles on ptu.Profile_Id equals p.ID
                                                   where p.Name == "CME Operation" && u.Status == true
                                                   orderby ptu.ID descending
                                                   select new UserModel
                                                   {
                                                       ID = u.ID,
                                                       Initial = u.Initials,
                                                       Role = p.Name,
                                                       EmailId = u.Email
                                                   }).ToList();

                                    foreach (var us in SCTuser)
                                    {
                                        PAFLastApproveEMail(latestPAF.ID, us.EmailId);
                                    }
                                }
                                #endregion

                                #region Final Approval Mail to CT
                                var CTuser = (from u in context.Users
                                              join ptu in context.Profiles_To_User on u.ID equals ptu.User_Id
                                              join p in context.Profiles on ptu.Profile_Id equals p.ID
                                              where p.Name == "CT" && u.Status == true
                                              orderby ptu.ID descending
                                              select new UserModel
                                              {
                                                  ID = u.ID,
                                                  Initial = u.Initials,
                                                  Role = p.Name,
                                                  EmailId = u.Email
                                              }).ToList();

                                foreach (var us in CTuser)
                                {
                                    PAFLastApproveEMail(latestPAF.ID, us.EmailId);
                                }
                                #endregion

                            }
                            #endregion
                            #region Final Approval Mail to requestor
                            if ((cprt.Name == "Approved by RBD" && latestPAF.Is_BTC == true) //_BTC is true
                               || (cprt.Name == "Send to RBD" && latestPAF.Is_BTC == false) //_BTC is false by SM
                               || (cprt.Name == "Approved by SM" && latestPAF.Is_BTC == false) //_BTC is false by SR
                               )
                            {
                                int user_id = latestPAF.Issued_By_Id;
                                var Ruser = (from u in context.Users
                                             where id.Equals(user_id)
                                             select new UserModel
                                             {
                                                 ID = u.ID,
                                                 Initial = u.Initials,
                                                 EmailId = u.Email
                                             }).FirstOrDefault();
                                if (Ruser != null)
                                {
                                    PAFLastApproveEMail(latestPAF.ID, Ruser.EmailId);
                                }
                            }
                            #endregion
                            #endregion

                            //pafDetail.Append(Environment.NewLine + Environment.NewLine + "PAF " + latestPAF.ID + " " + latestPAF?.PAF_Request_Type?.Name + " " + latestPAF?.ModifiedDate + Environment.NewLine);
                            //pafLog.FetchLogs(pafDetail, action, latestPAF.ID);
                        }

                    }
                    context.Dispose();
                }
            }
            catch (Exception ex)
            {
                //pafLog.WriteIntoFile(pafDetail + "Error: " + ex.ToString(), action, id.ToString());
                throw ex;
            }
        }

        public void RejectPAFBiopharm(int id, CMEDetails model)
        {
            //string action = "RejectPAFBioPharma";
            try
            {
                using (NovoNordiskEntities context = new NovoNordiskEntities())
                {
                    DataAccessLayer.PAF latestPAF = context.PAFs.Where(p => p.ID == id).OrderByDescending(o => o.ID).FirstOrDefault();
                    PAF_Transaction pt = context.PAF_Transaction.Where(p => p.PAF_Id == id).OrderByDescending(o => o.ID).FirstOrDefault();

                    if (latestPAF != null)
                    {
                        if (pt.CreatedBy != model.IssuedBy)
                        {
                            if (model.Issuer_Role.ToLower() == "rm")
                            {
                                byte pafRequestType;
                                if (latestPAF.PAF_Request_Type.Name == "Send to cancel approve")
                                {
                                    List<PAF_Transaction> transactionList = context.PAF_Transaction.Where(s => s.PAF_Id == id).OrderByDescending(o => o.ID).ToList<PAF_Transaction>();
                                    byte tr_id = transactionList[1].Request_Type_Id;
                                    latestPAF.Request_Type_Id = tr_id;
                                    context.SaveChanges();
                                    pafRequestType = tr_id;
                                }
                                else
                                {
                                    PAF_Request_Type rt = context.PAF_Request_Type.Where(s => s.Name == "reject by rm").FirstOrDefault();
                                    latestPAF.Request_Type_Id = rt.ID;
                                    context.SaveChanges();
                                    pafRequestType = rt.ID;
                                }
                                context.PAF_Transaction.Add(new PAF_Transaction { PAF_Id = latestPAF.ID, Request_Type_Id = pafRequestType, CreatedBy = model.IssuedBy, CreatedDate = DateTime.UtcNow });
                                context.SaveChanges();
                            }
                            if (model.Issuer_Role.ToLower() == "rbd")
                            {
                                PAF_Request_Type rt = context.PAF_Request_Type.Where(s => s.Name == "Reject by RBD").FirstOrDefault();
                                latestPAF.Request_Type_Id = rt.ID;
                                context.SaveChanges();
                                byte pafRequestType = rt.ID;
                                pafRequestType = context.PAF_Request_Type.Where(s => s.Name == "Reject by RBD").FirstOrDefault().ID;
                                context.PAF_Transaction.Add(new PAF_Transaction { PAF_Id = latestPAF.ID, Request_Type_Id = pafRequestType, CreatedBy = model.IssuedBy, CreatedDate = DateTime.UtcNow });
                                context.SaveChanges();
                            }
                            if (model.Issuer_Role.ToLower() == "rbd1")
                            {
                                PAF_Request_Type rt = context.PAF_Request_Type.Where(s => s.Name == "Reject by RBD1").FirstOrDefault();
                                latestPAF.Request_Type_Id = rt.ID;
                                context.SaveChanges();
                                byte pafRequestType = rt.ID;
                                pafRequestType = context.PAF_Request_Type.Where(s => s.Name == "Reject by RBD1").FirstOrDefault().ID;
                                context.PAF_Transaction.Add(new PAF_Transaction { PAF_Id = latestPAF.ID, Request_Type_Id = pafRequestType, CreatedBy = model.IssuedBy, CreatedDate = DateTime.UtcNow });
                                context.SaveChanges();
                            }
                            if (model.Issuer_Role.ToLower() == "rbd2")
                            {
                                PAF_Request_Type rt = context.PAF_Request_Type.Where(s => s.Name == "Reject by RBD2").FirstOrDefault();
                                latestPAF.Request_Type_Id = rt.ID;
                                context.SaveChanges();
                                byte pafRequestType = rt.ID;
                                pafRequestType = context.PAF_Request_Type.Where(s => s.Name == "Reject by RBD2").FirstOrDefault().ID;
                                context.PAF_Transaction.Add(new PAF_Transaction { PAF_Id = latestPAF.ID, Request_Type_Id = pafRequestType, CreatedBy = model.IssuedBy, CreatedDate = DateTime.UtcNow });
                                context.SaveChanges();
                            }
                            if (model.Issuer_Role.ToLower() == "hemophilia medical")
                            {
                                PAF_Request_Type rt = context.PAF_Request_Type.Where(s => s.Name == "Rejected by Hemophilia Medical").FirstOrDefault();
                                latestPAF.Request_Type_Id = rt.ID;
                                context.SaveChanges();
                                byte pafRequestType = rt.ID;
                                pafRequestType = context.PAF_Request_Type.Where(s => s.Name.ToLower() == "Rejected by Hemophilia Medical").FirstOrDefault().ID;
                                context.PAF_Transaction.Add(new PAF_Transaction { PAF_Id = latestPAF.ID, Request_Type_Id = pafRequestType, CreatedBy = model.IssuedBy, CreatedDate = DateTime.UtcNow });
                                context.SaveChanges();
                            }
                            if (model.Issuer_Role.ToLower() == "growth hormone medical")
                            {
                                PAF_Request_Type rt = context.PAF_Request_Type.Where(s => s.Name == "Rejected by Growth Hormone Medical").FirstOrDefault();
                                latestPAF.Request_Type_Id = rt.ID;
                                context.SaveChanges();
                                byte pafRequestType = rt.ID;
                                pafRequestType = context.PAF_Request_Type.Where(s => s.Name.ToLower() == "Rejected by Growth Hormone Medical").FirstOrDefault().ID;
                                context.PAF_Transaction.Add(new PAF_Transaction { PAF_Id = latestPAF.ID, Request_Type_Id = pafRequestType, CreatedBy = model.IssuedBy, CreatedDate = DateTime.UtcNow });
                                context.SaveChanges();
                            }
                            if (model.Issuer_Role.ToLower() == "ctb")
                            {
                                PAF_Request_Type rt = context.PAF_Request_Type.Where(s => s.Name == "Rejected by CTB").FirstOrDefault();
                                latestPAF.Request_Type_Id = rt.ID;
                                context.SaveChanges();
                                byte pafRequestType = rt.ID;
                                pafRequestType = context.PAF_Request_Type.Where(s => s.Name.ToLower() == "Rejected by CTB").FirstOrDefault().ID;
                                context.PAF_Transaction.Add(new PAF_Transaction { PAF_Id = latestPAF.ID, Request_Type_Id = pafRequestType, CreatedBy = model.IssuedBy, CreatedDate = DateTime.UtcNow });
                                context.SaveChanges();
                            }
                            if (latestPAF.PAF_Request_Type.Name == "Send to cancel approve")
                            {
                                context.PAF_Command.Add(new PAF_Command { PAF_Id = latestPAF.ID, CreatedBy = model.IssuedBy, CreatedDate = DateTime.UtcNow, Comment = "Rejected - Rejected cancelation request. " + model.Comment });
                                context.SaveChanges();
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(model.Comment))
                                {
                                    context.PAF_Command.Add(new PAF_Command { PAF_Id = latestPAF.ID, CreatedBy = model.IssuedBy, CreatedDate = DateTime.UtcNow, Comment = model.Comment });
                                    context.SaveChanges();
                                }
                            }

                            //pafDetail.Append(Environment.NewLine + "PAF: " + latestPAF.ID + " - is " + latestPAF?.PAF_Request_Type?.Name + " - By: " + model.IssuedBy + " - Comments: " + model.Comment + Environment.NewLine);
                            //pafLog.FetchLogs(pafDetail, action, latestPAF.ID);

                            #region Sending Email
                            string managerEmail = string.Empty;
                            if (latestPAF.PAF_Request_Type.Name != null)
                            {
                                var managerUser = (from u in context.Users
                                                   join ptu in context.Profiles_To_User on u.ID equals ptu.User_Id
                                                   join p in context.Profiles on ptu.Profile_Id equals p.ID
                                                   join paf in context.PAFs on u.ID equals paf.Issued_By_Id
                                                   where u.ID == latestPAF.Issued_By_Id && ptu.Status == true
                                                   orderby ptu.ID descending
                                                   select new UserModel
                                                   {
                                                       ID = u.ID,
                                                       Initial = u.Initials,
                                                       Role = p.Name,
                                                       EmailId = u.Email
                                                   }).FirstOrDefault();
                                if (managerUser != null)
                                {
                                    managerEmail = managerUser.EmailId;
                                }
                                else
                                {
                                    managerEmail = string.Empty;
                                }

                                PAFRejectedEMail(latestPAF.ID, managerEmail);

                            }
                            #endregion
                        }
                    }
                    context.Dispose();
                }
            }
            catch (Exception ex)
            {
                //pafLog.WriteIntoFile(pafDetail + "Error: " + ex.ToString(), action, id.ToString());
                throw ex;
            }
        }

        public void RejectPAF(int id, CMEDetails model)
        {
            //string action = "Paf Reject";
            try
            {
                using (NovoNordiskEntities context = new NovoNordiskEntities())
                {
                    DataAccessLayer.PAF latestPAF = context.PAFs.Where(p => p.ID == id).OrderByDescending(o => o.ID).FirstOrDefault();
                    PAF_Transaction pt = context.PAF_Transaction.Where(p => p.PAF_Id == id).OrderByDescending(o => o.ID).FirstOrDefault();
                    if (latestPAF != null)
                    {
                        if (pt.CreatedBy != model.IssuedBy)
                        {
                            #region Comment
                            if (latestPAF.PAF_Request_Type.Name == "Send to cancel approve")
                            {
                                context.PAF_Command.Add(new PAF_Command { PAF_Id = latestPAF.ID, CreatedBy = model.IssuedBy, CreatedDate = DateTime.UtcNow, Comment = "Rejected - Rejected cancelation request. " + model.Comment });
                                context.SaveChanges();
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(model.Comment))
                                {
                                    context.PAF_Command.Add(new PAF_Command { PAF_Id = latestPAF.ID, CreatedBy = model.IssuedBy, CreatedDate = DateTime.UtcNow, Comment = model.Comment });
                                    context.SaveChanges();
                                }
                            }

                            #endregion

                            #region PAF deviation
                            PAF_Transaction latestPAFTrn = context.PAF_Transaction.Where(s => s.PAF_Id == latestPAF.ID).OrderByDescending(o => o.ID).FirstOrDefault();
                            if ((DateTime.UtcNow - latestPAFTrn.CreatedDate).TotalDays >= 3)
                            {
                                byte deviationTypeId = 0;
                                if (model.Issuer_Role.ToLower() == "rm") { deviationTypeId = context.Deviation_Type.Where(s => s.Name.ToLower() == "rm reject").FirstOrDefault().ID; }
                                if (model.Issuer_Role.ToLower() == "rbd") { deviationTypeId = context.Deviation_Type.Where(s => s.Name.ToLower() == "rbd reject").FirstOrDefault().ID; }
                                if (model.Issuer_Role == "CME Operation") { deviationTypeId = context.Deviation_Type.Where(s => s.Name.ToLower() == "sct reject").FirstOrDefault().ID; }

                                context.PAF_Deviation.Add(new PAF_Deviation { PAF_Id = latestPAF.ID, Deviation_Type_Id = deviationTypeId, DeviationBy = model.IssuedBy, CreatedDate = DateTime.UtcNow });
                                context.SaveChanges();
                            }
                            #endregion

                            #region PAF Transaction
                            byte pafRequestType = 0;
                            if (model.Issuer_Role.ToLower() == "rm")
                            {
                                //byte pafRequestType;
                                if (latestPAF.PAF_Request_Type.Name == "Send to cancel approve")
                                {
                                    List<PAF_Transaction> transactionList = context.PAF_Transaction.Where(s => s.PAF_Id == id).OrderByDescending(o => o.ID).ToList<PAF_Transaction>();
                                    byte tr_id = transactionList[1].Request_Type_Id;
                                    latestPAF.Request_Type_Id = tr_id;
                                    context.SaveChanges();
                                    pafRequestType = tr_id;
                                }
                                else
                                {
                                    pafRequestType = context.PAF_Request_Type.Where(s => s.Name.ToLower() == "reject by rm").FirstOrDefault().ID;
                                    latestPAF.Request_Type_Id = pafRequestType;
                                    context.SaveChanges();

                                }

                            }
                            if (model.Issuer_Role.ToLower() == "rbd")
                            {
                                pafRequestType = context.PAF_Request_Type.Where(s => s.Name == "Reject by RBD").FirstOrDefault().ID;
                                latestPAF.Request_Type_Id = pafRequestType;
                            }
                            if (model.Issuer_Role == "CME Operation")
                            {
                                pafRequestType = context.PAF_Request_Type.Where(s => s.Name == "Reject by CME Operation").FirstOrDefault().ID;
                                latestPAF.Request_Type_Id = pafRequestType;
                            }
                            #region PAF Transaction
                            context.PAF_Transaction.Add(new PAF_Transaction { PAF_Id = latestPAF.ID, Request_Type_Id = pafRequestType, CreatedBy = model.IssuedBy, CreatedDate = DateTime.UtcNow });
                            context.SaveChanges();
                            //Helper.Log_ log = new Helper.Log_();
                            //try
                            //{
                            //    context.PAF_Transaction.Add(new PAF_Transaction { PAF_Id = latestPAF.ID, Request_Type_Id = pafRequestType, CreatedBy = model.IssuedBy, CreatedDate = DateTime.UtcNow });
                            //    context.SaveChanges();
                            //    StringBuilder s = new StringBuilder();
                            //    s.Append("PAF_Id:");
                            //    s.Append(latestPAF.ID);
                            //    s.Append(" Request_Type_Id:");
                            //    s.Append(pafRequestType);
                            //    s.Append(" Rejected By:");
                            //    string Initials = context.Users.Where(u => u.ID == model.IssuedBy).FirstOrDefault().Initials;
                            //    s.Append(Initials);
                            //    s.Append(" CreatedDate:");
                            //    s.Append(DateTime.UtcNow);

                            //    log.WriteIntoFile(s.ToString(), latestPAF.ID.ToString(), "Reject PAF Transaction");
                            //}
                            //catch (Exception ex)
                            //{
                            //    log.WriteIntoFile(ex.ToString(), latestPAF.ID.ToString(), "Reject PAF Transaction error");
                            //}
                            #endregion
                            #endregion

                            #region PAF transaction and sending Email
                            string managerEmail = string.Empty;
                            if (latestPAF.PAF_Request_Type.Name != null)
                            {
                                var managerUser = (from u in context.Users
                                                   join ptu in context.Profiles_To_User on u.ID equals ptu.User_Id
                                                   join p in context.Profiles on ptu.Profile_Id equals p.ID
                                                   join paf in context.PAFs on u.ID equals paf.Issued_By_Id
                                                   where u.ID == latestPAF.Issued_By_Id && ptu.Status == true
                                                   orderby ptu.ID descending
                                                   select new UserModel
                                                   {
                                                       ID = u.ID,
                                                       Initial = u.Initials,
                                                       Role = p.Name,
                                                       EmailId = u.Email
                                                   }).FirstOrDefault();
                                if (managerUser != null)
                                {
                                    managerEmail = managerUser.EmailId;
                                }
                                else
                                {
                                    managerEmail = string.Empty;
                                }
                            }

                            PAFRejectedEMail(latestPAF.ID, managerEmail);

                            #endregion

                            //pafDetail.Append(Environment.NewLine + "PAF: " + latestPAF.ID + " " + latestPAF?.PAF_Request_Type?.Name + " - by: " + model.Issuer_Role + " - Comments: " + model.Comment + Environment.NewLine);
                            //pafLog.FetchLogs(pafDetail, action, latestPAF.ID);
                        }
                    }
                    context.Dispose();
                }
            }
            catch (Exception ex)
            {
                //pafLog.WriteIntoFile(pafDetail + "Error: " + ex.ToString(), action, id.ToString());
                throw ex;
            }
        }

        public void SendtoClosePAFBiopharm(int id, CMEDetails model)
        {
            try
            {
                using (NovoNordiskEntities context = new NovoNordiskEntities())
                {
                    DataAccessLayer.PAF latestPAF = context.PAFs.Where(p => p.ID == id).OrderByDescending(o => o.ID).FirstOrDefault();
                    if (latestPAF != null)
                    {
                        #region PAF
                        PAF_Request_Type rt = context.PAF_Request_Type.Where(s => s.Name == "Send to CTB").FirstOrDefault();
                        latestPAF.Request_Type_Id = rt.ID;
                        latestPAF.ModifiedDate = DateTime.UtcNow;
                        context.SaveChanges();
                        #endregion

                        #region PAF transaction
                        context.PAF_Transaction.Add(new PAF_Transaction { PAF_Id = latestPAF.ID, Request_Type_Id = rt.ID, CreatedBy = model.IssuedBy, CreatedDate = DateTime.UtcNow });
                        context.SaveChanges();
                        //Helper.Log_ log = new Helper.Log_();
                        //try
                        //{
                        //    context.PAF_Transaction.Add(new PAF_Transaction { PAF_Id = latestPAF.ID, Request_Type_Id = rt.ID, CreatedBy = model.IssuedBy, CreatedDate = DateTime.UtcNow });
                        //    context.SaveChanges();
                        //    StringBuilder s = new StringBuilder();
                        //    s.Append("PAF_Id:");
                        //    s.Append(latestPAF.ID);
                        //    s.Append(" Request_Type_Id:");
                        //    s.Append(rt.ID);
                        //    s.Append(" Closed By:");
                        //    string Initials = context.Users.Where(u => u.ID == model.IssuedBy).FirstOrDefault().Initials;
                        //    s.Append(Initials);
                        //    s.Append(" CreatedDate:");
                        //    s.Append(DateTime.UtcNow);

                        //    log.WriteIntoFile(s.ToString(), latestPAF.ID.ToString(), "send Close PAF Biopharm Transaction");
                        //}
                        //catch (Exception ex)
                        //{
                        //    log.WriteIntoFile(ex.ToString(), latestPAF.ID.ToString(), "send Close PAF Biopharm Transaction error");
                        //}
                        #endregion

                        #region Comment
                        if (!string.IsNullOrEmpty(model.Comment))
                        {
                            context.PAF_Command.Add(new PAF_Command { PAF_Id = latestPAF.ID, CreatedBy = model.IssuedBy, CreatedDate = DateTime.UtcNow, Comment = model.Comment });
                            context.SaveChanges();
                        }
                        #endregion

                        #region Save PAF Attachment
                        foreach (LectureMaterials obj in model.ImagePath.Where(s => s.IsNew))
                        {
                            context.PAF_Attachment.Add(new PAF_Attachment
                            {
                                PAF_Id = id,
                                File_Name = obj.DocName.Replace(" ", String.Empty),
                                File_Path = obj.DocPath.Replace(" ", String.Empty),
                                File_Type = obj.Extension,
                                Saved_File_Name = obj.SavedDocName,
                                Type = obj.Type,
                                Post_Type = obj.PostType,
                                CreatedDate = DateTime.UtcNow,
                                ModifiedDate = DateTime.UtcNow
                            });
                            context.SaveChanges();
                        }
                        #endregion

                        #region Email
                        if (latestPAF.PAF_Request_Type.Name == "Send to CTB")
                        {
                            User user = context.Users.Where(s => s.ID == latestPAF.Issued_By_Id).FirstOrDefault();
                            var CTBuser = (from u in context.Users
                                           join ptu in context.Profiles_To_User on u.ID equals ptu.User_Id
                                           join p in context.Profiles on ptu.Profile_Id equals p.ID
                                           where p.Name == "CTB" && u.Status == true
                                           orderby ptu.ID descending
                                           select new UserModel
                                           {
                                               ID = u.ID,
                                               Initial = u.Initials,
                                               Role = p.Name,
                                               EmailId = u.Email
                                           }).ToList();
                            if (CTBuser != null)
                            {
                                foreach (var us in CTBuser)
                                {
                                    PAFSendClosedEMail(latestPAF.ID, user.Initials, us.EmailId);
                                }
                            }
                        }
                        #endregion
                    }
                    context.Dispose();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void ClosePAFBiopharm(int id, CMEDetails model)
        {
            //try
            //{
            //    using (NovoNordiskEntities context = new NovoNordiskEntities())
            //    {
            //        DataAccessLayer.PAF latestPAF = context.PAFs.Where(p => p.ID == id).OrderByDescending(o => o.ID).FirstOrDefault();
            //        if (latestPAF != null)
            //        {
            //            PAF_Request_Type rt = context.PAF_Request_Type.Where(s => s.Name == "Payment requested").FirstOrDefault();

            //            #region PAF
            //            latestPAF.Request_Type_Id = rt.ID;
            //            latestPAF.ModifiedDate = DateTime.UtcNow;
            //            context.SaveChanges();
            //            #endregion

            //            #region PAF transaction
            //            context.PAF_Transaction.Add(new PAF_Transaction { PAF_Id = latestPAF.ID, Request_Type_Id = rt.ID, CreatedBy = model.IssuedBy, CreatedDate = DateTime.UtcNow });
            //            context.SaveChanges();
            //            //Helper.Log_ log = new Helper.Log_();
            //            //try
            //            //{
            //            //    context.PAF_Transaction.Add(new PAF_Transaction { PAF_Id = latestPAF.ID, Request_Type_Id = rt.ID, CreatedBy = model.IssuedBy, CreatedDate = DateTime.UtcNow });
            //            //    context.SaveChanges();
            //            //    StringBuilder s = new StringBuilder();
            //            //    s.Append("PAF_Id:");
            //            //    s.Append(latestPAF.ID);
            //            //    s.Append(" Request_Type_Id:");
            //            //    s.Append(rt.ID);
            //            //    s.Append(" CreatedBy:");
            //            //    string Initials = context.Users.Where(u => u.ID == model.IssuedBy).FirstOrDefault().Initials;
            //            //    s.Append(Initials);
            //            //    s.Append(" CreatedDate:");
            //            //    s.Append(DateTime.UtcNow);

            //            //    log.WriteIntoFile(s.ToString(), latestPAF.ID.ToString(), "Close PAF Biopharm Transaction");
            //            //}
            //            //catch (Exception ex)
            //            //{
            //            //    log.WriteIntoFile(ex.ToString(), latestPAF.ID.ToString(), "Close PAF Biopharm Transaction error");
            //            //}
            //            #endregion

            //            #region Comment
            //            if (!string.IsNullOrEmpty(model.Comment))
            //            {
            //                context.PAF_Command.Add(new PAF_Command { PAF_Id = latestPAF.ID, CreatedBy = model.IssuedBy, CreatedDate = DateTime.UtcNow, Comment = model.Comment });
            //                context.SaveChanges();
            //            }
            //            #endregion

            //            #region Save PAF Attachment
            //            foreach (var obj in model.ImagePath.Where(s => s.IsNew))
            //            {
            //                context.PAF_Attachment.Add(new PAF_Attachment
            //                {
            //                    PAF_Id = id,
            //                    File_Name = obj.DocName.Replace(" ", String.Empty),
            //                    File_Path = obj.DocPath.Replace(" ", String.Empty),
            //                    File_Type = obj.Extension,
            //                    Saved_File_Name = obj.SavedDocName,
            //                    Type = obj.Type,
            //                    Post_Type = obj.PostType,
            //                    CreatedDate = DateTime.UtcNow,
            //                    ModifiedDate = DateTime.UtcNow
            //                });
            //                context.SaveChanges();
            //            }
            //            #endregion

            //            #region Inserting HCP List
            //            if (model.HCPs != null)
            //            {
            //                #region Remove/Update existing list for specific PAF
            //                List<PAF_HCP> hcpList = context.PAF_HCP.Where(s => s.PAF_Id == id).ToList<PAF_HCP>();
            //                foreach (PAF_HCP obj in hcpList)
            //                {
            //                    #region No_Of_Lect_Given calculations
            //                    DataAccessLayer.HCP latestHCP = context.HCPs.Where(p => p.ID == obj.HCP_Id).OrderByDescending(o => o.ID).FirstOrDefault();
            //                    if (latestHCP != null)
            //                    {
            //                        if (latestHCP.No_Of_Lect_Given <= latestHCP.No_Of_Lect)
            //                        {
            //                            latestHCP.No_Of_Lect_Given -= 1;
            //                            context.SaveChanges();
            //                        }
            //                    }
            //                    #endregion

            //                    #region Update HCP's
            //                    foreach (var mHCPs in model.HCPs)
            //                    {
            //                        if (obj.Is_Honorarium == false)
            //                        {
            //                            mHCPs.Invoice_Date = DateTime.UtcNow.ToString("dd/MM/yyyy");
            //                            mHCPs.Invoice_No = " ";
            //                        }

            //                        if (mHCPs.Id == obj.ID)
            //                        {
            //                            obj.PAF_Id = id;
            //                            obj.Type_Of_HCP_Id = mHCPs.Type;
            //                            obj.HCP_Id = mHCPs.HCPId;
            //                            obj.HCP_Name = mHCPs.Name;
            //                            obj.HCP_Name = mHCPs.Name;
            //                            obj.HCP_Payable = mHCPs.Cost;
            //                            obj.Invitation_Email = mHCPs.Invitation;
            //                            obj.Is_Honorarium = mHCPs.HCPId == null ? false : true;
            //                            obj.CreatedDate = DateTime.UtcNow;
            //                            obj.ModifiedDate = DateTime.UtcNow;
            //                            obj.HCP_Topic = mHCPs.HCPtopic;
            //                            obj.Invoice_No = mHCPs.Invoice_No;
            //                            obj.Invoice_Date = Convert.ToDateTime(mHCPs.Invoice_Date, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
            //                            obj.Net_Amount = mHCPs.Net_Amount;
            //                            obj.Tax_Amount = mHCPs.Tax_Amount;
            //                            obj.Total_Amount = mHCPs.Total_Amount;
            //                        }
            //                    }
            //                    #endregion
            //                    //context.PAF_HCP.Remove(obj);
            //                    context.SaveChanges();
            //                }

            //                #endregion

            //                foreach (var obj in model.HCPs)
            //                {                               
            //                    DataAccessLayer.HCP latestHCP = context.HCPs.Where(p => p.ID == obj.HCPId).OrderByDescending(o => o.ID).FirstOrDefault();
            //                    if (latestHCP != null)
            //                    {
            //                        if (latestHCP.No_Of_Lect_Given < latestHCP.No_Of_Lect)
            //                        {
            //                            latestHCP.No_Of_Lect_Given += 1;
            //                            context.SaveChanges();
            //                        }
            //                    }
            //                }
            //            }

            //            #endregion

            //            #region Email
            //            if (latestPAF.PAF_Request_Type.Name == "Payment requested")
            //            {
            //                User user = context.Users.Where(s => s.ID == model.IssuedBy).FirstOrDefault();
            //                var users = new List<UserModel>();
            //                users = (from u in context.Users
            //                         join ptu in context.Profiles_To_User on u.ID equals ptu.User_Id
            //                         join p in context.Profiles on ptu.Profile_Id equals p.ID
            //                         where ptu.Status == true && (p.Name == "CT" || p.Name == "CTB") /*&& u.Zone_Id == latestPAF.User.Zone_Id*/
            //                         select new UserModel
            //                         {
            //                             ID = u.ID,
            //                             Initial = u.Initials,
            //                             Role = p.Name,
            //                             EmailId = u.Email
            //                         }).ToList();
            //                if (users != null)
            //                {
            //                    foreach (var us in users)
            //                    {
            //                        PAFClosedEMail(latestPAF.ID, user.Initials, us.EmailId);
            //                    }
            //                }
            //                if (/*user.Zone.Name == "Rare Disease" && */latestPAF.CME_Type == "Non-Standard Meeting")
            //                {
            //                    if (latestPAF.Disease_Area == "Hemophilia Medical")
            //                    {
            //                       var users1 = new List<UserModel>();
            //                        users = (from u in context.Users
            //                                 join ptu in context.Profiles_To_User on u.ID equals ptu.User_Id
            //                                 where (u.Initials == "MUCJ" || u.Initials == "PUOB" || u.Initials == "RNJM") && ptu.Status == true
            //                                 select new UserModel
            //                                 {
            //                                     ID = u.ID,
            //                                     Initial = u.Initials,
            //                                     EmailId = u.Email
            //                                 }).ToList();
            //                        if (users1 != null)
            //                        {
            //                            foreach (var us in users)
            //                            {
            //                                PAFClosedEMail(latestPAF.ID, us.EmailId, user.Initials);
            //                            }
            //                        }
            //                    }
            //                    if (model.DiseaseArea_Type == "Growth Hormone Medical")
            //                    {
            //                        var users2 = new List<UserModel>();
            //                        users = (from u in context.Users
            //                                 join ptu in context.Profiles_To_User on u.ID equals ptu.User_Id
            //                                 where (u.Initials == "BBSG" || u.Initials == "RNJM") && ptu.Status == true
            //                                 select new UserModel
            //                                 {
            //                                     ID = u.ID,
            //                                     Initial = u.Initials,
            //                                     EmailId = u.Email
            //                                 }).ToList();
            //                        if (users2 != null)
            //                        {
            //                            foreach (var us in users)
            //                            {
            //                                PAFClosedEMail(latestPAF.ID, us.EmailId, user.Initials);
            //                            }
            //                        }
            //                    }

            //                }
            //            }
            //            #endregion
            //        }
            //        context.Dispose();
            //    }
            //}
            //catch (Exception ex)
            //{
            //    throw ex;
            //}
        }

        public void ClosePAF(int id, CMEDetails model)
        {
            //string action = "Paf Close";
            var closedPafAttach = new PAF_Attachment();
            int closedLatestHCP = 0;
            var closedHCPDetails = new PAF_HCP();
            try
            {
                using (NovoNordiskEntities context = new NovoNordiskEntities())
                {
                    DataAccessLayer.PAF latestPAF = context.PAFs.Where(p => p.ID == id).OrderByDescending(o => o.ID).FirstOrDefault();
                    if (latestPAF != null)
                    {
                        PAF_Request_Type rt = context.PAF_Request_Type.Where(s => s.Name == "Payment requested").FirstOrDefault();
                        #region PAF transaction
                        context.PAF_Transaction.Add(new PAF_Transaction { PAF_Id = latestPAF.ID, Request_Type_Id = rt.ID, CreatedBy = model.IssuedBy, CreatedDate = DateTime.UtcNow });
                        context.SaveChanges();
                        //Helper.Log_ log = new Helper.Log_();
                        //try
                        //{
                        //    context.PAF_Transaction.Add(new PAF_Transaction { PAF_Id = latestPAF.ID, Request_Type_Id = rt.ID, CreatedBy = model.IssuedBy, CreatedDate = DateTime.UtcNow });
                        //    context.SaveChanges();
                        //    StringBuilder s = new StringBuilder();
                        //    s.Append("PAF_Id:");
                        //    s.Append(latestPAF.ID);
                        //    s.Append(" Request_Type_Id:");
                        //    s.Append(rt.ID);
                        //    s.Append(" Closed By:");
                        //    string Initials = context.Users.Where(u => u.ID == model.IssuedBy).FirstOrDefault().Initials;
                        //    s.Append(Initials);
                        //    s.Append(" CreatedDate:");
                        //    s.Append(DateTime.UtcNow);
                        //    log.WriteIntoFile(s.ToString(), latestPAF.ID.ToString(), "Close PAF Transaction");
                        //}
                        //catch (Exception ex)
                        //{
                        //    log.WriteIntoFile(ex.ToString(), latestPAF.ID.ToString(), "Close PAF Transaction error");
                        //}
                        #endregion

                        #region PAF
                        latestPAF.Request_Type_Id = rt.ID;
                        latestPAF.ModifiedDate = DateTime.UtcNow;
                        latestPAF.Actual_Attendees = model.Actual_Attendees;
                        latestPAF.Final_CME_Expenses = model.Final_CME_Expenses;
                        context.SaveChanges();
                        #endregion

                        #region Save Comments
                        if (!string.IsNullOrEmpty(model.Comment))
                        {
                            context.PAF_Command.Add(new PAF_Command { PAF_Id = latestPAF.ID, CreatedBy = model.IssuedBy, CreatedDate = DateTime.UtcNow, Comment = model.Comment });
                            context.SaveChanges();
                        }
                        #endregion

                        #region Save PAF Attachment
                        foreach (var obj in model.ImagePath.Where(s => s.IsNew))
                        {
                            context.PAF_Attachment.Add(new PAF_Attachment
                            {
                                PAF_Id = id,
                                File_Name = obj.DocName.Replace(" ", String.Empty),
                                File_Path = obj.DocPath.Replace(" ", String.Empty),
                                File_Type = obj.Extension,
                                Saved_File_Name = obj.SavedDocName,
                                Type = obj.Type,
                                CreatedDate = DateTime.UtcNow,
                                ModifiedDate = DateTime.UtcNow
                            });
                            context.SaveChanges();

                            closedPafAttach.PAF_Id = id;
                            closedPafAttach.File_Name = obj.DocName.Replace(" ", String.Empty);
                            closedPafAttach.File_Path = obj.DocPath.Replace(" ", String.Empty);
                            closedPafAttach.File_Type = obj.Extension;
                            closedPafAttach.Saved_File_Name = obj.SavedDocName;
                            closedPafAttach.Type = obj.Type;
                            closedPafAttach.CreatedDate = DateTime.UtcNow;
                            closedPafAttach.ModifiedDate = DateTime.UtcNow;

                        }
                        #endregion

                        #region Inserting HCP List
                        if (model.HCPs != null)
                        {
                            #region Remove/Update existing list for specific PAF
                            List<PAF_HCP> hcpList = context.PAF_HCP.Where(s => s.PAF_Id == id).ToList<PAF_HCP>();
                            foreach (PAF_HCP obj in hcpList)
                            {
                                DataAccessLayer.HCP latestHCP = context.HCPs.Where(p => p.ID == obj.HCP_Id).OrderByDescending(o => o.ID).FirstOrDefault();
                                if (latestHCP != null)
                                {
                                    #region No_Of_Lect_Given calculations 
                                    //if (latestHCP.No_Of_Lect_Given <= latestHCP.No_Of_Lect)
                                    //{
                                    //    latestHCP.No_Of_Lect_Given -= 1;
                                    //    context.SaveChanges();
                                    //}
                                    #endregion

                                    #region Update HCP's
                                    foreach (var mHCPs in model.HCPs)
                                    {
                                        if (obj.Is_Honorarium == false)
                                        {
                                            mHCPs.Invoice_Date = DateTime.UtcNow.ToString("dd/MM/yyyy");
                                            mHCPs.Invoice_No = " ";
                                        }

                                        if (mHCPs.Id == obj.ID)
                                        {
                                            obj.PAF_Id = id;
                                            obj.Type_Of_HCP_Id = mHCPs.Type;
                                            obj.HCP_Id = mHCPs.HCPId;
                                            obj.HCP_First_Name = mHCPs.First_Name;
                                            obj.HCP_Last_Name = mHCPs.Last_Name;
                                            obj.TMF_No = mHCPs.TMF_No;
                                            obj.HCP_Payable = mHCPs.Cost;
                                            obj.Invitation_Email = mHCPs.Invitation;
                                            obj.Is_Honorarium = mHCPs.HCPId == null ? false : true;
                                            obj.CreatedDate = DateTime.UtcNow;
                                            obj.ModifiedDate = DateTime.UtcNow;
                                            obj.HCP_Topic = mHCPs.HCPtopic;
                                            obj.Invoice_No = mHCPs.Invoice_No;
                                            obj.Invoice_Date = Convert.ToDateTime(mHCPs.Invoice_Date, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                                            obj.Net_Amount = mHCPs.Net_Amount;
                                            obj.Tax_Amount = mHCPs.Tax_Amount;
                                            obj.Total_Amount = mHCPs.Total_Amount;
                                        }

                                        closedHCPDetails.PAF_Id = id;
                                        closedHCPDetails.HCP_First_Name = mHCPs.First_Name;
                                        closedHCPDetails.HCP_Last_Name = mHCPs.Last_Name;
                                        closedHCPDetails.Net_Amount = mHCPs.Net_Amount;
                                        closedHCPDetails.Tax_Amount = mHCPs.Tax_Amount;
                                        closedHCPDetails.Total_Amount = mHCPs.Total_Amount;
                                        closedHCPDetails.HCP_Payable = mHCPs.Cost;
                                    }
                                    #endregion
                                }
                                context.SaveChanges();
                            }
                            #endregion


                            //foreach (var obj in model.HCPs)
                            //{                               
                            //    DataAccessLayer.HCP latestHCP = context.HCPs.Where(p => p.ID == obj.HCPId).OrderByDescending(o => o.ID).FirstOrDefault();
                            //    if (latestHCP != null)
                            //    {
                            //        if (latestHCP.No_Of_Lect_Given < latestHCP.No_Of_Lect)
                            //        {
                            //            latestHCP.No_Of_Lect_Given += 1;
                            //            context.SaveChanges();
                            //        }
                            //    }
                            //    closedLatestHCP = latestHCP.No_Of_Lect_Given;
                            //}
                        }

                        #endregion

                        #region Email
                        PAF_Request_Type prt = context.PAF_Request_Type.Where(s => s.ID == latestPAF.Request_Type_Id).FirstOrDefault();
                        User user = context.Users.Where(s => s.ID == model.IssuedBy).FirstOrDefault();
                        if (prt.Name == "Payment requested")
                        {
                            var users = new List<UserModel>();
                            users = (from u in context.Users
                                     join ptu in context.Profiles_To_User on u.ID equals ptu.User_Id
                                     join p in context.Profiles on ptu.Profile_Id equals p.ID
                                     join cc in context.Cost_Centre on u.CC_Id equals cc.ID
                                     where p.Name == "CT" && cc.Code == latestPAF.Cost_Centre
                                     select new UserModel
                                     {
                                         ID = u.ID,
                                         Initial = u.Initials,
                                         Role = p.Name,
                                         EmailId = u.Email
                                     }).ToList();
                            if (users != null)
                            {
                                foreach (var us in users)
                                {
                                    PAFClosedEMail(latestPAF.ID, user.Initials, us.EmailId);
                                }
                            }
                            // }
                        }
                        #endregion

                        //pafDetail.Append(Environment.NewLine + "PAF: " + latestPAF.ID + " " + latestPAF?.PAF_Request_Type?.Name + " - By " + model.Issuer_Role + " " + latestPAF.ModifiedDate + " - Comments: " + model.Comment + Environment.NewLine);

                        //pafDetail?.AppendLine("Save PAF Attachment:" + " - Filename: " + closedPafAttach.File_Name + " - FilePath: " + closedPafAttach.File_Path);

                        //pafDetail?.AppendLine("Latest HCP List: " + " - Number of Lectures completed: " + closedLatestHCP);

                        //var serializedHCPList = JsonConvert.SerializeObject(closedHCPDetails, Formatting.Indented, new JsonSerializerSettings
                        //{
                        //    NullValueHandling = NullValueHandling.Ignore
                        //});
                        //pafDetail.Append(serializedHCPList);
                        //pafLog.FetchLogs(pafDetail, action, latestPAF.ID);
                    }
                    context.Dispose();
                }
            }
            catch (Exception ex)
            {
                //pafLog.WriteIntoFile(pafDetail + "Error: " + ex.ToString(), action, id.ToString());
                throw ex;
            }
        }

        public void RemovePAF(int id, CMEDetails model)
        {
            //string action = "Paf Remove: Cancel Event";
            try
            {
                using (NovoNordiskEntities context = new NovoNordiskEntities())
                {
                    DataAccessLayer.PAF latestPAF = context.PAFs.Where(p => p.ID == id).OrderByDescending(o => o.ID).FirstOrDefault();
                    PAF_Transaction pt = context.PAF_Transaction.Where(p => p.PAF_Id == id).OrderByDescending(o => o.ID).FirstOrDefault();
                    if (pt.CreatedBy != model.IssuedBy)
                    {
                        PAF_Request_Type rt = context.PAF_Request_Type.Where(s => s.Name == "Delete").FirstOrDefault();
                        if (model.Issuer_Role == "SR" || model.Issuer_Role == "KAM")
                        {
                            rt = context.PAF_Request_Type.Where(s => s.Name == "Send to cancel approve").FirstOrDefault();
                        }
                        if (latestPAF != null)
                        {
                            latestPAF.Request_Type_Id = rt.ID;
                            context.SaveChanges();
                        }
                        if (!string.IsNullOrEmpty(model.Comment))
                        {
                            context.PAF_Command.Add(new PAF_Command { PAF_Id = latestPAF.ID, CreatedBy = model.IssuedBy, CreatedDate = DateTime.UtcNow, Comment = model.Comment });
                            context.SaveChanges();
                        }

                        #region HCP count
                        var hcps = new List<HCPViewModel>();
                        hcps = (from hcp in context.HCPs
                                join phcp in context.PAF_HCP on hcp.ID equals phcp.HCP_Id
                                join p in context.PAFs on phcp.PAF_Id equals p.ID
                                where p.ID == latestPAF.ID
                                select new HCPViewModel
                                {
                                    ID = hcp.ID,
                                    //PAN = hcp.PAN_No,
                                    //NoLecture = hcp.No_Of_Lect,
                                    //NoLectureGiven = hcp.No_Of_Lect_Given
                                }).ToList();

                        //if (hcps != null)
                        //{
                        //    foreach (var hcp in hcps)
                        //    {

                        //        DataAccessLayer.HCP latestHCP = context.HCPs.Where(p => p.ID == hcp.ID).OrderByDescending(o => o.ID).FirstOrDefault();
                        //        if (latestHCP.No_Of_Lect_Given <= latestHCP.No_Of_Lect && latestHCP.No_Of_Lect_Given >= 1)
                        //        {

                        //            latestHCP.No_Of_Lect_Given -= 1;
                        //            context.SaveChanges();
                        //        }
                        //    }
                        //}
                        #endregion

                        #region PAF Transaction
                        context.PAF_Transaction.Add(new PAF_Transaction { PAF_Id = latestPAF.ID, Request_Type_Id = rt.ID, CreatedBy = model.IssuedBy, CreatedDate = DateTime.UtcNow });
                        context.SaveChanges();
                        //Helper.Log_ log = new Helper.Log_();

                        //try
                        //{
                        //    context.PAF_Transaction.Add(new PAF_Transaction { PAF_Id = latestPAF.ID, Request_Type_Id = rt.ID, CreatedBy = model.IssuedBy, CreatedDate = DateTime.UtcNow });
                        //    context.SaveChanges();
                        //    StringBuilder s = new StringBuilder();
                        //    s.Append("PAF_Id:");
                        //    s.Append(latestPAF.ID);
                        //    s.Append(" Request_Type_Id:");
                        //    s.Append(rt.ID);
                        //    s.Append(" Remove/Delete By:");
                        //    string Initials = context.Users.Where(u => u.ID == model.IssuedBy).FirstOrDefault().Initials;
                        //    s.Append(Initials);
                        //    s.Append(" CreatedDate:");
                        //    s.Append(DateTime.UtcNow);

                        //    log.WriteIntoFile(s.ToString(), latestPAF.ID.ToString(), "Remove/Delete PAF Transaction");
                        //}
                        //catch (Exception ex)
                        //{
                        //    log.WriteIntoFile(ex.ToString(), latestPAF.ID.ToString(), "Remove/Delete PAF Transaction error");
                        //}
                        #endregion

                        //pafDetail.Append(Environment.NewLine + "PAF: " + latestPAF.ID + " " + latestPAF?.PAF_Request_Type?.Name + " - By: " + model.Issuer_Role + " - Comments: " + model.Comment + Environment.NewLine);
                        //pafLog.FetchLogs(pafDetail, action, latestPAF.ID);

                    }
                    context.Dispose();
                }
            }
            catch (Exception ex)
            {
                throw ex;
                //   pafLog.WriteIntoFile(pafDetail + "Error: " + ex.ToString(), action, id.ToString());
            }
        }

        public void PAFApproveEMail(int id, string Initials, string managerEmail)
        {
            try
            {
                //Helper.EmailService emailService = new Helper.EmailService();
                //string subject = "Please approve Form Id [" + id + "]";
                //string body = "<style>a:link {color: black;}</style><h1 style=\"color:black;font-family:verdana;size:20;\">Activity Management System (PAF – Please approve)</h1><p style=\"color:black;font-family:verdana;size:11;\">Dear User,</br></br>Form ID [" + id + "] submitted by [" + Initials + "] is pending for your approval.</br></br>Please click below link for more details:</ br></ br></p><a style=\"font-family:verdana; size:11;\">" + System.Web.Configuration.WebConfigurationManager.AppSettings["WebUrl"] + "Home" /*id*/ + "</a><br/><br/><br/><p style=\"color:black;font-family:verdana;size:11;\">Best regards,</br>AMS Administrator</br>Novo Nordisk India</p>";
                //string emailId = Convert.ToString(ConfigurationManager.AppSettings["TestingEmail"]);
                //string outLookEmail = Convert.ToString(ConfigurationManager.AppSettings["OLFromEmail"]);
                //if (string.IsNullOrEmpty(emailId)) { emailId = managerEmail; }
                //if (!string.IsNullOrEmpty(emailId))
                //{
                //    if (!string.IsNullOrEmpty(outLookEmail))
                //    { emailService.SendOutlookMail(subject, body, true, emailId, id, null); }
                //    else
                //    { emailService.SendGridMail(subject, body, true, emailId, id, null); }
                //}
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void PAFLastApproveEMail(int id, string UserEmailId)
        {
            try
            {
                //Helper.EmailService emailService = new Helper.EmailService();
                //string subject = "Your PAF Form Id [" + id + "] is approved";
                //string body = "<style>a:link {color: black;}</style><h1 style=\"color:black;font-family:verdana;size:20;\">Activity Management System (PAF – Approved )</h1><p style=\"color:black;font-family:verdana;size:11;\">Dear User,</br></br>Form ID [" + id + "] is approved by Your Managers.</br></br>Please click below link for more details:</ br></ br></p><a style=\"font-family:verdana; size:11;\">" + System.Web.Configuration.WebConfigurationManager.AppSettings["WebUrl"] + "Home" /*+ id*/ + "</a><br/><br/><br/><p style=\"color:black;font-family:verdana;size:11;\">Best regards,</br>AMS Administrator</br>Novo Nordisk India</p>";
                //string emailId = Convert.ToString(ConfigurationManager.AppSettings["TestingEmail"]);
                //string outLookEmail = Convert.ToString(ConfigurationManager.AppSettings["OLFromEmail"]);
                //if (string.IsNullOrEmpty(emailId)) { emailId = UserEmailId; }
                //if (!string.IsNullOrEmpty(emailId))
                //{
                //    if (!string.IsNullOrEmpty(outLookEmail))
                //    { emailService.SendOutlookMail(subject, body, true, emailId, id, null); }
                //    else
                //    { emailService.SendGridMail(subject, body, true, emailId, id, null); }
                //}
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void PAFRejectedEMail(int id, string UserEmailId)
        {
            try
            {
                //Helper.EmailService emailService = new Helper.EmailService();
                //string subject = "Form Id [" + id + "]" + " is Rejected";
                //string body = "<style>a:link {color: black; font-family:verdana;}</style><h1 style=\"color:black;font-family:verdana;size:20;\">Activity Management System (PAF – Rejected)</h1><p style=\"color:black;font-family:verdana;size:11;\">Dear User,</br></br>Form ID [" + id + "] is rejected by your manager.</br></br>Please click below link for more details:</ br></ br></p><a style=\"font-family:verdana;font-size:11px;\">" + System.Web.Configuration.WebConfigurationManager.AppSettings["WebUrl"] + "Home" /*+ id*/ + "</a><br/><br/><br/><p style=\"color:black;font-family:verdana;size:11;\">Best regards,</br>AMS Administrator</br>Novo Nordisk India</p>";

                //string emailId = Convert.ToString(ConfigurationManager.AppSettings["TestingEmail"]);
                //string outLookEmail = Convert.ToString(ConfigurationManager.AppSettings["OLFromEmail"]);
                //if (string.IsNullOrEmpty(emailId)) { emailId = UserEmailId; }
                //if (!string.IsNullOrEmpty(emailId))
                //{
                //    if (!string.IsNullOrEmpty(outLookEmail))
                //    { emailService.SendOutlookMail(subject, body, true, emailId, id, null); }
                //    else
                //    { emailService.SendGridMail(subject, body, true, emailId, id, null); }
                //}
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void PAFSendClosedEMail(int id, string Initials, string managerEmail)
        {
            try
            {
                //Helper.EmailService emailService = new Helper.EmailService();
                //string subject = "Please Close Form Id [" + id + "]";
                //string body = "<style>a:link {color: black;}</style><h1 style=\"color:black;font-family:verdana;size:20;\">Activity Management System (PAF – Please Close)</h1><p style=\"color:black;font-family:verdana;size:11;\">Dear User,</br></br>Form ID [" + id + "] submitted by [" + Initials + "] is pending for Close.</br></br>Please click below link for more details:</ br></ br></p><a style=\"font-family:verdana; size:11;\">" + System.Web.Configuration.WebConfigurationManager.AppSettings["WebUrl"] + "Home" /*+ id*/ + "</a><br/><br/><br/><p style=\"color:black;font-family:verdana;size:11;\">Best regards,</br>AMS Administrator</br>Novo Nordisk India</p>";
                //string emailId = Convert.ToString(ConfigurationManager.AppSettings["TestingEmail"]);
                //string outLookEmail = Convert.ToString(ConfigurationManager.AppSettings["OLFromEmail"]);
                //if (string.IsNullOrEmpty(emailId)) { emailId = managerEmail; }
                //if (!string.IsNullOrEmpty(emailId))
                //{
                //    if (!string.IsNullOrEmpty(outLookEmail))
                //    { emailService.SendOutlookMail(subject, body, true, emailId, id, null); }
                //    else
                //    { emailService.SendGridMail(subject, body, true, emailId, id, null); }
                //}
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void PAFClosedEMail(int id, string Initials, string UserEmailId)
        {
            try
            {
                //Helper.EmailService emailService = new Helper.EmailService();
                //string subject = "PAF Form Id [" + id + "] is Closed";
                //string body = "<style>a:link {color: black;}</style><h1 style=\"color:black;font-family:verdana;size:20;\">Activity Management System (PAF – Closed )</h1><p style=\"color:black;font-family:verdana;size:11;\">Dear User,</br></br>Form ID [" + id + "] submitted by [" + Initials + "] is closed by the User.</br></br>Please click below link for more details:</ br></ br></p><a style=\"font-family:verdana; size:11;\">" + System.Web.Configuration.WebConfigurationManager.AppSettings["WebUrl"] + "Home" /*+ id*/ + "</a><br/><br/><br/><p style=\"color:black;font-family:verdana;size:11;\">Best regards,</br>AMS Administrator</br>Novo Nordisk India</p>";
                //string emailId = Convert.ToString(ConfigurationManager.AppSettings["TestingEmail"]);
                //string outLookEmail = Convert.ToString(ConfigurationManager.AppSettings["OLFromEmail"]);
                //if (string.IsNullOrEmpty(emailId)) { emailId = UserEmailId; }
                //if (!string.IsNullOrEmpty(emailId))
                //{
                //    if (!string.IsNullOrEmpty(outLookEmail))
                //    { emailService.SendOutlookMail(subject, body, true, emailId, id, null); }
                //    else
                //    { emailService.SendGridMail(subject, body, true, emailId, id, null); }
                //}
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
