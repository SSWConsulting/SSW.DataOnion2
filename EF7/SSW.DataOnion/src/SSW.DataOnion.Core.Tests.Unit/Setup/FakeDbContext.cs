using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Infrastructure;
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

        public override ChangeTracker ChangeTracker => new FakeChangeTracker(this);

        public override DatabaseFacade Database => new FakeDatabaseFacade(this);

        public override int SaveChanges()
        {
            return 2;
        }
    }
}

