using System.IO;
using SSW.DataOnion.CodeGenerator.Exceptions;
using Xunit;

namespace SSW.DataOnion.CodeGenerator.Tests.Unit
{
    public class AssemblyHelperTests
    {
        [Fact]
        public void Test01_Load_All_ExistingEntities()
        {
            var dll = Path.Combine(Directory.GetCurrentDirectory(),
                @"..\..\..\artifacts\bin\SSW.DataOnion.CodeGenerator.Tests.Unit\Debug\dnx451\SSW.DataOnion.CodeGenerator.Tests.Unit.dll");
            Assert.True(File.Exists(dll));

            var entityTypes =
                CodeGenerator.Helpers.AssemblyHelper.GetDomainTypes("SSW.DataOnion.CodeGenerator.Tests.Unit.TestEntities", dll);
            Assert.Equal(entityTypes.Count, 3);
        }

        [Fact]
        public void Test02_Load_Entities_With_Base_Type_Person()
        {
            var dll = Path.Combine(Directory.GetCurrentDirectory(),
                @"..\..\..\artifacts\bin\SSW.DataOnion.CodeGenerator.Tests.Unit\Debug\dnx451\SSW.DataOnion.CodeGenerator.Tests.Unit.dll");
            Assert.True(File.Exists(dll));

            var entityTypes =
                CodeGenerator.Helpers.AssemblyHelper.GetDomainTypes("SSW.DataOnion.CodeGenerator.Tests.Unit.TestEntities", dll, "Person");
            Assert.Equal(entityTypes.Count, 1);
        }

        [Fact]
        public void Test03_Load_Entities_With_Invalid_Base_Type()
        {
            var dll = Path.Combine(Directory.GetCurrentDirectory(),
                @"..\..\..\artifacts\bin\SSW.DataOnion.CodeGenerator.Tests.Unit\Debug\dnx451\SSW.DataOnion.CodeGenerator.Tests.Unit.dll");
            Assert.True(File.Exists(dll));

            var entityTypes =
                CodeGenerator.Helpers.AssemblyHelper.GetDomainTypes("SSW.DataOnion.CodeGenerator.Tests.Unit.TestEntities", dll, "Invalid");
            Assert.Equal(entityTypes.Count, 0);
        }

        [Fact]
        public void Test04_Load_Entities_For_Invalid_Assembly()
        {
            var dll = Path.Combine(Directory.GetCurrentDirectory(),
                @"..\invalid.dll");
            Assert.Throws<GenerationException>(() =>
            {
                CodeGenerator.Helpers.AssemblyHelper.GetDomainTypes("SSW.DataOnion.CodeGenerator.Tests.Unit.TestEntities", dll, "Invalid");
            });
        }
    }
}
