using AutoFixture.Idioms;
using InsightArchitectures.Utilities.ServiceModel;
using NUnit.Framework;

namespace Tests.ServiceModel
{
    [TestFixture]
    [TestOf(typeof (ChannelFactoryProxyWrapper<>))]
    public class ChannelFactoryProxyWrapperTests
    {
        [Test, CustomAutoData]
        public void Constructor_is_guarded(GuardClauseAssertion assertion) => assertion.Verify(typeof(TestChannelFactoryProxyWrapper).GetConstructors());
    }
}
