using DataAccessLayer.DAL.Model;
using System;
using System.Collections.Generic;

namespace DataAccessLayer.DAL.CTCostCentreRepository
{
    public interface ICTCostCentreMapRepository
    {
        List<CTCostcentreMapViewModel> GetAllCostCentre_To_CT();
        void InsertCostCentreToCT(CostCentre_To_CT model);
        CostCentreMapView GetById(int UserId);
        void DeactivateCTToUserMapping(int id);
        void DeleteCTToUserMapping(int id);
    }
}
