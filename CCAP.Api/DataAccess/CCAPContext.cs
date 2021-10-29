using CCAP.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace CCAP.Api.DataAccess {
    public class CCAPContext : DbContext {

        public CCAPContext(DbContextOptions<CCAPContext> options): base(options) {
            
        }

        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<AppRole> AppRoles { get; set; }
        public DbSet<AppUserRole> AppUserRoles { get; set; }
        
        //  New entities
        public DbSet<CreditCard> CreditCards { get; set; }
        public DbSet<CreditCardApplication> CreditCardApplications { get; set; }
        public DbSet<BankAccount> BankAccounts { get; set; }
        public DbSet<ApplicationStatus> ApplicationStatusList { get; set; }
        public DbSet<Address> Addresses { get; set; }

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
            
            //  configure for the application status
            modelBuilder.Entity<ApplicationStatus>()
                .HasOne(s => s.CreditCardApplication)
                .WithMany(a => a.StatusList)
                .HasForeignKey(s => s.CreditCardApplicationId);

            modelBuilder.Entity<ApplicationStatus>()
                .HasOne(s => s.AppUser)
                .WithMany(u => u.StatusList)
                .HasForeignKey(s => s.AppUserId);

            modelBuilder.Entity<ApplicationStatus>()
                .HasKey(s => new { s.AppUserId, s.CreditCardApplicationId });
        }
    }
}