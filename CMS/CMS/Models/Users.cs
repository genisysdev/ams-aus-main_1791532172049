using System;

namespace CME.Models
{
    public class Users
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public int CostCenter_Id { get; set; }
        public string CostCenter { get; set; }
        public int ManagerRoleID { get; set; }
        public string Initial { get; set; }
        public int Manager_Id { get; set; }
        public int Division_Id { get; set; }
        public string Manager { get; set; }
        public string Position { get; set; }
        public bool IsAdmin { get; set; }
        public bool Status { get; set; }
        public byte Profile_Id { get; set; }
        public string Role { get; set; }
        public string MManager { get; set; }
        public byte Zone_Id { get; set; }
        public string ZoneName { get; set; }
        public string EmailId { get; set; }
        public byte TherapyArea_Id { get; set; }
        public string TherapyArea { get; set; }
        public int EmpId { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CC_CreatedDate { get; set; }
    }
}