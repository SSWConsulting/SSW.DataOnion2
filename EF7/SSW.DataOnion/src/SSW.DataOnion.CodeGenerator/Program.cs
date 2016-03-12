using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.PlatformAbstractions;
using Serilog;
using System.Reflection;
using System.Text;
using SSW.DataOnion.CodeGenerator.Helpers;

namespace SSW.DataOnion.CodeGenerator
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var app = PlatformServices.Default.Application;
            var log = new LoggerConfiguration().WriteTo.ColoredConsole().CreateLogger();
            Log.Logger = log;
            log.Information("Hello world");

            var dbContextTemplate = ResourceReader.GetResourceContents("DbContext.template");
            log.Information(dbContextTemplate);
            Console.ReadLine();
        }
    }
}
