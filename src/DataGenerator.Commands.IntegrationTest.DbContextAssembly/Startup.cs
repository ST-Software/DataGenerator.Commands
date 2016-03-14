using System;
using Microsoft.AspNet.Hosting;
using Microsoft.Data.Entity;
using Microsoft.Extensions.DependencyInjection;

namespace DataGenerator.Commands.IntegrationTest.DbContextAssembly
{
    public class Startup
    {
        public static void Main(string[] args) => WebApplication.Run<Startup>(args);

        public void ConfigureServices(IServiceCollection services)
        {
            Console.WriteLine("DataGenerator.Commands.IntegrationTest.DbContextAssembly.Startup.ConfigureServices");
            services.AddEntityFramework()
                .AddInMemoryDatabase()
                .AddDbContext<AppDbContext>(options =>
                {
                    options.UseInMemoryDatabase();
                });
        }        
    }
}