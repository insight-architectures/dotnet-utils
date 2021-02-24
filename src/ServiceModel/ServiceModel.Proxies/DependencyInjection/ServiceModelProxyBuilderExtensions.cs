// ReSharper disable CheckNamespace

using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using InsightArchitectures.Utilities.DependencyInjection;
using InsightArchitectures.Utilities.ServiceModel;
using InsightArchitectures.Utilities.ServiceModel.Behaviors;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// A sets of extension methods to <see cref="IServiceModelProxyBuilder"/> and related interfaces.
    /// </summary>
    public static class ServiceModelProxyBuilderExtensions
    {
        /// <summary>
        /// Configures the <see cref="IServiceModelProxyBuilder"/> to use the given <see cref="Binding"/> when creating the proxy.
        /// </summary>
        /// <param name="builder">The <see cref="IServiceModelProxyBuilder"/>.</param>
        /// <param name="binding">The <see cref="Binding"/> used to connect to the remote service.</param>
        /// <returns>An instance of <see cref="IServiceModelProxyBuilder"/>.</returns>
        public static IServiceModelProxyBuilder SetBinding(this IServiceModelProxyBuilder builder, Binding binding)
        {
            _ = builder ?? throw new ArgumentNullException(nameof(builder));

            _ = binding ?? throw new ArgumentNullException(nameof(binding));

            builder.Services.Configure<ServiceModelProxyOptions>(builder.Name, options => options.BindingFactory = _ => binding);

            return builder;
        }

        /// <summary>
        /// Configures the <see cref="IServiceModelProxyBuilder"/> to use the given <see cref="Uri"/> when creating the proxy.
        /// </summary>
        /// <param name="builder">The <see cref="IServiceModelProxyBuilder"/>.</param>
        /// <param name="endpoint">The <see cref="Uri"/> to connect to.</param>
        /// <returns>An instance of <see cref="IServiceModelProxyBuilder"/>.</returns>
        public static IServiceModelProxyBuilder SetEndpointAddress(this IServiceModelProxyBuilder builder, Uri endpoint)
        {
            _ = builder ?? throw new ArgumentNullException(nameof(builder));

            _ = endpoint ?? throw new ArgumentNullException(nameof(endpoint));

            builder.Services.Configure<ServiceModelProxyOptions>(builder.Name, options => options.EndpointAddressFactory = _ => new EndpointAddress(endpoint.ToString()));

            return builder;
        }

        /// <summary>
        /// Configures the <see cref="IServiceModelProxyBuilder"/> to attach an instance of the specified <see cref="IClientMessageInspector"/> to the proxy.
        /// </summary>
        /// <typeparam name="TInspector">A class implementing the <see cref="IClientMessageInspector"/> interface.</typeparam>
        /// <param name="builder">The <see cref="IServiceModelProxyBuilder"/>.</param>
        /// <returns>An instance of <see cref="IServiceModelProxyBuilder"/>.</returns>
        public static IServiceModelProxyBuilder AddClientMessageInspector<TInspector>(this IServiceModelProxyBuilder builder)
            where TInspector : class, IClientMessageInspector
        {
            _ = builder ?? throw new ArgumentNullException(nameof(builder));

            builder.Services.AddSingleton<IClientMessageInspector, TInspector>();

            builder.Services.Configure<ServiceModelProxyOptions>(builder.Name, options =>
            {
                options.EndpointConfigurations.Add((services, endpoint) =>
                {
                    endpoint.EndpointBehaviors.Add(services.GetRequiredService<ClientMessageInspectorEndpointBehavior>());
                });
            });

            builder.Services.TryAddSingleton<ClientMessageInspectorEndpointBehavior>();

            return builder;
        }

        /// <summary>
        /// Configures the <see cref="IServiceModelProxyBuilder"/> to use a wrapper of the specified type.
        /// </summary>
        /// <typeparam name="TClient">The interface to be used to get an instance of the proxy wrapper.</typeparam>
        /// <typeparam name="TContract">The contract of the WCF service to build a proxy for.</typeparam>
        /// <typeparam name="TWrapper">The concrete type to be used for the wrapper. This type must inherit from <see cref="ChannelFactoryProxyWrapper{TContract}"/> and implement <typeparamref name="TClient"/>.</typeparam>
        /// <param name="builder">The <see cref="IServiceModelProxyBuilder{TContract}"/>.</param>
        /// <returns>An instance of <see cref="IServiceModelProxyBuilder{TContract}"/>.</returns>
        /// <remarks>
        /// <para>
        /// This overloads is best used when working with shared contracts.
        /// </para>
        /// </remarks>
        public static IServiceModelProxyBuilder<TContract> AddTypedWrapper<TClient, TContract, TWrapper>(this IServiceModelProxyBuilder<TContract> builder)
            where TContract : class
            where TClient : class, IProxyWrapper<TContract>
            where TWrapper : ChannelFactoryProxyWrapper<TContract>, TClient
        {
            _ = builder ?? throw new ArgumentNullException(nameof(builder));

            builder.Services.AddTransient<TClient, TWrapper>();

            return builder;
        }

        /// <summary>
        /// Configures the <see cref="IServiceModelProxyBuilder"/> to use a wrapper of the specified type.
        /// </summary>
        /// <typeparam name="TClient">The interface to be used to get an instance of the proxy wrapper.</typeparam>
        /// <typeparam name="TContract">The contract of the WCF service to build a proxy for.</typeparam>
        /// <typeparam name="TProxy">The type of the proxy to use to connect to the service.</typeparam>
        /// <typeparam name="TWrapper">The concrete type to be used for the wrapper. This type must inherit from <see cref="ClientBaseProxyWrapper{TContract,TProxy}"/> and implement <typeparamref name="TClient"/>.</typeparam>
        /// <param name="builder">The <see cref="IServiceModelProxyBuilder{TContract,TProxy}"/>.</param>
        /// <returns>An instance of <see cref="IServiceModelProxyBuilder{TContract,TProxy}"/>.</returns>
        /// <remarks>
        /// <para>
        /// This overloads is best used when working with clients generated by Visual Studio or dotnet-svcutil.
        /// </para>
        /// </remarks>
        public static IServiceModelProxyBuilder<TContract, TProxy> AddTypedWrapper<TClient, TContract, TProxy, TWrapper>(this IServiceModelProxyBuilder<TContract, TProxy> builder)
            where TContract : class
            where TClient : class, IProxyWrapper<TContract>
            where TProxy : ClientBase<TContract>, TContract
            where TWrapper : ClientBaseProxyWrapper<TContract, TProxy>, TClient
        {
            _ = builder ?? throw new ArgumentNullException(nameof(builder));

            builder.Services.AddTransient<TClient, TWrapper>();

            return builder;
        }
    }
}
