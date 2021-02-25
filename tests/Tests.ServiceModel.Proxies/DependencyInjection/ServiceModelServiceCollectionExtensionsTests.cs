using System;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Channels;
using AutoFixture.Idioms;
using InsightArchitectures.Utilities.ServiceModel;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Tests.DependencyInjection
{
    [TestFixture]
    [TestOf(typeof(ServiceModelServiceCollectionExtensions))]
    public class ServiceModelServiceCollectionExtensionsTests
    {
        [Test]
        [CustomInlineAutoData(new[] { typeof(IServiceCollection), typeof(string) }, new[] { typeof (ITestService) })]
        [CustomInlineAutoData(new[] { typeof(IServiceCollection) }, new[] { typeof(ITestService) })]
        [CustomInlineAutoData(new[] { typeof(IServiceCollection), typeof(string) }, new[] { typeof (ITestService), typeof (TestClient) })]
        [CustomInlineAutoData(new[] { typeof(IServiceCollection) }, new[] { typeof(ITestService), typeof(TestClient) })]
        public void AddServiceModelProxy_is_guarded_against_nulls(Type[] parameters, Type[] genericParameterArgumentTypes, GuardClauseAssertion assertion)
        {
            var type = typeof (ServiceModelServiceCollectionExtensions);

            var method = type.GetMethods(BindingFlags.Static | BindingFlags.Public)
                             .First(i => i.GetGenericArguments().Length == genericParameterArgumentTypes.Length && i.GetParameters().Length == parameters.Length)!
                             .MakeGenericMethod(genericParameterArgumentTypes);

            assertion.Verify(method);
        }

        [Test, CustomAutoData]
        public void AddServiceModelProxy_registers_ChannelFactory(ServiceCollection services, Binding binding, Uri endpoint)
        {
            services.AddServiceModelProxy<ITestService>()
                    .SetBinding(binding)
                    .SetEndpointAddress(endpoint);

            var serviceProvider = services.BuildServiceProvider();

            using var channelFactory = serviceProvider.GetService<ChannelFactory<ITestService>>();

            Assert.That(channelFactory, Is.Not.Null);
        }

        [Test, CustomAutoData]
        public void AddServiceModelProxy_registers_ChannelFactory(ServiceCollection services, string name, Binding binding, Uri endpoint)
        {
            services.AddServiceModelProxy<ITestService>(name)
                    .SetBinding(binding)
                    .SetEndpointAddress(endpoint);

            var serviceProvider = services.BuildServiceProvider();

            using var channelFactory = serviceProvider.GetService<ChannelFactory<ITestService>>();

            Assert.That(channelFactory, Is.Not.Null);
        }

        [Test, CustomAutoData]
        public void AddServiceModelProxy_registers_IProxyWrapper(ServiceCollection services, Binding binding, Uri endpoint)
        {
            services.AddServiceModelProxy<ITestService>()
                    .SetBinding(binding)
                    .SetEndpointAddress(endpoint);

            var serviceProvider = services.BuildServiceProvider();

            var proxy = serviceProvider.GetService<IProxyWrapper<ITestService>>();

            Assert.That(proxy, Is.Not.Null);
        }

        [Test, CustomAutoData]
        public void AddServiceModelProxy_registers_IProxyWrapper(ServiceCollection services, string name, Binding binding, Uri endpoint)
        {
            services.AddServiceModelProxy<ITestService>(name)
                    .SetBinding(binding)
                    .SetEndpointAddress(endpoint);

            var serviceProvider = services.BuildServiceProvider();

            var proxy = serviceProvider.GetService<IProxyWrapper<ITestService>>();

            Assert.That(proxy, Is.Not.Null);
        }

        [Test, CustomAutoData]
        public void AddServiceModelProxy_with_proxy_registers_Proxy_client(ServiceCollection services, Binding binding, Uri endpoint, Func<string, string> executor)
        {
            services.AddSingleton(executor);

            services.AddServiceModelProxy<ITestService, TestClient>()
                    .SetBinding(binding)
                    .SetEndpointAddress(endpoint);

            var serviceProvider = services.BuildServiceProvider();

            var client = serviceProvider.GetService<TestClient>();

            Assert.That(client, Is.Not.Null);
        }

        [Test, CustomAutoData]
        public void AddServiceModelProxy_with_proxy_registers_Proxy_client(ServiceCollection services, string name, Binding binding, Uri endpoint, Func<string, string> executor)
        {
            services.AddSingleton(executor);

            services.AddServiceModelProxy<ITestService, TestClient>(name)
                    .SetBinding(binding)
                    .SetEndpointAddress(endpoint);

            var serviceProvider = services.BuildServiceProvider();

            var client = serviceProvider.GetService<TestClient>();

            Assert.That(client, Is.Not.Null);
        }

        [Test, CustomAutoData]
        public void AddServiceModelProxy_with_proxy_registers_IProxyWrapper(ServiceCollection services, Binding binding, Uri endpoint, Func<string, string> executor)
        {
            services.AddSingleton(executor);

            services.AddServiceModelProxy<ITestService, TestClient>()
                    .SetBinding(binding)
                    .SetEndpointAddress(endpoint);

            var serviceProvider = services.BuildServiceProvider();

            var proxy = serviceProvider.GetService<IProxyWrapper<ITestService>>();

            Assert.That(proxy, Is.Not.Null);
        }

        [Test, CustomAutoData]
        public void AddServiceModelProxy_with_proxy_registers_IProxyWrapper(ServiceCollection services, string name, Binding binding, Uri endpoint, Func<string, string> executor)
        {
            services.AddSingleton(executor);

            services.AddServiceModelProxy<ITestService, TestClient>(name)
                    .SetBinding(binding)
                    .SetEndpointAddress(endpoint);

            var serviceProvider = services.BuildServiceProvider();

            var proxy = serviceProvider.GetService<IProxyWrapper<ITestService>>();

            Assert.That(proxy, Is.Not.Null);
        }
    }
}
