using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace EntityDatabase
{
    public sealed class ApplicationContext : DbContext
    {
        public DbSet<Models.User> Users { get; set; }
        public DbSet<Models.Company> Companies { get; set; }
        public DbSet<Models.Product> Products { get; set; }
        public DbSet<Models.Role> Roles { get; set; }
        public DbSet<Models.Category> Categories { get; set; }

        public ApplicationContext()
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Models.Role>().HasData(
                new Models.Role {Id = 1, RoleName = "Admin"}, new Models.Role {Id = 2, RoleName = "Customer"}
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