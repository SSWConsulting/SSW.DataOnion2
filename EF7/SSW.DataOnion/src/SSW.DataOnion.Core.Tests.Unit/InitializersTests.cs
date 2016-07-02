using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Moq;
using SSW.DataOnion.Core.Initializers;
using SSW.DataOnion.Core.Tests.Unit.Setup;
using SSW.DataOnion.Interfaces;
using Xunit;

namespace SSW.DataOnion.Core.Tests.Unit
{
    public class InitializersTests
    {
        [Fact]
        public void Test01_Initialize_Database_Using_CreateDatabaseIfNotExists_Without_DataSeeder()
        {
            var dbContext = new FakeDbContext();
            var initialiser = new CreateDatabaseIfNotExists();
            initialiser.Initialize(dbContext);

            dbContext.ensureCreatedCalled.Should().BeTrue();
            dbContext.ensureDeletedCalled.Should().BeFalse();
        }

        [Fact]
        public void Test02_Initialize_Database_Using_CreateDatabaseIfNotExists_With_DataSeeder()
        {
            var dbContext = new FakeDbContext();
            var dataSeederMock = new Mock<IDataSeeder>();
           
            var initialiser = new CreateDatabaseIfNotExists(dataSeederMock.Object);
            initialiser.Initialize(dbContext);

            dbContext.ensureCreatedCalled.Should().BeTrue();
            dbContext.ensureDeletedCalled.Should().BeFalse();

            dataSeederMock.Verify(m => m.Seed(It.IsAny<DbContext>()), Times.Once);
        }

        [Fact]
        public void Test03_Initialize_Database_Using_DropCreateDatabaseAlways_Without_DataSeeder()
        {
            var dbContext = new FakeDbContext();
            var initialiser = new DropCreateDatabaseAlways();
            initialiser.Initialize(dbContext);

            dbContext.ensureCreatedCalled.Should().BeTrue();
            dbContext.ensureDeletedCalled.Should().BeTrue();
        }

        [Fact]
        public void Test04_Initialize_Database_Using_DropCreateDatabaseAlways_With_DataSeeder()
        {
            var dbContext = new FakeDbContext();
            var dataSeederMock = new Mock<IDataSeeder>();

            var initialiser = new DropCreateDatabaseAlways(dataSeederMock.Object);
            initialiser.Initialize(dbContext);

            dbContext.ensureCreatedCalled.Should().BeTrue();
            dbContext.ensureDeletedCalled.Should().BeTrue();

            dataSeederMock.Verify(m => m.Seed(It.IsAny<DbContext>()), Times.Once);
        }

        [Fact]
        public void Test05_Initialize_Database_Using_CreateDatabaseIfNotExists_With_DataSeeder_For_Existing_Database()
        {
            var dbContext = new AnotherFakeDbContext();
            var dataSeederMock = new Mock<IDataSeeder>();

            var initialiser = new CreateDatabaseIfNotExists(dataSeederMock.Object);
            initialiser.Initialize(dbContext);

            dbContext.ensureCreatedCalled.Should().BeTrue();
            dbContext.ensureDeletedCalled.Should().BeFalse();

            dataSeederMock.Verify(m => m.Seed(It.IsAny<DbContext>()), Times.Never);
        }

        [Fact]
        public void Test06_Initialize_Database_Using_DropCreateDatabaseAlways_With_DataSeeder_For_Existing_Database()
        {
            var dbContext = new AnotherFakeDbContext();
            var dataSeederMock = new Mock<IDataSeeder>();

            var initialiser = new DropCreateDatabaseAlways(dataSeederMock.Object);
            initialiser.Initialize(dbContext);

            dbContext.ensureCreatedCalled.Should().BeTrue();
            dbContext.ensureDeletedCalled.Should().BeTrue();

            dataSeederMock.Verify(m => m.Seed(It.IsAny<DbContext>()), Times.Never);
        }
    }
}
