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
        public DbSet<VisitPlan> VisitSchedules { get; set; }
        public DbSet<VisitGuest> VisitGuests { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Area>()
               .HasOne(a => a.User)
               .WithMany()  
               .HasForeignKey(a => a.UserId);

            builder.Entity<Area>()
                .HasOne(a => a.Distributor)
                .WithMany()  
                .HasForeignKey(a => a.DistributorId);

            builder.Entity<VisitGuest>()
            .HasOne(vg => vg.VisitSchedule)
            .WithMany(vs => vs.VisitGuests)
            .HasForeignKey(vg => vg.VisitScheduleId);

            builder.Entity<VisitGuest>()
                .HasOne(vg => vg.Guest)
                .WithMany()
                .HasForeignKey(vg => vg.GuestId);
        }

    }
}
