using Microsoft.EntityFrameworkCore;

namespace UserSessions.Data
{
    public class DbContextEF : DbContext
    {
        internal DbSet<User> Users { get; set; }
        internal DbSet<Session> Sessions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
            });

            modelBuilder.Entity<Session>().ToTable("Sessions");
            modelBuilder.Entity<Session>(entity =>
            {
                entity.HasKey(e => e.Token);
                entity.HasOne<User>()
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);
            });

            base.OnModelCreating(modelBuilder);
        }

        public DbContextEF(DbContextOptions<DbContextEF> options) : base(options)
        {
            // Called once in Program.cs
            //Database.EnsureCreated();
        }
    }
}
