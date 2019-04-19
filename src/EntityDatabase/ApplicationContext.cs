using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EntityDatabase
{
    public sealed class ApplicationContext : DbContext
        {
            public DbSet<Models.User> Users { get; set; }
             
            public ApplicationContext()
            {
                Database.EnsureCreated();
            }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder.UseNpgsql("Host=localhost;Database=postgres;Username=postgres;Password=qwerty123");
        }
}
