using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.PlatformAbstractions;
using System.IO;

namespace DataGenerator.Commands
{
    public class Program
    {

        public static int Main(string[] args)
        {
            
            var app = new CommandLineApplication();
            app.Name = "DataGenerator.Commands (0.1.2)";
            app.Description = "Use existing DbContext in your project for different operations.";
            app.HelpOption("--help");

            var dbContextAssembly = app.Option("--dbContextAssembly <DB_CONTEXT_ASSEMBLY_NAME>",
                "Assembly name where the DbContext is located.", CommandOptionType.SingleValue);

            var generatorType = app.Option("--generatorType <GENERATOR_TYPE>",
                "Generator class name (this class has to have Generate(DbContext) method).",
                CommandOptionType.SingleValue);

            var generatorAssembly = app.Option("--generatorAssembly <GENERATOR_ASSEMBLY_NAME>",
                "Generator class name (this class has to have Generate(DbContext) method).",
                CommandOptionType.SingleValue);

            var startupAssembly = app.Option("--startupAssembly <STARTUP_ASSEMBLY>",
                "Assembly name where the Startup class which initialize DbContext is located.",
                CommandOptionType.SingleValue);

            var environment = app.Option("--environment <ENVIRONMENT>",
                "Used environment passed to Startup class for initialization (default is 'Development').",
                CommandOptionType.SingleValue,
                option => option.AssignDefault("Development"));

            var recreate = app.Option("--recreate <DATABASE_HOST>:<DATABASE_NAME>",
                "Drops and creates database you want to generate data to. The parameters are has to match connection string parameters. Except the '.' in connection string is replaced by machine name in that comparision.",
                CommandOptionType.SingleValue);

            var mode = app.Option("--mode", 
                "If mode is 'wait' it waits at the and for user to press enter before leaving a program.", 
                CommandOptionType.SingleValue);

            var argument = app.Option("--argument|-a <ARGUMENT>",
                "This argument is passed as the second argument to the data generator.",
                CommandOptionType.SingleValue);

            var logger = new ConsoleLogger("DataGenerator.Commands", (s, level) => true, true);

            app.OnExecute(() =>
            {
                var dbContextAssemblyName = dbContextAssembly.Value() ??
                                            PlatformServices.Default.Application.ApplicationName;

                var generatorAssemblyName = generatorAssembly.Value() ?? dbContextAssemblyName;

                var generatorTypeName = generatorType.Value() ?? FindGenerator(generatorAssemblyName);
                if (generatorTypeName == null)
                {
                    logger.WriteMessage(Microsoft.Extensions.Logging.LogLevel.Error, "datagen", 1,  $"Cannot find DataGenerator in assembly {generatorAssemblyName}");
                    return 1;
                }

                var options = new DbDataBoostraperOptions
                {
                    DbContextAssembly = dbContextAssemblyName,
                    GeneratorType = generatorTypeName,
                    GeneratorAssembly = generatorAssemblyName,
                    StartupAssembly = startupAssembly.Value() ?? dbContextAssemblyName,
                    Recreate = recreate.HasValue() && recreate.Value().Contains(":"),
                    Environment = environment.Value(),
                    Argument = argument.Value()
                };

                if (options.Recreate)
                {
                    var recreateOptions = recreate.Value().Split(':');
                    options.DeleteDatabase = recreateOptions[1];
                    options.DeleteHost = recreateOptions[0];
                }

                
                return DbDataBootstrapper.Bootsrap(options, logger, Directory.GetCurrentDirectory()) ? 0 : 2;
            });

            var result = app.Execute(args);

            if (mode.HasValue() && mode.Value() == "wait")
            {
                Console.ReadLine();
            }

            return result;
        }

        private static string FindGenerator(string generatorAssemblyName)
        {
            return Assembly.Load(new AssemblyName(generatorAssemblyName))?
                .GetTypes()
                .FirstOrDefault(i => i.Name == "DataGenerator")?
                .FullName;
        }
    }
}
