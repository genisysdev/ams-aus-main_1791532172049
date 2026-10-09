using DataAccessLayer.DAL.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DataAccessLayer.DAL.LecturesRepository
{
    public class LecturesRepository : ILecturesRepository
    {
        private readonly NovoNordiskEntities _dataContext;

        public LecturesRepository(NovoNordiskEntities dataContext)
        {
            this._dataContext = dataContext;
        }

        public List<LecturesView> GetAllLectures(string searchString = null, string sortOrder = null)
        {
            var result = new List<LecturesView>();

            try
            {
                using (NovoNordiskEntities context = new NovoNordiskEntities())
                {
                    result = (from h in context.Lectures
                              join ta in context.Events on h.Event_Id equals ta.ID
                              join z in context.Brands on h.Brand_Id equals z.ID
                              join ht in context.Package_Type on h.Package_Type_Id equals ht.ID
                              select new LecturesView
                              {
                                  ID = h.ID,
                                  BrandId = z.ID,
                                  BrandName = z.Name,
                                  EventId = ta.ID,
                                  EventName = ta.Name,
                                  PackageTypeName = ht.Name,
                                  Lecture = h.Lecture1,
                                  Status = h.Status,
                                  TherapyAreaId = z.Therapy_Area_Id
                              }).ToList();

                    if (!string.IsNullOrEmpty(searchString))
                        result = result.Where(s => s.Lecture.ToLower().Contains(searchString.ToLower())).ToList();

                    #region Orderby List

                    switch (sortOrder)
                    {
                        case "Brand":
                            result = result.OrderBy(o => o.BrandName).ToList();
                            break;
                        case "Brand_Desc":
                            result = result.OrderByDescending(o => o.BrandName).ToList();
                            break;
                        case "Event":
                            result = result.OrderBy(o => o.EventName).ToList();
                            break;
                        case "Event_Desc":
                            result = result.OrderByDescending(o => o.EventName).ToList();
                            break;
                        case "PackageType ":
                            result = result.OrderBy(o => o.PackageTypeName).ToList();
                            break;
                        case "PackageType_Desc":
                            result = result.OrderByDescending(o => o.PackageTypeName).ToList();
                            break;
                        case "Lecture":
                            result = result.OrderBy(o => o.Lecture).ToList();
                            break;
                        case "Lecture_Desc":
                            result = result.OrderByDescending(o => o.Lecture).ToList();
                            break;
                        case "Status":
                            result = result.OrderBy(o => o.Status).ToList();
                            break;
                        case "Status_Desc":
                            result = result.OrderByDescending(o => o.Status).ToList();
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
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public LecturesView GetLecturesById(int id)
        {
            try
            {
                var hcp = _dataContext.Lectures.Find(id);

                return hcp != null ? new LecturesView
                {
                    ID = hcp.ID,
                    Lecture = hcp.Lecture1,
                    TherapyAreaId = hcp.Brand.Therapy_Area_Id,
                    Status = hcp.Status,
                    PackageTypeId = hcp.Package_Type_Id,
                    EventId = hcp.Event_Id,
                    BrandId = hcp.Brand_Id,
                    Materials = hcp.Lecture_Materials.Where(s => s.Lecture_Id == id).Select(s => new LectureMaterials
                    {
                        DocName = s.Doc_Name,
                        DocPath = s.Doc_Path,
                        Extension = Path.GetExtension(s.Doc_Path)
                    })
                } : null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int GetPAFCountByLectureId(int id)
        {
            return _dataContext.PAFs.Where(z => z.Lecture_Id == id).Count();
        }

        public List<Lecture_Materials> GetLecturesMaterialsById(int id)
        {
            return _dataContext.Lecture_Materials.Where(z => z.Lecture_Id == id).ToList();
        }

        public void UpdateLecture(int id, Lecture reqModel)
        {
            var updateEvent = _dataContext.Lectures.Find(id);

            if (updateEvent != null)
            {
                updateEvent.Package_Type_Id = reqModel.Package_Type_Id;
                updateEvent.Lecture1 = reqModel.Lecture1;
                updateEvent.Status = reqModel.Status;
                _dataContext.SaveChanges();
            }
        }

        public void DeleteLecture(int id)
        {
            var deleteLectureMaterials = _dataContext.Lecture_Materials.Where(up => up.Lecture_Id == id).ToList();

            if (deleteLectureMaterials.Any())
            {
                _dataContext.Lecture_Materials.RemoveRange(deleteLectureMaterials);
            }

            var deleteLectures = _dataContext.Lectures.Find(id);

            if (deleteLectures != null)
            {
                _dataContext.Lectures.Remove(deleteLectures);
            }

            _dataContext.SaveChanges();
        }

        public List<Brand> GetAllBrands()
        {
            return _dataContext.Brands.OrderBy(z => z.ID).ToList();
        }

        public List<Event> GetAllEvents()
        {
            return _dataContext.Events.OrderBy(z => z.ID).ToList();
        }

        public List<EventViewModel> GetBiopharmEvent(byte? id)
        {
            try
            {
                using (NovoNordiskEntities context = new NovoNordiskEntities())
                {
                    var result = (from e in context.Events
                                                 join b in context.Brands
                                                 on e.Brand_Id equals b.ID
                                                 where b.ID == id
                                                 select new EventViewModel
                                                 {
                                                     Id = e.ID,
                                                     Name = e.Name,
                                                     Brand_Id = b.ID
                                                 }
                                                ).OrderBy(o => o.Name).ToList();
                    context.Dispose();
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Event GetEventById(int id)
        {
            var result = _dataContext.Events.Find(id);
            return result != null ? new Event
            {
                ID = result.ID,
                Name = result.Name,
                Description = result.Description,
                Sort_Order = result.Sort_Order,
                CreatedBy = result.CreatedBy,
                CreatedDate = result.CreatedDate,
                Brand_Id = result.Brand_Id,
                IsActive = result.IsActive,
                Cost_Centre = result.Cost_Centre
            } : null;
        }

        public void UpdateEvent(Event reqModel)
        {
            var updateEvent = _dataContext.Events.Find(reqModel.ID);

            if (updateEvent != null)
            {
                updateEvent.Name = reqModel.Name;
                updateEvent.IsActive = reqModel.IsActive;
                _dataContext.SaveChanges();
            }
        }

        public int GetPAFCountByEventId(int id)
        {
            return _dataContext.PAFs.Where(z => z.Event_Id == id).Count();
        }
        public int GetLecturesCountByEventId(int id)
        {
            return _dataContext.Lectures.Where(z => z.Event_Id == id).Count();
        }

        public void DeleteEvent(int id)
        {
            var deleteEvent = _dataContext.Events.Find(id);

            if (deleteEvent != null)
            {
                _dataContext.Events.Remove(deleteEvent);
            }
            _dataContext.SaveChanges();
        }

        public List<Package_Type> GetAllPackageType()
        {
            return _dataContext.Package_Type.OrderBy(z => z.ID).ToList();
        }

        public List<Brand> GetAllBrandsByTAId(int id)
        {
            return _dataContext.Brands.Where(p => p.Therapy_Area_Id == id).ToList();
        }

        public List<Event> GetEventsByBrandId(int id)
        {
            return _dataContext.Events.Where(p => p.Brand_Id == id && p.IsActive == true).ToList();
        }

        public int InsertLectures(Lecture lecture)
        {
            _dataContext.Lectures.Add(lecture);
            _dataContext.SaveChanges();
            return lecture.ID;
        }

        public void InsertLecturesMaterials(Lecture_Materials lecture_Materials)
        {
            _dataContext.Lecture_Materials.Add(lecture_Materials);
            _dataContext.SaveChanges();
        }

        public void InsertEvent(Event events)
        {
            _dataContext.Events.Add(events);
            _dataContext.SaveChanges();
        }

        public List<LecturesView> GetAllEventStatus(string sortOrder = null, string searchString = null)
        {
            try
            {
                var lectures = new List<LecturesView>();
                using (NovoNordiskEntities context = new NovoNordiskEntities())
                {
                    lectures = (from ev in context.Events
                                join b in context.Brands on ev.Brand_Id equals b.ID
                                join t in context.Therapy_Area on b.Therapy_Area_Id equals t.ID
                                orderby ev.ID descending
                                select new LecturesView
                                {
                                    TherapyAreaName = t.Name,
                                    BrandName = b.Name,
                                    EventName = ev.Name,
                                    IsActive = ev.IsActive,
                                    EventId = ev.ID
                                }).ToList();

                    if (!string.IsNullOrEmpty(searchString))
                        lectures = lectures.Where(s => s.EventName.ToLower().Contains(searchString.ToLower())).ToList();

                    #region Orderby List

                    switch (sortOrder)
                    {
                        case "TherapyArea":
                            lectures = lectures.OrderBy(o => o.TherapyAreaName).ToList();
                            break;
                        case "TherapyArea_Desc":
                            lectures = lectures.OrderByDescending(o => o.TherapyAreaName).ToList();
                            break;

                        case "Brand":
                            lectures = lectures.OrderBy(o => o.BrandName).ToList();
                            break;
                        case "Brand_Desc":
                            lectures = lectures.OrderByDescending(o => o.BrandName).ToList();
                            break;
                        case "Event":
                            lectures = lectures.OrderBy(o => o.EventName).ToList();
                            break;
                        case "Event_Desc":
                            lectures = lectures.OrderByDescending(o => o.EventName).ToList();
                            break;
                        default:
                            lectures = lectures.OrderByDescending(o => o.ID).ToList();
                            break;
                    }
                    #endregion

                    context.Dispose();
                    return lectures;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void UpdateEventStatus(int id, bool status)
        {
            var updateEventStatus = _dataContext.Events.Find(id);

            if (updateEventStatus != null)
            {
                updateEventStatus.IsActive = status;
                _dataContext.SaveChanges();
            }
        }

        public List<LecturesView> GetLectures(int id)
        {
            try
            {
                using (NovoNordiskEntities context = new NovoNordiskEntities())
                {
                    var response = (from l in context.Lectures
                                    join e in context.Events on l.Event_Id equals e.ID
                                    where e.ID == id && l.Status == true
                                    select new LecturesView
                                    {
                                        ID = l.ID,
                                        Lecture = l.Lecture1
                                    }).ToList();
                    context.Dispose();
                    return response;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<LectureMaterials> GetMaterialsById(int id)
        {
            try
            {
                using (NovoNordiskEntities context = new NovoNordiskEntities())
                {
                    var response = (from l in context.Lectures
                                    join lm in context.Lecture_Materials on l.ID equals lm.Lecture_Id
                                    where l.ID == id
                                    select new LectureMaterials
                                    {
                                        DocName = lm.Doc_Name,
                                        SavedDocName = lm.Doc_Name,
                                        DocPath = lm.Doc_Path
                                    }).ToList();

                    context.Dispose();
                    return response;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<LectureMaterials> GetMaterialsByEventId(int id)
        {
            //string action = "Event: GetMaterialsEvent";
            //pafDetail.Append(Environment.NewLine + Environment.NewLine + "PAF ID" + id);
            try
            {
                using (NovoNordiskEntities context = new NovoNordiskEntities())
                {
                    var response = (from l in context.Lectures
                                                               join lm in context.Lecture_Materials on l.ID equals lm.Lecture_Id
                                                               where l.Event_Id == id
                                                               select new LectureMaterials
                                                               {
                                                                   DocName = lm.Doc_Name,
                                                                   SavedDocName = lm.Doc_Name,
                                                                   DocPath = lm.Doc_Path
                                                               }).ToList();

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
    }
}
