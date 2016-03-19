using SSW.DataOnion.Interfaces;
using Moq;

namespace SSW.DataOnion.Core.Tests.Unit.Setup
{
    public class FakeObjectFactory
    {
        public static IDbContextFactory CreateDbContextFactory()
        {
            var mock = new Mock<IDbContextFactory>();
            mock.Setup(m => m.Create<FakeDbContext>()).Returns(new FakeDbContext());
            return mock.Object;
        }
    }
}
