using DataAccessLayer.DAL.Model;
using System.Collections.Generic;

namespace DataAccessLayer.DAL.PAFRepository
{
    public interface IPAFRepository
    {
        int GetPAFId();
        void InsertPAF(PAF paf);
        void InsertPAFHCP(PAF_HCP pafHcp);
        void InsertPAFParticipants(PAF_Participants pafPart);
        List<PAFList> GetDraft(PAFSearch filterModel);
        List<PAFList> GetAllForm(PAFSearch filterModel);
        List<PAFList> GetPreApprovalStatus(PAFSearch filterModel);
        List<PAFList> GetRejectedBiopharm(PAFSearch filterModel);
        List<PAFList> GetApprovedBiopharm(PAFSearch filterModel);
        List<PAFList> GetRejected(PAFSearch filterModel);
        List<PAFList> GetApproved(PAFSearch filterModel);
        List<PAFList> GetAttendBiopharm(PAFSearch filterModel);
        List<PAFList> GetAttend(PAFSearch filterModel);
        List<PAFList> GetPendingBiopharm(PAFSearch filterModel);
        List<PAFList> GetPending(PAFSearch filterModel);
        List<PAFList> GetClosed(PAFSearch filterModel);
        List<PAFList> GetRemoved(PAFSearch filterModel);
        List<PAFList> GetAllZoneForm(PAFSearch filterModel);
        List<PAFList> GetRejectedCloseBiopharm(PAFSearch filterModel);
        List<PAFList> GetAllPendingVarificationForm(PAFSearch filterModel);
    }
}
