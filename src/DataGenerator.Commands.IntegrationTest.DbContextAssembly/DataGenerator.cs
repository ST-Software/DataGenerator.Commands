using System;

namespace DataGenerator.Commands.IntegrationTest.DbContextAssembly
{
    public class DataGenerator
    {
        public void Generate(AppDbContext dbContext, string argument)
        {
            Console.WriteLine("DataGenerator.Commands.IntegrationTest.DbContextAssembly.DataGenerator.Generate");
            Console.WriteLine($"argument: {argument}");
            dbContext.Users.Add(new User { Name = "User1" });
            dbContext.Users.Add(new User { Name = "User2" });
            dbContext.SaveChanges();
            Console.WriteLine("DataGenerator.Commands.IntegrationTest.DbContextAssembly.DataGenerator.Generate-DONE");
        }
    }
}