using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Moq;

namespace SSW.DataOnion.Core.Tests.Unit.Setup
{
    public class AnotherFakeDbContext : DbContext
    {
        public bool ensureCreatedCalled = false;

        public bool ensureDeletedCalled = false;

        public AnotherFakeDbContext()
        {
            this.InstanceId = Guid.NewGuid();
        }

        public Guid InstanceId { get; set; }

        public override ChangeTracker ChangeTracker
            => new FakeChangeTracker(
                new Mock<IStateManager>().Object,
                new Mock<IChangeDetector>().Object,
                new Mock<IEntityEntryGraphIterator>().Object, this);

        public override DatabaseFacade Database => new FakeDatabaseFacadeWithCreateDatabase(this);

        public override int SaveChanges()
        {
            return 1;
        }
    }
}
