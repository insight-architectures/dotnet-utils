using System;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using Microsoft.Extensions.DependencyInjection;

namespace InsightArchitectures.Utilities.ServiceModel.Behaviors
{
    /// <summary>
    /// An endpoint behavior that fetches all registered instances of <see cref="IClientMessageInspector"/> and attaches them to a given proxy.
    /// </summary>
    public class ClientMessageInspectorEndpointBehavior : IEndpointBehavior
    {
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Creates an instance of <see cref="ClientMessageInspectorEndpointBehavior"/>.
        /// </summary>
        /// <param name="serviceProvider">The <see cref="IServiceProvider"/> used to fetch instances of <see cref="IClientMessageInspector"/> to use.</param>
        public ClientMessageInspectorEndpointBehavior(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        /// <inheritdoc />
        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
        }

        /// <inheritdoc />
        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            _ = endpoint ?? throw new ArgumentNullException(nameof(endpoint));

            _ = clientRuntime ?? throw new ArgumentNullException(nameof(clientRuntime));

            var inspectors = _serviceProvider.GetServices<IClientMessageInspector>();

            foreach (var inspector in inspectors)
            {
                clientRuntime.ClientMessageInspectors.Add(inspector);
            }
        }

        /// <inheritdoc />
        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
        }

        /// <inheritdoc />
        public void Validate(ServiceEndpoint endpoint)
        {
        }
    }
}
