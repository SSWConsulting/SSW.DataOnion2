using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.ChangeTracking;
using Microsoft.Data.Entity.ChangeTracking.Internal;

namespace SSW.DataOnion.Core.Tests.Unit.Setup
{
    public class FakeChangeTracker : ChangeTracker
    {
        public FakeChangeTracker(
            IStateManager stateManager, 
            IChangeDetector changeDetector, 
            IEntityEntryGraphIterator graphIterator, 
            DbContext context) : base(stateManager, changeDetector, graphIterator, context)
        {
        }

        public override bool AutoDetectChangesEnabled { get; set; }
    }
}
