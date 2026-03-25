using Microsoft.EntityFrameworkCore;
using UMS.Models;

namespace UMS.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<UsersModel> Users { get; set; }
        public DbSet<Studentsmodel> Students { get; set; }
        public DbSet<TeachersModel> Teachers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Users table
            modelBuilder.Entity<UsersModel>(entity =>
            {
                entity.ToTable("Users");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FirstName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Username).IsRequired().HasMaxLength(30);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
                entity.HasIndex(e => e.Username).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();
            });

            // Configure Students table
            modelBuilder.Entity<Studentsmodel>(entity =>
            {
                entity.ToTable("Students");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.batch).IsRequired().HasMaxLength(20);
                // Prevent duplicate students with same name and batch
                entity.HasIndex(e => new { e.Name, e.batch }).IsUnique();
            });

            // Configure Teachers table
            modelBuilder.Entity<TeachersModel>(entity =>
            {
                entity.ToTable("Teachers");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FirstName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.LastName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Subject).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
                // Prevent duplicate teachers with same email
                entity.HasIndex(e => e.Email).IsUnique();
                // Prevent duplicate teachers with same name, subject, and email
                entity.HasIndex(e => new { e.FirstName, e.LastName, e.Email }).IsUnique();
            });
        }
    }
}
