using Microsoft.EntityFrameworkCore;
using TimeSheet.AuthAPI.Domain;

namespace AuthAPI
{
    public class IdentityContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<UserSession> UserSessions { get; set; }

        public IdentityContext (DbContextOptions options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserSession>().HasKey(x => x.UserSessionId);
            modelBuilder.Entity<UserSession>().HasOne(x => x.User)
                .WithMany(y => y.UserSessions)
                .HasForeignKey(z => z.UserId);
        }
    }
}
