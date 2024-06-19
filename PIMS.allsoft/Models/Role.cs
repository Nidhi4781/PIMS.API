namespace PIMS.allsoft.Models
{
    public class Role
    {
        public int RoleID { get; set; }
      //  public int Id { get; set; }
        public string RoleName { get; set; }
      //  public string Name { get; set; }
        public string Description { get; set; }
    //    public ICollection<UserRole> UserRoles { get; set; } // Navigation property
    }
}
