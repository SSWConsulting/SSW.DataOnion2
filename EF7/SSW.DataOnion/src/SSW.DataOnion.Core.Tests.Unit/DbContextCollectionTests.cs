using FluentAssertions;
using SSW.DataOnion.Core.Tests.Unit.Setup;
using Xunit;

namespace SSW.DataOnion.Core.Tests.Unit
{
    public class DbContextCollectionTests
    {
        [Fact]
        public void Test01_Attempt_To_Get_DbContext_That_Was_Never_Registered_Before()
        {
            var dbContextCollection = new DbContextCollection(dbContextFactory: FakeObjectFactory.CreateDbContextFactory());
            var dbContext = dbContextCollection.Get<FakeDbContext>();
            dbContext.Should().NotBeNull();
        }

        [Fact]
        public void Test02_Attempt_To_Get_DbContext_That_Was_Registered_Before()
        {
            var dbContextCollection = new DbContextCollection(dbContextFactory: FakeObjectFactory.CreateDbContextFactory());
            var dbContext = dbContextCollection.Get<FakeDbContext>();
            var dbContextSecondAttempt = dbContextCollection.Get<FakeDbContext>();
            dbContextSecondAttempt.InstanceId.Should().Be(dbContext.InstanceId);
        }

        [Fact]
        public void Test03_Attempt_To_Get_DbContext_That_Was_Registered_Before_Without_Supplying_Factory()
        {
            var dbContextCollection = new DbContextCollection();
            var dbContext = dbContextCollection.Get<FakeDbContext>();
            var dbContextSecondAttempt = dbContextCollection.Get<FakeDbContext>();
            dbContextSecondAttempt.InstanceId.Should().Be(dbContext.InstanceId);
        }

        [Fact]
        public void Test04_Attempt_To_Commit_Changes_On_Two_DbContexts()
        {
            var dbContextCollection = new DbContextCollection();
            var dbContext1 = dbContextCollection.Get<FakeDbContext>();
            var dbContext2 = dbContextCollection.Get<AnotherFakeDbContext>();
            var commitedChanges = dbContextCollection.Commit();
            dbContext1.Should().NotBeNull();
            dbContext2.Should().NotBeNull();
            dbContext1.InstanceId.Should().NotBe(dbContext2.InstanceId);
            commitedChanges.Should().Be(3);
        }

        [Fact]
        public void Test05_Attempt_To_Commit_Changes_On_One_DbContext()
        {
            var dbContextCollection = new DbContextCollection();
            var dbContext = dbContextCollection.Get<FakeDbContext>();
            var commitedChanges = dbContextCollection.Commit();
            dbContext.Should().NotBeNull();
            commitedChanges.Should().Be(2);
        }

        [Fact]
        public void Test06_Attempt_To_Commit_Changes_For_ReadOnly_DbContext()
        {
            var dbContextCollection = new DbContextCollection(true);
            var dbContext = dbContextCollection.Get<FakeDbContext>();
            var commitedChanges = dbContextCollection.Commit();
            dbContext.Should().NotBeNull();
            commitedChanges.Should().Be(0);
        }
    }
}
