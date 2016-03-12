using System;
using Serilog;
using Fclp;
using SSW.DataOnion.CodeGenerator.Exceptions;
using SSW.DataOnion.CodeGenerator.Helpers;

namespace SSW.DataOnion.CodeGenerator
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var logger = new LoggerConfiguration().WriteTo.ColoredConsole().CreateLogger();
            Log.Logger = logger;
            
            logger.Information("Parsing options");
            var parser = CreateArgumentParser(logger);
            var parseResult = parser.Parse(args);

            if (parseResult.HelpCalled)
            {
                return;
            }

            if (parseResult.HasErrors)
            {
                logger.Warning("Some required options are missing (see below). Please use '-?', '/?', '/help' or '--help' parameter to get list of options.");
                foreach (var error in parseResult.Errors)
                {
                    logger.Error(
                        $"'{error.Option.Description}' was not found.");
                }
            }
            else
            {
                logger.Information("Options successfully parsed. Generating DbContext");
                var parsedArguments = parser.Object;
                var generator = new DbContextGenerator();
                try
                {
                    generator.Generate(parsedArguments.DbContextName, parsedArguments.EntitiesNamespace,
                        parsedArguments.DataNamespace, parsedArguments.EntitiesDll, parsedArguments.EntityBaseClass);
                }
                catch (GenerationException exception)
                {
                    logger.Error(exception, exception.Message);
                }
                catch (Exception exception)
                {
                    logger.Fatal(exception, "Unknow error occured. Details: {error}", exception.Message);
                }
            }
        }

        private static FluentCommandLineParser<ApplicationArguments> CreateArgumentParser(ILogger logger)
        {
            logger.Debug($"Create a generic parser for the '{nameof(ApplicationArguments)}' type");
            var parser = new FluentCommandLineParser<ApplicationArguments>();

            logger.Debug("Setup argument options");

            parser.Setup(arg => arg.EntitiesNamespace)
                .As("entitiesNamespace")
                .Required()
                .WithDescription("Namespace for domain entities. For multiple namespaces use comma separated string.");

            parser.Setup(arg => arg.DataNamespace)
                .As("dataNamespace")
                .Required()
                .WithDescription("\tNamespace for you data project, where DbContext resides.");

            parser.Setup(arg => arg.EntitiesDll)
                .As("entitiesDll")
                .Required()
                .WithDescription("\t\tFile path to the 'dll' file containing domain entities.");

            parser.Setup(arg => arg.DbContextName).As("name").WithDescription("\t\t\tName of your DbContext");

            parser.Setup(arg => arg.EntityBaseClass)
                .As("batch")
                .SetDefault(null)
                .WithDescription(
                    "\t\t\tOptional name of the base class entity if such exists. Only entites that use this base class will be used.");

            parser.SetupHelp("?", "help").Callback(text => logger.Information(text));
            return parser;
        }
    }
}
