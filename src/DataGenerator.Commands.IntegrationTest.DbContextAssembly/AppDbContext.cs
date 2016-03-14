using System;
using Microsoft.Data.Entity;

namespace DataGenerator.Commands.IntegrationTest.DbContextAssembly
{
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
    }

    public class AppDbContext: DbContext
    {
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            Console.WriteLine("DataGenerator.Commands.IntegrationTest.DbContextAssembly.AppDbContext.OnModelCreating");
        }
    }
}