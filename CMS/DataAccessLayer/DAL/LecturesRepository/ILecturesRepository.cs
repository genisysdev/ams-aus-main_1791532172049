using DataAccessLayer.DAL.Model;
using System.Collections.Generic;

namespace DataAccessLayer.DAL.LecturesRepository
{
    public interface ILecturesRepository
    {
        List<LecturesView> GetAllLectures(string searchString = null, string sortOrder = null);
        LecturesView GetLecturesById(int id);
        List<Brand> GetAllBrands();
        int GetPAFCountByLectureId(int id);
        List<Lecture_Materials> GetLecturesMaterialsById(int id);
        void UpdateLecture(int id, Lecture reqModel);
        void DeleteLecture(int id);
        List<Event> GetAllEvents();
        List<EventViewModel> GetBiopharmEvent(byte? id);
        Event GetEventById(int id);
        void UpdateEvent(Event reqModel);
        int GetPAFCountByEventId(int id);
        int GetLecturesCountByEventId(int id);
        void DeleteEvent(int id);
        List<Package_Type> GetAllPackageType();
        List<Brand> GetAllBrandsByTAId(int id);
        List<Event> GetEventsByBrandId(int id);
        int InsertLectures(Lecture lecture);
        void InsertLecturesMaterials(Lecture_Materials lecture_Materials);
        void InsertEvent(Event events);
        List<LecturesView> GetAllEventStatus(string sortOrder = null, string searchString = null);
        void UpdateEventStatus(int id, bool status);
        List<LecturesView> GetLectures(int id);
        List<LectureMaterials> GetMaterialsById(int id);
        List<LectureMaterials> GetMaterialsByEventId(int id);
    }
}
