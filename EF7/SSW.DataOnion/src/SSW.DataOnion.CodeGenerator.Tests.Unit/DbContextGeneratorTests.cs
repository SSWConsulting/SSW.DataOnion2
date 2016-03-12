using System.IO;
using System.Runtime.InteropServices;
using SSW.DataOnion.CodeGenerator.Helpers;
using Xunit;
using ResourceReader = SSW.DataOnion.CodeGenerator.Tests.Unit.Helpers.ResourceReader;

namespace SSW.DataOnion.CodeGenerator.Tests.Unit
{
    public class DbContextGeneratorTests
    {
        [Fact]
        public void Test01_Generate_TestDbContext_For_All_Entities()
        {
            var dll = Path.Combine(Directory.GetCurrentDirectory(),
                @"..\..\src\artifacts\bin\SSW.DataOnion.CodeGenerator.Tests.Unit\Debug\dnx451\SSW.DataOnion.CodeGenerator.Tests.Unit.dll");
            var generator = new DbContextGenerator();
            generator.Generate("TestDbContext", "SSW.DataOnion.CodeGenerator.Tests.Unit.TestEntities", "SSW.DataOnion.CodeGenerator.Tests.Unit", dll);
            var expectedDbContextContents =
                ResourceReader.GetResourceContents(
                    "SSW.DataOnion.CodeGenerator.Tests.Unit.Resources.TestDbContext.output");
            var generatedFileContent = File.ReadAllText("TestDbContext.cs");
            try
            {
                Assert.Equal(expectedDbContextContents, generatedFileContent);
            }
            finally
            {
                File.Delete("TestDbContext.cs");
            }
        }
    }
}
