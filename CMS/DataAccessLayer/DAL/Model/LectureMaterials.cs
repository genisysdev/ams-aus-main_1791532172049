
namespace DataAccessLayer.DAL.Model
{
    public class LectureMaterials
    {
        public string DocName { get; set; }
        public string SavedDocName { get; set; }
        public string DocPath { get; set; }
        public string Extension { get; set; }
        public string Type { get; set; }
        public bool IsNew { get; set; }
        public string HCP_First_Name { get; set; }
        public string HCP_Last_Name { get; set; }
        public string PostType { get; set; }
    }
}
