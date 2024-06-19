using PIMS.allsoft.Models;
using Microsoft.EntityFrameworkCore;

namespace PIMS.allsoft.Context
{
    public class PIMSContext: DbContext
    {
        public PIMSContext(DbContextOptions<PIMSContext> options): base(options)
        {

        }
        public DbSet<Product> Products { get; set; }
        public DbSet<Inventory> Inventories { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<ProductCategory>()
            .HasKey(pc => new { pc.ProductID, pc.CategoryID });

            modelBuilder.Entity<ProductCategory>()
                .HasOne(pc => pc.Product)
                .WithMany(p => p.ProductCategories)
                .HasForeignKey(pc => pc.ProductID);

            modelBuilder.Entity<ProductCategory>()
                .HasOne(pc => pc.Category)
                .WithMany(c => c.ProductCategories)
                .HasForeignKey(pc => pc.CategoryID);

            //modelBuilder.Entity<Product>()
            //    .HasIndex(p => p.SKU)
            //    .IsUnique();

            //modelBuilder.Entity<Inventory>()
            //    .Property(i => i.Timestamp)
            //    .HasDefaultValueSql("GETDATE()");
            //// Configure UserRole relationships
            //modelBuilder.Entity<UserRole>()
            //    .HasKey(ur => ur.UserRoleID);

            //modelBuilder.Entity<UserRole>()
            //    .HasOne(ur => ur.User)
            //    .WithMany()
            //    .HasForeignKey(ur => ur.UserId)
            //    .OnDelete(DeleteBehavior.Cascade);

            //modelBuilder.Entity<UserRole>()
            //    .HasOne(ur => ur.Role)
            //    .WithMany()
            //    .HasForeignKey(ur => ur.RoleId)
            //    .OnDelete(DeleteBehavior.Cascade);

            //// Configure User entity
            //modelBuilder.Entity<User>()
            //    .HasKey(u => u.UserID);

            //modelBuilder.Entity<User>()
            //    .Property(u => u.Name)
            //    .IsRequired()
            //    .HasMaxLength(255);

            //modelBuilder.Entity<User>()
            //    .Property(u => u.Username)
            //    .IsRequired()
            //    .HasMaxLength(255);

            //modelBuilder.Entity<User>()
            //    .Property(u => u.Password)
            //    .IsRequired();

            //modelBuilder.Entity<User>()
            //    .Property(u => u.CreatedDate)
            //    .HasDefaultValueSql("GETDATE()")
            //    .IsRequired();

            //// Configure Role entity
            //modelBuilder.Entity<Role>()
            //    .HasKey(r => r.RoleID);

            //modelBuilder.Entity<Role>()
            //    .Property(r => r.RoleName)
            //    .IsRequired()
            //    .HasMaxLength(255);

            //modelBuilder.Entity<Role>()
            //    .Property(r => r.Description)
            //    .HasMaxLength(255);

            base.OnModelCreating(modelBuilder);
        }

    }
}
