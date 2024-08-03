using Microsoft.EntityFrameworkCore;
using ProgressTrackerSubjectService.Models;

namespace ProgressTrackerSubjectService.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<SubjectModel> Subjects { get; set; }
        public DbSet<UserSubjectsModel> UserSubjects { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<SubjectModel>()
                .HasIndex(u => u.Name)
                .IsUnique();
            
            modelBuilder.Entity<UserSubjectsModel>()
                .HasIndex(u => u.UserId)
                .IsUnique();
        }
    }
}