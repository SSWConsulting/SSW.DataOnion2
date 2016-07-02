using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace SSW.DataOnion.Core.Tests.Unit.Setup
{
    public class FakeDatabaseFacadeWithCreateDatabase : DatabaseFacade
    {
        public DbContext Context { get; set; }

        public FakeDatabaseFacadeWithCreateDatabase(DbContext context) : base(context)
        {
            Context = context;
        }

        public override bool EnsureCreated()
        {
            var context = this.Context as AnotherFakeDbContext;
            if (context != null)
            {
                context.ensureCreatedCalled = true;
            }

            return false;
        }

        public override bool EnsureDeleted()
        {
            var context = this.Context as AnotherFakeDbContext;
            if (context != null)
            {
                context.ensureDeletedCalled = true;
            }

            return false;
        }
    }
}
