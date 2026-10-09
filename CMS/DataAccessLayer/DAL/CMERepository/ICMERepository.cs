using DataAccessLayer.DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.DAL.CMERepository
{
    public interface ICMERepository
    {
        CMEDetails GetById(int id);
        CMEModel GetPAFById(int id);
        CMEDetails GetRoleID(int id);
        List<PAFList> GetLatestviolation(int Id);
        void UpdateDocument(int id, CMEModel model);
        void ApprovePAFBiopharm(int id, CMEDetails model);
        void ApprovePAF(int id, CMEDetails model);
        void RejectPAFBiopharm(int id, CMEDetails model);
        void RejectPAF(int id, CMEDetails model);
        void SendtoClosePAFBiopharm(int id, CMEDetails model);
        void ClosePAFBiopharm(int id, CMEDetails model);
        void ClosePAF(int id, CMEDetails model);
        void RemovePAF(int id, CMEDetails model);
    }
}
