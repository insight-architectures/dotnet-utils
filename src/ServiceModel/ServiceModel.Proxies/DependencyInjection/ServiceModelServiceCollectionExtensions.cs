// ReSharper disable CheckNamespace

using System;
using System.ServiceModel;
using InsightArchitectures.Utilities.DependencyInjection;
using InsightArchitectures.Utilities.ServiceModel;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// A set of extension methods to <see cref="IServiceCollection" /> to register WCF proxies.
    /// </summary>
    public static class ServiceModelServiceCollectionExtensions
    {
        private static string GetConfigurationName(Type type) => type?.FullName ?? throw new ArgumentNullException(nameof(type));

        /// <summary>
        /// Adds to the <see cref="IServiceCollection"/> the components needed to consume a <see cref="IProxyWrapper{TContract}"/>.
        /// </summary>
        /// <typeparam name="TContract">The contract of the WCF service to build a proxy for.</typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <param name="name">The logical name of the proxy to configure.</param>
        /// <returns>A builder of type <see cref="IServiceModelProxyBuilder{TContract}"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="services"/> or <paramref name="name"/> is null.</exception>
        /// <remarks>
        /// <para>
        /// This overload registers the proxy so that a <see cref="ChannelFactoryProxyWrapper{TContract}"/> is used. Best used when working with shared contracts.
        /// </para>
        /// </remarks>
        public static IServiceModelProxyBuilder<TContract> AddServiceModelProxy<TContract>(this IServiceCollection services, string name)
            where TContract : class
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            _ = name ?? throw new ArgumentNullException(nameof(name));

            services.AddOptions();

            services.AddLogging();

            var builder = new ChannelFactoryServiceModelProxyBuilder<TContract>(name, services);

            builder.Services.AddSingleton<ChannelFactory<TContract>>(sp =>
            {
                var optionMonitor = sp.GetRequiredService<IOptionsMonitor<ServiceModelProxyOptions>>();

                var options = optionMonitor.Get(builder.Name);

                var binding = options.BindingFactory(sp);

                var endpointAddress = options.EndpointAddressFactory(sp);

                var channelFactory = ActivatorUtilities.CreateInstance<ChannelFactory<TContract>>(sp, binding, endpointAddress);

                foreach (var action in options.EndpointConfigurations)
                {
                    action(sp, channelFactory.Endpoint);
                }

                return channelFactory;
            });

            builder.Services.AddTransient<IProxyWrapper<TContract>, ChannelFactoryProxyWrapper<TContract>>();

            return builder;
        }

        /// <summary>
        /// Adds to the <see cref="IServiceCollection"/> the components needed to consume a <see cref="IProxyWrapper{TContract}"/>. The name of the configuration is automatically generated.
        /// </summary>
        /// <typeparam name="TContract">The contract of the WCF service to build a proxy for.</typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <returns>A builder of type <see cref="IServiceModelProxyBuilder{TContract}"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="services"/> is null.</exception>
        /// <remarks>
        /// <para>
        /// This overload registers the proxy so that a <see cref="ChannelFactoryProxyWrapper{TContract}"/> is used. Best used when working with shared contracts.
        /// </para>
        /// </remarks>
        public static IServiceModelProxyBuilder<TContract> AddServiceModelProxy<TContract>(this IServiceCollection services)
            where TContract : class
            => AddServiceModelProxy<TContract>(services, GetConfigurationName(typeof(TContract)));

        /// <summary>
        /// Adds to the <see cref="IServiceCollection"/> the components needed to consume a <see cref="IProxyWrapper{TContract}"/>.
        /// </summary>
        /// <typeparam name="TContract">The contract of the WCF service to build a proxy for.</typeparam>
        /// <typeparam name="TProxy">The type of the proxy to use to connect to the service.</typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <param name="name">The logical name of the proxy to configure.</param>
        /// <returns>A builder of type <see cref="IServiceModelProxyBuilder{TContract, TProxy}"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="services"/> or <paramref name="name"/> is null.</exception>
        /// <remarks>
        /// <para>
        /// This overload registers the proxy so that a <see cref="ClientBaseProxyWrapper{TContract,TProxy}"/> is used. Best used when working with clients generated by Visual Studio or dotnet-svcutil.
        /// </para>
        /// </remarks>
        public static IServiceModelProxyBuilder<TContract, TProxy> AddServiceModelProxy<TContract, TProxy>(this IServiceCollection services, string name)
            where TContract : class
            where TProxy : ClientBase<TContract>, TContract
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            _ = name ?? throw new ArgumentNullException(nameof(name));

            services.AddOptions();

            services.AddLogging();

            var builder = new ClientBaseServiceModelProxyBuilder<TContract, TProxy>(name, services);

            builder.Services.AddTransient<TProxy>(sp =>
            {
                var optionMonitor = sp.GetRequiredService<IOptionsMonitor<ServiceModelProxyOptions>>();

                var options = optionMonitor.Get(builder.Name);

                var binding = options.BindingFactory(sp);

                var endpointAddress = options.EndpointAddressFactory(sp);

                var proxy = ActivatorUtilities.CreateInstance<TProxy>(sp, binding, endpointAddress);

                foreach (var action in options.EndpointConfigurations)
                {
                    action(sp, proxy.Endpoint);
                }

                return proxy;
            });

            builder.Services.AddTransient<IProxyWrapper<TContract>, ClientBaseProxyWrapper<TContract, TProxy>>();

            return builder;
        }

        /// <summary>
        /// Adds to the <see cref="IServiceCollection"/> the components needed to consume a <see cref="IProxyWrapper{TContract}"/>. The name of the configuration is automatically generated.
        /// </summary>
        /// <typeparam name="TContract">The contract of the WCF service to build a proxy for.</typeparam>
        /// <typeparam name="TProxy">The type of the proxy to use to connect to the service.</typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <returns>A builder of type <see cref="IServiceModelProxyBuilder{TContract, TProxy}"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="services"/> is null.</exception>
        /// <remarks>
        /// <para>
        /// This overload registers the proxy so that a <see cref="ClientBaseProxyWrapper{TContract,TProxy}"/> is used. Best used when working with clients generated by Visual Studio or dotnet-svcutil.
        /// </para>
        /// </remarks>
        public static IServiceModelProxyBuilder<TContract, TProxy> AddServiceModelProxy<TContract, TProxy>(this IServiceCollection services)
            where TContract : class
            where TProxy : ClientBase<TContract>, TContract
            => AddServiceModelProxy<TContract, TProxy>(services, GetConfigurationName(typeof(TContract)));
    }
}
