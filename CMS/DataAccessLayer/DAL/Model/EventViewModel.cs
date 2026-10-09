
namespace DataAccessLayer.DAL.Model
{
    public class EventViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string LM_Doc_Name { get; set; }
        public int? Sort_Order { get; set; }
        public byte Brand_Id { get; set; }
        public bool IsActive { get; set; }
    }
}
