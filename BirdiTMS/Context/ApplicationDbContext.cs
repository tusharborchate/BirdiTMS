using Microsoft.EntityFrameworkCore;
using BirdiTMS.Models.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace BirdiTMS.Context
{
    public class ApplicationDbContext: IdentityDbContext<ApplicationUser, IdentityRole, string, IdentityUserClaim<string>, IdentityUserRole<string>, IdentityUserLogin<string>, IdentityRoleClaim<string>, IdentityUserToken<string>>
    {
        public virtual DbSet<BirdiTask> BirdiTasks { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<ApplicationUser>(e =>
            {
                e.Property(ep => ep.Id).ValueGeneratedOnAdd();

            });
        }
    }
}
