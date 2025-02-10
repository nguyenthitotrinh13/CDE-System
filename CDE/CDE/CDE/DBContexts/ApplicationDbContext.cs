using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using CDE.Models;
using System.Collections;
using System.Reflection.Emit;

namespace CDE.DBContexts
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
          : base(options)
        {
        }
        public DbSet<Area> AreaLists { get; set; }
        public DbSet<Distributor> Distributors { get; set; }
        public DbSet<VisitPlan> VisitPlans { get; set; }

        public DbSet<VisitDistributor> VisitDistributors { get; set; }

        public DbSet<VisitGuest> VisitGuests { get; set; }
        public DbSet<VisitTask> Tasks { get; set; }
        public DbSet<TaskAttachment> TaskAttachments { get; set; }
        public DbSet<TaskComment> TaskComments { get; set; }
        public DbSet<TaskRating> TaskRatings { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Area>()
    .HasOne(a => a.User) // Liên kết với người dùng (ApplicationUser)
    .WithMany()           // Một người dùng có thể có nhiều Area (nếu cần)
    .HasForeignKey(a => a.UserId)  // Khóa ngoại là UserId trong bảng Area
    .OnDelete(DeleteBehavior.Restrict);  // Cấu hình hành vi khi xóa

            builder.Entity<Area>()
               .HasOne(a => a.User)
               .WithMany()  
               .HasForeignKey(a => a.UserId);

            builder.Entity<Area>()
                .HasOne(a => a.Distributor)
                .WithMany()  
                .HasForeignKey(a => a.DistributorId);
            builder.Entity<VisitPlan>()
                .Property(v => v.CreatedBy)
                .IsRequired();

            // Cấu hình khóa ngoại cho VisitDistributor -> VisitPlan
            builder.Entity<VisitDistributor>()
                .HasOne(vd => vd.VisitPlan)
                .WithMany(v => v.VisitDistributors)
                .HasForeignKey(vd => vd.VisitPlanId)
                .OnDelete(DeleteBehavior.Cascade);

            // Cấu hình khóa ngoại cho VisitDistributor -> Distributor
            builder.Entity<VisitDistributor>()
                .HasOne(vd => vd.Distributor)
                .WithMany()
                .HasForeignKey(vd => vd.DistributorId)
                .OnDelete(DeleteBehavior.Cascade);

            // Cấu hình khóa ngoại cho VisitGuest -> VisitPlan
            builder.Entity<VisitGuest>()
                .HasOne(vg => vg.VisitPlan)
                .WithMany(v => v.VisitGuests)
                .HasForeignKey(vg => vg.VisitPlanId)
                .OnDelete(DeleteBehavior.Cascade);

            // Cấu hình khóa ngoại cho VisitGuest -> ApplicationUser
            builder.Entity<VisitGuest>()
                .HasOne(vg => vg.User)
                .WithMany()
                .HasForeignKey(vg => vg.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<VisitTask>()
                .HasOne(t => t.VisitPlan)
                .WithMany(vp => vp.Tasks)
                .HasForeignKey(t => t.VisitPlanId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<VisitTask>()
                .HasOne(t => t.Assignee)
                .WithMany()
                .HasForeignKey(t => t.AssigneeId)
                .OnDelete(DeleteBehavior.Restrict);
            // Cấu hình quan hệ Task - Attachments
            builder.Entity<VisitTask>()
                .HasMany(t => t.Attachments)
                .WithOne(a => a.Task)
                .HasForeignKey(a => a.TaskId)
                .OnDelete(DeleteBehavior.Cascade);

            // Cấu hình quan hệ Task - Comments
            builder.Entity<VisitTask>()
                .HasMany(t => t.Comments)
                .WithOne(c => c.Task)
                .HasForeignKey(c => c.TaskId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<VisitTask>()
            .HasMany(t => t.Ratings)
            .WithOne(r => r.Task)
            .HasForeignKey(r => r.TaskId)
            .OnDelete(DeleteBehavior.Cascade);
        }

    }
}
