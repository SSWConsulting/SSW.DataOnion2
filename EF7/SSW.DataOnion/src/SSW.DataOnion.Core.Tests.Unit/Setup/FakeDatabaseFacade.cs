using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Infrastructure;

namespace SSW.DataOnion.Core.Tests.Unit.Setup
{
    public class FakeDatabaseFacade : DatabaseFacade
    {
        public DbContext Context { get; set; }

        public FakeDatabaseFacade(DbContext context) : base(context)
        {
            Context = context;
        }

        public override bool EnsureCreated()
        {
            var context = this.Context as FakeDbContext;
            if (context != null)
            {
                context.ensureCreatedCalled = true;
            }

            return true;
        }

        public override bool EnsureDeleted()
        {
            var context = this.Context as FakeDbContext;
            if (context != null)
            {
                context.ensureDeletedCalled = true;
            }

            return true;
        }
    }
}
