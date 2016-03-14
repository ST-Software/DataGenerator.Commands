using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.PlatformAbstractions;

namespace DataGenerator.Commands
{
    public class Program
    {

        public static int Main(string[] args)
        {
            var app = new CommandLineApplication();
            app.Name = "DataGenerator.Commands";
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

            var recreate = app.Option("--recreate <DATABASE_HOST>|<DATABASE_NAME>",
                "Drops and creates database you want to generate data to. The parameters are has to match connection string parameters. Except the '.' in connection string is replaced by machine name in that comparision.",
                CommandOptionType.MultipleValue);

            var mode = app.Option("--mode", 
                "If mode is 'wait' it waits at the and for user to press enter before leaving a program.", 
                CommandOptionType.SingleValue);

            app.OnExecute(() =>
            {
                var dbContextAssemblyName = dbContextAssembly.Value() ??
                                            PlatformServices.Default.Application.ApplicationName;

                var generatorAssemblyName = generatorAssembly.Value() ?? dbContextAssemblyName;

                var generatorTypeName = generatorType.Value() ?? FindGenerator(generatorAssemblyName);
                if (generatorTypeName == null)
                {
                    Console.WriteLine($"Cannot find DataGenerator in assembly {generatorAssemblyName}");
                    return 1;
                }

                var options = new DbDataBoostraperOptions
                {
                    DbContextAssembly = dbContextAssemblyName,
                    GeneratorType = generatorTypeName,
                    GeneratorAssembly = generatorAssemblyName,
                    StartupAssembly = startupAssembly.Value() ?? dbContextAssemblyName,
                    Recreate = recreate.Values.Count == 2,
                    Environment = environment.Value()
                };

                if (options.Recreate)
                {
                    options.DeleteDatabase = recreate.Values[1];
                    options.DeleteHost = recreate.Values[0];
                }

                var logger = new ConsoleLogger("DataGenerator.Commands", (s, level) => true, true);
                return DbDataBootstrapper.Bootsrap(options, logger) ? 0 : 2;
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
