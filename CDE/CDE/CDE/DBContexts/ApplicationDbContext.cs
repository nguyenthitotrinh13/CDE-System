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
        public DbSet<VisitTask> VisitTasks { get; set; }
        public DbSet<TaskAttachment> TaskAttachments { get; set; }
        public DbSet<TaskComment> TaskComments { get; set; }
        public DbSet<TaskRating> TaskRatings { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Area>()
            .HasOne(a => a.User) 
            .WithMany()           
            .HasForeignKey(a => a.UserId)  
            .OnDelete(DeleteBehavior.Restrict);  

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

            builder.Entity<VisitTask>()
                .HasOne(t => t.VisitPlan)
                .WithMany(vp => vp.Tasks)
                .HasForeignKey(t => t.VisitPlanId)
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
