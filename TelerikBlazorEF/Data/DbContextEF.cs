﻿using Microsoft.EntityFrameworkCore;

namespace TelerikBlazorEF.Data
{
    public class DbContextEF : DbContext
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Employee> Employees { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>().ToTable("Products");
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne<Category>()
                    .WithMany()
                    .HasForeignKey(p => p.CategoryId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Category>().ToTable("Categories");
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(e => e.Id);
            });

            modelBuilder.Entity<Employee>().ToTable("Employee");
            modelBuilder.Entity<Employee>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne<Employee>()
                    .WithMany()
                    .HasForeignKey(e => e.ManagerId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            base.OnModelCreating(modelBuilder);
        }

        public DbContextEF(DbContextOptions<DbContextEF> options) : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
