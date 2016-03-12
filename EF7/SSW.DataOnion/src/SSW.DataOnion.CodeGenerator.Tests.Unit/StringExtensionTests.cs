using SSW.DataOnion.CodeGenerator.Exceptions;
using SSW.DataOnion.CodeGenerator.Helpers;
using Xunit;

namespace SSW.DataOnion.CodeGenerator.Tests.Unit
{
    public class StringExtensionTests
    {
        [Fact]
        public void Test01_Pluralize_Typical_Input()
        {
            var input = "test";
            var pluralized = input.Pluralize();
            Assert.Equal(pluralized, "tests");
        }

        [Fact]
        public void Test02_Pluralize_String_With_Y_At_The_End()
        {
            var input = "testy";
            var pluralized = input.Pluralize();
            Assert.Equal(pluralized, "testies");
        }

        [Fact]
        public void Test03_Pluralize_String_With_S_At_The_End()
        {
            var input = "process";
            var pluralized = input.Pluralize();
            Assert.Equal(pluralized, "processes");
        }

        [Fact]
        public void Test04_Pluralize_Empty_String()
        {
            try
            {
                string.Empty.Pluralize();
                Assert.False(true);
            }
            catch (GenerationException)
            {
                Assert.True(true);
            }
        }
    }
}
