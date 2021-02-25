using System;
using System.Linq;
using System.Reflection;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using AutoFixture.Idioms;
using InsightArchitectures.Utilities.DependencyInjection;
using InsightArchitectures.Utilities.ServiceModel.Behaviors;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NUnit.Framework;

namespace Tests.DependencyInjection
{
    [TestFixture]
    [TestOf(typeof(ServiceModelProxyBuilderExtensions))]
    public class ServiceModelProxyBuilderExtensionsTests
    {
        [Test, CustomAutoData]
        public void SetBindings_is_guarded_against_nulls(GuardClauseAssertion assertion) => assertion.Verify(typeof(ServiceModelProxyBuilderExtensions).GetMethod(nameof(ServiceModelProxyBuilderExtensions.SetBinding)));

        [Test, CustomAutoData]
        public void SetBindings_configures_options_to_return_given_binding(IServiceModelProxyBuilder builder, Binding binding)
        {
            builder.SetBinding(binding);

            var sp = builder.Services.BuildServiceProvider();

            var options = sp.GetRequiredService<IOptionsMonitor<ServiceModelProxyOptions>>().Get(builder.Name);

            var actual = options.BindingFactory(sp);

            Assert.That(actual, Is.SameAs(binding));
        }

        [Test, CustomAutoData]
        public void SetEndpointAddress_is_guarded_against_nulls(GuardClauseAssertion assertion) => assertion.Verify(typeof(ServiceModelProxyBuilderExtensions).GetMethod(nameof(ServiceModelProxyBuilderExtensions.SetEndpointAddress)));

        [Test, CustomAutoData]
        public void SetEndpointAddress_configures_options_to_return_endpointAddress_matching_given_uri(IServiceModelProxyBuilder builder, Uri uri)
        {
            builder.SetEndpointAddress(uri);

            var sp = builder.Services.BuildServiceProvider();

            var options = sp.GetRequiredService<IOptionsMonitor<ServiceModelProxyOptions>>().Get(builder.Name);

            var actual = options.EndpointAddressFactory(sp);

            Assert.That(actual.Uri, Is.EqualTo(uri));
        }

        [Test, CustomAutoData]
        public void AddClientMessageInspector_is_guarded_against_nulls(GuardClauseAssertion assertion) => assertion.Verify(typeof(ServiceModelProxyBuilderExtensions).GetMethod(nameof(ServiceModelProxyBuilderExtensions.AddClientMessageInspector))!.MakeGenericMethod(typeof(TestClientMessageInspector)));

        [Test, CustomAutoData]
        public void AddClientMessageInspector_registers_inspector_type(IServiceModelProxyBuilder builder)
        {
            builder.AddClientMessageInspector<TestClientMessageInspector>();

            var sp = builder.Services.BuildServiceProvider();

            var inspectors = sp.GetServices<IClientMessageInspector>();

            Assert.That(inspectors, Has.Exactly(1).InstanceOf<TestClientMessageInspector>());
        }

        [Test, CustomAutoData]
        public void AddClientMessageInspector_registers_ClientMessageInspectorEndpointBehavior(IServiceModelProxyBuilder builder)
        {
            builder.AddClientMessageInspector<TestClientMessageInspector>();

            var sp = builder.Services.BuildServiceProvider();

            var actual = sp.GetService<ClientMessageInspectorEndpointBehavior>();

            Assert.That(actual, Is.Not.Null);
        }

        [Test, CustomAutoData]
        public void AddClientMessageInspector_adds_ClientMessageInspectorEndpointBehavior(IServiceModelProxyBuilder builder, ServiceEndpoint endpoint)
        {
            builder.AddClientMessageInspector<TestClientMessageInspector>();

            var sp = builder.Services.BuildServiceProvider();

            var options = sp.GetRequiredService<IOptionsMonitor<ServiceModelProxyOptions>>().Get(builder.Name);

            Assert.Multiple(() =>
            {
                Assert.That(options.EndpointConfigurations, Has.Exactly(1).InstanceOf<Action<IServiceProvider, ServiceEndpoint>>());

                options.EndpointConfigurations[0](sp, endpoint);

                Assert.That(endpoint.EndpointBehaviors.Contains(typeof(ClientMessageInspectorEndpointBehavior)));
            });
        }

        [Test, CustomAutoData]
        public void ConfigureServiceEndpoint_is_guarded_against_nulls(GuardClauseAssertion assertion) => assertion.Verify(typeof(ServiceModelProxyBuilderExtensions).GetMethod(nameof(ServiceModelProxyBuilderExtensions.ConfigureServiceEndpoint)));

        [Test, CustomAutoData]
        public void ConfigureServiceEndpoint_registers_customization(IServiceModelProxyBuilder builder, Action<IServiceProvider, ServiceEndpoint> configuration)
        {
            builder.ConfigureServiceEndpoint(configuration);

            var sp = builder.Services.BuildServiceProvider();

            var options = sp.GetRequiredService<IOptionsMonitor<ServiceModelProxyOptions>>().Get(builder.Name);

            Assert.That(options.EndpointConfigurations, Contains.Item(configuration));
        }

        [Test]
        [CustomInlineAutoData(3, new[] { typeof(ITestClient), typeof(ITestService), typeof(TestChannelFactoryProxyWrapper) })]
        [CustomInlineAutoData(4, new[] { typeof(ITestClient), typeof(ITestService), typeof(TestClient), typeof(TestClientBaseProxyWrapper) })]
        public void AddTypedWrapper_is_guarded_against_nulls(int genericParameterCount, Type[] genericParameterArgumentTypes, GuardClauseAssertion assertion)
        {
            var type = typeof (ServiceModelProxyBuilderExtensions);

            var method = type.GetMethods(BindingFlags.Public | BindingFlags.Static)
                             .FirstOrDefault(p => p.Name == nameof(ServiceModelProxyBuilderExtensions.AddTypedWrapper) && p.GetGenericArguments().Length == genericParameterCount)!
                             .MakeGenericMethod(genericParameterArgumentTypes);

            assertion.Verify(method);
        }

        [Test, CustomAutoData]
        public void AddTypedWrapper_with_proxy_registers_proxy_type(IServiceModelProxyBuilder<ITestService, TestClient> builder)
        {
            builder.AddTypedWrapper<ITestClient, ITestService, TestClient, TestClientBaseProxyWrapper>();

            Assert.That(builder.Services.Any(sd => sd.ServiceType == typeof(ITestClient) && sd.ImplementationType == typeof(TestClientBaseProxyWrapper)));
        }

        [Test, CustomAutoData]
        public void AddTypedWrapper_registers_proxy_type(IServiceModelProxyBuilder<ITestService> builder)
        {
            builder.AddTypedWrapper<ITestClient, ITestService, TestChannelFactoryProxyWrapper>();

            Assert.That(builder.Services.Any(sd => sd.ServiceType == typeof(ITestClient) && sd.ImplementationType == typeof(TestChannelFactoryProxyWrapper)));
        }
    }
}
