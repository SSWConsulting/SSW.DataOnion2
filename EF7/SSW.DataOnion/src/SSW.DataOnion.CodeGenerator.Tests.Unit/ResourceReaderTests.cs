using SSW.DataOnion.CodeGenerator.Exceptions;
using SSW.DataOnion.CodeGenerator.Helpers;
using Xunit;
using ResourceReader = SSW.DataOnion.CodeGenerator.Helpers.ResourceReader;

namespace SSW.DataOnion.CodeGenerator.Tests.Unit
{
    public class ResourceReaderTests
    {
        [Fact]
        public void Test01_Load_Existing_Resource_By_Specifying_Short_Name()
        {
            var content =
                ResourceReader.GetResourceContents("DbContext.template");
            Assert.False(string.IsNullOrEmpty(content));
        }

        [Fact]
        public void Test02_Load_Existing_Resource_By_Specifying_Full_Name()
        {
            var content =
                ResourceReader.GetResourceContents("SSW.DataOnion.CodeGenerator.templates.DbContext.template");
            Assert.False(string.IsNullOrEmpty(content));
        }

        [Fact]
        public void Test03_Try_To_Load_Resource_That_Doesnt_Exist()
        {
            Assert.Null(ResourceReader.GetResourceContents("invalid resource"));
        }
    }
}
