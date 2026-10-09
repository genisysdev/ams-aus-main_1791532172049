
namespace DataAccessLayer.DAL.Model
{
    public class WorkUnitModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool? Status { get; set; }
        public string DocName { get; set; }
        public string DocPath { get; set; }
    }
}
