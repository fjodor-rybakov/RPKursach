using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ShowcaseDatabase.Models;


namespace ShowcaseDatabase
{
    public sealed class ShowcaseDatabase : DbContext
    {
        public DbSet<Company> Companies { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }

        public ShowcaseDatabase()
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().HasData(
                new Category {Id = 1, CategoryName = "Игровой"}, new Category {Id = 2, CategoryName = "Домашний"}
            );
            base.OnModelCreating(modelBuilder);
        }

        private static IConfigurationRoot Config => new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json").Build();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseNpgsql(Config.GetConnectionString("ShowcaseConnection"));
    }
}