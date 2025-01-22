using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using CDE.Models;
using System.Collections;

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
        }

    }
}
