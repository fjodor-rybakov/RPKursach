using System;
using System.IO;
using EntityDatabase.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace EntityDatabase
{
    public sealed class ApplicationContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<UserProduct> UserProducts { get; set; }

        public ApplicationContext()
        {
           // Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Role>().HasData(
                new Role {Id = 1, RoleName = "Администратор"}, new Role {Id = 2, RoleName = "Покупатель"}
            );
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, CategoryName = "Игровой"}, new Category { Id = 2, CategoryName = "Домашний"}
            );
            base.OnModelCreating(modelBuilder);
        }

        private static IConfigurationRoot Config => new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json").Build();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseNpgsql(Config.GetConnectionString("DefaultConnection"));
    }
}