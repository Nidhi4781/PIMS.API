namespace PIMS.allsoft.Models
{
    public class UserRole
    {
        public int UserRoleID { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public int RoleId { get; set; }
        public Role Role { get; set; }
    }
    //public class UserRole
    //{
    //   // public int Id { get; set; }      

    //    //
    //    public int UserRoleID { get; set; }
    //    public User UserId { get; set; }
    //  //  public User User { get; set; } // Navigation property
    //    public Role RoleId { get; set; }
    //  //  public Role Role { get; set; } // Navigation property

    //}
}
