using DataAccessLayer;
using DataAccessLayer.DAL.CommonRespository;
using DataAccessLayer.DAL.Model;
using DataAccessLayer.DAL.PAFRepository;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace CME.Controllers
{
    public class CMEController : Controller
    {
        private readonly IPAFRepository _pAFRepository;
        private readonly ICommonRespository _commonRepository;

        public CMEController()
        {
            this._pAFRepository = new PAFRepository(new NovoNordiskEntities());
            this._commonRepository = new CommonRespository(new NovoNordiskEntities());
        }

        public FileResult DownLoadFileLM(string fileName, string filePath)
        {
            try
            {
                if (System.IO.File.Exists(filePath))
                {
                    return File(filePath, "application/force-download", Path.GetFileName(fileName));
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        [ActionName("Insert")]
        [HttpPost]
        public void Insert(CMEModel model)
        {
            //string action = "Paf Create";
            try
            {
                using (NovoNordiskEntities context = new NovoNordiskEntities())
                {
                    User user = context.Users.Where(s => s.ID == model.IssuedBy).FirstOrDefault();
                    string managerEmail = string.Empty;
                    if (model.PAFType == "Send")
                    {
                        var managerUser = (from u in context.Users
                                           join ptu in context.Profiles_To_User on u.ID equals ptu.User_Id
                                           join p in context.Profiles on ptu.Profile_Id equals p.ID
                                           where u.Initials.ToLower() == model.ManagerInitial.ToLower() && ptu.Status
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
                            if (managerUser.Role.ToUpper() == "RBD")
                            {
                                model.PAFType = "Send to RBD";
                            }
                            else
                            {
                                model.PAFType = "Send";
                            }
                            managerEmail = managerUser.EmailId;
                        }

                        if (model.IssuerZone == "Rare Disease" && (model.IssuerRole == "SM" || model.IssuerRole == "CT"))
                        {
                            model.PAFType = "Send to RBD";
                        }
                    }

                    var pafRequest = context.PAF_Request_Type.Where(s => s.Name == model.PAFType).FirstOrDefault();

                    var newPaf = new PAF()
                    {
                        Issued_By_Id = model.IssuedBy,
                        Request_Type_Id = pafRequest.ID,
                        CME_Type = model.CME_Type,
                        Disease_Area = model.DiseaseArea_Type,
                        Issued_Date = model.IssuedDate,
                        Therapy_Area_Id = model.TherapyAreaId,
                        Brand_Id = model.BrandId,
                        Event_Id = model.EventId,
                        Lecture_Id = model.LectureId,
                        Activity_Name = model.ActivityName,
                        Activity_Start_Date = model.ActivityStartDate,
                        Activity_End_Date = model.ActivityEndDate,
                        No_Of_Days = model.NumberofDays,
                        Actual_Start_Date = model.ActivityEndDate,
                        Actual_End_Date = model.ActivityEndDate,
                        Change_Start_Date = model.ActivityStartDate,
                        Change_End_Date = model.ActivityEndDate,
                        Proposed_Attendee_Count = model.Proposednumberofattendee,
                        Min_Guarantee_Count = model.Minimumnumberofgaurantee,
                        Approximate_Count = model.Approximatecost,
                        Room_Rate = model.Roomrate,
                        Banquet_Room_Rate = model.Banquetroomrate,
                        Cost_Per_Pax = model.CostPerPAX,
                        City = model.City,
                        Hotel = model.Venu_Hotel,
                        Hotel_Star = model.Hotel_Star ? 5 : 4,
                        Hotel_Document_Name = null,
                        Hotel_Document_Path = null,
                        // IsDateDeviation = (model.ChangeStartDate.HasValue && model.ChangeEndDate.HasValue),
                        PAF_Status_Id = context.PAF_Status.Where(s => s.Name == "Active").FirstOrDefault().ID,
                        CreatedDate = DateTime.UtcNow,
                        ModifiedDate = DateTime.UtcNow,
                        ModifiedBy = model.IssuedBy,
                        Approver = model.Approver,
                        Hospital_Meeting = model.HospitalMeeting,
                        IsVirtual = model.IsVirtual,
                        //IsDelete = model.IsDelete,
                        Cost_Centre = model.Cost_Center,
                        Is_BTC = model.IsBTC,
                        //Is_Assessment=model.Is_Assessment,
                        Meetingtype_Id = model.MeetingTypeId,
                        IsDateDeviation = model.IsDateDeviation,
                        Policy_Violation = model.Policy_Violation
                    };

                    _pAFRepository.InsertPAF(newPaf);

                    decimal totalPAFCost = 0;
                    var latestPAF = context.PAFs.Where(p => p.Issued_By_Id == model.IssuedBy).OrderByDescending(o => o.ID).FirstOrDefault();
                    if (user.Therapy_Area.Name == "Rare Disease")
                    {
                        totalPAFCost = totalPAFCost + latestPAF.Room_Rate + latestPAF.Banquet_Room_Rate + (latestPAF.Cost_Per_Pax * model.Proposednumberofattendee);
                    }
                    else
                    {
                        //totalPAFCost = totalPAFCost + latestPAF.Approximate_Count + latestPAF.Room_Rate + latestPAF.Banquet_Room_Rate + latestPAF.Cost_Per_Pax;
                        totalPAFCost = totalPAFCost + latestPAF.Room_Rate + latestPAF.Banquet_Room_Rate + (latestPAF.Cost_Per_Pax * model.Proposednumberofattendee);
                    }

                    newPaf = new PAF
                    {
                        ID = latestPAF.ID,
                        Issued_By_Id = model.IssuedBy,
                        Request_Type_Id = pafRequest.ID,
                        CME_Type = model.CME_Type,
                        Disease_Area = model.DiseaseArea_Type,
                        Issued_Date = model.IssuedDate,
                        Therapy_Area_Id = model.TherapyAreaId,
                        Brand_Id = model.BrandId,
                        Event_Id = model.EventId,
                        Lecture_Id = model.LectureId,
                        Activity_Name = model.ActivityName,
                        Activity_Start_Date = model.ActivityStartDate,
                        Activity_End_Date = model.ActivityEndDate,
                        No_Of_Days = model.NumberofDays,
                        Actual_Start_Date = model.ActivityEndDate,
                        Actual_End_Date = model.ActivityEndDate,
                        Change_Start_Date = model.ActivityStartDate,
                        Change_End_Date = model.ActivityEndDate,
                        Cost_Per_Pax = model.CostPerPAX,
                        City = model.City,
                        Hotel = model.Venu_Hotel,
                        Hotel_Star = model.Hotel_Star ? 5 : 4,
                        PAF_Status_Id = context.PAF_Status.Where(s => s.Name == "Active").FirstOrDefault().ID,
                        CreatedDate = DateTime.UtcNow,
                        ModifiedDate = DateTime.UtcNow,
                        ModifiedBy = model.IssuedBy,
                        Approver = model.Approver,
                        Hospital_Meeting = model.HospitalMeeting,
                        IsVirtual = model.IsVirtual,
                        Cost_Centre = model.Cost_Center,
                        Is_BTC = model.IsBTC,
                        IsDateDeviation = model.IsDateDeviation,
                        Policy_Violation = model.Policy_Violation
                    };

                    var serializedLatestPaf = JsonConvert.SerializeObject(newPaf, Formatting.Indented, new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    });
                    //pafDetail.Append(Environment.NewLine + Environment.NewLine + serializedLatestPaf);
                    //pafDetail.Append(Environment.NewLine + Environment.NewLine + "Inserting PAF Detail started: " + newPaf.CreatedDate + Environment.NewLine);

                    #region Inserting Miscellaneous                    

                    if (model.Miscellaneous != null)
                    {
                        var newPafMisc = new PAF_Miscellaneous();
                        //pafDetail.Append(Environment.NewLine + Environment.NewLine + "Inserting Paf Miscellaneous started: " + Environment.NewLine);

                        foreach (var obj in model.Miscellaneous)
                        {
                            totalPAFCost += obj.Cost;
                            string MiscellaneousName = (context.Miscellaneous_Type.Where(s => s.Name == obj.Name).OrderByDescending(o => o.Name).FirstOrDefault() != null) ? context.Miscellaneous_Type.Where(s => s.Name == obj.Name).OrderByDescending(o => o.Name).FirstOrDefault().Name : "";
                            if (MiscellaneousName != obj.Name)
                            {
                                var newMiscellaneous = new Miscellaneous_Type()
                                {
                                    Name = obj.Name,
                                };
                                _commonRepository.InsertMiscellaneousType(newMiscellaneous);
                            }

                            var newPAFMiscellaneous = new PAF_Miscellaneous
                            {
                                PAF_Id = latestPAF.ID,
                                Type_Id = context.Miscellaneous_Type.Where(s => s.Name == obj.Name).OrderByDescending(o => o.ID).FirstOrDefault().ID,
                                Price = obj.Cost,
                                CreatedBy = model.IssuedBy,
                                CreatedDate = DateTime.UtcNow
                            };

                            _commonRepository.InsertPAFMiscellaneous(newPAFMiscellaneous);

                            newPafMisc.PAF_Id = latestPAF.ID;
                            newPafMisc.Type_Id = context.Miscellaneous_Type.Where(s => s.Name == obj.Name).OrderByDescending(o => o.ID).FirstOrDefault().ID;
                            newPafMisc.Price = obj.Cost;
                            newPafMisc.CreatedBy = model.IssuedBy;
                            newPafMisc.CreatedDate = DateTime.UtcNow;
                        };

                        var serializedPafCost = JsonConvert.SerializeObject(newPafMisc, Formatting.Indented, new JsonSerializerSettings
                        {
                            NullValueHandling = NullValueHandling.Ignore
                        });
                        //pafDetail.Append(serializedPafCost);
                        //pafDetail.Append(Environment.NewLine + Environment.NewLine + "Inserting Paf Miscellaneous is complete " + newPafMisc.CreatedDate + Environment.NewLine);
                    }


                    #endregion

                    #region Inserting HCP List                    

                    if (model.HCPs != null)
                    {
                        var newPafHCP = new PAF_HCP();
                        //pafDetail.Append(Environment.NewLine + Environment.NewLine + "Inserting HCP List started: " + Environment.NewLine);

                        foreach (var obj in model.HCPs)
                        {
                            var newPafHcp = new PAF_HCP
                            {
                                PAF_Id = latestPAF.ID,
                                Type_Of_HCP_Id = obj.Type,
                                HCP_Id = obj.HCPId,
                                HCP_First_Name = obj.First_Name,
                                HCP_Last_Name = obj.Last_Name,
                                TMF_No = obj.TMF_No,
                                HCP_Payable = obj.Cost,
                                Invitation_Email = obj.Invitation,
                                Is_Honorarium = obj.HCPId != null,
                                CreatedDate = DateTime.UtcNow,
                                ModifiedDate = DateTime.UtcNow,
                                HCP_Topic = obj.HCPtopic
                            };
                            totalPAFCost += obj.Cost;

                            _pAFRepository.InsertPAFHCP(newPafHcp);

                            newPafHCP.PAF_Id = latestPAF.ID;
                            newPafHCP.Type_Of_HCP_Id = obj.Type;
                            newPafHCP.HCP_Id = obj.HCPId;
                            newPafHCP.HCP_First_Name = obj.First_Name;
                            newPafHCP.HCP_Last_Name = obj.Last_Name;
                            newPafHCP.Is_Honorarium = obj.HCPId != null;

                        }
                        var serializedPafHcp = JsonConvert.SerializeObject(newPafHCP, Formatting.Indented, new JsonSerializerSettings
                        {
                            NullValueHandling = NullValueHandling.Ignore
                        });
                        //pafDetail.Append(serializedPafHcp);
                        //pafDetail.Append(Environment.NewLine + Environment.NewLine + "Inserting HCP List is complete " + newPafHCP.CreatedDate + Environment.NewLine);
                    }

                    #endregion

                    #region Inserting Participants List

                    if (model.Participants != null)
                    {
                        var newPafParticipant = new PAF_Participants();

                        //pafDetail.Append(Environment.NewLine + Environment.NewLine + "Inserting Participants List started: " + Environment.NewLine);
                        foreach (PAFParticipants obj in model.Participants)
                        {
                            var newPafParticipants = new PAF_Participants
                            {
                                PAF_Id = latestPAF.ID,
                                HCP_Id = obj.ID,
                                Participants_Name = obj.Name,
                                PAN_No = obj.PAN,
                                Participants_Email_ID = obj.Email_Id,
                                Participants_Phone_Number = obj.Phone_Number,
                                Participants_Customer_ID = obj.Customer_ID
                            };


                            _pAFRepository.InsertPAFParticipants(newPafParticipants);

                            newPafParticipant.PAF_Id = latestPAF.ID;
                            newPafParticipant.Participants_Name = obj.Name;
                            newPafParticipant.Participants_Customer_ID = obj.Customer_ID;
                        }

                        var serializedPafPart = JsonConvert.SerializeObject(newPafParticipant, Formatting.Indented, new JsonSerializerSettings
                        {
                            NullValueHandling = NullValueHandling.Ignore
                        });
                        //pafDetail.Append(serializedPafPart);
                        //pafDetail.Append(Environment.NewLine + Environment.NewLine + "Inserting Participants List is complete " + DateTime.UtcNow + Environment.NewLine);
                    }

                    #endregion
                    latestPAF.TotalCost = Convert.ToInt32(totalPAFCost);
                    context.SaveChanges();


                    #region Save Comments
                    //pafDetail.Append(Environment.NewLine + Environment.NewLine + "Saving Paf Comments started." + Environment.NewLine);

                    context.PAF_Command.Add(new PAF_Command { PAF_Id = latestPAF.ID, CreatedBy = model.IssuedBy, CreatedDate = DateTime.UtcNow, Comment = model.Comment });
                    context.SaveChanges();

                    var newComments = new PAF_Command { PAF_Id = latestPAF.ID, CreatedBy = model.IssuedBy, CreatedDate = DateTime.UtcNow, Comment = model.Comment };

                    var deserializedComments = JsonConvert.SerializeObject(newComments, Formatting.Indented, new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    });
                    //pafDetail.Append(deserializedComments);
                    //pafDetail.Append(Environment.NewLine + Environment.NewLine + "Saving Paf Comments is complete " + newComments.CreatedDate + Environment.NewLine);

                    #endregion

                    #region PAF transaction
                    context.PAF_Transaction.Add(new PAF_Transaction { PAF_Id = latestPAF.ID, Request_Type_Id = pafRequest.ID, CreatedBy = model.IssuedBy, CreatedDate = DateTime.UtcNow });
                    context.SaveChanges();
                    //Helper.Log_ log = new Helper.Log_();
                    //try
                    //{
                    //    context.PAF_Transaction.Add(new PAF_Transaction { PAF_Id = latestPAF.ID, Request_Type_Id = pafRequest.ID, CreatedBy = model.IssuedBy, CreatedDate = DateTime.UtcNow });
                    //    context.SaveChanges();
                    //    StringBuilder s = new StringBuilder();
                    //    s.Append("PAF_Id:");
                    //    s.Append(latestPAF.ID);
                    //    s.Append(" Request_Type_Id:");
                    //    s.Append(pafRequest.ID);
                    //    s.Append(" CreatedBy:");
                    //    string Initials = context.Users.Where(u => u.ID == model.IssuedBy).FirstOrDefault().Initials;
                    //    s.Append(Initials);
                    //    s.Append(" CreatedDate:");
                    //    s.Append(DateTime.UtcNow);
                    //    //log.WriteIntoFile(s.ToString(), latestPAF.ID.ToString(), "PAF Insert Transaction");
                    //}
                    //catch (Exception ex)
                    //{
                    //    throw ex;
                    //    //log.WriteIntoFile(ex.ToString(), latestPAF.ID.ToString(), "PAF Insert Transaction error");
                    //}
                    #endregion

                    #region HCP count
                    //var hcps = new List<HCPViewModel>();
                    //hcps = (from hcp in context.HCPs
                    //        join phcp in context.PAF_HCP on hcp.ID equals phcp.HCP_Id
                    //        join p in context.PAFs on phcp.PAF_Id equals p.ID
                    //        where p.ID == latestPAF.ID
                    //        select new HCPViewModel
                    //        {
                    //            ID = hcp.ID,
                    //            //PAN = hcp.PAN_No,
                    //            //NoLecture = hcp.No_Of_Lect,
                    //            //NoLectureGiven = hcp.No_Of_Lect_Given
                    //        }).ToList();
                    ////if (hcps != null)
                    ////{
                    ////    foreach (var hcp in hcps)
                    ////    {
                    ////        DataAccessLayer.HCP latestHCP = context.HCPs.Where(p => p.ID == hcp.ID).OrderByDescending(o => o.ID).FirstOrDefault();
                    ////        if (latestHCP.No_Of_Lect_Given < latestHCP.No_Of_Lect)
                    ////        {
                    ////            latestHCP.No_Of_Lect_Given += 1;
                    ////            context.SaveChanges();
                    ////        }
                    ////    }
                    ////}
                    #endregion

                    #region Save PAF Attachment                 
                    if (model.ImagePath != null)
                    {
                        //pafDetail.Append(Environment.NewLine + Environment.NewLine + "Saving PAF Attachment started: " + Environment.NewLine);
                        foreach (var obj in model.ImagePath)
                        {
                            context.PAF_Attachment.Add(new PAF_Attachment
                            {
                                PAF_Id = latestPAF.ID,
                                File_Name = obj.DocName.Replace(" ", String.Empty),
                                File_Path = obj.DocPath.Replace(" ", String.Empty),
                                File_Type = obj.Extension,
                                Saved_File_Name = obj.SavedDocName,
                                Type = obj.Type,
                                CreatedDate = DateTime.UtcNow,
                                ModifiedDate = DateTime.UtcNow,
                                HCP_Name = obj.HCP_First_Name + obj.HCP_Last_Name,
                            });
                            context.SaveChanges();

                            var newAttachment = new PAF_Attachment
                            {
                                PAF_Id = latestPAF.ID,
                                File_Name = obj.DocName.Replace(" ", String.Empty),
                                File_Path = obj.DocPath.Replace(" ", String.Empty),
                                File_Type = obj.Extension,
                                Saved_File_Name = obj.SavedDocName,
                                Type = obj.Type,
                                CreatedDate = DateTime.UtcNow,
                                ModifiedDate = DateTime.UtcNow,
                                HCP_Name = obj.HCP_First_Name + obj.HCP_Last_Name,
                            };

                            var serializedImagePath = JsonConvert.SerializeObject(newAttachment, Formatting.Indented, new JsonSerializerSettings
                            {
                                NullValueHandling = NullValueHandling.Ignore
                            });
                            //pafDetail.Append(serializedImagePath);
                            //pafDetail.Append(Environment.NewLine + Environment.NewLine + "Saving PAF Attachment is complete " + newAttachment.CreatedDate + Environment.NewLine);
                        }
                    }
                    #endregion

                    #region Sending Email
                    if (model.PAFType.ToLower() == "send" || model.PAFType == "Send to RBD")
                    {
                        //if (model.ChangeStartDate != null && model.ChangeEndDate != null)
                        //{
                        //    PAFDDApproveEMail(latestPAF.ID, user.Initials, managerEmail);
                        //}
                        //else
                        //{
                        if (model.PAFType.ToLower() == "send" && model.IssuerRole == "SM" && model.CME_Type == "Standard Meeting" && user.Therapy_Area.Name == "Rare Disease")//&& model.IsDateDeviation == false
                        {
                            var RBD1user = (from u in context.Users
                                            join ptu in context.Profiles_To_User on u.ID equals ptu.User_Id
                                            join p in context.Profiles on ptu.Profile_Id equals p.ID
                                            where p.Name == "RBD1" || p.Name == "RBD" && u.Status == true
                                            orderby ptu.ID descending
                                            select new UserModel
                                            {
                                                ID = u.ID,
                                                Initial = u.Initials,
                                                Role = p.Name,
                                                EmailId = u.Email
                                            }).ToList();
                            if (RBD1user != null)
                            {
                                foreach (var us in RBD1user)
                                {
                                    //PAFApproveEMail(latestPAF.ID, user.Initials, us.EmailId);
                                }
                            }

                        }
                        else
                        {
                            //PAFApproveEMail(latestPAF.ID, user.Initials, managerEmail);
                        }
                        // }
                        if (user.Therapy_Area.Name == "GLP 1" || user.Therapy_Area.Name == "HO" || user.Therapy_Area.Name == "Institution Business")
                        {
                            //Cost_Centre cost_Centre = null;
                            //string cost_Centre_code = latestPAF.Cost_Centre;
                            //cost_Centre = context.Cost_Centre.Where(s => s.Code == cost_Centre_code).FirstOrDefault();
                            //int cost_Centre_id = cost_Centre.ID;

                            //List<Models.User> users_id = new List<Models.User>();
                            //users_id = (from ctc in context.CostCentre_To_CT
                            //            where ctc.CostCentre_Id == cost_Centre_id
                            //            select new Models.User
                            //            {
                            //                ID = (int)ctc.User_Id
                            //            }).ToList<Models.User>();

                            //List<Models.User> users = new List<Models.User>();
                            //for (int i = 0; i < users_id.Count(); i++)
                            //{
                            //    var id = users_id[i].ID;
                            //    var us = new Models.User();
                            //    us = (from u in context.Users
                            //          join ptu in context.Profiles_To_User on u.ID equals ptu.User_Id
                            //          //join p in context.Profiles on ptu.Profile_Id equals p.ID
                            //          where u.ID.Equals(id) && ptu.Status.Equals(true)
                            //          select new Models.User
                            //          {
                            //              ID = u.ID,
                            //              Initial = u.Initials,
                            //              //Role = p.Name,
                            //              EmailId = u.Email
                            //          }).SingleOrDefault();
                            //    users.Add(us);
                            //}

                            //List<Models.User> users = new List<Models.User>();
                            //users = (from u in context.Users
                            //         join ptu in context.Profiles_To_User on u.ID equals ptu.User_Id
                            //         //join p in context.Profiles on ptu.Profile_Id equals p.ID
                            //         where u.ID in users_id && ptu.Status == true
                            //         select new Models.User
                            //         {
                            //             ID = u.ID,
                            //             Initial = u.Initials,
                            //             Role = p.Name,
                            //             EmailId = u.Email
                            //         }).ToList<Models.User>();

                            //if (users != null)
                            //{
                            //    foreach (var us in users)
                            //    {
                            //        PAFSendEMail(latestPAF.ID, us.EmailId, user.Initials);
                            //    }
                            //}
                        }
                        //else
                        //{
                        Cost_Centre cost_Centre = null;
                        string cost_Centre_code = latestPAF.Cost_Centre;
                        cost_Centre = context.Cost_Centre.Where(s => s.Code == cost_Centre_code).FirstOrDefault();

                        //extra check to avoid updating User table/CostCentre_To_CT table as per story "New Cost Centre for an Event"
                        if (cost_Centre != null)
                        {
                            int cost_Centre_id = cost_Centre.ID;

                            var users_id = new List<UserModel>();
                            users_id = (from ctc in context.CostCentre_To_CT
                                        where ctc.CostCentre_Id == cost_Centre_id
                                        select new UserModel
                                        {
                                            ID = (int)ctc.User_Id
                                        }).ToList();

                            var users1 = new List<UserModel>();
                            for (int i = 0; i < users_id.Count(); i++)
                            {
                                var id = users_id[i].ID;
                                var us = new UserModel();
                                us = (from u in context.Users
                                      join ptu in context.Profiles_To_User on u.ID equals ptu.User_Id
                                      //join p in context.Profiles on ptu.Profile_Id equals p.ID
                                      where u.ID.Equals(id) && ptu.Status.Equals(true)
                                      select new UserModel
                                      {
                                          ID = u.ID,
                                          Initial = u.Initials,
                                          //Role = p.Name,
                                          EmailId = u.Email
                                      }).SingleOrDefault();
                                users1.Add(us);
                            }
                            //List<Models.User> users = new List<Models.User>();
                            //users = (from u in context.Users
                            //         join ptu in context.Profiles_To_User on u.ID equals ptu.User_Id
                            //         join p in context.Profiles on ptu.Profile_Id equals p.ID
                            //         where ptu.Status == true && (p.Name == "CT" || p.Name == "CTB") && u.Zone_Id == latestPAF.User.Zone_Id
                            //         select new Models.User
                            //         {
                            //             ID = u.ID,
                            //             Initial = u.Initials,
                            //             Role = p.Name,
                            //             EmailId = u.Email
                            //         }).ToList<Models.User>();
                            if (users1 != null)
                            {
                                foreach (var us in users1)
                                {
                                    //PAFSendEMail(latestPAF.ID, us.EmailId, user.Initials);
                                }
                            }
                            //}
                        }

                        if (user.Therapy_Area.Name == "Rare Disease" && model.CME_Type == "Non-Standard Meeting")
                        {
                            if (model.DiseaseArea_Type == "Hemophilia Medical")
                            {
                                var users = new List<UserModel>();
                                users = (from u in context.Users
                                         join ptu in context.Profiles_To_User on u.ID equals ptu.User_Id
                                         join p in context.Profiles on ptu.Profile_Id equals p.ID
                                         where (p.Name == "PMH" || p.Name == "RMA") && ptu.Status == true
                                         select new UserModel
                                         {
                                             ID = u.ID,
                                             Initial = u.Initials,
                                             EmailId = u.Email
                                         }).ToList();
                                if (users != null)
                                {
                                    foreach (var us in users)
                                    {
                                        //PAFSendEMail(latestPAF.ID, us.EmailId, user.Initials);
                                    }
                                }
                            }
                            if (model.DiseaseArea_Type == "Growth Hormone Medical")
                            {
                                var users = new List<UserModel>();
                                users = (from u in context.Users
                                         join ptu in context.Profiles_To_User on u.ID equals ptu.User_Id
                                         join p in context.Profiles on ptu.Profile_Id equals p.ID
                                         where (p.Name == "PMGH" || p.Name == "RMA") && ptu.Status == true
                                         select new UserModel
                                         {
                                             ID = u.ID,
                                             Initial = u.Initials,
                                             EmailId = u.Email
                                         }).ToList();
                                if (users != null)
                                {
                                    foreach (var us in users)
                                    {
                                        //PAFSendEMail(latestPAF.ID, us.EmailId, user.Initials);
                                    }
                                }
                            }

                        }
                    }


                    if (latestPAF.PAF_Request_Type.Name == "Approved Date Deviation")
                    {
                        //if (latestPAF.IsDateDeviation && !latestPAF.IsDeviationApproved)
                        //{
                        //    List<Models.User> users = new List<Models.User>();
                        //    users = (from u in context.Users
                        //             join ptu in context.Profiles_To_User on u.ID equals ptu.User_Id
                        //             join p in context.Profiles on ptu.Profile_Id equals p.ID
                        //             where (p.Name == "Admin" && ptu.Status == true && (u.Email != "zspy@novonordisk.com")) ||
                        //             ((p.Name == "CT" || p.Name == "CTB") && u.Zone_Id == latestPAF.User.Zone_Id)
                        //             select new Models.User
                        //             {
                        //                 ID = u.ID,
                        //                 Initial = u.Initials,
                        //                 Role = p.Name,
                        //                 EmailId = u.Email
                        //             }).ToList<Models.User>();
                        //    if (users != null)
                        //    {
                        //        foreach (var us in users)
                        //        {
                        //            DeviationPAFApproveEMail(latestPAF.ID, user.Initials, us.EmailId);
                        //        }
                        //    }
                        //}
                        if (user.Therapy_Area.Name == "Rare Disease" && model.CME_Type == "Non-Standard Meeting")
                        {
                            if (model.DiseaseArea_Type == "Hemophilia Medical")
                            {
                                var users = new List<UserModel>();
                                users = (from u in context.Users
                                         join ptu in context.Profiles_To_User on u.ID equals ptu.User_Id
                                         join p in context.Profiles on ptu.Profile_Id equals p.ID
                                         where (p.Name == "PMH" || p.Name == "RMA") && ptu.Status == true
                                         select new UserModel
                                         {
                                             ID = u.ID,
                                             Initial = u.Initials,
                                             EmailId = u.Email
                                         }).ToList();
                                if (users != null)
                                {
                                    foreach (var us in users)
                                    {
                                        //PAFSendEMail(latestPAF.ID, us.EmailId, user.Initials);
                                    }
                                }
                            }
                            if (model.DiseaseArea_Type == "Growth Hormone Medical")
                            {
                                var users = new List<UserModel>();
                                users = (from u in context.Users
                                         join ptu in context.Profiles_To_User on u.ID equals ptu.User_Id
                                         join p in context.Profiles on ptu.Profile_Id equals p.ID
                                         where (p.Name == "PMGH" || p.Name == "RMA") && ptu.Status == true
                                         select new UserModel
                                         {
                                             ID = u.ID,
                                             Initial = u.Initials,
                                             EmailId = u.Email
                                         }).ToList();
                                if (users != null)
                                {
                                    foreach (var us in users)
                                    {
                                        //PAFSendEMail(latestPAF.ID, us.EmailId, user.Initials);
                                    }
                                }
                            }

                        }
                    }
                    #endregion

                    //#region Sending HCP Remainder email

                    //var hcps1 = new List<HCPViewModel>();
                    //hcps1 = (from hcp in context.HCPs
                    //         join phcp in context.PAF_HCP on hcp.ID equals phcp.HCP_Id
                    //         join p in context.PAFs on phcp.PAF_Id equals p.ID
                    //         where p.ID == latestPAF.ID
                    //         select new HCPViewModel
                    //         {
                    //             ID = hcp.ID,
                    //             //PAN = hcp.PAN_No,
                    //             //NoLecture = hcp.No_Of_Lect,
                    //             //NoLectureGiven = hcp.No_Of_Lect_Given
                    //         }).ToList();
                    //if (hcps1 != null)
                    //{
                    //    foreach (var hcp in hcps1)
                    //    {
                    //        var hcplist = context.HCPs.Where(x => x.ID == hcp.ID).OrderByDescending(x => x.ID).FirstOrDefault();
                    //        var pending_lecture = hcplist.No_Of_Lect - hcplist.No_Of_Lect_Given;
                    //        if (pending_lecture < 4)
                    //        {
                    //            //var therapyArea = hcplist.TA_Id;
                    //            var admin = new List<UserModel>();
                    //            admin = (from u in context.Users
                    //                     join ptu in context.Profiles_To_User on u.ID equals ptu.User_Id
                    //                     join p in context.Profiles on ptu.Profile_Id equals p.ID
                    //                     where (u.Initials.ToLower() == "zspy" || u.Initials.ToLower() == "ahtc")
                    //                     select new UserModel
                    //                     {
                    //                         ID = u.ID,
                    //                         Initial = u.Initials,
                    //                         Role = p.Name,
                    //                         EmailId = u.Email
                    //                     }).ToList();
                    //            foreach (var ad in admin)
                    //            {
                    //                //HCPPendingLectureEmail(hcplist.ID, ad.EmailId, hcplist.Name, hcplist.PAN_No, hcplist.Address, pending_lecture);
                    //            }
                    //            var ct_all_zone = new List<UserModel>();
                    //            ct_all_zone = (from u in context.Users
                    //                           join ptu in context.Profiles_To_User on u.ID equals ptu.User_Id
                    //                           join p in context.Profiles on ptu.Profile_Id equals p.ID
                    //                           where (ptu.Profile_Id == 3)
                    //                           select new UserModel
                    //                           {
                    //                               ID = u.ID,
                    //                               Initial = u.Initials,
                    //                               Role = p.Name,
                    //                               EmailId = u.Email
                    //                           }).ToList();
                    //            //foreach (var ctaz in ct_all_zone)
                    //            //{
                    //            //    if (zone == 6 || zone == 8)
                    //            //    {
                    //            //        HCPPendingLectureEmail(hcplist.ID, ctaz.EmailId, hcplist.Name, hcplist.PAN_No, hcplist.Address, pending_lecture);
                    //            //    }
                    //            //}
                    //            //if (zone == 1 || zone == 2 || zone == 4 || zone == 5)
                    //            //{
                    //            //    var ct = new List<UserModel>();
                    //            //    ct = (from u in context.Users
                    //            //          join ptu in context.Profiles_To_User on u.ID equals ptu.User_Id
                    //            //          join p in context.Profiles on ptu.Profile_Id equals p.ID
                    //            //          where u.Zone_Id == zone && (ptu.Profile_Id == 3)
                    //            //          select new UserModel
                    //            //          {
                    //            //              ID = u.ID,
                    //            //              Initial = u.Initials,
                    //            //              Role = p.Name,
                    //            //              EmailId = u.Email
                    //            //          }).ToList();
                    //            //    foreach (var cts in ct)
                    //            //    {
                    //            //        HCPPendingLectureEmail(hcplist.ID, cts.EmailId, hcplist.Name, hcplist.PAN_No, hcplist.Address, pending_lecture);
                    //            //    }
                    //            //    var ctb = context.Users.Where(i => i.Initials.ToLower() == "xrby").OrderByDescending(z => z.ID).FirstOrDefault();
                    //            //    var ctb_email = ctb.Email;
                    //            //    HCPPendingLectureEmail(hcplist.ID, ctb_email, hcplist.Name, hcplist.PAN_No, hcplist.Address, pending_lecture);
                    //            //}
                    //            //if (zone == 6)
                    //            //{
                    //            //    var ctb = context.Users.Where(i => i.Initials.ToLower() == "xrby").OrderByDescending(z => z.ID).FirstOrDefault();
                    //            //    var ctb_email = ctb.Email;
                    //            //    HCPPendingLectureEmail(hcplist.ID, ctb_email, hcplist.Name, hcplist.PAN_No, hcplist.Address, pending_lecture);
                    //            //}
                    //        }
                    //    }
                    //}
                    //#endregion

                    //pafDetail.Append(Environment.NewLine + "PAF: " + latestPAF.ID + " " + latestPAF?.PAF_Request_Type?.Name + " - By: " + model.IssuerRole + Environment.NewLine);
                    //pafDetail.Append(Environment.NewLine + Environment.NewLine + "New Paf Creation is complete: " + Environment.NewLine);

                    //pafLog.FetchLogs(pafDetail, action, latestPAF.ID);

                    context.Dispose();
                }
            }
            catch (Exception ex)
            {
                throw ex;
                //pafLog.WriteIntoFile(pafDetail.ToString(), action, ex.ToString());
            }
        }

        public void Update(int id, CMEModel model)
        {
            //string action = "PAF Update";

            try
            {
                using (NovoNordiskEntities context = new NovoNordiskEntities())
                {
                    User user = context.Users.Where(s => s.ID == model.IssuedBy).FirstOrDefault();
                    string managerEmail = string.Empty;
                    if (model.PAFType == "Send")
                    {
                        var managerUser = (from u in context.Users
                                           join ptu in context.Profiles_To_User on u.ID equals ptu.User_Id
                                           join p in context.Profiles on ptu.Profile_Id equals p.ID
                                           where u.Initials.ToLower() == model.ManagerInitial.ToLower() && ptu.Status
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
                            if (managerUser.Role.ToUpper() == "RBD")
                            {
                                model.PAFType = "Send to RBD";
                            }
                            else
                            {
                                model.PAFType = "Send";
                            }
                            managerEmail = managerUser.EmailId;

                            if (model.IssuerZone == "Rare Disease" && (model.IssuerRole == "SM" || model.IssuerRole == "CT"))
                            {
                                model.PAFType = "Send to RBD";
                            }
                        }
                    }
                    PAF_Request_Type pafRequest = context.PAF_Request_Type.Where(s => s.Name == model.PAFType).FirstOrDefault();
                    DataAccessLayer.PAF cme = context.PAFs.Where(s => s.ID == id).FirstOrDefault();
                    if (cme != null)
                    {
                        cme.Issued_By_Id = model.IssuedBy;
                        cme.Request_Type_Id = pafRequest.ID;
                        cme.CME_Type = model.CME_Type;
                        cme.Disease_Area = model.DiseaseArea_Type;
                        cme.Therapy_Area_Id = model.TherapyAreaId;
                        cme.Brand_Id = model.BrandId;
                        cme.Event_Id = model.EventId;
                        cme.Lecture_Id = model.LectureId;
                        cme.Activity_Name = model.ActivityName;
                        cme.Activity_Start_Date = model.ActivityStartDate;
                        cme.Activity_End_Date = model.ActivityEndDate;
                        cme.No_Of_Days = model.NumberofDays;
                        cme.Change_Start_Date = model.ActivityStartDate;
                        cme.Change_End_Date = model.ActivityEndDate;
                        cme.Proposed_Attendee_Count = model.Proposednumberofattendee;
                        cme.Min_Guarantee_Count = model.Minimumnumberofgaurantee;
                        cme.Approximate_Count = model.Approximatecost;
                        cme.Room_Rate = model.Roomrate;
                        cme.Banquet_Room_Rate = model.Banquetroomrate;
                        cme.Cost_Per_Pax = model.CostPerPAX;
                        cme.City = model.City;
                        cme.Hotel = model.Venu_Hotel;
                        cme.Hotel_Star = model.Hotel_Star ? 5 : 4;
                        cme.Hotel_Document_Name = null;
                        cme.Hotel_Document_Path = null;
                        cme.IsDeviationApproved = model.IsDeviationApproved;
                        cme.PAF_Status_Id = context.PAF_Status.Where(s => s.Name == "Active").FirstOrDefault().ID;
                        cme.CreatedDate = DateTime.UtcNow;
                        cme.ModifiedDate = DateTime.UtcNow;
                        cme.ModifiedBy = model.IssuedBy;
                        cme.Hospital_Meeting = model.HospitalMeeting;
                        cme.IsVirtual = model.IsVirtual;
                        cme.Cost_Centre = model.Cost_Center;
                        cme.Meetingtype_Id = model.MeetingTypeId;
                        cme.IsDateDeviation = model.IsDateDeviation;
                        cme.Policy_Violation = model.Policy_Violation;
                        cme.Is_BTC = model.IsBTC;
                        cme.IsDateDeviation = model.IsDateDeviation;
                        cme.Policy_Violation = model.Policy_Violation;
                        decimal totalPAFCost = 0;
                        if (user.Therapy_Area.Name == "Rare Disease")
                        {
                            totalPAFCost = totalPAFCost + model.Roomrate + model.Banquetroomrate + (model.CostPerPAX * model.Proposednumberofattendee);
                        }
                        else
                        {
                            totalPAFCost = totalPAFCost + model.Roomrate + model.Banquetroomrate + (model.CostPerPAX * model.Proposednumberofattendee);
                        }

                        var newPafUpdate = new PAF
                        {
                            ID = id,
                            Issued_By_Id = model.IssuedBy,
                            Request_Type_Id = pafRequest.ID,
                            CME_Type = model.CME_Type,
                            Disease_Area = model.DiseaseArea_Type,
                            Issued_Date = model.IssuedDate,
                            Therapy_Area_Id = model.TherapyAreaId,
                            Brand_Id = model.BrandId,
                            Event_Id = model.EventId,
                            Lecture_Id = model.LectureId,
                            Activity_Name = model.ActivityName,
                            Activity_Start_Date = model.ActivityStartDate,
                            Activity_End_Date = model.ActivityEndDate,
                            No_Of_Days = model.NumberofDays,
                            Actual_Start_Date = model.ActivityEndDate,
                            Actual_End_Date = model.ActivityEndDate,
                            Change_Start_Date = model.ActivityStartDate,
                            Change_End_Date = model.ActivityEndDate,
                            Cost_Per_Pax = model.CostPerPAX,
                            City = model.City,
                            Hotel = model.Venu_Hotel,
                            Hotel_Star = model.Hotel_Star ? 5 : 4,
                            PAF_Status_Id = context.PAF_Status.Where(s => s.Name == "Active").FirstOrDefault().ID,
                            CreatedDate = DateTime.UtcNow,
                            ModifiedDate = DateTime.UtcNow,
                            ModifiedBy = model.IssuedBy,
                            Approver = model.Approver,
                            Hospital_Meeting = model.HospitalMeeting,
                            IsVirtual = model.IsVirtual,
                            Cost_Centre = model.Cost_Center,
                            Is_BTC = model.IsBTC,
                            IsDateDeviation = model.IsDateDeviation,
                            Policy_Violation = model.Policy_Violation
                        };

                        //pafDetail.Append(Environment.NewLine + "Paf Update Activity Started: ");
                        //var serializedLatestPaf = JsonConvert.SerializeObject(newPafUpdate, Formatting.Indented, new JsonSerializerSettings
                        //{
                        //    NullValueHandling = NullValueHandling.Ignore
                        //});
                        //pafDetail.Append(Environment.NewLine + Environment.NewLine + serializedLatestPaf);
                        //pafDetail.Append(Environment.NewLine + Environment.NewLine + "Updating PAF Detail started: " + newPafUpdate.CreatedDate + Environment.NewLine);


                        #region Save Comments
                        if (!string.IsNullOrEmpty(model.Comment))
                        {
                            //pafDetail.Append(Environment.NewLine + Environment.NewLine + "Updating Paf Comments started." + Environment.NewLine);

                            context.PAF_Command.Add(new PAF_Command { PAF_Id = cme.ID, CreatedBy = model.IssuedBy, CreatedDate = DateTime.UtcNow, Comment = model.Comment });
                            context.SaveChanges();

                            var newComments = new PAF_Command { PAF_Id = cme.ID, CreatedBy = model.IssuedBy, CreatedDate = DateTime.UtcNow, Comment = model.Comment };

                            //var deserializedComments = JsonConvert.SerializeObject(newComments, Formatting.Indented, new JsonSerializerSettings
                            {
                                //    NullValueHandling = NullValueHandling.Ignore
                                //});
                                //pafDetail.Append(deserializedComments);
                                //pafDetail.Append(Environment.NewLine + Environment.NewLine + "Updating Paf Comments is complete " + newComments.CreatedDate + Environment.NewLine);
                            }
                            #endregion

                            #region Inserting Miscellaneous

                            if (model.Miscellaneous != null)
                            {
                                var newPafMisc = new PAF_Miscellaneous();
                                //pafDetail.Append(Environment.NewLine + Environment.NewLine + "Updating Paf Miscellaneous started: " + Environment.NewLine);

                                #region Delete all Miscellaneous for specific PAF

                                List<PAF_Miscellaneous> misList = context.PAF_Miscellaneous.Where(s => s.PAF_Id == id).ToList<PAF_Miscellaneous>();
                                foreach (PAF_Miscellaneous obj in misList)
                                {
                                    context.PAF_Miscellaneous.Remove(obj);
                                    context.SaveChanges();
                                }

                                #endregion

                                foreach (var obj in model.Miscellaneous)
                                {
                                    totalPAFCost += obj.Cost;

                                    string MiscellaneousName = (context.Miscellaneous_Type.Where(s => s.Name == obj.Name).OrderByDescending(o => o.Name).FirstOrDefault() != null) ? context.Miscellaneous_Type.Where(s => s.Name == obj.Name).OrderByDescending(o => o.Name).FirstOrDefault().Name : "";
                                    if (MiscellaneousName != obj.Name)
                                    {
                                        var newMiscellaneous = new Miscellaneous_Type()
                                        {
                                            Name = obj.Name,
                                        };
                                        _commonRepository.InsertMiscellaneousType(newMiscellaneous);
                                    }
                                    var newPAFMiscellaneous = new PAF_Miscellaneous
                                    {
                                        PAF_Id = id,
                                        Type_Id = context.Miscellaneous_Type.Where(s => s.Name == obj.Name).OrderByDescending(o => o.ID).FirstOrDefault().ID,
                                        Price = obj.Cost,
                                        CreatedBy = model.IssuedBy,
                                        CreatedDate = DateTime.UtcNow
                                    };

                                    newPafMisc.PAF_Id = id;
                                    newPafMisc.Type_Id = context.Miscellaneous_Type.Where(s => s.Name == obj.Name).OrderByDescending(o => o.ID).FirstOrDefault().ID;
                                    newPafMisc.Price = obj.Cost;
                                    newPafMisc.CreatedBy = model.IssuedBy;
                                    newPafMisc.CreatedDate = DateTime.UtcNow;
                                }

                                var serializedPafCost = JsonConvert.SerializeObject(newPafMisc, Formatting.Indented, new JsonSerializerSettings
                                {
                                    NullValueHandling = NullValueHandling.Ignore
                                });
                                //pafDetail.Append(serializedPafCost);
                                //pafDetail.Append(Environment.NewLine + Environment.NewLine + "Updating Paf Miscellaneous is complete " + newPafMisc.CreatedDate + Environment.NewLine);
                            }

                            #endregion

                            #region Inserting HCP List

                            if (model.HCPs != null)
                            {
                                var newPafHCP = new PAF_HCP();
                                //pafDetail.Append(Environment.NewLine + Environment.NewLine + "Updating HCP List started: " + Environment.NewLine);

                                #region Remove existing list for specific PAF
                                //List<PAF_HCP> hcpList = context.PAF_HCP.Where(s => s.PAF_Id == id).ToList<PAF_HCP>();
                                //foreach (PAF_HCP obj in hcpList)
                                //{
                                //    DataAccessLayer.HCP latestHCP = context.HCPs.Where(p => p.ID == obj.HCP_Id).OrderByDescending(o => o.ID).FirstOrDefault();
                                //    if (latestHCP != null)
                                //    {
                                //        if (latestHCP.No_Of_Lect_Given <= latestHCP.No_Of_Lect)
                                //        {
                                //            latestHCP.No_Of_Lect_Given -= 1;
                                //            context.SaveChanges();
                                //        }
                                //    }
                                //    context.PAF_HCP.Remove(obj);
                                //    context.SaveChanges();
                                //}

                                #endregion

                                foreach (var obj in model.HCPs)
                                {
                                    var pafHcp = new PAF_HCP()
                                    {
                                        PAF_Id = id,
                                        Type_Of_HCP_Id = obj.Type,
                                        HCP_Id = obj.HCPId,
                                        HCP_First_Name = obj.First_Name,
                                        HCP_Last_Name = obj.Last_Name,
                                        TMF_No = obj.TMF_No,
                                        HCP_Payable = obj.Cost,
                                        Invitation_Email = obj.Invitation,
                                        Is_Honorarium = obj.HCPId != null,
                                        CreatedDate = DateTime.UtcNow,
                                        ModifiedDate = DateTime.UtcNow,
                                        HCP_Topic = obj.HCPtopic
                                    };

                                    totalPAFCost += obj.Cost;
                                    _pAFRepository.InsertPAFHCP(pafHcp);

                                    //var latestHCP = context.HCPs.Where(p => p.ID == obj.HCPId).OrderByDescending(o => o.ID).FirstOrDefault();
                                    //if (latestHCP != null)
                                    //{
                                    //    if (latestHCP.No_Of_Lect_Given < latestHCP.No_Of_Lect)
                                    //    {
                                    //        latestHCP.No_Of_Lect_Given += 1;
                                    //        context.SaveChanges();
                                    //    }
                                    //}

                                    newPafHCP.PAF_Id = id;
                                    newPafHCP.Type_Of_HCP_Id = obj.Type;
                                    newPafHCP.HCP_Id = obj.HCPId;
                                    newPafHCP.HCP_First_Name = obj.First_Name;
                                    newPafHCP.HCP_Last_Name = obj.Last_Name;
                                    newPafHCP.TMF_No = obj.TMF_No;
                                    newPafHCP.Is_Honorarium = obj.HCPId != null;
                                }

                                var serializedPafHcp = JsonConvert.SerializeObject(newPafHCP, Formatting.Indented, new JsonSerializerSettings
                                {
                                    NullValueHandling = NullValueHandling.Ignore
                                });
                                //pafDetail.Append(serializedPafHcp);
                                //pafDetail.Append(Environment.NewLine + Environment.NewLine + "Updating HCP List is complete " + newPafHCP.CreatedDate + Environment.NewLine);
                            }

                            #endregion

                            #region Inserting Participants List

                            if (model.Participants != null)
                            {
                                var newPafParticipant = new PAF_Participants();
                                //pafDetail.Append(Environment.NewLine + Environment.NewLine + "Updating Participants List started: " + Environment.NewLine);

                                #region Delete all Participants for specific PAF

                                List<PAF_Participants> pList = context.PAF_Participants.Where(s => s.PAF_Id == id).ToList<PAF_Participants>();
                                foreach (PAF_Participants obj in pList)
                                {
                                    context.PAF_Participants.Remove(obj);
                                    context.SaveChanges();
                                }

                                #endregion

                                foreach (var obj in model.Participants)
                                {
                                    var newPafParticipants = new PAF_Participants()
                                    {
                                        PAF_Id = id,
                                        HCP_Id = obj.ID,
                                        Participants_Name = obj.Name,
                                        PAN_No = obj.PAN,
                                        Participants_Email_ID = obj.Email_Id,
                                        Participants_Phone_Number = obj.Phone_Number,
                                        Participants_Customer_ID = obj.Customer_ID
                                    };
                                    _pAFRepository.InsertPAFParticipants(newPafParticipants);

                                    newPafParticipant.PAF_Id = id;
                                    newPafParticipant.Participants_Name = obj.Name;
                                    newPafParticipant.Participants_Customer_ID = obj.Customer_ID;
                                }

                                var serializedPafPart = JsonConvert.SerializeObject(newPafParticipant, Formatting.Indented, new JsonSerializerSettings
                                {
                                    NullValueHandling = NullValueHandling.Ignore
                                });
                                //pafDetail.Append(serializedPafPart);
                                //pafDetail.Append(Environment.NewLine + Environment.NewLine + "Updating Participants List is complete " + DateTime.UtcNow + Environment.NewLine);
                            }

                            #endregion

                            cme.TotalCost = totalPAFCost;
                            context.SaveChanges();

                            #region PAF transaction             
                            context.PAF_Transaction.Add(new PAF_Transaction { PAF_Id = cme.ID, Request_Type_Id = pafRequest.ID, CreatedBy = model.IssuedBy, CreatedDate = DateTime.UtcNow });
                            context.SaveChanges();
                            //Helper.Log_ log = new Helper.Log_();
                            //try
                            //{
                            //    context.PAF_Transaction.Add(new PAF_Transaction { PAF_Id = cme.ID, Request_Type_Id = pafRequest.ID, CreatedBy = model.IssuedBy, CreatedDate = DateTime.UtcNow });
                            //    context.SaveChanges();
                            //    StringBuilder s = new StringBuilder();
                            //    s.Append("PAF_Id:");
                            //    s.Append(cme.ID);
                            //    s.Append(" Request_Type_Id:");
                            //    s.Append(pafRequest.ID);
                            //    s.Append(" UpdatedBy:");
                            //    string Initials = context.Users.Where(u => u.ID == model.IssuedBy).FirstOrDefault().Initials;
                            //    s.Append(Initials);
                            //    s.Append(" CreatedDate:");
                            //    s.Append(DateTime.UtcNow);

                            //    log.WriteIntoFile(s.ToString(), cme.ID.ToString(), "PAF Update Transaction");
                            //}
                            //catch (Exception ex)
                            //{
                            //    log.WriteIntoFile(ex.ToString(), cme.ID.ToString(), "PAF Update Transaction error");
                            //}
                            #endregion

                            #region Save PAF Attachment
                            if (model.ImagePath != null)
                            {
                                foreach (var obj in model.ImagePath.Where(s => s.IsNew))
                                {
                                    //pafDetail.Append(Environment.NewLine + Environment.NewLine + "Updating PAF Attachment started: " + Environment.NewLine);

                                    context.PAF_Attachment.Add(new PAF_Attachment
                                    {
                                        PAF_Id = id,
                                        File_Name = obj.DocName.Replace(" ", String.Empty),
                                        File_Path = obj.DocPath.Replace(" ", String.Empty),
                                        File_Type = obj.Extension,
                                        Saved_File_Name = obj.SavedDocName,
                                        Type = obj.Type,
                                        CreatedDate = DateTime.UtcNow,
                                        ModifiedDate = DateTime.UtcNow,
                                        HCP_Name = obj.HCP_First_Name + obj.HCP_Last_Name
                                    });
                                    context.SaveChanges();

                                    var newAttachment = new PAF_Attachment
                                    {
                                        PAF_Id = id,
                                        File_Name = obj.DocName.Replace(" ", String.Empty),
                                        File_Path = obj.DocPath.Replace(" ", String.Empty),
                                        File_Type = obj.Extension,
                                        Saved_File_Name = obj.SavedDocName,
                                        Type = obj.Type,
                                        CreatedDate = DateTime.UtcNow,
                                        ModifiedDate = DateTime.UtcNow,
                                        HCP_Name = obj.HCP_First_Name + obj.HCP_Last_Name
                                    };

                                    var serializedImagePath = JsonConvert.SerializeObject(newAttachment, Formatting.Indented, new JsonSerializerSettings
                                    {
                                        NullValueHandling = NullValueHandling.Ignore
                                    });
                                    //pafDetail.Append(serializedImagePath);
                                    //pafDetail.Append(Environment.NewLine + Environment.NewLine + "Updating PAF Attachment is complete " + newAttachment.CreatedDate + Environment.NewLine);


                                }
                                #endregion

                                #region Sending Email
                                if (model.PAFType.ToLower() == "send" || model.PAFType == "Send to RBD")
                                {
                                    //PAFApproveEMail(cme.ID, user.Initials, managerEmail);

                                    var usersZone = new List<UserModel>();
                                    usersZone = (from u in context.Users
                                                 join ptu in context.Profiles_To_User on u.ID equals ptu.User_Id
                                                 join p in context.Profiles on ptu.Profile_Id equals p.ID
                                                 where ptu.Status == true && (p.Name == "CT" || p.Name == "CTB") && u.TA_Id == cme.User.Therapy_Area.ID
                                                 select new UserModel
                                                 {
                                                     ID = u.ID,
                                                     Initial = u.Initials,
                                                     Role = p.Name,
                                                     EmailId = u.Email
                                                 }).ToList();
                                    if (usersZone != null)
                                    {
                                        foreach (var us in usersZone)
                                        {
                                            //PAFSendEMail(cme.ID, us.EmailId, user.Initials);
                                        }
                                    }

                                    if (user.Therapy_Area.Name == "Rare Disease" && model.CME_Type == "Non-Standard Meeting")
                                    {
                                        if (model.DiseaseArea_Type == "Hemophilia Medical")
                                        {
                                            var users = new List<UserModel>();
                                            users = (from u in context.Users
                                                     join ptu in context.Profiles_To_User on u.ID equals ptu.User_Id
                                                     join p in context.Profiles on ptu.Profile_Id equals p.ID
                                                     where (p.Name == "PMH" || p.Name == "RMA") && ptu.Status == true
                                                     select new UserModel
                                                     {
                                                         ID = u.ID,
                                                         Initial = u.Initials,
                                                         EmailId = u.Email
                                                     }).ToList();
                                            if (users != null)
                                            {
                                                foreach (var us in users)
                                                {
                                                    //PAFSendEMail(cme.ID, us.EmailId, user.Initials);
                                                }
                                            }
                                        }
                                        if (model.DiseaseArea_Type == "Growth Hormone Medical")
                                        {
                                            var users = new List<UserModel>();
                                            users = (from u in context.Users
                                                     join ptu in context.Profiles_To_User on u.ID equals ptu.User_Id
                                                     join p in context.Profiles on ptu.Profile_Id equals p.ID
                                                     where (p.Name == "PMGH" || p.Name == "RMA") && ptu.Status == true
                                                     select new UserModel
                                                     {
                                                         ID = u.ID,
                                                         Initial = u.Initials,
                                                         EmailId = u.Email
                                                     }).ToList();
                                            if (users != null)
                                            {
                                                foreach (var us in users)
                                                {
                                                    //PAFSendEMail(cme.ID, us.EmailId, user.Initials);
                                                }
                                            }
                                        }

                                    }
                                }
                                #endregion

                                //pafDetail.Append(Environment.NewLine + "PAF: " + id + " " + pafRequest.Name + " - By: " + model.IssuerRole + Environment.NewLine);
                                //pafDetail.Append(Environment.NewLine + Environment.NewLine + "Paf Update is complete." + Environment.NewLine);
                                //pafLog.FetchLogs(pafDetail, action, id);
                            }
                            context.Dispose();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
                //pafLog.WriteIntoFile(pafDetail + "Error: " + ex.ToString(), action, id.ToString());
            }
        }

        public void Change(int id, CMEModel model)
        {
            string action = "Paf Change";
            try
            {
                using (NovoNordiskEntities context = new NovoNordiskEntities())
                {
                    var cme = context.PAFs.Where(s => s.ID == id).FirstOrDefault();
                    PAF_Request_Type pafRequest = context.PAF_Request_Type.Where(s => s.ID == cme.Request_Type_Id).FirstOrDefault();
                    User user = context.Users.Where(s => s.ID == cme.Issued_By_Id).FirstOrDefault();


                    if (cme != null)
                    {
                        cme.Issued_By_Id = cme.Issued_By_Id;
                        cme.Request_Type_Id = pafRequest.ID;
                        cme.CME_Type = model.CME_Type;
                        cme.Disease_Area = model.DiseaseArea_Type;
                        cme.Issued_Date = cme.Issued_Date;
                        cme.Therapy_Area_Id = model.TherapyAreaId;
                        cme.Brand_Id = model.BrandId;
                        cme.Event_Id = model.EventId;
                        cme.Lecture_Id = model.LectureId;
                        cme.Activity_Name = model.ActivityName;
                        cme.Activity_Start_Date = model.ActivityStartDate;
                        cme.Activity_End_Date = model.ActivityEndDate;
                        cme.No_Of_Days = model.NumberofDays;
                        cme.Change_Start_Date = model.ActivityStartDate;
                        cme.Change_End_Date = model.ActivityEndDate;
                        cme.Proposed_Attendee_Count = model.Proposednumberofattendee;
                        cme.Min_Guarantee_Count = model.Minimumnumberofgaurantee;
                        cme.Approximate_Count = model.Approximatecost;
                        cme.Room_Rate = model.Roomrate;
                        cme.Banquet_Room_Rate = model.Banquetroomrate;
                        cme.Cost_Per_Pax = model.CostPerPAX;
                        cme.City = model.City;
                        cme.Hotel = model.Venu_Hotel;
                        cme.Hotel_Star = model.Hotel_Star ? 5 : 4;
                        cme.Hotel_Document_Name = null;
                        cme.Hotel_Document_Path = null;
                        cme.PAF_Status_Id = context.PAF_Status.Where(s => s.Name == "Active").FirstOrDefault().ID;
                        cme.CreatedDate = cme.CreatedDate;
                        cme.ModifiedDate = DateTime.UtcNow;
                        cme.ModifiedBy = model.IssuedBy;
                        cme.Hospital_Meeting = model.HospitalMeeting;
                        cme.IsVirtual = model.IsVirtual;
                        cme.Meetingtype_Id = model.MeetingTypeId;
                        cme.Is_BTC = model.IsBTC;
                        cme.Policy_Violation = model.Policy_Violation;
                        cme.IsDateDeviation = model.IsDateDeviation;
                        decimal totalPAFCost = 0;
                        if (user.Therapy_Area.Name == "Rare Disease")
                        {
                            totalPAFCost = totalPAFCost + model.Roomrate + model.Banquetroomrate + (model.CostPerPAX * model.Proposednumberofattendee);
                        }
                        else
                        {
                            totalPAFCost = totalPAFCost + model.Roomrate + model.Banquetroomrate + (model.CostPerPAX * model.Proposednumberofattendee);
                        }

                        var newPafUpdate = new PAF
                        {
                            ID = id,
                            Issued_By_Id = model.IssuedBy,
                            Request_Type_Id = pafRequest.ID,
                            CME_Type = model.CME_Type,
                            Disease_Area = model.DiseaseArea_Type,
                            Issued_Date = model.IssuedDate,
                            Therapy_Area_Id = model.TherapyAreaId,
                            Brand_Id = model.BrandId,
                            Event_Id = model.EventId,
                            Lecture_Id = model.LectureId,
                            Activity_Name = model.ActivityName,
                            Activity_Start_Date = model.ActivityStartDate,
                            Activity_End_Date = model.ActivityEndDate,
                            No_Of_Days = model.NumberofDays,
                            Actual_Start_Date = model.ActivityEndDate,
                            Actual_End_Date = model.ActivityEndDate,
                            Change_Start_Date = model.ActivityStartDate,
                            Change_End_Date = model.ActivityEndDate,
                            Cost_Per_Pax = model.CostPerPAX,
                            City = model.City,
                            Hotel = model.Venu_Hotel,
                            Hotel_Star = model.Hotel_Star ? 5 : 4,
                            PAF_Status_Id = context.PAF_Status.Where(s => s.Name == "Active").FirstOrDefault().ID,
                            CreatedDate = DateTime.UtcNow,
                            ModifiedDate = DateTime.UtcNow,
                            ModifiedBy = model.IssuedBy,
                            Approver = model.Approver,
                            Hospital_Meeting = model.HospitalMeeting,
                            IsVirtual = model.IsVirtual,
                            Cost_Centre = model.Cost_Center,
                            Is_BTC = model.IsBTC,
                            IsDateDeviation = model.IsDateDeviation,
                            Policy_Violation = model.Policy_Violation
                        };

                        //pafDetail.Append(Environment.NewLine + "Paf Change Activity Started: ");
                        //var serializedLatestPaf = JsonConvert.SerializeObject(newPafUpdate, Formatting.Indented, new JsonSerializerSettings
                        //{
                        //    NullValueHandling = NullValueHandling.Ignore
                        //});
                        //pafDetail.Append(Environment.NewLine + Environment.NewLine + serializedLatestPaf);
                        //pafDetail.Append(Environment.NewLine + Environment.NewLine + "Updating PAF Detail started: " + newPafUpdate.CreatedDate + Environment.NewLine);

                        #region Save Comments
                        if (!string.IsNullOrEmpty(model.Comment))
                        {
                            //pafDetail.Append(Environment.NewLine + Environment.NewLine + "Updating Paf Comments started." + Environment.NewLine);

                            context.PAF_Command.Add(new PAF_Command { PAF_Id = cme.ID, CreatedBy = model.IssuedBy, CreatedDate = DateTime.UtcNow, Comment = model.Comment });
                            context.SaveChanges();

                            var newComments = new PAF_Command { PAF_Id = cme.ID, CreatedBy = model.IssuedBy, CreatedDate = DateTime.UtcNow, Comment = model.Comment };

                            //var deserializedComments = JsonConvert.SerializeObject(newComments, Formatting.Indented, new JsonSerializerSettings
                            //{
                            //    NullValueHandling = NullValueHandling.Ignore
                            //});
                            //pafDetail.Append(deserializedComments);
                            //pafDetail.Append(Environment.NewLine + Environment.NewLine + "Updating Paf Comments is complete " + newComments.CreatedDate + Environment.NewLine);
                        }
                        #endregion

                        #region Inserting Miscellaneous

                        if (model.Miscellaneous != null)
                        {
                            var newPafMisc = new PAF_Miscellaneous();
                            //pafDetail.Append(Environment.NewLine + Environment.NewLine + "Updating Paf Miscellaneous started: " + Environment.NewLine);

                            #region Delete all Miscellaneous for specific PAF

                            List<PAF_Miscellaneous> misList = context.PAF_Miscellaneous.Where(s => s.PAF_Id == id).ToList<PAF_Miscellaneous>();
                            foreach (PAF_Miscellaneous obj in misList)
                            {
                                context.PAF_Miscellaneous.Remove(obj);
                                context.SaveChanges();
                            }

                            #endregion

                            foreach (var obj in model.Miscellaneous)
                            {
                                totalPAFCost += obj.Cost;

                                string MiscellaneousName = (context.Miscellaneous_Type.Where(s => s.Name == obj.Name).OrderByDescending(o => o.Name).FirstOrDefault() != null) ? context.Miscellaneous_Type.Where(s => s.Name == obj.Name).OrderByDescending(o => o.Name).FirstOrDefault().Name : "";
                                var newMiscellaneous = new Miscellaneous_Type()
                                {
                                    Name = obj.Name,
                                };
                                _commonRepository.InsertMiscellaneousType(newMiscellaneous);

                                var newPAFMiscellaneous = new PAF_Miscellaneous
                                {
                                    PAF_Id = id,
                                    Type_Id = context.Miscellaneous_Type.Where(s => s.Name == obj.Name).OrderByDescending(o => o.ID).FirstOrDefault().ID,
                                    Price = obj.Cost,
                                    CreatedBy = cme.Issued_By_Id,
                                    CreatedDate = DateTime.UtcNow
                                };

                                _commonRepository.InsertPAFMiscellaneous(newPAFMiscellaneous);

                                newPafMisc.PAF_Id = id;
                                newPafMisc.Type_Id = context.Miscellaneous_Type.Where(s => s.Name == obj.Name).OrderByDescending(o => o.ID).FirstOrDefault().ID;
                                newPafMisc.Price = obj.Cost;
                                newPafMisc.CreatedBy = model.IssuedBy;
                                newPafMisc.CreatedDate = DateTime.UtcNow;
                            }
                            var serializedPafCost = JsonConvert.SerializeObject(newPafMisc, Formatting.Indented, new JsonSerializerSettings
                            {
                                NullValueHandling = NullValueHandling.Ignore
                            });
                            //pafDetail.Append(serializedPafCost);
                            //pafDetail.Append(Environment.NewLine + Environment.NewLine + "Updating Paf Miscellaneous is complete " + newPafMisc.CreatedDate + Environment.NewLine);
                        }

                        #endregion

                        #region Inserting Participants List

                        if (model.Participants != null)
                        {
                            var newPafParticipant = new PAF_Participants();
                            //pafDetail.Append(Environment.NewLine + Environment.NewLine + "Updating Participants List started: " + Environment.NewLine);

                            #region Delete all Participants for specific PAF

                            var pList = context.PAF_Participants.Where(s => s.PAF_Id == id).ToList();
                            foreach (var obj in pList)
                            {
                                context.PAF_Participants.Remove(obj);
                                context.SaveChanges();
                            }

                            #endregion

                            foreach (var obj in model.Participants)
                            {
                                var newPafParticipants = new PAF_Participants
                                {
                                    PAF_Id = id,
                                    HCP_Id = obj.ID,
                                    Participants_Name = obj.Name,
                                    PAN_No = obj.PAN,
                                    Participants_Email_ID = obj.Email_Id,
                                    Participants_Phone_Number = obj.Phone_Number,
                                    Participants_Customer_ID = obj.Customer_ID
                                };

                                _pAFRepository.InsertPAFParticipants(newPafParticipants);

                                newPafParticipant.PAF_Id = id;
                                newPafParticipant.Participants_Name = obj.Name;
                                newPafParticipant.Participants_Customer_ID = obj.Customer_ID;
                            }

                            var serializedPafPart = JsonConvert.SerializeObject(newPafParticipant, Formatting.Indented, new JsonSerializerSettings
                            {
                                NullValueHandling = NullValueHandling.Ignore
                            });
                            //pafDetail.Append(serializedPafPart);
                            //pafDetail.Append(Environment.NewLine + Environment.NewLine + "Updating Participants List is complete " + DateTime.UtcNow + Environment.NewLine);
                        }

                        #endregion

                        #region Inserting HCP List

                        if (model.HCPs != null)
                        {
                            var newPafHCP = new PAF_HCP();
                            //pafDetail.Append(Environment.NewLine + Environment.NewLine + "Updating HCP List started: " + Environment.NewLine);

                            #region Remove existing list for specific PAF
                            //var hcpList = context.PAF_HCP.Where(s => s.PAF_Id == id).ToList<PAF_HCP>();
                            //foreach (var obj in hcpList)
                            //{
                            //    var latestHCP = context.HCPs.Where(p => p.ID == obj.HCP_Id).OrderByDescending(o => o.ID).FirstOrDefault();
                            //    if (latestHCP != null)
                            //    {
                            //        if (latestHCP.No_Of_Lect_Given <= latestHCP.No_Of_Lect)
                            //        {
                            //            latestHCP.No_Of_Lect_Given -= 1;
                            //            context.SaveChanges();
                            //        }
                            //    }
                            //    context.PAF_HCP.Remove(obj);
                            //    context.SaveChanges();
                            //}

                            #endregion

                            foreach (var obj in model.HCPs)
                            {
                                var newPafHcp = new PAF_HCP
                                {
                                    PAF_Id = id,
                                    Type_Of_HCP_Id = obj.Type,
                                    HCP_Id = obj.HCPId,
                                    HCP_First_Name = obj.First_Name,
                                    HCP_Last_Name = obj.Last_Name,
                                    TMF_No = obj.TMF_No,
                                    HCP_Payable = obj.Cost,
                                    Invitation_Email = obj.Invitation,
                                    Is_Honorarium = obj.HCPId != null,
                                    CreatedDate = DateTime.UtcNow,
                                    ModifiedDate = DateTime.UtcNow,
                                    HCP_Topic = obj.HCPtopic
                                };
                                totalPAFCost += obj.Cost;
                                _pAFRepository.InsertPAFHCP(newPafHcp);

                                //var latestHCP = context.HCPs.Where(p => p.ID == obj.HCPId).OrderByDescending(o => o.ID).FirstOrDefault();
                                //if (latestHCP != null)
                                //{
                                //    if (latestHCP.No_Of_Lect_Given < latestHCP.No_Of_Lect)
                                //    {
                                //        latestHCP.No_Of_Lect_Given += 1;
                                //        context.SaveChanges();
                                //    }
                                //}

                                newPafHCP.PAF_Id = id;
                                newPafHCP.Type_Of_HCP_Id = obj.Type;
                                newPafHCP.HCP_Id = obj.HCPId;
                                newPafHCP.HCP_First_Name = obj.First_Name;
                                newPafHCP.HCP_Last_Name = obj.Last_Name;
                                newPafHcp.TMF_No = obj.TMF_No;
                                newPafHCP.Is_Honorarium = obj.HCPId != null;
                            }
                            var serializedPafHcp = JsonConvert.SerializeObject(newPafHCP, Formatting.Indented, new JsonSerializerSettings
                            {
                                NullValueHandling = NullValueHandling.Ignore
                            });
                            //pafDetail.Append(serializedPafHcp);
                            //pafDetail.Append(Environment.NewLine + Environment.NewLine + "Updating HCP List is complete " + newPafHCP.CreatedDate + Environment.NewLine);
                        }

                        #endregion

                        cme.TotalCost = totalPAFCost;
                        context.SaveChanges();

                        //Bug fix 03/02/2021 CT/Admin edit PAF, not inserting record in transaction table. 
                        //#region PAF transaction                    
                        //context.PAF_Transaction.Add(new PAF_Transaction { PAF_Id = cme.ID, Request_Type_Id = pafRequest.ID, CreatedBy = cme.Issued_By_Id, CreatedDate = DateTime.UtcNow });
                        //context.SaveChanges();
                        //#endregion

                        #region Save PAF Attachment
                        if (model.ImagePath != null)
                        {
                            foreach (var obj in model.ImagePath.Where(s => s.IsNew))
                            {
                                //pafDetail.Append(Environment.NewLine + Environment.NewLine + "Updating PAF Attachment started: " + Environment.NewLine);

                                context.PAF_Attachment.Add(new PAF_Attachment
                                {
                                    PAF_Id = id,
                                    File_Name = obj.DocName.Replace(" ", String.Empty),
                                    File_Path = obj.DocPath.Replace(" ", String.Empty),
                                    File_Type = obj.Extension,
                                    Saved_File_Name = obj.SavedDocName,
                                    Type = obj.Type,
                                    CreatedDate = DateTime.UtcNow,
                                    ModifiedDate = DateTime.UtcNow,
                                    HCP_Name = obj.HCP_First_Name + obj.HCP_Last_Name
                                });
                                context.SaveChanges();

                                var newAttachment = new PAF_Attachment
                                {
                                    PAF_Id = id,
                                    File_Name = obj.DocName.Replace(" ", String.Empty),
                                    File_Path = obj.DocPath.Replace(" ", String.Empty),
                                    File_Type = obj.Extension,
                                    Saved_File_Name = obj.SavedDocName,
                                    Type = obj.Type,
                                    CreatedDate = DateTime.UtcNow,
                                    ModifiedDate = DateTime.UtcNow,
                                    HCP_Name = obj.HCP_First_Name + obj.HCP_Last_Name
                                };

                                var serializedImagePath = JsonConvert.SerializeObject(newAttachment, Formatting.Indented, new JsonSerializerSettings
                                {
                                    NullValueHandling = NullValueHandling.Ignore
                                });
                                //pafDetail.Append(serializedImagePath);
                                //pafDetail.Append(Environment.NewLine + Environment.NewLine + "Updating PAF Attachment is complete " + newAttachment.CreatedDate + Environment.NewLine);
                            }

                        }
                        //pafDetail.Append(Environment.NewLine + "PAF: " + id + " " + pafRequest.Name + " - By: " + model.IssuerRole + Environment.NewLine);
                        //pafDetail.Append(Environment.NewLine + Environment.NewLine + "Paf Change is complete." + Environment.NewLine);
                        //pafLog.FetchLogs(pafDetail, action, id);

                        #endregion
                    }
                    context.Dispose();
                }
            }
            catch (Exception ex)
            {
                throw ex;
                //pafLog.WriteIntoFile(pafDetail + "Error: " + ex.ToString(), action, id.ToString());
            }
        }

        public void RecallPAF(int id, CMEDetails model)
        {
            //string action = "PAF Recall";
            try
            {
                using (NovoNordiskEntities context = new NovoNordiskEntities())
                {
                    DataAccessLayer.PAF latestPAF = context.PAFs.Where(p => p.ID == id).OrderByDescending(o => o.ID).FirstOrDefault();
                    PAF_Request_Type rt = context.PAF_Request_Type.Where(s => s.Name == "Draft").FirstOrDefault();
                    if (latestPAF != null)
                    {
                        latestPAF.Request_Type_Id = rt.ID;
                        context.SaveChanges();
                    }

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
                    //    s.Append(" Recalled By:");
                    //    string Initials = context.Users.Where(u => u.ID == model.IssuedBy).FirstOrDefault().Initials;
                    //    s.Append(Initials);
                    //    s.Append(" CreatedDate:");
                    //    s.Append(DateTime.UtcNow);

                    //    log.WriteIntoFile(s.ToString(), latestPAF.ID.ToString(), "Recall PAF Transaction");
                    //}
                    //catch (Exception ex)
                    //{
                    //    log.WriteIntoFile(ex.ToString(), latestPAF.ID.ToString(), "Recall PAF Transaction error");
                    //}
                    #endregion

                    //pafDetail.Append(Environment.NewLine + "PAF: " + latestPAF.ID + " is recalled or saved as " + latestPAF?.PAF_Request_Type?.Name + " - By: " + model.Issuer_Role + " " + DateTime.UtcNow + Environment.NewLine);
                    //pafLog.FetchLogs(pafDetail, action, latestPAF.ID);
                    context.Dispose();
                }
            }
            catch (Exception ex)
            {
                throw ex;
                //pafLog.WriteIntoFile(pafDetail + "Error: " + ex.ToString(), action, id.ToString());
            }
        }

        public void Delete(int id)
        {
            //string action = "Paf Delete";
            try
            {
                using (NovoNordiskEntities context = new NovoNordiskEntities())
                {
                    PAF paf = context.PAFs.Where(s => s.ID == id).FirstOrDefault();
                    if (paf != null)
                    {
                        List<PAF_Miscellaneous> mList = context.PAF_Miscellaneous.Where(s => s.PAF_Id == id).ToList<PAF_Miscellaneous>();
                        foreach (PAF_Miscellaneous obj in mList)
                        {
                            context.PAF_Miscellaneous.Remove(obj);
                            context.SaveChanges();
                        }

                        #region HCP count
                        //var hcps = new List<HCPViewModel>();
                        //hcps = (from hcp in context.HCPs
                        //        join phcp in context.PAF_HCP on hcp.ID equals phcp.HCP_Id
                        //        join p in context.PAFs on phcp.PAF_Id equals p.ID
                        //        where p.ID == paf.ID
                        //        select new HCPViewModel
                        //        {
                        //            ID = hcp.ID,
                        //            //PAN = hcp.PAN_No,
                        //            //NoLecture = hcp.No_Of_Lect,
                        //            //NoLectureGiven = hcp.No_Of_Lect_Given
                        //        }).ToList();
                        //if (hcps != null)
                        //{
                        //    foreach (var hcp in hcps)
                        //    {
                        //        var latestHCP = context.HCPs.Where(p => p.ID == hcp.ID).OrderByDescending(o => o.ID).FirstOrDefault();
                        //        if (latestHCP.No_Of_Lect_Given <= latestHCP.No_Of_Lect && latestHCP.No_Of_Lect_Given >= 1)
                        //        {
                        //            latestHCP.No_Of_Lect_Given -= 1;
                        //            context.SaveChanges();
                        //        }
                        //    }
                        //}
                        #endregion
                        List<PAF_HCP> hcpList = context.PAF_HCP.Where(s => s.PAF_Id == id).ToList();
                        foreach (PAF_HCP obj in hcpList)
                        {
                            context.PAF_HCP.Remove(obj);
                            context.SaveChanges();
                        }
                        List<PAF_Command> commandList = context.PAF_Command.Where(s => s.PAF_Id == id).ToList();
                        foreach (PAF_Command obj in commandList)
                        {
                            context.PAF_Command.Remove(obj);
                            context.SaveChanges();
                        }
                        List<PAF_Transaction> transactionList = context.PAF_Transaction.Where(s => s.PAF_Id == id).ToList();
                        foreach (PAF_Transaction obj in transactionList)
                        {
                            context.PAF_Transaction.Remove(obj);
                            context.SaveChanges();
                        }
                        List<PAF_Deviation> deviationList = context.PAF_Deviation.Where(s => s.PAF_Id == id).ToList();
                        foreach (PAF_Deviation obj in deviationList)
                        {
                            context.PAF_Deviation.Remove(obj);
                            context.SaveChanges();
                        }
                        List<PAF_Attachment> attachmentList = context.PAF_Attachment.Where(s => s.PAF_Id == id).ToList();
                        foreach (PAF_Attachment obj in attachmentList)
                        {
                            context.PAF_Attachment.Remove(obj);
                            context.SaveChanges();
                        }

                        context.PAFs.Remove(paf);
                        context.SaveChanges();

                        //pafDetail.AppendLine("PAF: " + id + " " + " - Removed/Deleted " + " - by: " + paf.Issued_By_Id + " " + paf.ModifiedDate);
                        //pafLog.FetchLogs(pafDetail, action, id);
                    }
                    context.Dispose();
                }
            }
            catch (Exception ex)
            {
                throw ex;
                //pafLog.WriteIntoFile(pafDetail + "Error: " + ex.ToString(), action, id.ToString());
            }
        }

        public void RemoveFileNOH(int id, string fileName, string fileType, string hcpName)
        {
            //string action = "Paf Remove File NOH";
            try
            {
                using (NovoNordiskEntities context = new NovoNordiskEntities())
                {
                    PAF_Attachment pa = context.PAF_Attachment.Where(s => s.PAF_Id == id && s.Saved_File_Name == fileName && s.Type == fileType && s.HCP_Name == hcpName).SingleOrDefault();
                    if (pa != null)
                    {
                        context.PAF_Attachment.Remove(pa);
                        context.SaveChanges();
                    }

                    //pafDetail.Append(Environment.NewLine + "PAF: " + id + " - FileName: " + fileName + "File NOH is removed " + DateTime.UtcNow + Environment.NewLine);
                    //pafLog.FetchLogs(pafDetail, action, id);

                    context.Dispose();
                }
            }
            catch (Exception ex)
            {
                throw ex;
                //pafLog.WriteIntoFile(pafDetail + "Error: " + ex.ToString(), action, id.ToString());
            }
        }
    }
}
