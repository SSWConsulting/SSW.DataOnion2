using System;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.ChangeTracking;
using Microsoft.Data.Entity.ChangeTracking.Internal;
using Microsoft.Data.Entity.Infrastructure;
using Moq;

namespace SSW.DataOnion.Core.Tests.Unit.Setup
{
    public class FakeDbContext : DbContext
    {
        public bool ensureCreatedCalled = false;

        public bool ensureDeletedCalled = false;

        public FakeDbContext()
        {
            this.InstanceId = Guid.NewGuid();
        }

        public Guid InstanceId { get; set; }

        public override ChangeTracker ChangeTracker 
            => new FakeChangeTracker(
                new Mock<IStateManager>().Object, 
                new Mock<IChangeDetector>().Object, 
                new Mock<IEntityEntryGraphIterator>().Object, this);

        public override DatabaseFacade Database => new FakeDatabaseFacade(this);

        public override int SaveChanges()
        {
            return 2;
        }
    }
}

