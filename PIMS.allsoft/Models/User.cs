namespace PIMS.allsoft.Models
{
    public class User
    {
        public int UserID { get; set; }
       // public int Id { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public DateTime CreatedDate { get; set; }
       // public ICollection<UserRole> UserRoles { get; set; } // Navigation property
    }
}

