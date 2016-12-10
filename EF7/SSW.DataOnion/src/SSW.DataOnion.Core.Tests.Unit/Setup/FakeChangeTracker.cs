using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;

namespace SSW.DataOnion.Core.Tests.Unit.Setup
{
    public class FakeChangeTracker : ChangeTracker
    {
        public FakeChangeTracker(DbContext context) : base(context)
        {
        }

        public override bool AutoDetectChangesEnabled { get; set; }
    }
}
