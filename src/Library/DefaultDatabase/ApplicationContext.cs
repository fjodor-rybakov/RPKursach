using System.IO;
using Assets.User;
using DefaultDatabase.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace DefaultDatabase
{
    public sealed class ApplicationContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserProduct> UserProducts { get; set; }
        public DbSet<PurchaseHistory> PurchaseHistories { get; set; }

        public ApplicationContext()
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Role>().HasData(
                new Role {Id = 1, RoleName = ERoles.Administrator}, new Role {Id = 2, RoleName = ERoles.Customer}
            );
            modelBuilder.Entity<User>(entity => { entity.HasIndex(e => e.Email).IsUnique(); });
            base.OnModelCreating(modelBuilder);
        }

        private static IConfigurationRoot Config => new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json").Build();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseNpgsql(Config.GetConnectionString("DefaultConnection"));
    }
}