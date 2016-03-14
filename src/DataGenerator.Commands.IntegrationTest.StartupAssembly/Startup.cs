using System;
using DataGenerator.Commands.IntegrationTest.DbContextAssembly;
using Microsoft.AspNet.Hosting;
using Microsoft.Data.Entity;
using Microsoft.Extensions.DependencyInjection;

namespace DataGenerator.Commands.IntegrationTest.StartupAssembly
{
    public class Startup
    {
        public static void Main(string[] args) => WebApplication.Run<Startup>(args);

        public void ConfigureServices(IServiceCollection services)
        {
            Console.WriteLine("DataGenerator.Commands.IntegrationTest.StartupAssembly.Startup.ConfigureServices");
            services.AddEntityFramework()
                .AddInMemoryDatabase()
                .AddDbContext<AppDbContext>(options =>
                {
                    options.UseInMemoryDatabase();
                });
        }        
    }
}