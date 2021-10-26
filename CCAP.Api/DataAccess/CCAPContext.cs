using CCAP.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace CCAP.Api.DataAccess {
    public class CCAPContext : DbContext {

        public CCAPContext(DbContextOptions<CCAPContext> options): base(options) {
            
        }

        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<AppRole> AppRoles { get; set; }
        public DbSet<AppUserRole> AppUserRoles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<AppUserRole>()
                .HasOne(ur => ur.AppUser)
                .WithMany(u => u.AppUserRoles)
                .HasForeignKey(ur => ur.AppUserId);
            
            modelBuilder.Entity<AppUserRole>()
                .HasOne(ur => ur.AppRole)
                .WithMany(r => r.AppUserRoles)
                .HasForeignKey(ur => ur.AppRoleId);

            modelBuilder.Entity<AppUserRole>()
                .HasKey(ur => new { ur.AppUserId, ur.AppRoleId });
        }
    }
}