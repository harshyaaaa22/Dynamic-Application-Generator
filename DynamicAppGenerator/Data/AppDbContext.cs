using DynamicAppGenerator.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace DynamicAppGenerator.Data
{

    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Role> Roles { get; set; }
        public DbSet<CustomLayout> CustomLayouts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Role entity
            modelBuilder.Entity<Role>().HasKey(r => r.Id);

            // Configure CustomLayout entity
            modelBuilder.Entity<CustomLayout>().HasKey(l => l.Id);
        }
    }
}
