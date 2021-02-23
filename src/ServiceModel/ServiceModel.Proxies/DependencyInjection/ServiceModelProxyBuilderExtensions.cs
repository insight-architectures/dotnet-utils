// ReSharper disable CheckNamespace

using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using InsightArchitectures.Utilities.DependencyInjection;
using InsightArchitectures.Utilities.ServiceModel.Behaviors;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceModelProxyBuilderExtensions
    {
        public static IServiceModelProxyBuilder SetBinding(this IServiceModelProxyBuilder builder, Binding binding)
        {
            _ = builder ?? throw new ArgumentNullException(nameof(builder));

            _ = binding ?? throw new ArgumentNullException(nameof(binding));

            builder.Services.Configure<ServiceModelProxyOptions>(builder.Name, options => options.BindingFactory = _ => binding);

            return builder;
        }

        public static IServiceModelProxyBuilder SetEndpointAddress(this IServiceModelProxyBuilder builder, Uri endpoint)
        {
            _ = builder ?? throw new ArgumentNullException(nameof(builder));

            _ = endpoint ?? throw new ArgumentNullException(nameof(endpoint));

            builder.Services.Configure<ServiceModelProxyOptions>(builder.Name, options => options.EndpointAddressFactory = _ => new EndpointAddress(endpoint.ToString()));

            return builder;
        }

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
    }
}
