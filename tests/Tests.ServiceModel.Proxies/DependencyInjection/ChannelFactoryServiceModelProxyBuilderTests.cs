using AutoFixture.Idioms;
using InsightArchitectures.Utilities.DependencyInjection;
using NUnit.Framework;

namespace Tests.DependencyInjection
{
    [TestFixture]
    [TestOf(typeof(ChannelFactoryServiceModelProxyBuilder<>))]
    public class ChannelFactoryServiceModelProxyBuilderTests
    {
        [Test, CustomAutoData]
        public void Constructor_is_guarded(GuardClauseAssertion assertion) => assertion.Verify(typeof (ChannelFactoryServiceModelProxyBuilder<ITestService>).GetConstructors());

        [Test, CustomAutoData]
        public void Properties_are_correctly_set(ConstructorInitializedMemberAssertion assertion) => assertion.Verify(typeof (ChannelFactoryServiceModelProxyBuilder<ITestService>));
    }
}
