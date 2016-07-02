using System;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using SSW.DataOnion.Core.Initializers;
using SSW.DataOnion.Core.Tests.Unit.Setup;
using SSW.DataOnion.Interfaces;
using Xunit;

namespace SSW.DataOnion.Core.Tests.Unit
{
    public class DbContextFactoryTests
    {
        [Fact]
        public void Test01_Create_Database_Using_Valid_Initialiser_And_Connection_String()
        {
            var mock = new Mock<IDatabaseInitializer>();
            
            var dbContextFactory = new DbContextFactory(new DbContextConfig("connection string", typeof(FakeDbContext), mock.Object));
            var dbContext = dbContextFactory.Create<FakeDbContext>();

            mock.Verify(m => m.Initialize(It.IsAny<FakeDbContext>()), Times.Once);
            dbContext.Should().NotBeNull();
        }

        [Fact]
        public void Test02_Create_Database_Using_Null_Initialiser()
        {
            try
            {
                var dbContextFactory = new DbContextFactory(new DbContextConfig("connection string", typeof(FakeDbContext), null));
            }
            catch (Exception ex)
            {
                ex.Should().BeOfType<ArgumentNullException>();
            }
        }

        [Fact]
        public void Test03_Create_Database_Using_Null_Connection_String()
        {
            var mock = new Mock<IDatabaseInitializer>();
            
            try
            {
                var dbContextFactory = new DbContextFactory(new DbContextConfig(null, typeof(FakeDbContext), mock.Object));
            }
            catch (Exception ex)
            {
                ex.Should().BeOfType<DataOnionException>();
            }
        }
    }
}
