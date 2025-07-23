using Microsoft.EntityFrameworkCore;
using T1694183.Shared.Data;

namespace T1694183.Data
{
    public class DbContextEF : DbContext
    {
        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>().ToTable("Products");
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.Id);
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
