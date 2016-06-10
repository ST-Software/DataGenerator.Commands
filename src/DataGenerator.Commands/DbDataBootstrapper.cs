using System;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace DataGenerator.Commands
{
    public class DbDataBootstrapper
    {
        public static bool Bootsrap(DbDataBoostraperOptions options, ILogger logger, string targetDir)
        {
            var dbContextOperations = new DbContextOperations(
                new SimpleLoggerProvider(logger),
                Assembly.Load(options.DbContextAssembly),
                Assembly.Load(options.StartupAssembly),
                options.Environment,
                targetDir);

            logger.LogInformation("Creating db context...");
            using (var dbContext = dbContextOperations.CreateContext(null))
            {
                logger.LogInformation("Searching data gen type: {0}", options.GeneratorType);

                var dataGenAssembly = Assembly.Load(new AssemblyName(options.GeneratorAssembly));
                var dataGenType = dataGenAssembly.GetType(options.GeneratorType);
                logger.LogInformation("Data gen type found: {0}", dataGenType);
                var dataGen = Activator.CreateInstance(dataGenType);

                if (options.Recreate)
                {
                    var dbConnection = dbContext.Database.GetDbConnection();
                    var machineName = dbConnection.DataSource.Replace(".", Environment.MachineName); //Environment.GetEnvironmentVariable("COMPUTERNAME")
                    var dbName = dbConnection.Database;

                    var canDelete = string.Equals(machineName, options.DeleteHost, StringComparison.CurrentCultureIgnoreCase) 
                        && string.Equals(dbName, options.DeleteDatabase, StringComparison.CurrentCultureIgnoreCase);

                    logger.LogInformation($"--recreate command check -  {machineName} == {options.DeleteHost} && {dbName} == {options.DeleteDatabase} => {canDelete}");
                    if (canDelete)
                    {
                        logger.LogInformation($"Deleting database: {dbConnection.ConnectionString}");
                        dbContext.Database.EnsureDeleted();                        
                    }
                    else
                    {
                        logger.LogError($"if you want to delete the database you need to specify --delete:db_host:db_name (for current connection string --delete:{machineName}:{dbName})");
                        return false;
                    }
                }
                dbContext.Database.EnsureCreated();

                var dataGenMethod = dataGenType.GetMethod("Generate");

                logger.LogInformation("Starting generating data...");
                var numberOfExpectedParameters = dataGenMethod.GetParameters().Length;
                if (numberOfExpectedParameters == 1)
                {
                    dataGenMethod.Invoke(dataGen, new[] {dbContext});
                }
                else if (numberOfExpectedParameters == 2)
                {
                    dataGenMethod.Invoke(dataGen, new object[] {dbContext, options.Argument});
                }
                else
                {
                    logger.LogError("Generator has a wrong signiture. Expected signitures are: Generate(YourDbContext dbContext), Generate(YourDbContext dbContext, string argument)");
                }
                logger.LogInformation("Done");

                return true;
            }
        }
    }
}