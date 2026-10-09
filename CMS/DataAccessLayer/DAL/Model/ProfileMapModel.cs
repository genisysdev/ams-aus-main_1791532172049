using System.Collections.Generic;

namespace DataAccessLayer.DAL.Model
{
    public class ProfileMapModel
    {
        public int Id { get; set; }
        public byte ProfileId { get; set; }
        public string ProfileName { get; set; }
        public string UserInitial { get; set; }
        public List<string> UserInitials { get; set; }
    }
}
