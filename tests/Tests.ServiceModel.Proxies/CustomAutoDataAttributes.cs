using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.NUnit3;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Tests
{
    [AttributeUsage(AttributeTargets.Method)]
    public class CustomAutoDataAttribute : AutoDataAttribute
    {
        public CustomAutoDataAttribute() : base(FixtureHelpers.CreateFixture)
        {
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class CustomInlineAutoDataAttribute : InlineAutoDataAttribute
    {
        public CustomInlineAutoDataAttribute(params object[] args) : base(FixtureHelpers.CreateFixture, args)
        {
        }
    }

    internal static class FixtureHelpers
    {
        public static IFixture CreateFixture()
        {
            var fixture = new Fixture();

            fixture.Customize(new AutoMoqCustomization
            {
                ConfigureMembers = true,
                GenerateDelegates = true
            });

            fixture.Register((Uri uri) => new EndpointAddress(uri.ToString()));

            fixture.Register<Binding>(() => new BasicHttpBinding());

            fixture.Register<IServiceCollection>(() => new ServiceCollection());

            fixture.Register((EndpointAddress address, Binding binding) => new Mock<ChannelFactory<ITestService>>(binding, address));

            return fixture;
        }
    }
}
