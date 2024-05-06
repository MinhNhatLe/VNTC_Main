using dotnetstartermvc.Models.Contacts;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace dotnetstartermvc.Models
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            base.OnConfiguring(builder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var tableName = entityType.GetTableName();
                if (tableName.StartsWith("AspNet"))
                {
                    entityType.SetTableName(tableName.Substring(6));
                }
            }

            modelBuilder.Entity<UserDuty>().HasKey(ud => new { ud.UserId, ud.DutyId });
        }

        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Recruitment> Recruitments { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<WorkSchedule> WorkSchedules { get; set; }
        public DbSet<Assignment> Assignments { get; set; }
        public DbSet<ServicePhoto> ServicePhotos { get; set; }
        public DbSet<NotificationPhoto> NotificationPhotos { get; set; }
        public DbSet<RecruitmentPhoto> RecruitmentPhotos { get; set; }
        public DbSet<Duty> Duties { get; set; }
        public DbSet<UserDuty> UserDuties { get; set; }
    }
}