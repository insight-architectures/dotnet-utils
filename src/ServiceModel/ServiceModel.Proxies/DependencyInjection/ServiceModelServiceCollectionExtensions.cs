// ReSharper disable CheckNamespace

using System;
using System.ServiceModel;
using InsightArchitectures.Utilities.DependencyInjection;
using InsightArchitectures.Utilities.ServiceModel;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceModelServiceCollectionExtensions
    {
        private static string GetConfigurationName(Type type) => type?.FullName ?? throw new ArgumentNullException(nameof(type));

        public static IServiceModelProxyBuilder AddServiceModelProxy<TContract>(this IServiceCollection services, string name)
            where TContract : class
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            _ = name ?? throw new ArgumentNullException(nameof(name));

            services.AddOptions();

            services.AddLogging();

            var builder = new DefaultServiceModelProxyBuilder(name, services);

            builder.Services.AddTransient<IProxyWrapper<TContract>>(sp =>
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

                var wrapper = ActivatorUtilities.CreateInstance<ChannelFactoryProxyWrapper<TContract>>(sp, channelFactory);

                return wrapper;
            });

            return builder;
        }

        public static IServiceModelProxyBuilder AddServiceModelProxy<TContract>(this IServiceCollection services)
            where TContract : class
            => AddServiceModelProxy<TContract>(services, GetConfigurationName(typeof(TContract)));

        public static IServiceModelProxyBuilder AddServiceModelProxy<TContract, TProxy>(this IServiceCollection services, string name)
            where TContract : class
            where TProxy : ClientBase<TContract>, TContract
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            _ = name ?? throw new ArgumentNullException(nameof(name));

            services.AddOptions();

            services.AddLogging();

            var builder = new DefaultServiceModelProxyBuilder(name, services);

            builder.Services.AddTransient<IProxyWrapper<TContract>>(sp =>
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

                var wrapper = ActivatorUtilities.CreateInstance<ClientBaseProxyWrapper<TContract, TProxy>>(sp, proxy);

                return wrapper;
            });

            return builder;
        }

        public static IServiceModelProxyBuilder AddServiceModelProxy<TContract, TProxy>(this IServiceCollection services)
            where TContract : class
            where TProxy : ClientBase<TContract>, TContract
            => AddServiceModelProxy<TContract, TProxy>(services, GetConfigurationName(typeof(TContract)));
    }
}
